using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatManager : MonoBehaviour {
    public static BeatManager Instance;
    
    public AudioSource audioSource;
    public double secondsPerBeat;

    [SerializeField] public float tempo = 120f; // Tempo of the song used

    public double StartDspTime { get; private set; }

    void Awake() {
        Instance = this;
    }

    void Start() {
        // Sync when audio starts
        StartDspTime = AudioSettings.dspTime;
        secondsPerBeat = 60f / tempo;

        Debug.Log("Beat Manager start");
    }

    public double GetNextBeatTime() {
        double timeSinceStart = AudioSettings.dspTime - StartDspTime;
        int beatsPassed = Mathf.FloorToInt((float)(timeSinceStart / secondsPerBeat));
        return StartDspTime + (beatsPassed + 1) * secondsPerBeat;
    }

    public int GetCurrentBeatNumber() {
        //return Mathf.FloorToInt((float)((AudioSettings.dspTime - StartDspTime) / secondsPerBeat));

        double elapsed = AudioSettings.dspTime - StartDspTime;
        if (elapsed <= 0) return 0; // Audio hasn¡¯t started yet
        return (int) Mathf.Clamp(Mathf.FloorToInt((float)(elapsed / secondsPerBeat)), 0f, Mathf.Infinity);
    }
}