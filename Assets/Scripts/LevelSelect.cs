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

    public bool[][] unlocked;

    public int currentPage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        easyButtons[0].SetActive(true);
        hardButtons[0].SetActive(true);
        endlessButtons[0].SetActive(true);
        for (int i = 0; i < 8; i++) {
            easyButtons[i].SetActive(unlocked[i][0]);
            hardButtons[i].SetActive(unlocked[i][1]);
            endlessButtons[i].SetActive(unlocked[i][2]);
        }
        currentPage = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
