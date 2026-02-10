using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class NewYouWinCutscene : MonoBehaviour
{

    [System.Serializable]
    public class LyricLineWithColor {
        public string text;
        public Color speakerColor;
    }

    [SerializeField] private TMP_Text lyricsDisplay;
    [SerializeField] public string[] lyricsText;
    private List<LyricLineWithColor> lyrics;
    private int currentLine = 0;

    void ParseLyrics() {
        lyrics = new List<LyricLineWithColor>();
        string[] lines = lyricsText;

        foreach (string line in lines) {
            if (string.IsNullOrWhiteSpace(line)) {
                continue;
            }
            string[] parts = line.Split(':', 4);
            if (parts.Length < 4) {
                continue;
            }
            float.TryParse(parts[1], out float color_r);
            float.TryParse(parts[2], out float color_g);
            float.TryParse(parts[3], out float color_b);
            Color theSpeakerColor = new Color(color_r, color_g, color_b);
            lyrics.Add(new LyricLineWithColor { text = parts[0].Trim(), speakerColor = theSpeakerColor });
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lyricsDisplay.text = lyrics[currentLine].text;
    }

    public void increaseCurrentLine() {
        currentLine++;
    }

    public void goToFinalCutscene()
    {
        SceneManager.LoadScene(18);
    }

    
}
