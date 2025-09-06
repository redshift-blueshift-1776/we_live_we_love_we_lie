using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialogueCanvas;
    public Button option1, option2, option3;

    private Action<int> onChoice;

    public void ShowDialogueOptions(string[] options, Action<int> callback)
    {
        dialogueCanvas.SetActive(true);
        onChoice = callback;

        option1.GetComponentInChildren<TMP_Text>().text = options[0];
        option2.GetComponentInChildren<TMP_Text>().text = options[1];
        option3.GetComponentInChildren<TMP_Text>().text = options[2];

        option1.onClick.RemoveAllListeners();
        option2.onClick.RemoveAllListeners();
        option3.onClick.RemoveAllListeners();

        option1.onClick.AddListener(() => Choose(0));
        option2.onClick.AddListener(() => Choose(1));
        option3.onClick.AddListener(() => Choose(2));

        Debug.Log("Set up?");
    }

    void Choose(int index)
    {
        dialogueCanvas.SetActive(false);
        onChoice?.Invoke(index);
    }
}

