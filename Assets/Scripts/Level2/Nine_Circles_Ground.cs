using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class Nine_Circles_Ground : MonoBehaviour
{
    public List<GameObject> group1;
    public List<GameObject> group2;
    public List<GameObject> group3;
    public List<GameObject> group4;

    public List<GameObject> squares;

    private float secondsPerBeat;

    private double nextChangeTime;

    private int lastColorBeat = -1;

    [SerializeField] public float ringGap;
    [SerializeField] public int gridSize;
    [SerializeField] public int squareSize;

    [SerializeField] public Color white;
    [SerializeField] public Color blue;
    [SerializeField] public Color black;

    private int useEffect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nextChangeTime = BeatManager.Instance.GetNextBeatTime();
        MakeGrid();
        ClassifyGrid();

        useEffect = PlayerPrefs.GetInt("useVisualEffects", 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!BeatManager.Instance.audioSource.isPlaying) return;

        if (useEffect == 0) return;

        int currentBeat = BeatManager.Instance.GetCurrentBeatNumber();

        if (currentBeat != lastColorBeat)
        {
            lastColorBeat = currentBeat;
            ChangeColors();
        }
    }

    public void MakeGrid() {
        // Make a square grid of size gridSize with squares of side squareSize
        // clear old children if rerun
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        squares.Clear();

        // Center grid at (0,0)
        float offset = (gridSize - 1) * squareSize / 2f;
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                GameObject square = GameObject.CreatePrimitive(PrimitiveType.Quad);
                square.transform.SetParent(transform);

                square.transform.localScale = new Vector3(squareSize, squareSize, 1f);

                float worldX = x * squareSize - offset;
                float worldZ = z * squareSize - offset;

                square.transform.localPosition = new Vector3(worldX, 0f, worldZ);
                square.transform.rotation = Quaternion.Euler(90f, 0f, 0f); // lay flat

                squares.Add(square);
            }
        }
    }

    public void ClassifyGrid() {
        // reset groups
        group1.Clear();
        group2.Clear();
        group3.Clear();
        group4.Clear();
        
        foreach (GameObject square in squares) {
            float manhattanDistance = Mathf.Abs(square.transform.position.x)
                + Math.Abs(square.transform.position.z);
            int ringLevel = (int) Mathf.Floor(manhattanDistance / ringGap);
            if (ringLevel % 4 == 0) {
                group4.Add(square);
            } else if (ringLevel % 4 == 1) {
                group1.Add(square);
            } else if (ringLevel % 4 == 2) {
                group2.Add(square);
            } else {
                group3.Add(square);
            }
        }

        // Set all of group 1 to white, group 3 to blue, and groups 2 and 4 to black.
        foreach (GameObject sq in group1) sq.GetComponent<Renderer>().material.color = white;
        foreach (GameObject sq in group3) sq.GetComponent<Renderer>().material.color = blue;
        foreach (GameObject sq in group2) sq.GetComponent<Renderer>().material.color = black;
        foreach (GameObject sq in group4) sq.GetComponent<Renderer>().material.color = black;
    }

    public void ChangeColors() {
        // Snapshot current colors
        Color c1 = group1[0].GetComponent<Renderer>().material.color;
        Color c2 = group2[0].GetComponent<Renderer>().material.color;
        Color c3 = group3[0].GetComponent<Renderer>().material.color;
        Color c4 = group4[0].GetComponent<Renderer>().material.color;

        // Rotate: 1←2, 2←3, 3←4, 4←1
        foreach (GameObject sq in group1) sq.GetComponent<Renderer>().material.color = c2;
        foreach (GameObject sq in group2) sq.GetComponent<Renderer>().material.color = c3;
        foreach (GameObject sq in group3) sq.GetComponent<Renderer>().material.color = c4;
        foreach (GameObject sq in group4) sq.GetComponent<Renderer>().material.color = c1;
    }
}
