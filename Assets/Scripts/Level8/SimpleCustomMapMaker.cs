using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class SimpleCustomMapMaker : MonoBehaviour
{
    [Header("Recording Output")]
    public List<int> tapBeats = new List<int>();

    [Header("Recording Control Flags")]
    public bool recording;
    public bool doneRecording;

    [Header("Song Settings")]
    [SerializeField] public int bpm = 120;
    public int scatterAmount = 2;

    [Header("Audio")]
    [SerializeField] public AudioSource songToPlay;

    private SongDataSO[] songs;
    private string songName;
    private readonly KeyCode mainKey = KeyCode.Space;

    // Timing
    private bool hasStartedSong = false;
    private double scheduledStartDSP = -1;
    private float secondsPerBeat;

    void Start()
    {
        // Load song data
        songs = Resources.LoadAll<SongDataSO>("Songs");
        songName = PlayerPrefs.GetString("SelectedSong", "UNKNOWN");

        foreach (SongDataSO song in songs)
        {
            if (song.songName == songName)
            {
                bpm = song.bpm;
                songToPlay.clip = song.audioClip;
            }
        }

        secondsPerBeat = 60f / bpm;
    }

    void Update()
    {
        if (!recording) return;

        if (!hasStartedSong)
        {
            hasStartedSong = true;

            scheduledStartDSP = AudioSettings.dspTime + 0.15f;
            songToPlay.PlayScheduled(scheduledStartDSP);

            Debug.Log($"Song scheduled for DSP time {scheduledStartDSP:F6}");
        }

        if (Input.GetKeyDown(mainKey) ||
            Input.GetKeyDown(KeyCode.A) ||
            Input.GetKeyDown(KeyCode.D))
        {
            RegisterNote();
        }
    }

    public void StartRecording()
    {
        tapBeats.Clear();
        recording = true;
        doneRecording = false;
        hasStartedSong = false;

        Debug.Log("Started recording map.");
    }

    public void StopRecording()
    {
        recording = false;
        doneRecording = true;

        SaveToJson();
    }

    private void RegisterNote()
    {
        int sampleRate = AudioSettings.outputSampleRate;

        float songTime =
            (songToPlay != null && songToPlay.isPlaying)
            ? (float)songToPlay.timeSamples / sampleRate
            : 0f;

        float beatFloat = songTime / secondsPerBeat;

        // Sixteenth note quantization
        int snappedBeat = Mathf.RoundToInt(beatFloat * 4f);

        tapBeats.Add(snappedBeat);

        Debug.Log(
            $"RegisterNote: samples={songToPlay.timeSamples}, " +
            $"songTime={songTime:F4}, beat={beatFloat:F4}, snapped={snappedBeat}");
    }

    private void SaveToJson()
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