using UnityEngine;

[CreateAssetMenu(fileName = "SongData", menuName = "RhythmGame/SongData")]
public class SongDataSO : ScriptableObject
{
    public string songName;
    public int bpm;
    public int approxLengthInSeconds;
    public string genre;
    public string timeSignature;

    public AudioClip audioClip;
    public Sprite coverArt;

    public float previewStartTime = 0f;
}
