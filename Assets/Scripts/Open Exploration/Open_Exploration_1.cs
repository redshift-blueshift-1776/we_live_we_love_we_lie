using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

public class Open_Exploration_1 : MonoBehaviour
{
    [SerializeField] public GameObject FadeLightpole;
    [SerializeField] public GameObject FadeLightpoleReference;
    [SerializeField] public int offset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < 5; i++) {
            GameObject newFadeLightpole = Instantiate(FadeLightpole);
            newFadeLightpole.transform.SetParent(FadeLightpoleReference.transform);
            newFadeLightpole.transform.localPosition = new Vector3(10, -0.01f, -i * offset);
            newFadeLightpole = Instantiate(FadeLightpole);
            newFadeLightpole.transform.SetParent(FadeLightpoleReference.transform);
            newFadeLightpole.transform.localPosition = new Vector3(-10, -0.01f, -i * offset);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
