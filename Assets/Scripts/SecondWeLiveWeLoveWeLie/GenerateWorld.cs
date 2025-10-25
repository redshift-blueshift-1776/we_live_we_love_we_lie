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
    [SerializeField] public GameObject player;
    [SerializeField] public GameObject Building;
    [SerializeField] public GameObject gameManager;
    [SerializeField] public SecondWeLiveWeLoveWeLie gm;

    private double secondsPerBeat;

    private double nextChangeTime;

    public bool doingStuff;

    private int initialBeat = -1;

    public int phase = 0;

    private Color spectreColor = new Color(127f / 255f, 224f / 255f, 255f / 255f);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gm = gameManager.GetComponent<SecondWeLiveWeLoveWeLie>();
        nextChangeTime = BeatManager.Instance.GetNextBeatTime();
        secondsPerBeat = 60.0 / 145.0 / 4.0;
        Debug.Log(secondsPerBeat);
        phase = 0;
        doingStuff = false;

        GenerateLevel1();
    }

    // Update is called once per frame
    void Update()
    {
        if (gm.gameActive) {
            if (!BeatManager.Instance.audioSource.isPlaying) return;

            if (initialBeat == -1) {
                initialBeat = BeatManager.Instance.GetCurrentBeatNumber();
            }

            int currentBeat = BeatManager.Instance.GetCurrentBeatNumber();

            if (currentBeat - initialBeat == 32) {
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
            if (currentBeat - initialBeat == 192) {
                if (!doingStuff) {
                    StartCoroutine(MoveThings());
                    doingStuff = true;
                }
            }
            if (currentBeat - initialBeat == 320) {
                if (!doingStuff) {
                    StartCoroutine(MoveThings());
                    doingStuff = true;
                }
            }
        }
    }

    public IEnumerator MoveThings() {
        Debug.Log("Moving things" + phase);
        if (phase == 0) {
            double duration = 15 * secondsPerBeat;
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
            double duration = 15 * secondsPerBeat;
            double elapsed = 0;
            Vector3 startPos = player.transform.position;
            Vector3 targetPos = player.transform.position + new Vector3(0, 0, -10);
            while (elapsed < duration) {
                player.transform.position = Vector3.Lerp(startPos, targetPos, (float) (elapsed / duration));
                elapsed += Time.deltaTime;
                yield return null;
            }
            player.transform.position = targetPos;
        }
        if (phase == 2) {
            double duration = 31 * secondsPerBeat;
            double elapsed = 0;
            Vector3 startPos = player.transform.position;
            Vector3 targetPos = player.transform.position + new Vector3(100, 0, 100);
            while (elapsed < duration) {
                player.transform.position = Vector3.Lerp(startPos, targetPos, (float) (elapsed / duration));
                elapsed += Time.deltaTime;
                yield return null;
            }
            player.transform.position = targetPos;
        }
        if (phase == 3) {
            double duration = 127 * secondsPerBeat;
            double elapsed = 0;
            Vector3 startPos = player.transform.position;
            Vector3 targetPos = player.transform.position + new Vector3(0, 50, 0);
            while (elapsed < duration) {
                player.transform.position = Vector3.Lerp(startPos, targetPos, (float) (elapsed / duration));
                elapsed += Time.deltaTime;
                yield return null;
            }
            player.transform.position = targetPos;
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
}
