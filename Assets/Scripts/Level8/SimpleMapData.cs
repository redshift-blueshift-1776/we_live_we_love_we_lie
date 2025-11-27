using System;
using System.Collections.Generic;

[Serializable]
public class SimpleMapData
{
    public string mapType = "simple";  // for the future
    public int bpm;
    public float msPerSixteenth;
    public List<NoteData> notes = new List<NoteData>();
}

[Serializable]
public class NoteData
{
    public int time;
    public int x;
    public int y;

    public NoteData(int time, int x, int y)
    {
        this.time = time;
        this.x = x;
        this.y = y;
    }
}
