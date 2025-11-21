using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ImprovedCLSDPreLevel1Cutscene : MonoBehaviour
{
    [System.Serializable]
    public class DualLyricLine {
        public string text;
        public bool rightSide;
    }

    public string[] lyricsText;
    private List<DualLyricLine> lyrics;

    [SerializeField] private TMP_Text lyricsDisplay;
    [SerializeField] private TMP_Text lyricsDisplayRight;
    [SerializeField] private GameObject lyricsBackground;
    [SerializeField] private GameObject lyricsBackgroundRight;
    [SerializeField] private CanvasGroup lyricsBackgroundGroup;
    [SerializeField] private CanvasGroup lyricsBackgroundRightGroup;
    [SerializeField] private float beatsPerMinute = 900f;
    [SerializeField] private GameObject canvas;

    private int currentLine = 0;
    private double nextLyricTime;
    private float secondsPerBeat;
    private Coroutine doLyricsCoroutine;
    private Coroutine fillLyricsCoroutine;
    private Coroutine lineFillCoroutine;
    private bool isAutoPlaying = true;

    [SerializeField] private RectTransform lyricsProgressBarFill;
    private float originalLyricsBarWidth;

    [SerializeField] private RectTransform lyricsLineFillBar;
    [SerializeField] private bool autoStart;

    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float fadeOutDuration = 1f;
    [SerializeField] private float minLineDuration = 2.0f;
    [SerializeField] private float holdDuration = 0.5f;

    [SerializeField] public int nextSceneIndex;

    private bool? lastSideRight = null;

    void Start() {
        lyricsText = new string[] {
            "l:",
            "l:Hello, Professor.",
            "r:Oh, it's you...",
            "l:...",
            "r:Here to sign up for Spectre Games?",
            "l:Yes.",
            "r:I knew you were going to sign up...",
            "l:Oh... Yeah...",
            "r:Lan Attis says that you should be in the game dev program...",
            "r:But there really was no choice after you made that game... Love Sees Differences...",
            "r:I had to kick you out...",
            "l:I did remake it though... and Lan says that it's the best game we've ever made here...",
            "r:But I can't just give you the spot back...",
            "r:So yes, this is the only way back in.",
            "l:And I believe I can do it...",
            "r:It's eight tough challenges, and a thousand people in total. Are you sure you're up to the task?",
            "l:I'm sure of it... As Lan and Lana said, it's time for me to Pick It Up...",
            "r:Ok... go drive your truck to the filming location. We'll start at 9.",
            "r:"
        };
        secondsPerBeat = 60f / beatsPerMinute;
        ParseLyrics();

        nextLyricTime = 0;
        currentLine = 0;
        lyricsDisplay.text = "";
        lyricsDisplayRight.text = "";
        lyricsBackgroundRight.SetActive(false);
        lyricsBackground.SetActive(false);
        canvas.SetActive(false);
        originalLyricsBarWidth = lyricsProgressBarFill.sizeDelta.x;

        if (autoStart) showLyrics();
    }

    void ParseLyrics() {
        lyrics = new List<DualLyricLine>();
        foreach (string line in lyricsText) {
            if (string.IsNullOrWhiteSpace(line)) continue;
            string[] parts = line.Split(':', 2);
            if (parts.Length < 2) continue;

            string side = parts[0].Trim().ToLower();
            string text = parts[1].Trim();
            if (string.IsNullOrEmpty(text)) continue;

            lyrics.Add(new DualLyricLine {
                text = text,
                rightSide = (side == "r")
            });
        }
    }

    public void StopAutoPlay() {
        isAutoPlaying = false;

        if (doLyricsCoroutine != null) StopCoroutine(doLyricsCoroutine);
        if (fillLyricsCoroutine != null) StopCoroutine(fillLyricsCoroutine);
        if (lineFillCoroutine != null) StopCoroutine(lineFillCoroutine);

        doLyricsCoroutine = fillLyricsCoroutine = lineFillCoroutine = null;
        lyricsLineFillBar.sizeDelta = new Vector2(0f, lyricsLineFillBar.sizeDelta.y);
    }

    public void OnNextPressed() {
        StopAutoPlay();
        if (currentLine < lyrics.Count - 1) {
            currentLine++;
            UpdateLyricsDisplay();
        } else {
            ClearLyrics();
        }
    }

    public void OnBackPressed() {
        StopAutoPlay();
        if (currentLine > 0) {
            currentLine--;
            UpdateLyricsDisplay();
        }
    }

    public void OnSkipPressed() {
        StopAutoPlay();
        ClearLyrics();
    }

    public void showLyrics() {
        nextLyricTime = AudioSettings.dspTime;
        currentLine = 0;
        lyricsDisplay.text = "";
        lyricsDisplayRight.text = "";
        canvas.SetActive(true);
        isAutoPlaying = true;

        ResetLyricsProgressBar();

        if (doLyricsCoroutine != null)
            StopCoroutine(doLyricsCoroutine);
        doLyricsCoroutine = StartCoroutine(doLyrics());
    }

    private IEnumerator doLyrics()
    {
        nextLyricTime = AudioSettings.dspTime;
        currentLine = 0;
        lastSideRight = null;

        while (currentLine < lyrics.Count && isAutoPlaying)
        {
            yield return fillLyrics(lyrics[currentLine]);
            currentLine++;
        }

        // Fade everything out and move to next scene
        lyricsDisplay.text = "";
        lyricsDisplayRight.text = "";
        canvas.SetActive(false);

        if (nextSceneIndex >= 0)
            SceneManager.LoadScene(nextSceneIndex);

        doLyricsCoroutine = null;
    }

    private void UpdateLyricsDisplay() {
        if (currentLine < 0 || currentLine >= lyrics.Count) return;

        DualLyricLine line = lyrics[currentLine];
        if (fillLyricsCoroutine != null) StopCoroutine(fillLyricsCoroutine);
        fillLyricsCoroutine = StartCoroutine(fillLyrics(line));

        // UpdateLyricsProgressBar(startLineFill: true);
    }

    private IEnumerator fillLyrics(DualLyricLine line)
    {
        // Progess bar
        // float segmentWidth = originalLyricsBarWidth / (lyrics.Count - 1);
        float segmentWidth = originalLyricsBarWidth / (lyrics.Count);
        float offsetX = segmentWidth * currentLine;
        float duration = Mathf.Max(minLineDuration, line.text.Length * secondsPerBeat);
        UpdateLyricsProgressBar(segmentWidth, offsetX, duration + fadeInDuration + fadeOutDuration);

        TMP_Text target = line.rightSide ? lyricsDisplayRight : lyricsDisplay;
        TMP_Text other = line.rightSide ? lyricsDisplay : lyricsDisplayRight;
        CanvasGroup bgGroup = line.rightSide ? lyricsBackgroundRightGroup : lyricsBackgroundGroup;
        CanvasGroup otherBgGroup = line.rightSide ? lyricsBackgroundGroup : lyricsBackgroundRightGroup;

        bool sameSideAsBefore = lastSideRight.HasValue && lastSideRight.Value == line.rightSide;

        // Background
        if (!sameSideAsBefore)
        {
            otherBgGroup.alpha = 0f;
            otherBgGroup.gameObject.SetActive(false);

            bgGroup.gameObject.SetActive(true);
            bgGroup.alpha = 0f;
            target.alpha = 0f;

            float t = 0f;
            while (t < fadeInDuration)
            {
                float a = t / fadeInDuration;
                bgGroup.alpha = a;
                target.alpha = a;
                t += Time.deltaTime;
                yield return null;
            }
            bgGroup.alpha = 1f;
            target.alpha = 1f;
        }
        else
        {
            // same side → background stays, text fades in quickly
            bgGroup.alpha = 1f;
            target.alpha = 0f;
            float t = 0f;
            while (t < fadeInDuration * 0.5f) // quick text fade only
            {
                float a = t / (fadeInDuration * 0.5f);
                target.alpha = a;
                t += Time.deltaTime;
                yield return null;
            }
            target.alpha = 1f;
        }

        // bad typing animtions
        float elapsed = 0f;
        target.text = "";
        while (elapsed < duration)
        {
            int charsShown = Mathf.Clamp((int)(line.text.Length * (elapsed / duration)), 0, line.text.Length);
            target.text = line.text.Substring(0, charsShown);
            elapsed += Time.deltaTime;
            yield return null;
        }
        target.text = line.text;

        // Hold
        yield return new WaitForSecondsRealtime(holdDuration);

        // Fade
        float ft = 0f;
        while (ft < fadeOutDuration)
        {
            float a = 1f - (ft / fadeOutDuration);
            target.alpha = a;
            if (!sameSideAsBefore)
                bgGroup.alpha = a;
            ft += Time.deltaTime;
            yield return null;
        }

        target.alpha = 0f;
        if (!sameSideAsBefore)
            bgGroup.alpha = 0f;

        target.text = "";
        lastSideRight = line.rightSide;

        // Advance the total progress bar (accumulated fill)
        lyricsProgressBarFill.sizeDelta = new Vector2(offsetX + segmentWidth, lyricsProgressBarFill.sizeDelta.y);
    }


    private void UpdateLyricsProgressBar(float segmentWidth, float offsetX, float duration)
    {
        if (lineFillCoroutine != null)
            StopCoroutine(lineFillCoroutine);

        lineFillCoroutine = StartCoroutine(FillLineBar(duration, segmentWidth, offsetX));
    }

    private IEnumerator FillLineBar(float duration, float segmentWidth, float offsetX)
    {
        // lyricsProgressBarFill.sizeDelta = new Vector2(offsetX, lyricsProgressBarFill.sizeDelta.y);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            lyricsLineFillBar.sizeDelta = new Vector2(segmentWidth * t, lyricsLineFillBar.sizeDelta.y);
            lyricsLineFillBar.anchoredPosition = new Vector2(offsetX, lyricsLineFillBar.anchoredPosition.y);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        lyricsLineFillBar.sizeDelta = new Vector2(segmentWidth, lyricsLineFillBar.sizeDelta.y);
    }

    private void ResetLyricsProgressBar() {
        lyricsProgressBarFill.sizeDelta = new Vector2(0, lyricsProgressBarFill.sizeDelta.y);
    }

    private void ClearLyrics() {
        lyricsDisplay.text = "";
        lyricsDisplayRight.text = "";
        lyricsBackground.SetActive(false);
        lyricsBackgroundRight.SetActive(false);
        canvas.SetActive(false);
        SceneManager.LoadScene(nextSceneIndex);
    }
}