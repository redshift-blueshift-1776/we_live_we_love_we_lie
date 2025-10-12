using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] public GameObject[] easyButtons;
    [SerializeField] public GameObject[] hardButtons;
    [SerializeField] public GameObject[] endlessButtons;
    [SerializeField] public GameObject currentBackground;
    [SerializeField] public GameObject leftBackground;
    [SerializeField] public GameObject rightBackground;

    [SerializeField] public bool[] unlockedEasy;
    [SerializeField] public bool[] unlockedHard;
    [SerializeField] public bool[] unlockedEndless;

    public int currentPage;

    void Awake() { }

    void Start()
    {
        currentPage = 0;

        // Safety checks — prevent null/short arrays
        if (easyButtons == null || easyButtons.Length < 8 ||
            hardButtons == null || hardButtons.Length < 8 ||
            endlessButtons == null || endlessButtons.Length < 8)
        {
            Debug.LogError("LevelSelectManager: One or more button arrays are not properly assigned in the Inspector!");
            return;
        }

        if (unlockedEasy == null || unlockedEasy.Length < 8)
            unlockedEasy = new bool[8];
        if (unlockedHard == null || unlockedHard.Length < 8)
            unlockedHard = new bool[8];
        if (unlockedEndless == null || unlockedEndless.Length < 8)
            unlockedEndless = new bool[8];

        for (int i = 0; i < 8; i++)
        {
            easyButtons[i].SetActive(true);
            hardButtons[i].SetActive(true);
            endlessButtons[i].SetActive(true);

            easyButtons[i].transform.localPosition = new Vector3(-420, -420, 0);
            hardButtons[i].transform.localPosition = new Vector3(0, -420, 0);
            endlessButtons[i].transform.localPosition = new Vector3(420, -420, 0);

            easyButtons[i].SetActive(unlockedEasy[i]);
            hardButtons[i].SetActive(unlockedHard[i]);
            endlessButtons[i].SetActive(unlockedEndless[i]);
        }

        easyButtons[0].SetActive(true);
    }


}
