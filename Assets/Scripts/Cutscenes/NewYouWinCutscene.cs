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
            "I always believed in you, Wade!:255:0:255",
            "We'll make even more great games!:32:0:82",
            "I'll be honest, the new LSD is really fun...:255:255:0",
            "Even without an artist.:255:255:0",
            "As I said, I knew you deserved to get back in.:255:0:0",
            "I look forward to your next game.:255:0:0",
            "Your first game back.:255:0:0",
            "Even if you don't have an artist...:255:0:0",
            "I mean, I can be the artist...:255:0:255",
            "Welcome back to the game dev program!:32:0:82",
            "I do truly welcome you back!:255:255:0",
            "Congratulations again!:255:255:0",
            "Now let's celebrate!:255:0:0",
            "Stick, Stick, Stickity Stack!:255:0:255",
            "Stick, Stick, Stick, Stick, Stickity Stack!:255:0:255",
            "Stick, Stick, Stickity Stack!:255:0:255",
            "Stick, Stick, Stick, Stick, Stickity Stack!:255:0:255",
            "Stick, Stick, Stickity Stack!:255:0:255",
            "Stick, Stick, Stick, Stick, Stickity Stack!:255:0:255",
            "Stick, Stick, Stickity Stack!:255:0:255",
            "Stick, Stick, Stick, Stick, Stickity Stack!:255:0:255",
            "Stick, Stick, Stickity Stack!:32:0:82",
            "Stick, Stick, Stick, Stick, Stickity Stack!:32:0:82",
            "Stick, Stick, Stickity Stack!:32:0:82",
            "Stick, Stick, Stick, Stick, Stickity Stack!:32:0:82",
            "Stick, Stick, Stickity Stack!:32:0:82",
            "Stick, Stick, Stick, Stick, Stickity Stack!:32:0:82",
            "Stick, Stick, Stickity Stack!:32:0:82",
            "Stick, Stick, Stick, Stick, Stickity Stack!:32:0:82",
            "Stick, Stick, Stickity Stack!:255:0:255",
            "Stick, Stick, Stick, Stick, Stickity Stack!:255:0:255",
            "Stick, Stick, Stickity Stack!:255:0:0",
            "Stick, Stick, Stick, Stick, Stickity Stack!:255:0:0",
            "Stick, Stick, Stickity Stack!:32:0:82",
            "Stick, Stick, Stick, Stick, Stickity Stack!:32:0:82",
            "Stick, Stick, Stickity Stack!:255:255:0",
            "Stick, Stick, Stick, Stick, Stickity Stack!:255:255:0",
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
