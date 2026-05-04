using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Playables;

public class NewYouWinCutscene : MonoBehaviour
{

    [System.Serializable]
    public class LyricLineWithColor {
        public string text;
        public Color speakerColor;
    }

    [SerializeField] private TMP_Text lyricsDisplay;
    [SerializeField] public GameObject lyricsBackground;
    [SerializeField] public string[] lyricsText;
    private List<LyricLineWithColor> lyrics;
    private int currentLine = 0;

    [SerializeField] public PlayableDirector director;

    public static double dspStartTime;

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
            byte.TryParse(parts[1], out byte color_r);
            byte.TryParse(parts[2], out byte color_g);
            byte.TryParse(parts[3], out byte color_b);
            Color theSpeakerColor = new Color32(color_r, color_g, color_b, 128);
            if (color_r + color_b + color_g == 0)
            {
                theSpeakerColor = new Color32(0, 0, 0, 0);
            }
            lyrics.Add(new LyricLineWithColor { text = parts[0].Trim(), speakerColor = theSpeakerColor });
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lyricsText = new string[]
        {
            ":0:0:0",
            "Congratulations, Wade!:255:255:0",
            "I knew you could do it!:255:0:0",
        };
        ParseLyrics();
        StartCoroutine(StartCutsceneDSP());
    }

    IEnumerator StartCutsceneDSP()
    {
        yield return null;

        dspStartTime = AudioSettings.dspTime + 0.2;

        director.time = 0;
        director.initialTime = 0;

        director.Play();

        while (AudioSettings.dspTime < dspStartTime)
        {
            yield return null;
        }

        director.playableGraph.GetRootPlayable(0).SetSpeed(1);
    }

    // Update is called once per frame
    void Update()
    {
        lyricsDisplay.text = lyrics[currentLine].text;
        Image i = lyricsBackground.GetComponent<Image>();
        i.color = lyrics[currentLine].speakerColor;
    }

    public void increaseCurrentLine() {
        currentLine++;
    }

    public void goToFinalCutscene()
    {
        SceneManager.LoadScene(18);
    }

    
}
