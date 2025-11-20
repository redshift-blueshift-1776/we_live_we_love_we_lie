using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class AnimationSyncDisplay : MonoBehaviour
{

    [SerializeField] public Texture[] images;
    [SerializeField] public int[] durations;
    [SerializeField] public int nextSceneNumber;

    [SerializeField] private RawImage imageDisplay;

    [SerializeField] private GameObject canvas;

    private int currentLine = 0;
    private double nextImageTime;
    private float secondsPerBeat;

    private Coroutine doAnimationCoroutine;
    private bool isAutoPlaying = true;

    [SerializeField] private RectTransform animationProgressBarFill;
    private float originalAnimationBarWidth;

    [SerializeField] private RectTransform animationLineFillBar;


    // Start is called before the first frame update
    void Start() {
        secondsPerBeat = 0.1f;
        
        // Don't start the animations until the audio actually starts playing
        nextImageTime = 0;
        currentLine = 0;
        imageDisplay.texture = images[0];
        canvas.SetActive(false);
        originalAnimationBarWidth = animationProgressBarFill.sizeDelta.x;

        showLyrics();
    }

    // Update is called once per frame
    void Update() {

    }

    public void StopAutoPlay() {
        isAutoPlaying = false;

        if (doAnimationCoroutine != null) {
            StopCoroutine(doAnimationCoroutine);
            doAnimationCoroutine = null;
        }
        if (lineFillCoroutine != null)
            StopCoroutine(lineFillCoroutine);

        animationLineFillBar.sizeDelta = new Vector2(0f, animationLineFillBar.sizeDelta.y);

    }

    public void OnNextPressed() {
        if (currentLine < images.Length - 1) {
            if (!isAutoPlaying) {
                currentLine++;
            }
            StopAutoPlay();
            UpdateAnimationDisplay();
        } else {
            // Reached the end manually – treat like skip
            StopAutoPlay();
            // lyricsDisplay.text = "";
            canvas.SetActive(false);
            doAnimationCoroutine = null;
            SceneManager.LoadScene(nextSceneNumber);
        }
        if (currentLine >= images.Length - 1) {
            // lyricsDisplay.text = "";
            canvas.SetActive(false);
            doAnimationCoroutine = null;
            SceneManager.LoadScene(nextSceneNumber);
        }
    }

    public void OnBackPressed() {
        if (currentLine > 0) {
            currentLine--;
            if (isAutoPlaying) {
                currentLine--;
            }
            StopAutoPlay();
            UpdateAnimationDisplay();
        }
    }

    public void OnSkipPressed() {
        //StopCoroutine(doLyricsCoroutine);
        // lyricsDisplay.text = "";
        canvas.SetActive(false);
        SceneManager.LoadScene(nextSceneNumber);
    }

    public void showLyrics() {
        nextImageTime = 0;
        currentLine = 0;
        // lyricsDisplay.text = "";
        canvas.SetActive(true);
        isAutoPlaying = true;

        ResetAnimationProgressBar();

        if (doAnimationCoroutine != null)
            StopCoroutine(doAnimationCoroutine);

        doAnimationCoroutine = StartCoroutine(doAnimation());
        // doProgressCoroutine = StartCoroutine(progressBar());
    }

    public IEnumerator doAnimation() {
        if (nextImageTime == 0) {
            nextImageTime = AudioSettings.dspTime;
        }

        while (currentLine < images.Length && isAutoPlaying) {
            double currentTime = AudioSettings.dspTime;
            double waitTime = nextImageTime - currentTime;

            if (waitTime > 0)
                yield return new WaitForSecondsRealtime((float)waitTime);

            UpdateAnimationDisplay();
            UpdateAnimationProgressBar(startLineFill: true);
            nextImageTime += durations[currentLine] * secondsPerBeat;
            currentLine++;
        }

        if (isAutoPlaying) {
            // lyricsDisplay.text = "";
            canvas.SetActive(false);
            SceneManager.LoadScene(nextSceneNumber);
        }

        doAnimationCoroutine = null;
    }

    private void UpdateAnimationDisplay() {
        if (currentLine >= 0 && currentLine < images.Length) {
            imageDisplay.texture = images[currentLine];
        }
        UpdateAnimationProgressBar();
    }

    private void UpdateAnimationProgressBar(bool startLineFill = false) {
        float percentage = (float)currentLine / (images.Length - 1);
        float barWidth = percentage * originalAnimationBarWidth;

        animationProgressBarFill.sizeDelta = new Vector2(barWidth, animationProgressBarFill.sizeDelta.y);

        if (startLineFill && currentLine < images.Length) {
            float segmentWidth = originalAnimationBarWidth / (images.Length - 1);
            float offsetX = segmentWidth * currentLine;

            if (lineFillCoroutine != null)
                StopCoroutine(lineFillCoroutine);

            float lineDuration = durations[currentLine] * secondsPerBeat;

            // Reset the fill width to 0
            animationLineFillBar.sizeDelta = new Vector2(0f, animationLineFillBar.sizeDelta.y);

            lineFillCoroutine = StartCoroutine(FillLineBar(lineDuration, segmentWidth, offsetX));
        }
    }

    private void ResetAnimationProgressBar() {
        animationProgressBarFill.sizeDelta = new Vector2(0, animationProgressBarFill.sizeDelta.y);
    }

    private Coroutine lineFillCoroutine;

    private IEnumerator FillLineBar(float duration, float segmentWidth, float offsetX) {
        float elapsed = 0f;

        while (elapsed < duration) {
            float t = elapsed / duration;
            animationLineFillBar.sizeDelta = new Vector2(segmentWidth * t, animationLineFillBar.sizeDelta.y);
            animationLineFillBar.anchoredPosition = new Vector2(offsetX, animationLineFillBar.anchoredPosition.y);
            elapsed += Time.unscaledDeltaTime;  // Use unscaled for Realtime
            yield return null;
        }

        // Snap to full at the end
        animationLineFillBar.sizeDelta = new Vector2(segmentWidth, animationLineFillBar.sizeDelta.y);
    }

}
