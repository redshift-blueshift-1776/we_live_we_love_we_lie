using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class SimpleCustomMapMaker : MonoBehaviour
{
    public List<int> tapBeats = new List<int>();
    public float startTime;

    public bool recording;
    public bool doneRecording;

    [SerializeField] public int bpm = 120;
    public int scatterAmount = 2;

    private readonly KeyCode mainKey = KeyCode.Space;

    public BeatManager beatManager;
    public double dspStartTime = -1;

    private SongDataSO[] songs;

    private string songName;

    [SerializeField] public AudioSource songToPlay;

    void Start() {
        beatManager = BeatManager.Instance;
        songs = Resources.LoadAll<SongDataSO>("Songs");
        songName = PlayerPrefs.GetString("SelectedSong", "UNKNOWN");
        foreach (SongDataSO song in songs) {
            if (song.songName == songName) {
                bpm = song.bpm;
                songToPlay.clip = song.audioClip;
            }
        }
    }

    void Update()
    {
        if (recording)
        {
            if (dspStartTime == -1) {
                dspStartTime = beatManager.StartDspTime;
            }
            if (Input.GetKeyDown(mainKey) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
            {
                RegisterNote();
            }
        }
    }

    public void StartRecording()
    {
        tapBeats.Clear();
        startTime = Time.time;
        recording = true;
        doneRecording = false;

        Debug.Log("Started recording.");
    }

    // Call this from UI
    public void StopRecording()
    {
        recording = false;
        doneRecording = true;
        SaveToJson();
    }

    void RegisterNote()
    {
        double songDspTime = AudioSettings.dspTime - dspStartTime;

        float currentBeat = (float)(songDspTime / beatManager.secondsPerBeat);

        // snap to nearest 16th note, assuming that the tempo is set at the actual tempo
        // float snappedBeat = Mathf.Round(currentBeat * 4f) / 4f;
        int snappedBeat = (int) Mathf.Round(currentBeat * 4f);

        // float x = Random.Range(-scatterAmount, scatterAmount + 1);
        // float y = Random.Range(-scatterAmount, scatterAmount + 1);

        tapBeats.Add(snappedBeat);
    }


    void SaveToJson()
    {
        SimpleMapData map = new SimpleMapData();
        map.mapType = "simple";
        map.songName = songName;
        map.bpm = bpm;
        map.msPerSixteenth = (60000f / bpm) / 4f;

        foreach (int beat in tapBeats)
        {
            int x = Random.Range(-scatterAmount, scatterAmount + 1);
            int y = Random.Range(-scatterAmount, scatterAmount + 1);

            map.notes.Add(new NoteData(beat, x, y));
        }

        string json = JsonUtility.ToJson(map, true);

        string fileName = "SimpleMap_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".json";
        string path = Path.Combine(Application.persistentDataPath, fileName);

        File.WriteAllText(path, json);

        Debug.Log("Saved map to: " + path);
    }
}