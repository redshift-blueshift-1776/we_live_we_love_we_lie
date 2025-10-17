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

public class OutsideWorld : MonoBehaviour
{
    [SerializeField] public GameObject Building;
    [SerializeField] public GameObject Level1Reference;
    [SerializeField] public GameObject Level2Reference;
    [SerializeField] public GameObject Level3Reference;
    [SerializeField] public GameObject Level4Reference;
    [SerializeField] public GameObject Level5Reference;
    [SerializeField] public GameObject Level6Reference;
    [SerializeField] public GameObject Level7Reference;

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
        GenerateLevel2();
        GenerateLevel3();
        GenerateLevel4();
        GenerateLevel5();
        GenerateLevel6();
        GenerateLevel7();
    }

    // Update is called once per frame
    void Update()
    {
        if (!BeatManager.Instance.audioSource.isPlaying) return;

        int currentBeat = BeatManager.Instance.GetCurrentBeatNumber();

        if (currentBeat - initialBeat == 16) {
            if (!doingStuff) {
                StartCoroutine(MoveThings());
                doingStuff = true;
            }
        }
        if (currentBeat - initialBeat == 32) {
            if (!doingStuff) {
                StartCoroutine(MoveThings());
                doingStuff = true;
            }
        }
        if (currentBeat - initialBeat == 48) {
            if (!doingStuff) {
                StartCoroutine(MoveThings());
                doingStuff = true;
            }
        }
        if (currentBeat - initialBeat == 56) {
            if (!doingStuff) {
                StartCoroutine(MoveThings());
                doingStuff = true;
            }
        }
        if (currentBeat - initialBeat == 64) {
            if (!doingStuff) {
                StartCoroutine(MoveThings());
                doingStuff = true;
            }
        }
        if (currentBeat - initialBeat == 72) {
            if (!doingStuff) {
                StartCoroutine(MoveThings());
                doingStuff = true;
            }
        }
        if (currentBeat - initialBeat == 80) {
            if (!doingStuff) {
                StartCoroutine(MoveThings());
                doingStuff = true;
            }
        }
        if (currentBeat - initialBeat == 88) {
            if (!doingStuff) {
                StartCoroutine(MoveThings());
                doingStuff = true;
            }
        }
        if (currentBeat - initialBeat == 96) {
            if (!doingStuff) {
                StartCoroutine(MoveThings());
                doingStuff = true;
            }
        }
    }

    public IEnumerator MoveThings() {
        Debug.Log("Moving things" + phase);
        if (phase == 0) {
            double duration = 4 * secondsPerBeat;
            double elapsed = 0;
            Vector3 startPos = Building.transform.position;
            Vector3 targetPos = Building.transform.position + new Vector3(0, -150, 0);
            while (elapsed < duration) {
                Building.transform.position = Vector3.Lerp(startPos, targetPos, (float) (elapsed / duration));
                elapsed += Time.deltaTime;
                yield return null;
            }
            Building.transform.position = targetPos;
        }
        if (phase == 1) {
            double duration = 7 * secondsPerBeat;
            double elapsed = 0;
            Vector3 startPos = transform.position;
            Vector3 targetPos = transform.position + new Vector3(0, 0, -500);
            Vector3 startPos2 = Level2Reference.transform.localPosition;
            Vector3 targetPos2 = Level2Reference.transform.localPosition + new Vector3(0, -1000, 0);
            while (elapsed < duration) {
                transform.position = Vector3.Lerp(startPos, targetPos, (float) (elapsed / duration));
                Level2Reference.transform.localPosition = Vector3.Lerp(startPos2, targetPos2, (float) (elapsed / duration));
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.position = targetPos;
            Level2Reference.transform.localPosition = targetPos2;
        }
        if (phase == 2) {
            double duration = 7 * secondsPerBeat;
            double elapsed = 0;
            Vector3 startPos = transform.position;
            Vector3 targetPos = transform.position + new Vector3(0, 0, -5000);
            Vector3 startPos2 = Level3Reference.transform.localPosition;
            Vector3 targetPos2 = Level3Reference.transform.localPosition + new Vector3(0, -500, 0);
            while (elapsed < duration) {
                transform.position = Vector3.Lerp(startPos, targetPos, (float) (elapsed / duration));
                Level3Reference.transform.localPosition = Vector3.Lerp(startPos2, targetPos2, (float) (elapsed / duration));
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.position = targetPos;
            Level3Reference.transform.localPosition = targetPos2;
        }
        if (phase == 3) {
            double duration = 7 * secondsPerBeat;
            double elapsed = 0;
            Vector3 startPos = transform.position;
            Vector3 targetPos = transform.position + new Vector3(0, 0, -500);
            Quaternion startRotation = Level3Reference.transform.localRotation;
            Quaternion targetRotation = Quaternion.Euler(-45, 0, 0);
            Vector3 startPos2 = Level3Reference.transform.localPosition;
            Vector3 targetPos2 = Level3Reference.transform.localPosition + new Vector3(0, 500, 100);
            while (elapsed < duration) {
                transform.position = Vector3.Lerp(startPos, targetPos, (float) (elapsed / duration));
                Level3Reference.transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, (float) (elapsed / duration));
                Level3Reference.transform.localPosition = Vector3.Lerp(startPos2, targetPos2, (float) (elapsed / duration));
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.position = targetPos;
            Level3Reference.transform.localRotation = targetRotation;
            Level3Reference.transform.localPosition = targetPos2;
        }
        if (phase == 4) {
            double duration = 7 * secondsPerBeat;
            double elapsed = 0;
            Vector3 startPos = transform.position;
            Vector3 targetPos = transform.position + new Vector3(0, 0, -990);
            Quaternion startRotation = Level4Reference.transform.localRotation;
            Quaternion targetRotation = Quaternion.Euler(-45, 0, 0);
            Vector3 startPos2 = Level3Reference.transform.localPosition;
            Vector3 targetPos2 = Level3Reference.transform.localPosition + new Vector3(0, 1000, 100);
            while (elapsed < duration) {
                transform.position = Vector3.Lerp(startPos, targetPos, (float) (elapsed / duration));
                Level3Reference.transform.localPosition = Vector3.Lerp(startPos2, targetPos2, (float) (elapsed / duration));
                Level4Reference.transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, (float) (elapsed / duration));
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.position = targetPos;
            Level4Reference.transform.localRotation = targetRotation;
            Level3Reference.transform.localPosition = targetPos2;
        }
        if (phase == 5) {
            double duration = 7 * secondsPerBeat;
            double elapsed = 0;
            Vector3 startPos = transform.position;
            Vector3 targetPos = transform.position + new Vector3(0, 0, -420);
            Vector3 startPos2 = Level4Reference.transform.localPosition;
            Vector3 targetPos2 = Level4Reference.transform.localPosition + new Vector3(0, -500, 100);
            while (elapsed < duration) {
                transform.position = Vector3.Lerp(startPos, targetPos, (float) (elapsed / duration));
                Level4Reference.transform.localPosition = Vector3.Lerp(startPos2, targetPos2, (float) (elapsed / duration));
                elapsed += Time.deltaTime;
                yield return null;
            }
            Level4Reference.transform.localPosition = targetPos2;
            transform.position = targetPos;
        }
        phase++;
        doingStuff = false;
        yield return null;
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

    public void GenerateLevel2() {
        
    }

    public void GenerateLevel3() {
        
    }

    public void GenerateLevel4() {
        
    }

    public void GenerateLevel5() {
        
    }

    public void GenerateLevel6() {
        
    }

    public void GenerateLevel7() {
        
    }
}
