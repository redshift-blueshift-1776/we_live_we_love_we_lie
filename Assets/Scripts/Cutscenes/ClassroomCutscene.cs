using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class ClassroomCutscene : MonoBehaviour
{
    [System.Serializable]
    public class CutsceneSlide
    {
        [TextArea(3, 8)]
        public string dialogue;

        public string speaker;

        public Color speakerColor = Color.white;

        [Header("Visuals")]
        // public Sprite backgroundSprite;
        // public RawImage renderTextureDisplay;
        public Texture renderTexture;
        [TextArea(3, 8)]
        public string slideText;

        [Header("Timing")]
        public float holdTime = 2f;

        [Header("Effects")]
        public bool shake;
        public bool slowZoom;
    }

    [System.Serializable]
    public class LyricLineWithColor {
        public string text;
        public Color speakerColor;
    }

    [Header("UI")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private CanvasGroup fadeGroup;
    [SerializeField] private CanvasGroup textGroup;

    [Header("Optional RT Display")]
    [SerializeField] private RawImage renderTextureImage;
    [SerializeField] private TMP_Text slideText;
    [SerializeField] private GameObject rtCamera;
    [SerializeField] private GameObject rtCameraPivot;

    [Header("Buttons")]
    [SerializeField] private GameObject skipPrompt;

    [Header("Slides and Lyrics")]
    [SerializeField] private List<CutsceneSlide> slides;
    [SerializeField] private string[] lyricsText;
    public List<LyricLineWithColor> lyrics;
    private int currentLine = 0;

    [Header("Timing")]
    [SerializeField] private float typeSpeed = 0.025f;
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("Cameras")]
    [SerializeField] private GameObject cam1;
    [SerializeField] private GameObject cam2;

    private bool skipping = false;

    public void SkipCutscene()
    {
        SceneManager.LoadScene("Open Exploration 4");
    }

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

    void Start()
    {
        lyricsText = new string[]
        {
            "Wade, meet me in the library after the next class...:255:0:255",
            "We're rooting for you. You can do this.:255:0:255",
            "I can do this...:136:247:255",
            "They support me...:136:247:255",
        };
        ParseLyrics();
        StartCoroutine(PlayCutscene());
    }

    void Update()
    {
        rtCameraPivot.transform.Rotate(0, Time.deltaTime * 3f, 0);
        if (Input.GetKeyDown(KeyCode.Space)
            || Input.GetKeyDown(KeyCode.Return)
            || Input.GetKeyDown(KeyCode.Escape))
        {
            skipping = true;
        }
    }

    IEnumerator PlayCutscene()
    {
        cam1.SetActive(true);
        cam2.SetActive(false);
        fadeGroup.alpha = 1f;

        yield return StartCoroutine(FadeIn());

        foreach (CutsceneSlide slide in slides)
        {
            yield return StartCoroutine(PlaySlide(slide));
        }

        cam1.SetActive(false);
        cam2.SetActive(true);

        speakerNameText.gameObject.SetActive(false);

        currentLine = 0;

        while (currentLine < lyrics.Count)
        {
            yield return FillLyrics(lyrics[currentLine]);
            currentLine++;
        }

        yield return StartCoroutine(FadeOut());

        SceneManager.LoadScene("Open Exploration 4");
    }

    IEnumerator PlaySlide(CutsceneSlide slide)
    {
        textGroup.alpha = 1f;
        skipping = false;

        speakerNameText.text = slide.speaker;
        backgroundImage.color = new Color(slide.speakerColor.r, slide.speakerColor.g, slide.speakerColor.b, 0.5f);

        dialogueText.text = "";
        dialogueText.alpha = 1f;

        slideText.text = slide.slideText;

        if (slide.renderTexture != null)
        {
            renderTextureImage.gameObject.SetActive(true);
            renderTextureImage.texture = slide.renderTexture;
        }
        else
        {
            renderTextureImage.gameObject.SetActive(false);
        }

        yield return StartCoroutine(TypeDialogue(slide.dialogue));

        float t = 0f;

        Vector2 originalPosition = backgroundImage.rectTransform.localPosition;

        while (t < slide.holdTime / 2f)
        {
            if (skipping)
            {
                break;
            }
            
            t += Time.deltaTime;

            if (slide.shake)
            {
                backgroundImage.rectTransform.localPosition = originalPosition + Random.insideUnitCircle * 5f;
            }

            if (slide.slowZoom)
            {
                backgroundImage.rectTransform.localScale +=
                    0.01f * Time.deltaTime * Vector3.one;
            }

            yield return null;
        }

        Color originalTextColor = dialogueText.color;

        t = 0f;
        float totalFadeDuration = fadeDuration + slide.holdTime / 2f;
        while (t < totalFadeDuration)
        {
            t += Time.deltaTime;
            dialogueText.color = new Color(originalTextColor.r, originalTextColor.g, originalTextColor.b, 1f - t / totalFadeDuration);
            yield return null;
        }
        dialogueText.alpha = 0f;

        backgroundImage.rectTransform.localPosition = originalPosition;
        backgroundImage.rectTransform.localScale = Vector3.one;
    }

    IEnumerator TypeDialogue(string line)
    {
        for (int i = 0; i <= line.Length; i++)
        {
            if (skipping)
            {
                dialogueText.text = line;
                yield break;
            }

            dialogueText.text = line[..i];

            yield return new WaitForSeconds(typeSpeed);
        }
    }

    private IEnumerator FillLyrics(LyricLineWithColor llwc)
    {
        string line = llwc.text;
        Color llwcColor = llwc.speakerColor;
        backgroundImage.color = new Color(llwcColor.r, llwcColor.g, llwcColor.b, 0.5f);

        TMP_Text target = dialogueText;

        // bad typing animtions
        float elapsed = 0f;
        float duration = 3f;
        target.text = "";
        target.alpha = 1f;
        while (elapsed < duration)
        {
            int charsShown = Mathf.Clamp((int)(line.Length * (elapsed / duration)), 0, line.Length);
            target.text = line.Substring(0, charsShown);
            elapsed += Time.deltaTime;
            yield return null;
        }
        target.text = line;

        // Hold
        yield return new WaitForSecondsRealtime(1f);

        // Fade
        float ft = 0f;
        while (ft < fadeDuration)
        {
            float a = 1f - (ft / fadeDuration);
            target.alpha = a;
            ft += Time.deltaTime;
            yield return null;
        }

        target.alpha = 0f;

        target.text = "";
    }

    IEnumerator FadeIn()
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            fadeGroup.alpha = 1f - (t / fadeDuration);

            t += Time.deltaTime;

            yield return null;
        }

        fadeGroup.alpha = 0f;
    }

    IEnumerator FadeOut()
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            fadeGroup.alpha = t / fadeDuration;

            t += Time.deltaTime;

            yield return null;
        }

        fadeGroup.alpha = 1f;
    }
}