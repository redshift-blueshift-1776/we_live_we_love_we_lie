using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;

public enum CutsceneType { HumanSteal, HumanPass, AISteal, AIPass, HumanDefend }


public class CutsceneManager : MonoBehaviour
{
    [SerializeField] public GameObject cutsceneCanvas;
    [SerializeField] public Button skipButton;

    private Action onComplete;

    public void PlayCutscene(CutsceneType type, Action onComplete)
    {
        this.onComplete = onComplete;
        cutsceneCanvas.SetActive(true);
        skipButton.onClick.RemoveAllListeners();
        skipButton.onClick.AddListener(Skip);

        // TODO: trigger animations, text, voiceover depending on `type`
        Debug.Log($"Playing cutscene: {type}");
        
        // For now, auto-complete after 2 seconds
        StartCoroutine(AutoEnd(2f));
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
        cutsceneCanvas.SetActive(false);
        onComplete?.Invoke();
    }
}
