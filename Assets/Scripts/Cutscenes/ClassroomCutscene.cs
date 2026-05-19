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

    [Header("UI")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private CanvasGroup fadeGroup;
    [SerializeField] private CanvasGroup textGroup;

    [Header("Optional RT Display")]
    [SerializeField] private RawImage renderTextureImage;
    [SerializeField] private TMP_Text slideText;

    [Header("Buttons")]
    [SerializeField] private GameObject skipPrompt;

    [Header("Slides")]
    [SerializeField] private List<CutsceneSlide> slides;

    [Header("Timing")]
    [SerializeField] private float typeSpeed = 0.025f;
    [SerializeField] private float fadeDuration = 0.5f;

    private bool skipping = false;

    public void SkipCutscene()
    {
        SceneManager.LoadScene("Open Exploration 4");
    }

    void Start()
    {
        StartCoroutine(PlayCutscene());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)
            || Input.GetKeyDown(KeyCode.Return)
            || Input.GetKeyDown(KeyCode.Escape))
        {
            skipping = true;
        }
    }

    IEnumerator PlayCutscene()
    {
        fadeGroup.alpha = 1f;

        yield return StartCoroutine(FadeIn());

        foreach (CutsceneSlide slide in slides)
        {
            yield return StartCoroutine(PlaySlide(slide));
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

        while (t < slide.holdTime)
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
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            dialogueText.color = new Color(originalTextColor.r, originalTextColor.g, originalTextColor.b, 1f - t / fadeDuration);
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