using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField] public GameObject doorOpenSound;
    [SerializeField] public GameObject hinge;
    public bool doorOpened;
    [SerializeField] public bool side;
    public float originalYRotation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        doorOpened = false;
        doorOpenSound.SetActive(false);
        originalYRotation = hinge.transform.rotation.eulerAngles.y;
        hinge.transform.rotation = Quaternion.Euler(0, originalYRotation, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact() {
        Debug.Log("Interacting");
        doorOpenSound.SetActive(false);
        doorOpenSound.SetActive(true);
        doorOpened = !doorOpened;
        if (doorOpened) {
            hinge.transform.rotation = Quaternion.Euler(0, originalYRotation + (side ? 90f : -90f), 0);
        } else {
            hinge.transform.rotation = Quaternion.Euler(0, originalYRotation, 0);
        }
    }
}
