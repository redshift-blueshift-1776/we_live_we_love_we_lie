using UnityEngine;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class GenerateWorld : MonoBehaviour
{
    [SerializeField] public GameObject Level1Reference;
    [SerializeField] public GameObject Pillar;

    private double secondsPerBeat;

    private double nextChangeTime;

    public bool doingStuff;

    private int initialBeat = -1;

    public int phase = 0;

    private Color spectreColor = new Color(127f / 255f, 224f / 255f, 255f / 255f);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nextChangeTime = BeatManager.Instance.GetNextBeatTime();
        initialBeat = BeatManager.Instance.GetCurrentBeatNumber();
        secondsPerBeat = 60.0 / 145.0;
        Debug.Log(secondsPerBeat);
        phase = 0;
        doingStuff = false;

        GenerateLevel1();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateLevel1() {
        float zSoFar = Level1Reference.transform.position.z;
        float gap = 20f;
        for (int x = 0; x < 5; x++) {
            for (int z = 0; z < 20; z++) {
                GameObject newPillar = Instantiate(Pillar);
                newPillar.transform.SetParent(Level1Reference.transform);
                newPillar.transform.localScale = new Vector3(1f, 1f, 1f);
                newPillar.transform.localPosition = new Vector3(gap * x - 40, 0, zSoFar + gap * z);
            }
        }
    }
}
