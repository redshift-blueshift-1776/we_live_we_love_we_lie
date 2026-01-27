using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OMGLyricsSyncDisplay : MonoBehaviour
{
    [System.Serializable]
    public class LyricLine {
        public float duration; // In beats
        public string text;
    }

    public string[] lyricsText;
    public List<LyricLine> lyrics;

    void ParseLyrics() {
        lyrics = new List<LyricLine>();
        string[] lines = lyricsText;

        foreach (string line in lines) {
            if (string.IsNullOrWhiteSpace(line)) continue;
            string[] parts = line.Split(':', 2);
            if (parts.Length < 2) continue;

            if (float.TryParse(parts[0], out float duration)) {
                lyrics.Add(new LyricLine { duration = duration, text = parts[1].Trim() });
            }
        }
    }

    [SerializeField] private GameObject gameAudio;
    [SerializeField] private TMP_Text lyricsDisplay;
    [SerializeField] private float beatsPerMinute = 120f;

    private AudioSource audioSource;
    private int currentLine = 0;
    private double nextLyricTime;
    private float secondsPerBeat;


    // Start is called before the first frame update
    void Start() {
        lyricsText = new string[] {
            "31:",
"8:I was lost in the abyss",
"8:The signs I really missed",
"8:You're the one that I should see",
"8:The goddess built for me",

"16:You're the color and you hover in my mind fit for you",
"16:You fix broken pieces chosen by a great mind, make it two",

"8:When I saw you at this school",
"8:I thought that you were cool",
"8:Now a cycle has been done",
"8:The two of us are one.",

"9:I say O-o-o-o-o-o-o-o-oh.",
"8:O-o-o-o-o-o-o-o-oh.",
"8:O-o-o-o-o-o-o-o-oh.",
"8:O-o-o-o-o-o-o-o-oh.",

"63:",
"8:When we go to any place,",
"8:My thoughts fill up the space.",
"8:When I look into your eyes,",
"8:The sadness in me dies.",

"16:You make any moment plenty joyful when I'm with you.",
"16:Oh My Goodness, when you're hoodless, I see why I grow with you.",

"8:If you ever need to cry,",
"8:Iʼll be there by your side.",
"8:I can make you full of joy,",
"8:Your sadness be destroyed.",

"9:I say O-o-o-o-o-o-o-o-oh.",
"8:O-o-o-o-o-o-o-o-oh.",
"8:O-o-o-o-o-o-o-o-oh.",
"8:O-o-o-o-o-o-o-o-oh.",
        };
        audioSource = gameAudio.GetComponent<AudioSource>();
        secondsPerBeat = 60f / beatsPerMinute;
        ParseLyrics();
        
        // Don't start the lyrics until the audio actually starts playing
        nextLyricTime = 0;
        currentLine = 0;
        lyricsDisplay.text = ""; // Start with an empty display
    }

    // Update is called once per frame
    void Update() {
        if (!audioSource.isPlaying) {
            // If the audio isn't playing, reset and show nothing
            currentLine = 0;
            nextLyricTime = 0;
            lyricsDisplay.text = "";
            return;
        }

        if (nextLyricTime == 0) {
            // Sync with the exact DSP time when the audio starts playing
            // nextLyricTime = AudioSettings.dspTime - 0.5;
            nextLyricTime = AudioSettings.dspTime;
        }

        if (currentLine < lyrics.Count && AudioSettings.dspTime >= nextLyricTime) {
            lyricsDisplay.text = lyrics[currentLine].text;
            nextLyricTime += lyrics[currentLine].duration * secondsPerBeat;
            currentLine++;
        }
    }

}
