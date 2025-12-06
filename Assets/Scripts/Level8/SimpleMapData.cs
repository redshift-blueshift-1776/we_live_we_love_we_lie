using System;
using System.Collections.Generic;

[Serializable]
public class SimpleMapData
{
    public string mapType = "simple";  // for the future
    public string songName;
    public int bpm;
    public float msPerSixteenth;
    public List<NoteData> notes = new List<NoteData>();
}

[Serializable]
public class NoteData
{
    public int beat;
    public float x;
    public float y;

    public NoteData(int time, float x, float y)
    {
        this.beat = time;
        this.x = x;
        this.y = y;
    }
}
