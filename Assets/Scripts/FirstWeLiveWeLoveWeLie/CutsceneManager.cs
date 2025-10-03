using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;

public enum CutsceneType { HumanSteal, HumanPass, AISteal, AIPass, HumanDefendSteal, HumanDefendPass }


public class CutsceneManager : MonoBehaviour
{
    [SerializeField] public GameObject cutsceneCanvas;
    [SerializeField] public Button skipButton;

    [Header("Cameras")]
    [SerializeField] public GameObject mainCamera;
    [SerializeField] public GameObject cutsceneCamera;

    [Header("Briefcase")]
    [SerializeField] public GameObject briefcase;
    [SerializeField] public GameObject briefcasePivot;

    private Action onComplete;

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
        StartCoroutine(OpenBriefcase(2f));
        // StartCoroutine(AutoEnd(2f));
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
        EndCutscene();
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
