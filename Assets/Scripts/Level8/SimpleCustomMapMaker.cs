using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class SimpleCustomMapMaker : MonoBehaviour
{
    public List<int> tapTimes = new List<int>();
    public float startTime;

    public bool recording;
    public bool doneRecording;

    [SerializeField] public int bpm = 120;
    public int scatterAmount = 2;

    private readonly KeyCode mainKey = KeyCode.Space;

    void Update()
    {
        if (recording)
        {
            if (Input.GetKeyDown(mainKey) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
            {
                RegisterNote();
            }
        }
    }

    public void StartRecording()
    {
        tapTimes.Clear();
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
        int rawMs = Mathf.RoundToInt((Time.time - startTime) * 1000f);
        tapTimes.Add(rawMs);
        Debug.Log("Tap at: " + rawMs);
    }

    void SaveToJson()
    {
        SimpleMapData map = new SimpleMapData();
        map.bpm = bpm;
        map.msPerSixteenth = (60000f / bpm) / 4f;

        foreach (int rawTime in tapTimes)
        {
            int snapped = Mathf.RoundToInt(rawTime / map.msPerSixteenth) * (int)map.msPerSixteenth;

            int x = Random.Range(-scatterAmount, scatterAmount + 1);
            int y = Random.Range(-scatterAmount, scatterAmount + 1);

            map.notes.Add(new NoteData(snapped, x, y));
        }

        string json = JsonUtility.ToJson(map, true);

        string fileName = "SimpleMap_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".json";
        string path = Path.Combine(Application.persistentDataPath, fileName);

        File.WriteAllText(path, json);

        Debug.Log("Saved map to: " + path);
    }
}