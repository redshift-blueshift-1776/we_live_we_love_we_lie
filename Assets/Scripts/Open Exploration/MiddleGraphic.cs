using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MiddleGraphic : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text rankText;
    public TMP_Text titleText;
    public TMP_Text authorText;
    public RawImage gameImage;
    public CanvasGroup canvasGroup;

    [Header("Animation Settings")]
    public float fadeInTime = 0.4f;
    public float holdTime = 1.2f;
    public float fadeOutTime = 0.4f;

    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>(); // Might not be implemented yet
        if (!canvasGroup) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    // Sets all text fields and the image on the middle reveal panel.
    public void SetData(string gameString, Texture img, int rank)
    {
        // gameString will be something like:
        // "LSD:Wade Zunic"
        // Split into title + authors
        string[] parts = gameString.Split(':');
        string title = parts.Length > 0 ? parts[0] : "UNKNOWN";
        string authors = parts.Length > 1 ? parts[1] : "";

        rankText.text = $"#{rank}";
        titleText.text = title;
        authorText.text = authors;
        gameImage.texture = img;
    }

    // Plays the reveal animation. If no animator is present, uses a coroutine fallback.
    public void PlayReveal()
    {
        if (anim != null)
        {
            anim.SetTrigger("Reveal");
        }
        else
        {
            StartCoroutine(FadeRoutine());
        }
    }

    // Fallback smooth fade if no Animator exists.
    public IEnumerator FadeRoutine()
    {
        canvasGroup.alpha = 0;

        // Fade in
        float t = 0;
        while (t < fadeInTime)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeInTime);
            yield return null;
        }

        // Hold
        yield return new WaitForSeconds(holdTime);

        // Fade out
        t = 0;
        while (t < fadeOutTime)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeOutTime);
            yield return null;
        }

        Destroy(gameObject);
    }

    // Called by the Animator at the end of the reveal animation.
    public void OnRevealFinished()
    {
        Destroy(gameObject);
    }
}