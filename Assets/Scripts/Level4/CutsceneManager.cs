using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;

public enum CutsceneType { HumanSteal, HumanPass, AISteal, AIPass, HumanDefendSteal, HumanDefendPass }


public class CutsceneManager : MonoBehaviour
{
    [SerializeField] public GameObject cutsceneCanvas;
    [SerializeField] public Button skipButton;
    [SerializeField] public GameObject gameManager;
    public FirstWeLiveWeLoveWeLie gm;
    [SerializeField] public GameObject blackScreen;
    [SerializeField] public TMP_Text quoteText;

    [Header("Cameras")]
    [SerializeField] public GameObject mainCamera;
    [SerializeField] public GameObject cutsceneCamera;

    [Header("Briefcase")]
    [SerializeField] public GameObject briefcase;
    [SerializeField] public GameObject briefcasePivot;

    private Action onComplete;

    void Start() {
        gm = gameManager.GetComponent<FirstWeLiveWeLoveWeLie>();
        blackScreen.SetActive(false);
        quoteText.text = "";
    }

    public void PlayCutscene(CutsceneType type, Action onComplete)
    {
        this.onComplete = onComplete;
        cutsceneCanvas.SetActive(true);
        skipButton.onClick.RemoveAllListeners();
        skipButton.onClick.AddListener(Skip);

        // TODO: trigger animations, text, voiceover depending on `type`
        Debug.Log($"Playing cutscene: {type}");

        if ((type == CutsceneType.HumanSteal) || (type == CutsceneType.AISteal) || (type == CutsceneType.HumanDefendPass)) {
            briefcase.transform.localRotation = Quaternion.Euler(0, 45, 0);
        } else {
            briefcase.transform.localRotation = Quaternion.Euler(0, 225, 0);
        }
        
        // For now, auto-complete after 2 seconds
        if (type == CutsceneType.HumanSteal) {
            StartCoroutine(RevealWhatWeChose());
        } else {
            StartCoroutine(OpenBriefcase(2f));
        }
        // StartCoroutine(AutoEnd(2f));
    }

    public IEnumerator RevealWhatWeChose() {
        blackScreen.SetActive(true);
        quoteText.text = "";
        if (gm.aiLied && gm.defenseCard.Color == CardColor.Red) {
            yield return new WaitForSeconds(3f);
            float duration = 3f;
            float elapsed = 0f;
            while (elapsed < duration) {
                float t = elapsed / duration;

                string chars = "But sometimes, the people you think you can trust the most...";
                int numChars = (int) (chars.Length * t);
                string charsToPut = chars.Substring(0, numChars);
                quoteText.text = charsToPut;
                elapsed += Time.deltaTime;
                yield return null;
            }
            quoteText.text = "But sometimes, the people you think you can trust the most...";
            yield return new WaitForSeconds(3.5f);
            duration = 3f;
            elapsed = 0f;
            while (elapsed < duration) {
                float t = elapsed / duration;

                string chars = "Are actually the people you can trust the least...";
                int numChars = (int) (chars.Length * t);
                string charsToPut = chars.Substring(0, numChars);
                quoteText.text = charsToPut;
                elapsed += Time.deltaTime;
                yield return null;
            }
            quoteText.text = "Are actually the people you can trust the least...";
            yield return new WaitForSeconds(3.5f);
            blackScreen.SetActive(false);
            quoteText.text = "";
        } else {
            yield return new WaitForSeconds(3f);
            float duration = 3f;
            float elapsed = 0f;
            while (elapsed < duration) {
                float t = elapsed / duration;

                string chars = "Your chance to rejoin the game development program depends on this...";
                int numChars = (int) (chars.Length * t);
                string charsToPut = chars.Substring(0, numChars);
                quoteText.text = charsToPut;
                elapsed += Time.deltaTime;
                yield return null;
            }
            quoteText.text = "Your chance to rejoin the game development program depends on this...";
            yield return new WaitForSeconds(3.5f);
            blackScreen.SetActive(false);
            quoteText.text = "";
        }
        StartCoroutine(OpenBriefcase(5f));
        yield return null;
    }

    public IEnumerator OpenBriefcase(float delay)
    {
        
        float duration = delay / 3f;
        float elapsed = 0f;
        Quaternion briefcasePivotStart = briefcasePivot.transform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(90, 0, 0);
        yield return new WaitForSeconds(duration);
        cutsceneCamera.SetActive(true);
        mainCamera.SetActive(false);
        while (elapsed < duration) {
            float t = elapsed / duration;

            briefcasePivot.transform.localRotation = Quaternion.Slerp(briefcasePivotStart, targetRotation, t);

            elapsed += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(duration);
        EndCutscene();
    }

    public IEnumerator AutoEnd(float delay)
    {
        yield return new WaitForSeconds(delay);
        EndCutscene();
    }

    void Skip()
    {
        Debug.Log("Cutscene skipped.");
        gm.SkipToEnd();
    }

    void EndCutscene()
    {
        briefcasePivot.transform.localRotation = Quaternion.Euler(0, 0, 0);
        cutsceneCanvas.SetActive(false);
        cutsceneCamera.SetActive(false);
        mainCamera.SetActive(true);
        onComplete?.Invoke();
    }
}
