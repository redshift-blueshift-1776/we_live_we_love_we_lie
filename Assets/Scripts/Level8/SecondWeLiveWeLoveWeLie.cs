using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;
using System.Globalization;
using System.IO;

public class SecondWeLiveWeLoveWeLie : MonoBehaviour
{
    [SerializeField] public GameObject player;

    [SerializeField] public GameObject note;

    [SerializeField] public bool hard;

    [SerializeField] public bool debug;

    [SerializeField] public GameObject maze;

    [SerializeField] public GameObject[] briefcases;
    [SerializeField] public GameObject[] briefcasePivots;

    [SerializeField] public int scoreThreshold;

    [Header("Canvasses")]
    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private TMP_Text scoreGame;

    [Header("Audio")]
    [SerializeField] public GameObject loadingAudio;
    [SerializeField] public GameObject gameAudio;

    // public bool gameActive;

    public float timer;

    // public List<GameObject> notes;

    public int score;

    public BeatManager beatManager;
    public bool gameActive = false;

    public List<string> notes; // each: "beat,x,y"

    public List<GameObject> noteblocks; // each: "beat,x,y"

    private double dspStartTime;
    private float secondsPerBeat;
    private int nextNoteIndex = 0;

    // How early (in seconds) to spawn a note before it should appear
    private const double SPAWN_LEAD_TIME = 20.0;

    public bool madeNotes;
    public bool didBriefcases;

    [SerializeField] public bool customLevel;
    [SerializeField] public bool levelEditor;

    public GameObject simpleCustomMapMaker;
    public SimpleCustomMapMaker scmm;

    public string customMapPath;

    [Header("Custom Level Tuning")]
    [Tooltip("If true, 'simple' maps keep the x/y saved in JSON (editor positions). If false, randomize as before (legacy bug).")]
    public bool preserveSimpleMapPositions = true;
    [Tooltip("If true, use the map's BPM for timing in custom levels; if false, keep story 145 BPM.")]
    public bool useMapBpmForCustomLevels = true;
    [Tooltip("If true and customMapPath is empty/missing, search persistentDataPath for a JSON matching PlayerPrefs SelectedSong.")]
    public bool resolveCustomMapBySelectedSong = true;
    [Tooltip("Extra seconds after the last note before win/fail is evaluated in custom levels.")]
    public float customLevelEndBufferSeconds = 8f;

    // Runtime custom-map state (populated after load)
    private SimpleMapData loadedCustomMap;
    private float customLevelDurationSeconds = 192f;
    private bool customLevelLoadFailed = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        madeNotes = false;
        didBriefcases = false;
        secondsPerBeat = 60.0f / 145.0f / 4.0f;
        startCanvas.SetActive(true);
        gameCanvas.SetActive(false);
        loadingAudio.SetActive(true);
        gameAudio.SetActive(false);
        gameActive = false;
        timer = 0f;

        beatManager = BeatManager.Instance;
        dspStartTime = beatManager.StartDspTime;

        Debug.Log(SPAWN_LEAD_TIME);
        // secondsPerBeat = (float)beatManager.secondsPerBeat;

        if (levelEditor) {
            scmm = simpleCustomMapMaker.GetComponent<SimpleCustomMapMaker>();
        }
    }

    private string ResolveCustomMapPath()
    {
        // 1) Explicit customMapPath (if set and exists)
        if (!string.IsNullOrEmpty(customMapPath))
        {
            string direct = Path.Combine(Application.persistentDataPath, customMapPath);
            if (File.Exists(direct))
                return direct;
            // Allow absolute path as fallback (in case customMapPath is already full path)
            if (File.Exists(customMapPath))
                return customMapPath;
        }
        // 2) Lookup by PlayerPrefs SelectedSong (matches LevelEditorScene.cs:61)
        if (resolveCustomMapBySelectedSong)
        {
            string selected = PlayerPrefs.GetString("SelectedSong", "");
            if (!string.IsNullOrEmpty(selected) && selected != "UNKNOWN")
            {
                try
                {
                    string[] jsonFiles = Directory.GetFiles(Application.persistentDataPath, "*.json");
                    foreach (string file in jsonFiles)
                    {
                        try
                        {
                            string j = File.ReadAllText(file);
                            SimpleMapData m = JsonUtility.FromJson<SimpleMapData>(j);
                            if (m != null && m.songName == selected)
                                return file;
                        }
                        catch (Exception ex)
                        {
                            Debug.LogWarning($"ResolveCustomMapPath: failed to parse {file}: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"ResolveCustomMapPath: Directory scan failed: {ex.Message}");
                }
            }
        }
        return null;
    }

    public IEnumerator CallGenerateNotes()
    {
        if (customLevel)
        {
            string realPath = ResolveCustomMapPath();
            if (string.IsNullOrEmpty(realPath) || !File.Exists(realPath))
            {
                Debug.LogError($"CallGenerateNotes: custom map not found. customMapPath='{customMapPath}' resolved='{realPath}' persistent='{Application.persistentDataPath}' SelectedSong='{PlayerPrefs.GetString("SelectedSong", "UNKNOWN")}'");
                customLevelLoadFailed = true;
                // Fallback to story notes so the level is still playable in editor validation
                GenerateNotes();
                madeNotes = true;
                yield break;
            }
            SimpleMapData map = null;
            try
            {
                string json = File.ReadAllText(realPath);
                map = JsonUtility.FromJson<SimpleMapData>(json);
            }
            catch (Exception ex)
            {
                Debug.LogError($"CallGenerateNotes: failed to read/parse {realPath}: {ex}");
                customLevelLoadFailed = true;
                GenerateNotes();
                madeNotes = true;
                yield break;
            }
            if (map == null || map.notes == null)
            {
                Debug.LogError($"CallGenerateNotes: parsed map is null at {realPath}");
                customLevelLoadFailed = true;
                GenerateNotes();
                madeNotes = true;
                yield break;
            }
            loadedCustomMap = map;

            // Use map BPM for timing if tunable enabled (story default 145 BPM -> secondsPerBeat = 60/145/4)
            if (useMapBpmForCustomLevels && map.bpm > 0)
            {
                secondsPerBeat = 60f / map.bpm / 4f;
                // Propagate to BeatManager so GetCurrentBeatNumber / visual sync matches map BPM
                if (beatManager != null)
                {
                    beatManager.tempo = map.bpm;
                    beatManager.secondsPerBeat = 60f / map.bpm;
                }
                else if (BeatManager.Instance != null)
                {
                    BeatManager.Instance.tempo = map.bpm;
                    BeatManager.Instance.secondsPerBeat = 60f / map.bpm;
                }
                // Also swap audio clip to the selected song if available
                try
                {
                    var songs = Resources.LoadAll<SongDataSO>("Songs");
                    foreach (var s in songs)
                    {
                        if (s.songName == map.songName && s.audioClip != null)
                        {
                            // gameAudio is the main gameplay audio source holder
                            AudioSource gameAudioSource = null;
                            if (gameAudio != null) gameAudioSource = gameAudio.GetComponent<AudioSource>();
                            if (gameAudioSource == null && gameAudio != null) gameAudioSource = gameAudio.GetComponentInChildren<AudioSource>();
                            if (gameAudioSource != null && gameAudioSource.clip != s.audioClip)
                                gameAudioSource.clip = s.audioClip;
                            if (beatManager != null && beatManager.audioSource != null && beatManager.audioSource.clip != s.audioClip)
                                beatManager.audioSource.clip = s.audioClip;
                            else if (BeatManager.Instance != null && BeatManager.Instance.audioSource != null && BeatManager.Instance.audioSource.clip != s.audioClip)
                                BeatManager.Instance.audioSource.clip = s.audioClip;
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"CallGenerateNotes: failed to assign custom audio clip for '{map.songName}': {ex.Message}");
                }
            }

            // Build note list — preserve editor x/y when tunable is on
            // float z = note.z;
            int maxBeat = int.MinValue;
            foreach (var note in map.notes)
            {
                float beat = note.beat;
                float x = note.x;
                float y = note.y;

                // Legacy bug: simple maps randomized positions. Tunable preserves editor positions.
                if (map.mapType == "simple" && !preserveSimpleMapPositions) {
                    x = UnityEngine.Random.Range(-2, 3);
                    y = UnityEngine.Random.Range(-2, 3);
                }
                notes.Add($"{beat},{x},{y}");
                if (beat > maxBeat) maxBeat = Mathf.RoundToInt(beat);
            }
            // Dynamic duration: last beat -> seconds + buffer (instead of fixed 192f)
            if (maxBeat != int.MinValue)
            {
                float lastNoteSeconds = Mathf.Abs(maxBeat * secondsPerBeat);
                // Duration covers note window (16 sixteenths ~= 4 beats) plus buffer
                customLevelDurationSeconds = lastNoteSeconds + 16f * secondsPerBeat + customLevelEndBufferSeconds;
                // Clamp to at least story length to avoid instant win on tiny maps, but respect longer songs
                customLevelDurationSeconds = Mathf.Max(customLevelDurationSeconds, 30f);
            }
            else
            {
                customLevelDurationSeconds = 192f;
            }
            madeNotes = true;
        }
        else {
            GenerateNotes();
        }
        yield return null;
    }


    public IEnumerator CallMakeRandomNotes(List<int> fallbackNoteTimes) {
        notes.AddRange(randomScatterYOffset(fallbackNoteTimes.ToArray(), 0, -3));
        yield return null;
    }

    void Update() {
        if (Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.M)) {
            SceneManager.LoadScene("Menu");
        }
        if (gameActive) {
            if (levelEditor) {
                double currentDspTime = AudioSettings.dspTime;
                double songTime = currentDspTime - beatManager.StartDspTime;
                // if (double.IsNaN(songTime) || songTime < 0) {
                //     songTime = Time.time - dspStartTime;
                // }
                timer += Time.deltaTime;

                // levelEditor recording mode uses fixed 192s timeout (song length approx)
                float levelEditorTimeout = 192f;
                // For custom playback we use dynamic duration computed from map; story stays 192f
                if (timer >= levelEditorTimeout) {
                    gameActive = false;
                    if (score > scoreThreshold) {
                        Win();
                    } else {
                        Fail();
                    }
                }
                scoreGame.text = "Score: " + score;

                if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.R)) {
                    scmm.StopRecording();
                }
            } else {
                if (!madeNotes) {
                    StartCoroutine(CallGenerateNotes());
                }

                // Get the final note in the list, check if the z position is near 17645.
                // If not, clear notes and replace it with random notes

                double currentDspTime = AudioSettings.dspTime;
                double songTime = currentDspTime - beatManager.StartDspTime;
                timer += Time.deltaTime;

                // The DSP-based note spawning code goes here
                while (nextNoteIndex < notes.Count)
                {
                    string n = notes[nextNoteIndex];
                    string[] parts = n.Split(',');
                    if (parts.Length < 3)
                    {
                        nextNoteIndex++;
                        continue;
                    }
                    if (float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float beatTime) &&
                        float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float x_pos) &&
                        float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float y_pos))
                    {
                        float z_pos = 0f;
                        if (parts.Length > 3)
                            float.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out z_pos);

                        double noteTime = beatTime * secondsPerBeat;

                        SpawnNote(beatTime, x_pos, y_pos, z_pos);
                        nextNoteIndex++;
                    }
                    else {
                        nextNoteIndex++;
                    }
                }

                // Briefcases are story-only (beat 550+); skip for custom levels
                if (!customLevel && timer / (float) secondsPerBeat > 550f && !didBriefcases) {
                    Debug.Log(timer / (float) secondsPerBeat);
                    DoBriefcases(new int[] { 576+64, 576+64+8, 576+64+16 }, briefcases, briefcasePivots);
                    // DoBriefcases(new int[] { 64, 64+8, 64+16 }, briefcases, briefcasePivots);
                    didBriefcases = true;
                }


                float endTime = customLevel ? customLevelDurationSeconds : 192f;
                if (timer >= endTime) {
                    gameActive = false;
                    if (score > scoreThreshold) {
                        Win();
                    } else {
                        Fail();
                    }
                }
                scoreGame.text = "Score: " + score;
            }
        }
    }

    private void SpawnNote(float beatTime, float x_pos, float y_pos, float z_pos)
    {
        GameObject newNote = Instantiate(note, transform);
        newNote.transform.localPosition = player.transform.localPosition +
            new Vector3(3f * x_pos, 3f * y_pos, (Mathf.Abs(beatTime) - 64f) * 10f + 205f);
        newNote.transform.localScale = new Vector3(3f, 3f, 1f);
        noteblocks.Add(newNote);

        Note newNoteScript = newNote.GetComponent<Note>();
        newNoteScript.gm = this;

        // Convert beat-based timings to seconds
        newNoteScript.duration = 16f * secondsPerBeat;
        if (debug) {
            newNoteScript.debug = true;
            newNoteScript.delay = Mathf.Abs(beatTime * secondsPerBeat);
        } else {
            newNoteScript.delay = Mathf.Abs(beatTime * secondsPerBeat) - 8f * secondsPerBeat;
        }

        newNoteScript.realNote = (beatTime > 0);
    }

    public string[] SonicBlasterPattern(int time_start, float x_start, float y_start, float z_start) {
        string[] ret = new string[18];
        if (hard) {
            ret = new string[18];
            ret[0] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 2;
            ret[1] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 2;
            ret[2] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start -= 2;
            ret[3] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start += 1;
            ret[4] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start += 1;
            ret[5] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start += 0.2f;
            ret[6] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 0.2f;
            ret[7] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 0.2f;
            ret[8] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 0.2f;
            ret[9] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 0.2f;
            ret[10] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start += 1;
            ret[11] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start += 1;
            ret[12] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 1;
            ret[13] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 1;
            ret[14] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 1;
            ret[15] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start += 1;
            ret[16] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start += 1;
            ret[17] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
        } else {
            ret = new string[10];
            ret[0] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 1;
            time_start += 2;
            time_start += 2;
            y_start += 1;
            time_start += 2;
            ret[1] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start += 3;
            time_start += 2;
            y_start += 0.5f;
            ret[2] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 0.5f;
            ret[3] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 0.5f;
            ret[4] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 0.5f;
            ret[5] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 0.5f;
            ret[6] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 1;
            time_start += 2;
            y_start -= 1;
            ret[7] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 1;
            time_start += 2;
            x_start -= 1;
            ret[8] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 1;
            time_start += 2;
            y_start -= 1;
            ret[9] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
        }
        return ret;
    }

    public string[] SpectrePattern(int time_start, float x_start, float y_start, float z_start) {
        string[] ret = new string[12];
        if (hard) {
            ret = new string[12];
            ret[0] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 4;
            y_start += 1;
            ret[1] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 2;
            ret[2] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 2;
            ret[3] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 4;
            y_start += 0.2f;
            ret[4] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 0.2f;
            ret[5] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 0.2f;
            ret[6] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 0.2f;
            ret[7] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 0.2f;
            ret[8] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 4;
            x_start += 2;
            ret[9] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start += 2;
            ret[10] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start += 1;
            ret[11] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
        } else {
            ret = new string[4];
            ret[0] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 4;
            y_start += 1;
            ret[1] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 1;
            time_start += 2;
            time_start += 4;
            y_start += 0.2f;
            time_start += 1;
            y_start += 0.2f;
            time_start += 1;
            y_start += 0.2f;
            time_start += 1;
            y_start += 0.2f;
            time_start += 1;
            y_start += 0.2f;
            time_start += 4;
            x_start += 1;
            ret[2] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start += 1;
            time_start += 2;
            ret[3] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
        }
        return ret;
    }

    public string[] YellowClockPattern(int time_start, float x_start, float y_start, float z_start) {
        string[] ret = new string[12];
        if (hard) {
            ret = new string[30];

            ret[0] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            ret[1] = "" + (0 - time_start) + "," + x_start + "," + (y_start - 1) + "," + z_start;
            time_start += 4;
            y_start += 1;
            ret[2] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 1;
            ret[3] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 1;
            ret[4] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start += 1f;
            ret[5] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start += 1f;
            ret[6] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 4;
            x_start += 2f;

            ret[7] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start -= 1f;
            ret[8] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            x_start -= 0.5f;
            ret[9] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            x_start -= 0.5f;
            ret[10] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start -= 1;
            ret[11] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start += 1;
            ret[12] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start += 1;
            ret[13] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start += 1;
            ret[14] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 4;
            x_start -= 2;

            ret[15] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start += 1;
            ret[16] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start += 1;
            ret[17] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start -= 1;
            ret[18] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start -= 1;
            ret[19] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start += 1;
            ret[20] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start -= 1;
            ret[21] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 4;
            x_start += 2;

            ret[22] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start += 1;
            ret[23] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start += 1;
            ret[24] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start -= 1;
            ret[25] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start -= 1;
            ret[26] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start += 1;
            ret[27] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start -= 1;
            ret[28] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 2;
            ret[29] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
        } else {
            ret = new string[20];

            ret[0] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 4;
            y_start += 1;
            ret[1] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 1;
            time_start += 2;
            ret[2] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start += 1f;
            time_start += 2;
            ret[3] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 4;
            x_start += 2f;

            ret[4] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start -= 1f;
            time_start += 1;
            x_start -= 0.5f;
            time_start += 1;
            x_start -= 0.5f;
            ret[5] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start -= 1;
            ret[6] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start += 1;
            ret[7] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start += 1;
            time_start += 2;
            // y_start += 1;
            ret[8] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 4;
            x_start -= 2;

            ret[9] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start += 1;
            time_start += 2;
            // x_start += 1;
            ret[10] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start -= 1;
            time_start += 2;
            // y_start -= 1;
            ret[11] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start += 1;
            ret[12] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start -= 1;
            ret[13] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 4;
            x_start += 1;

            ret[14] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            // y_start += 1;
            time_start += 2;
            x_start += 1;
            ret[15] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start -= 1;
            time_start += 2;
            // y_start -= 1;
            ret[16] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start += 1;
            ret[17] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start -= 1;
            ret[18] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 1;
            ret[19] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
        }
        return ret;
    }

    public string[] wallSeparatedPattern(int time_start, float x_start, float y_start, float z_start, int repeats) {
        string[] ret = new string[8 * repeats];
        
        for (int i = 0; i < repeats; i++) {
            if (hard) {
                ret[8 * i] = "" + time_start + "," + (x_start - 2) + "," + y_start + "," + z_start;
            } else {
                ret[8 * i] = "" + time_start + "," + (x_start - 1) + "," + y_start + "," + z_start;
            }
            time_start += 4;
            if (hard) {
                ret[8 * i + 1] = "" + (0 - time_start) + "," + x_start + "," + (y_start - 1) + "," + z_start;
                ret[8 * i + 2] = "" + (0 - time_start) + "," + x_start + "," + (y_start - 2) + "," + z_start;
                ret[8 * i + 3] = "" + time_start + "," + x_start + "," + (y_start + 2) + "," + z_start;
            } else {
                ret[8 * i + 1] = "" + (0 - time_start) + "," + x_start + "," + y_start + "," + z_start;
                ret[8 * i + 2] = "" + (0 - time_start) + "," + x_start + "," + (y_start + 1) + "," + z_start;
                ret[8 * i + 3] = "" + (0 - time_start) + "," + x_start + "," + (y_start - 1) + "," + z_start;
            }
            time_start += 4;
            if (hard) {
                ret[8 * i + 4] = "" + time_start + "," + (x_start + 2) + "," + y_start + "," + z_start;
            } else {
                ret[8 * i + 4] = "" + time_start + "," + (x_start + 1) + "," + y_start + "," + z_start;
            }
            time_start += 4;
            if (hard) {
                ret[8 * i + 5] = "" + (0 - time_start) + "," + x_start + "," + (y_start + 1) + "," + z_start;
                ret[8 * i + 6] = "" + (0 - time_start) + "," + x_start + "," + (y_start + 2) + "," + z_start;
                ret[8 * i + 7] = "" + time_start + "," + x_start + "," + (y_start - 2) + "," + z_start;
            } else {
                ret[8 * i + 5] = "" + (0 - time_start) + "," + x_start + "," + y_start + "," + z_start;
                ret[8 * i + 6] = "" + (0 - time_start) + "," + x_start + "," + (y_start + 1) + "," + z_start;
                ret[8 * i + 7] = "" + (0 - time_start) + "," + x_start + "," + (y_start - 1) + "," + z_start;
            }
            time_start += 4;
        }
        return ret;
    }

    public string[] randomScatter(int[] times, int spread) {
        string[] ret = new string[times.Length];
        for (int i = 0; i < times.Length; i++) {
            ret[i] = "" + times[i] + "," + UnityEngine.Random.Range(-1 * spread, spread + 1) + "," + UnityEngine.Random.Range(-1 * spread, spread + 1);
        }
        return ret;
    }

    public string[] randomScatterYOffset(int[] times, int spread, int yOffset) {
        string[] ret = new string[times.Length];
        for (int i = 0; i < times.Length; i++) {
            ret[i] = "" + times[i] + "," + UnityEngine.Random.Range(-1 * spread, spread + 1) + "," + (yOffset + UnityEngine.Random.Range(-1 * spread, spread + 1));
        }
        return ret;
    }

    public void solveMaze(int[] times, GameObject maze) {
        string[] newNotes = new string[9];
        Maze_Generator_Level_8 mg = maze.GetComponent<Maze_Generator_Level_8>();
        List<(int from, int to)> mstEdges = mg.mstEdges;
        int gridSize = 5;
        int totalCells = gridSize * gridSize;
        int start = 0;
        int goal = totalCells - 1;
        Dictionary<int, List<int>> graph = new Dictionary<int, List<int>>();
        for (int i = 0; i < totalCells; i++) graph[i] = new List<int>();

        foreach (var edge in mstEdges)
        {
            graph[edge.from].Add(edge.to);
            graph[edge.to].Add(edge.from);
        }
        // BFS to solve from 0 to 24, since it's 5x5.
        Queue<int> queue = new Queue<int>();
        Dictionary<int, int> parent = new Dictionary<int, int>();
        HashSet<int> visited = new HashSet<int>();

        queue.Enqueue(start);
        visited.Add(start);
        parent[start] = -1;

        bool found = false;
        while (queue.Count > 0 && !found)
        {
            int current = queue.Dequeue();
            foreach (int neighbor in graph[current])
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    parent[neighbor] = current;
                    queue.Enqueue(neighbor);
                    if (neighbor == goal)
                    {
                        found = true;
                        break;
                    }
                }
            }
        }

        List<int> path = new List<int>();
        if (found)
        {
            int current = goal;
            while (current != -1)
            {
                path.Add(current);
                current = parent[current];
            }
            path.Reverse();
        }
        else
        {
            Debug.LogWarning("No path found in maze!");
            return;
        }

        // Keep the first 9 squares.
        int stepsToKeep = Mathf.Min(9, path.Count);
        List<int> limitedPath = path.GetRange(0, stepsToKeep);

        // Each grid square is 50 x 50, 0 in the top left, 24 in the bottom right.
        List<(float x, float y)> coords = new List<(float, float)>();
        foreach (int idx in limitedPath)
        {
            int row = idx / gridSize;
            int col = idx % gridSize;

            // Convert to offsets
            float x_pos = col * 50f - 100f;
            float y_pos = -row * 50f + 100f;
            coords.Add((x_pos, y_pos));
        }
        // Debug.Log("making maze notes");

        // Instantiate notes corresponding to path steps
        for (int i = 0; i < coords.Count && i < times.Length; i++)
        {
            // Debug.Log("Making note " + i);
            float duration = times[i];
            float x_pos = coords[i].x;
            float y_pos = coords[i].y;

            GameObject newNote = Instantiate(note);
            newNote.transform.position = new Vector3(x_pos + maze.transform.position.x, y_pos+ maze.transform.position.y, maze.transform.position.z);
            newNote.transform.localScale = new Vector3(25f, 25f, 1f);

            Note newNoteScript = newNote.GetComponent<Note>();
            newNoteScript.gm = gameObject.GetComponent<SecondWeLiveWeLoveWeLie>();
            newNoteScript.duration = 16f * (float)secondsPerBeat;
            if (debug) {
                newNoteScript.debug = true;
                newNoteScript.delay = Mathf.Abs(duration * (float)secondsPerBeat);
            } else {
                newNoteScript.delay = Mathf.Abs(duration * (float)secondsPerBeat) - 8f * secondsPerBeat;
            }
            newNoteScript.realNote = (duration > 0);
        }
        return;
    }

    public void DoBriefcases(int[] times, GameObject[] briefcases, GameObject[] briefcasePivots)
    {
        StartCoroutine(SpawnBriefcaseNotes(times, briefcases, briefcasePivots));
    }

    private IEnumerator SpawnBriefcaseNotes(int[] times, GameObject[] briefcases, GameObject[] briefcasePivots)
    {
        if (briefcases.Length < 3 || briefcasePivots.Length < 3)
        {
            Debug.LogWarning("Need exactly 3 briefcases and pivots!");
            yield break;
        }

        for (int i = 0; i < 3; i++)
        {
            int beatTime = times[i];
            float spawnTime = beatTime * (float)secondsPerBeat;
            float waitTime = Mathf.Max(0, spawnTime - (float)timer);

            // Wait until it's time to open the briefcase
            yield return new WaitForSeconds(waitTime);

            GameObject b = briefcases[i];
            GameObject bp = briefcasePivots[i];

            bool isReal = UnityEngine.Random.value < 0.5f;

            StartCoroutine(OpenBriefcase(bp, isReal));

            Debug.Log($"[Briefcase {i}] Spawning {(isReal ? "real" : "fake")} note at {timer:F2}s");

            GameObject newNote = Instantiate(note);
            newNote.transform.position = b.transform.position;
            newNote.transform.localScale = new Vector3(25f, 5f, 25f);
            newNote.transform.rotation = b.transform.rotation;
            // Avoid parent scaling/rotation issues — don’t parent to b unless needed
            // newNote.transform.SetParent(b.transform);

            Note newNoteScript = newNote.GetComponent<Note>();
            newNoteScript.gm = gameObject.GetComponent<SecondWeLiveWeLoveWeLie>();
            newNoteScript.duration = 32f * (float)secondsPerBeat;

            // set delay relative to current timer
            newNoteScript.delay = (float)timer + 4f * (float)secondsPerBeat; 
            newNoteScript.realNote = isReal;
        }
    }


    private IEnumerator OpenBriefcase(GameObject pivot, bool isReal)
    {
        float openAngle = isReal ? 105f : 90f; // real ones open wider
        float duration = 0.5f;
        float elapsed = 0f;
        Quaternion startRot = pivot.transform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(openAngle, 0f, 0f);

        while (elapsed < duration)
        {
            pivot.transform.rotation = Quaternion.Slerp(startRot, endRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        pivot.transform.rotation = endRot;
    }



    public void GenerateNotes() {
        if (madeNotes) return;
        madeNotes = true;
        if (hard) {
            this.notes = new List<string> {
            "64,2,0",
            "-72,1,1",
            "-72,2,1",
            "-72,3,1",
            "80,1,0",
            "-80,1,1",
            "-80,1,-1",
            "-96,0,-1",
            "-100,-1,-1",
            "-104,0,-1",
            "-108,1,-1",
            "112,-1,0",
            "-116,0,-1",
            "-120,3,0",
            "-124,1,-1",
            "128,-3,0",
            "-128,-4,0",
            "-132,-4,1",
            "-132,-4,-1",
            "-136,-3,-1",
            "-136,-3,1",
            "-140,-2,-1",
            "144,-2,1",
            "-144,-2,0",
            "-148,-2,3",
            "152,-1,1",
            "-160,0,-1",
            "168,2,1",
            "176,1,1",

            "200,0,0",
            "-208,0,3",
            "216,0,-3",
            "-220,0,3",
            "224,0,0",
            "-228,0,-3",
            "232,0,3",
            "-236,0,-3",
            "240,0,0",
            "-244,0,3",
            "248,0,-3",

            "256,1,0,120",
            "-256,1,1,120",
            "-256,1,-1,120",
            "-256,2,-1,120",
            "272,0,0,120",
            "288,1,0,120",
            "-289,1,2,120",
            "-290,1,-2,120",
            "296,0,1,120",
            "304,1,1,120",

            // Drop
            "320,0,0,120",
            "-320,-1,0,120",
            "-320,1,0,120",
            "-322,0,-3,120",
            "324,0,1,120",
            "326,-1,1,120",
            "328,-1,0,120",
            "330,0,0,120",
            "332,1,0,120",
            "-334,-2,-2,120",

            "336,1,1,120",
            "-336,2,1,120",
            "-336,0,1,120",
            "338,1,0.8,120",
            "339,1,0.6,120",
            "340,1,0.4,120",
            "342,1,0,120",
            "344,-1,0,120",
            "346,0,0,120",
            "348,0,-1,120",

            "352,-2,0,120",
            "354,-1,0,120",
            "356,0,0,120",
            "358,1,0,120",
            "360,2,0,120",
            "362,2,1,120",
            "364,1,1,120",

            "368,0,0,120",
            "370,-1,0,120",
            "372,-1,-1,120",
            "374,0,-1,120",
            "376,1,-1,120",
            "378,1,0,120",
            "380,1,1,120",
            "382,0,1,120",

            "384,1,0,120",
            "-384,1,1,120",
            "-384,1,-1,120",
            "388,0,0,120",
            "390,-0.5,0,120",
            "392,-0.5,-1,120",
            "396,-0.6,-1,120",
            "397,-0.7,-1,120",
            "398,-0.8,-1,120",
            "399,-0.9,-1,120",
            "400,-1,-1,120",
            "404,0,1,120",
            "406,1,1,120",
            "408,1,0,120",

            "416,0,0,120",
            "420,-1,0,120",
            "422,-1,-1,120",
            "424,0,-1,120",
            "428,1,-1,120",
            "429,1,0,120",
            "430,1,1,120",
            "431,0,1,120",
            "432,0,0,120",
            "436,-1,0,120",
            "438,-1,1,120",
            "440,0,2,120",
            };

            notes.AddRange(SonicBlasterPattern(448, 0f, 0f, 0f));
            notes.AddRange(SonicBlasterPattern(480, 0f, 0f, 0f));
            notes.AddRange(SpectrePattern(512, 0f, 0f, 0f));
            notes.AddRange(SpectrePattern(544, 0f, 0f, 0f));

            int[] times = {
                582,
                584,
                588,
                590,
                596,
                598,
                600,
                604,
                606
            };

            // notes = notes.Concat(randomScatter(times,3)).ToArray();
            solveMaze(times, maze);

            times = new int[] {
                704,
                720,
                736,
                752,
                768,
                784,
                792,
                800,
                816
            };

            notes.AddRange(randomScatter(times,1));

            // times = new int[] {
            //     704 + 128,
            //     712 + 128,
            //     720+ 128,
            //     728 + 128,
            //     736+ 128,
            //     744 + 128,
            //     752+ 128,
            //     760 + 128,
            //     768+ 128,
            //     776 + 128,
            //     784+ 128,
            //     792+ 128,
            //     800+ 128,
            //     808 + 128,
            //     816+ 128
            // };

            // notes.AddRange(randomScatter(times,2));
            notes.AddRange(wallSeparatedPattern(704 + 128, 0f, -2f, 0f, 8));

            notes.AddRange(YellowClockPattern(704 + 256, 0f, 0f, 0f));
            notes.AddRange(YellowClockPattern(704 + 256 + 64, 0f, 0f, 0f));
            notes.Add("1088,0,0");
            notes.AddRange(SonicBlasterPattern(704 + 256 + 64 + 64 + 16, 0f, 0f, 0f));
            notes.AddRange(SpectrePattern(704 + 256 + 64 + 64 + 16 + 32, 0f, 0f, 0f));
            notes.AddRange(YellowClockPattern(704 + 256 + 64 + 64 + 16 + 64, 0f, 0f, 0f));
            notes.AddRange(SonicBlasterPattern(704 + 256 + 64 + 64 + 16 + 128, 2f, 0f, 0f));
            notes.AddRange(SonicBlasterPattern(704 + 256 + 64 + 64 + 16 + 128 + 32, 0f, -1f, 0f));
            notes.AddRange(SonicBlasterPattern(704 + 256 + 64 + 64 + 16 + 128 + 64, 2f, 0f, 0f));
            notes.AddRange(SonicBlasterPattern(704 + 256 + 64 + 64 + 16 + 128 + 32 + 64, 0f, -1f, 0f));
            notes.AddRange(YellowClockPattern(1360, 0f, 0f, 0f));
            notes.AddRange(YellowClockPattern(1360 + 64, 0f, -1f, 0f));
            // notes.Add("1488,0,0");
            notes.AddRange(wallSeparatedPattern(1488, 0f, -2f, 0f, 4));

            times = new int[33];
            for (int i = 0; i < 33; i++) {
                times[i] = 1488 + 64 + 8 * i;
            }
            notes.AddRange(randomScatterYOffset(times, 2, 3));
        } else {
            this.notes = new List<string> {
            "64,2,0",
            "80,1,0",
            "-96,0,-1",
            "112,-1,0",
            "128,-3,0",
            "144,-2,1",
            "152,-1,1",
            "-160,0,-1",
            "168,2,1",
            "176,1,1",

            "192,0,0",
            "208,0,-1",
            "224,0,1",
            "240,0,-1",

            "256,1,0,120",
            "272,0,0,120",
            "288,1,0,120",
            "296,0,1,120",
            "304,1,1,120",

            // Drop
            "320,0,0,120",
            // "324,0,1,120",
            // "326,-1,1,120",
            "328,-1,0,120",
            // "330,0,0,120",
            // "332,1,0,120",

            "336,1,1,120",
            // "338,1,0.8,120",
            // "339,1,0.6,120",
            // "340,1,0.4,120",
            // "342,1,0,120",
            // "344,-1,0,120",
            // "346,0,0,120",
            // "348,0,-1,120",

            "352,-2,0,120",
            // "354,-1,0,120",
            // "356,0,0,120",
            // "358,1,0,120",
            "360,2,0,120",
            // "362,2,1,120",
            // "364,1,1,120",

            "368,0,0,120",
            // "370,-1,0,120",
            // "372,-1,-1,120",
            // "374,0,-1,120",
            // "376,1,-1,120",
            // "378,1,0,120",
            // "380,1,1,120",
            // "382,0,1,120",

            "384,1,0,120",
            // "388,0,0,120",
            // "390,-0.5,0,120",
            "392,-0.5,-1,120",
            // "396,-0.6,-1,120",
            // "397,-0.7,-1,120",
            // "398,-0.8,-1,120",
            // "399,-0.9,-1,120",
            "400,-1,-1,120",
            // "404,0,1,120",
            // "406,1,1,120",
            "408,1,0,120",

            "416,0,0,120",
            "420,-1,0,120",
            // "422,-1,-1,120",
            "424,0,-1,120",
            "428,1,-1,120",
            // "429,1,0,120",
            // "430,1,1,120",
            // "431,0,1,120",
            "432,0,0,120",
            // "436,-1,0,120",
            // "438,-1,1,120",
            // "440,0,2,120",
            };

            notes.AddRange(SonicBlasterPattern(448, 0f, 0f, 0f));
            notes.AddRange(SonicBlasterPattern(480, 0f, 0f, 0f));
            notes.AddRange(SpectrePattern(512, 0f, 0f, 0f));

            int[] times = {
                582,
                584,
                588,
                590,
                596,
                598,
                600,
                604,
                606
            };

            solveMaze(times, maze);

            times = new int[] {
                704,
                720,
                736,
                752,
                768,
                784,
                792,
                800,
                816
            };

            notes.AddRange(randomScatter(times,1));

            times = new int[] {
                704 + 128,
                720+ 128,
                736+ 128,
                752+ 128,
                768+ 128,
                784+ 128,
                792+ 128,
                800+ 128,
                816+ 128
            };

            notes.AddRange(randomScatter(times,2));

            notes.AddRange(YellowClockPattern(704 + 256, 0f, 0f, 0f));
            notes.AddRange(YellowClockPattern(704 + 256 + 64, 0f, 0f, 0f));
            notes.Add("1088,0,0");
            notes.AddRange(SonicBlasterPattern(704 + 256 + 64 + 64 + 16, 0f, 0f, 0f));
            notes.AddRange(SpectrePattern(704 + 256 + 64 + 64 + 16 + 32, 0f, 0f, 0f));
            notes.AddRange(YellowClockPattern(704 + 256 + 64 + 64 + 16 + 64, 0f, 0f, 0f));
            notes.AddRange(SonicBlasterPattern(704 + 256 + 64 + 64 + 16 + 128, 2f, 0f, 0f));
            notes.AddRange(SonicBlasterPattern(704 + 256 + 64 + 64 + 16 + 128 + 32, 0f, -1f, 0f));
            notes.AddRange(SonicBlasterPattern(704 + 256 + 64 + 64 + 16 + 128 + 64, 2f, 0f, 0f));
            notes.AddRange(SonicBlasterPattern(704 + 256 + 64 + 64 + 16 + 128 + 32 + 64, 0f, -1f, 0f));
            notes.AddRange(YellowClockPattern(1360, 0f, 0f, 0f));
            notes.AddRange(YellowClockPattern(1360 + 64, 0f, -1f, 0f));
            // notes.Add("1488,0,0");
            notes.AddRange(wallSeparatedPattern(1488, 0f, -2f, 0f, 4));

            times = new int[17];
            for (int i = 0; i < 17; i++) {
                times[i] = 1488 + 64 + 16 * i;
            }
            notes.AddRange(randomScatterYOffset(times, 1, 3));
        }
        madeNotes = true;
    }

    public void startGameButton() {
        startCanvas.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        gameCanvas.SetActive(true);
        loadingAudio.SetActive(false);
        gameAudio.SetActive(true);
        gameActive = true;
        timer = 0f;
        if (levelEditor) {
            scmm.StartRecording();
        }
    }

    public void Fail() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (levelEditor) {
            scmm.StopRecording();
            SceneManager.LoadScene("Menu");
            return;
        }
        Scene currentScene = SceneManager.GetActiveScene();
        PlayerPrefs.SetInt("PreviousLevel", currentScene.buildIndex);
        gameActive = false;
        StartCoroutine(LoadFailScene());
        // transitionScript.ToFail();
    }

    public void Win() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameActive = false;
        GameObject foundObject2 = GameObject.Find("Universal_Manager");
        if (foundObject2 != null) {
            Debug.Log("Found Universal_Manager");
            Universal_Manager um = foundObject2.GetComponent<Universal_Manager>();
            um.beatStoryModeLevels[7] = true;
            um.unlockedHard[7] = true;
            PlayerPrefs.SetInt("beatStoryModeLevels8", 1);
            PlayerPrefs.SetInt("unlockedHard8", 1);
            if (score >= 1500) {
                um.level8Get1500 = true;
                PlayerPrefs.SetInt("level8Get1500", 1);
            }
            if (score >= 2000) {
                um.level8Get2000 = true;
                PlayerPrefs.SetInt("level8Get2000", 1);
            }
            if (hard) {
                um.beatHardLevels[7] = true;
                PlayerPrefs.SetInt("beatHardLevels8", 1);
            }

            um.justBeatLevel8 = true;
        } else {
            Debug.Log("No Universal_Manager");
        }
        GameObject foundObject = GameObject.Find("StoryMode");
        // Check if the foundObject is not null
        if (foundObject != null)
        {
            Debug.Log("GameObject '" + "StoryMode" + "' found in the scene.");
            SceneManager.LoadScene("Final Elimination");
            return;
        }
        else
        {
            SceneManager.LoadScene("Menu"); // Not in story mode, goes back to the menu page
            return;
        } 
    }

    public void addScore(int scoreToAdd) {
        score += scoreToAdd;
    }

    public IEnumerator LoadFailScene() {
        GameObject foundObject2 = GameObject.Find("Universal_Manager");
        if (foundObject2 != null) {
            Debug.Log("Found Universal_Manager");
            Universal_Manager um = foundObject2.GetComponent<Universal_Manager>();
            um.justBeatLevel8 = false;
        }
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Final Elimination"); // Change when we have the actual scene
    }
}
