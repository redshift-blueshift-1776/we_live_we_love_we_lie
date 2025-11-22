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

    [Header("Square Rings")]
    [SerializeField] public GameObject SquareRingsReference;
    [SerializeField] public GameObject SquareRing;
    [SerializeField] public float ringOffset;
    [SerializeField] public float ringOffsetRotation;
    [SerializeField] public int numRings;
    [SerializeField] public int numRingsInRotation;

    [Header("A Place Called Home")]
    [SerializeField] public GameObject APlaceCalledHomeReference;
    [SerializeField] public GameObject Fence;
    [SerializeField] public float fenceOffset;
    [SerializeField] public float fenceVariance;
    [SerializeField] public int numFences;
    [SerializeField] public GameObject FadeBuilding;
    [SerializeField] public GameObject FadeLightpole;
    [SerializeField] public float fadeBuildingOffset;
    [SerializeField] public int numFadeBuildings;

    private double secondsPerBeat;

    private double nextChangeTime;

    public bool doingStuff;

    private int initialBeat = -1;

    public int phase = 0;

    int useEffect;

    private Color spectreColor = new Color(127f / 255f, 224f / 255f, 255f / 255f);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        useEffect = PlayerPrefs.GetInt("useVisualEffects", 0);

        gm = gameManager.GetComponent<SecondWeLiveWeLoveWeLie>();
        nextChangeTime = BeatManager.Instance.GetNextBeatTime();
        secondsPerBeat = 60.0 / 145.0 / 4.0;
        Debug.Log(secondsPerBeat);
        phase = 0;
        doingStuff = false;

        if (useEffect != 0) {
            GenerateLevel1();
            GenerateSquareRings();
            GenerateAPlaceCalledHome();
        }
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
                    Debug.Log("calling coroutine MoveThings() on currentBeat - initialBeat == 32");
                    StartCoroutine(MoveThings());
                    doingStuff = true;
                }
            }
            if (currentBeat - initialBeat == 48) {
                if (!doingStuff) {
                    Debug.Log("calling coroutine MoveThings() on currentBeat - initialBeat == 48");
                    StartCoroutine(MoveThings());
                    doingStuff = true;
                }
            }
        }
    }

    public IEnumerator MoveThings()
    {
        Debug.Log("Moving things " + phase);

        // Cache current beat-based duration in seconds
        double startDSP = AudioSettings.dspTime;
        double duration = 0;
        Vector3 startPos, targetPos;

        if (phase == 0)
        {
            // Move building downward for 15 beats
            duration = 15 * secondsPerBeat;
            startPos = Building.transform.position;
            targetPos = startPos + new Vector3(0, -150f, 0);
        }
        else if (phase == 1)
        {
            // Move player forward for 2047 beats
            duration = 2047 * secondsPerBeat;
            startPos = player.transform.position;
            targetPos = startPos + new Vector3(0, 0, 20470f);
        }
        else
        {
            // No more phases
            yield break;
        }

        // Interpolate smoothly over DSP time
        double endDSP = startDSP + duration;

        while (AudioSettings.dspTime < endDSP)
        {
            double t = (AudioSettings.dspTime - startDSP) / duration;
            float smoothT = Mathf.SmoothStep(0f, 1f, (float)t);

            if (phase == 0)
                Building.transform.position = Vector3.Lerp(startPos, targetPos, smoothT);
            else if (phase == 1)
                player.transform.position = Vector3.Lerp(startPos, targetPos, (float) t);

            yield return null;
        }

        // Snap exactly to final position (no floating error)
        if (phase == 0)
            Building.transform.position = targetPos;
        else if (phase == 1)
            player.transform.position = targetPos;

        phase++;
        doingStuff = false;
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

    public void GenerateSquareRings() {
        for (int i = 0; i < numRings; i++) {
            GameObject newSquareRing = Instantiate(SquareRing);
            newSquareRing.transform.SetParent(SquareRingsReference.transform);
            newSquareRing.transform.localPosition = new Vector3(0, 0, i * ringOffset);
            newSquareRing.transform.localRotation = Quaternion.Euler(0, 0, i * ringOffsetRotation);
            SquareRing sr = newSquareRing.GetComponent<SquareRing>();
            sr.H = ((float) i) / numRingsInRotation;
        }
    }

    public void GenerateAPlaceCalledHome() {
        for (int i = 0; i < numFences; i++) {
            GameObject newFence = Instantiate(Fence);
            newFence.transform.SetParent(APlaceCalledHomeReference.transform);
            newFence.transform.localPosition = new Vector3(0f - (5f + UnityEngine.Random.Range(0f, fenceVariance)), 0, i * fenceOffset);
            newFence = Instantiate(Fence);
            newFence.transform.SetParent(APlaceCalledHomeReference.transform);
            newFence.transform.localPosition = new Vector3(0f + (5f + UnityEngine.Random.Range(0f, fenceVariance)), 0, i * fenceOffset);
        }
        for (int i = 0; i < numFadeBuildings; i++) {
            GameObject newFadeBuilding = Instantiate(FadeBuilding);
            newFadeBuilding.transform.SetParent(APlaceCalledHomeReference.transform);
            newFadeBuilding.transform.localPosition = new Vector3(-100, 0, (i + 0.5f) * fadeBuildingOffset);
            newFadeBuilding.transform.localRotation = Quaternion.Euler(0, 90, 0);
            newFadeBuilding = Instantiate(newFadeBuilding);
            newFadeBuilding.transform.SetParent(APlaceCalledHomeReference.transform);
            newFadeBuilding.transform.localPosition = new Vector3(100, 0, (i + 0.5f) * fadeBuildingOffset);
            newFadeBuilding.transform.localRotation = Quaternion.Euler(0, 90, 0);
            GameObject newFadeLightpole = Instantiate(FadeLightpole);
            newFadeLightpole.transform.SetParent(APlaceCalledHomeReference.transform);
            newFadeLightpole.transform.localPosition = new Vector3(25, 0, i * fadeBuildingOffset);
            newFadeLightpole = Instantiate(FadeLightpole);
            newFadeLightpole.transform.SetParent(APlaceCalledHomeReference.transform);
            newFadeLightpole.transform.localPosition = new Vector3(-25, 0, i * fadeBuildingOffset);
        }
    }
}
