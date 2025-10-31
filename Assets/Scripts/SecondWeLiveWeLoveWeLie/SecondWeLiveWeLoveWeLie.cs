using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class SecondWeLiveWeLoveWeLie : MonoBehaviour
{
    [SerializeField] public GameObject player;

    [SerializeField] public GameObject note;

    [SerializeField] public bool hard;

    [SerializeField] public GameObject maze;

    [SerializeField] public GameObject[] briefcases;
    [SerializeField] public GameObject[] briefcasePivots;

    [Header("Canvasses")]
    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private TMP_Text scoreGame;

    [Header("Audio")]
    [SerializeField] public GameObject loadingAudio;
    [SerializeField] public GameObject gameAudio;

    // public bool gameActive;

    public float timer;

    // public List<GameObject> notes;

    public int score;

    public BeatManager beatManager;
    public bool gameActive = false;

    public List<string> notes; // each: "beat,x,y"

    private double dspStartTime;
    private float secondsPerBeat;
    private int nextNoteIndex = 0;

    // How early (in seconds) to spawn a note before it should appear
    private const double SPAWN_LEAD_TIME = 2.0;

    public bool madeNotes;
    public bool didBriefcases;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        madeNotes = false;
        didBriefcases = false;
        secondsPerBeat = 60.0f / 145.0f / 4.0f;
        startCanvas.SetActive(true);
        gameCanvas.SetActive(false);
        loadingAudio.SetActive(true);
        gameAudio.SetActive(false);
        gameActive = false;
        timer = 0f;

        beatManager = BeatManager.Instance;
        dspStartTime = beatManager.StartDspTime;
        // secondsPerBeat = (float)beatManager.secondsPerBeat;
    }

    void Update() {
        if (gameActive) {
            if (!madeNotes) {
                GenerateNotes();
            }

            double currentDspTime = AudioSettings.dspTime;
            double songTime = currentDspTime - beatManager.StartDspTime;
            timer += Time.deltaTime;

            // The DSP-based note spawning code goes here
            while (nextNoteIndex < notes.Count)
            {
                string n = notes[nextNoteIndex];
                string[] parts = n.Split(',');
                if (parts.Length < 3)
                {
                    nextNoteIndex++;
                    continue;
                }

                if (float.TryParse(parts[0], out float beatTime))
                {
                    float.TryParse(parts[1], out float x_pos);
                    float.TryParse(parts[2], out float y_pos);

                    double noteTime = Mathf.Abs(beatTime) * secondsPerBeat; // when the note should hit

                    // Spawn slightly before its play time
                    if (noteTime - songTime <= SPAWN_LEAD_TIME)
                    {
                        SpawnNote(beatTime, x_pos, y_pos);
                        nextNoteIndex++;
                    }
                    else
                    {
                        // Not yet time to spawn this note
                        break;
                    }
                }
                else
                {
                    nextNoteIndex++;
                }
            }

            if (timer / (float) secondsPerBeat > 550f && !didBriefcases) {
                Debug.Log(timer / (float) secondsPerBeat);
                DoBriefcases(new int[] { 576+64, 576+64+8, 576+64+16 }, briefcases, briefcasePivots);
                // DoBriefcases(new int[] { 64, 64+8, 64+16 }, briefcases, briefcasePivots);
                didBriefcases = true;
            }


            // if (songTime >= 169f) {
            //     Fail();
            // }

            scoreGame.text = "Score: " + score;
        }
    }

    private void SpawnNote(float beatTime, float x_pos, float y_pos)
    {
        GameObject newNote = Instantiate(note);
        newNote.transform.localPosition = player.transform.localPosition +
            new Vector3(3f * x_pos, 3f * y_pos, (Mathf.Abs(beatTime) - 64f) * 10f + 205f);
        newNote.transform.localScale = new Vector3(3f, 3f, 1f);

        Note newNoteScript = newNote.GetComponent<Note>();
        newNoteScript.gm = this;

        // Convert beat-based timings to seconds
        newNoteScript.duration = 16f * secondsPerBeat;
        newNoteScript.delay = Mathf.Abs(beatTime * secondsPerBeat) - 8f * secondsPerBeat;

        newNoteScript.realNote = (beatTime > 0);
    }

    public string[] SonicBlasterPattern(int time_start, float x_start, float y_start, float z_start) {
        string[] ret = new string[18];
        if (hard) {
            ret = new string[18];
            ret[0] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 1;
            ret[1] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 1;
            ret[2] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start += 1;
            ret[3] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start += 1;
            ret[4] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start += 1;
            ret[5] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start += 0.2f;
            ret[6] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 0.2f;
            ret[7] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 0.2f;
            ret[8] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 0.2f;
            ret[9] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 0.2f;
            ret[10] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start += 1;
            ret[11] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start += 1;
            ret[12] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 1;
            ret[13] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 1;
            ret[14] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 1;
            ret[15] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start += 1;
            ret[16] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start += 1;
            ret[17] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
        } else {
            ret = new string[10];
            ret[0] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 1;
            time_start += 2;
            time_start += 2;
            y_start += 1;
            time_start += 2;
            ret[1] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start += 3;
            time_start += 2;
            y_start += 0.5f;
            ret[2] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 0.5f;
            ret[3] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 0.5f;
            ret[4] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 0.5f;
            ret[5] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 0.5f;
            ret[6] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 1;
            time_start += 2;
            y_start -= 1;
            ret[7] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 1;
            time_start += 2;
            x_start -= 1;
            ret[8] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 1;
            time_start += 2;
            y_start += 1;
            ret[9] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
        }
        return ret;
    }

    public string[] SpectrePattern(int time_start, float x_start, float y_start, float z_start) {
        string[] ret = new string[12];
        if (hard) {
            ret = new string[12];
            ret[0] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 4;
            y_start += 1;
            ret[1] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 1;
            ret[2] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 1;
            ret[3] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 4;
            y_start += 0.2f;
            ret[4] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 0.2f;
            ret[5] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 0.2f;
            ret[6] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 0.2f;
            ret[7] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 0.2f;
            ret[8] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 4;
            x_start += 1;
            ret[9] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start += 1;
            ret[10] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start += 1;
            ret[11] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
        } else {
            ret = new string[4];
            ret[0] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 4;
            y_start += 1;
            ret[1] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 1;
            time_start += 2;
            time_start += 4;
            y_start += 0.2f;
            time_start += 1;
            y_start += 0.2f;
            time_start += 1;
            y_start += 0.2f;
            time_start += 1;
            y_start += 0.2f;
            time_start += 1;
            y_start += 0.2f;
            time_start += 4;
            x_start += 1;
            ret[2] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start += 1;
            time_start += 2;
            ret[3] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
        }
        return ret;
    }

    public string[] YellowClockPattern(int time_start, float x_start, float y_start, float z_start) {
        string[] ret = new string[12];
        if (hard) {
            ret = new string[15];

            ret[0] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 4;
            y_start += 1;
            ret[1] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 1;
            ret[2] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 1;
            ret[3] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start += 1f;
            ret[4] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start += 1f;
            ret[5] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 4;
            x_start += 2f;

            ret[6] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start -= 1f;
            ret[7] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            x_start -= 0.5f;
            ret[8] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            x_start -= 0.5f;
            ret[9] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start -= 1;
            ret[10] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start += 1;
            ret[11] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start += 1;
            ret[12] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start += 1;
            ret[13] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 4;
            x_start -= 2;

            ret[14] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
        } else {
            ret = new string[4];
            ret[0] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 4;
            y_start += 1;
            ret[1] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 1;
            time_start += 2;
            time_start += 4;
            y_start += 0.2f;
            time_start += 1;
            y_start += 0.2f;
            time_start += 1;
            y_start += 0.2f;
            time_start += 1;
            y_start += 0.2f;
            time_start += 1;
            y_start += 0.2f;
            time_start += 4;
            x_start += 1;
            ret[2] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start += 1;
            time_start += 2;
            ret[3] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
        }
        return ret;
    }

    public string[] randomScatter(int[] times, int spread) {
        string[] ret = new string[times.Length];
        for (int i = 0; i < times.Length; i++) {
            ret[i] = "" + times[i] + "," + UnityEngine.Random.Range(-1 * spread, spread + 1) + "," + UnityEngine.Random.Range(-1 * spread, spread + 1);
        }
        return ret;
    }

    public void solveMaze(int[] times, GameObject maze) {
        string[] newNotes = new string[9];
        Maze_Generator_Level_8 mg = maze.GetComponent<Maze_Generator_Level_8>();
        List<(int from, int to)> mstEdges = mg.mstEdges;
        int gridSize = 5;
        int totalCells = gridSize * gridSize;
        int start = 0;
        int goal = totalCells - 1;
        Dictionary<int, List<int>> graph = new Dictionary<int, List<int>>();
        for (int i = 0; i < totalCells; i++) graph[i] = new List<int>();

        foreach (var edge in mstEdges)
        {
            graph[edge.from].Add(edge.to);
            graph[edge.to].Add(edge.from);
        }
        // BFS to solve from 0 to 24, since it's 5x5.
        Queue<int> queue = new Queue<int>();
        Dictionary<int, int> parent = new Dictionary<int, int>();
        HashSet<int> visited = new HashSet<int>();

        queue.Enqueue(start);
        visited.Add(start);
        parent[start] = -1;

        bool found = false;
        while (queue.Count > 0 && !found)
        {
            int current = queue.Dequeue();
            foreach (int neighbor in graph[current])
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    parent[neighbor] = current;
                    queue.Enqueue(neighbor);
                    if (neighbor == goal)
                    {
                        found = true;
                        break;
                    }
                }
            }
        }

        List<int> path = new List<int>();
        if (found)
        {
            int current = goal;
            while (current != -1)
            {
                path.Add(current);
                current = parent[current];
            }
            path.Reverse();
        }
        else
        {
            Debug.LogWarning("No path found in maze!");
            return;
        }

        // Keep the first 9 squares.
        int stepsToKeep = Mathf.Min(9, path.Count);
        List<int> limitedPath = path.GetRange(0, stepsToKeep);

        // Each grid square is 50 x 50, 0 in the top left, 24 in the bottom right.
        List<(float x, float y)> coords = new List<(float, float)>();
        foreach (int idx in limitedPath)
        {
            int row = idx / gridSize;
            int col = idx % gridSize;

            // Convert to offsets
            float x_pos = col * 50f - 100f;
            float y_pos = -row * 50f + 100f;
            coords.Add((x_pos, y_pos));
        }
        // Debug.Log("making maze notes");

        // Instantiate notes corresponding to path steps
        for (int i = 0; i < coords.Count && i < times.Length; i++)
        {
            // Debug.Log("Making note " + i);
            float duration = times[i];
            float x_pos = coords[i].x;
            float y_pos = coords[i].y;

            GameObject newNote = Instantiate(note);
            newNote.transform.position = new Vector3(x_pos + maze.transform.position.x, y_pos+ maze.transform.position.y, maze.transform.position.z);
            newNote.transform.localScale = new Vector3(25f, 25f, 1f);

            Note newNoteScript = newNote.GetComponent<Note>();
            newNoteScript.gm = gameObject.GetComponent<SecondWeLiveWeLoveWeLie>();
            newNoteScript.duration = 16f * (float)secondsPerBeat;
            newNoteScript.delay = Mathf.Abs(duration * (float)secondsPerBeat) - 8f * (float)secondsPerBeat;
            newNoteScript.realNote = (duration > 0);
        }
        return;
    }

    public void DoBriefcases(int[] times, GameObject[] briefcases, GameObject[] briefcasePivots)
    {
        StartCoroutine(SpawnBriefcaseNotes(times, briefcases, briefcasePivots));
    }

    private IEnumerator SpawnBriefcaseNotes(int[] times, GameObject[] briefcases, GameObject[] briefcasePivots)
    {
        if (briefcases.Length < 3 || briefcasePivots.Length < 3)
        {
            Debug.LogWarning("Need exactly 3 briefcases and pivots!");
            yield break;
        }

        for (int i = 0; i < 3; i++)
        {
            int beatTime = times[i];
            float spawnTime = beatTime * (float)secondsPerBeat;
            float waitTime = Mathf.Max(0, spawnTime - (float)timer);

            // Wait until it's time to open the briefcase
            yield return new WaitForSeconds(waitTime);

            GameObject b = briefcases[i];
            GameObject bp = briefcasePivots[i];

            bool isReal = UnityEngine.Random.value < 0.5f;

            StartCoroutine(OpenBriefcase(bp, isReal));

            Debug.Log($"[Briefcase {i}] Spawning {(isReal ? "real" : "fake")} note at {timer:F2}s");

            GameObject newNote = Instantiate(note);
            newNote.transform.position = b.transform.position;
            newNote.transform.localScale = new Vector3(25f, 5f, 25f);
            newNote.transform.rotation = b.transform.rotation;
            // Avoid parent scaling/rotation issues — don’t parent to b unless needed
            // newNote.transform.SetParent(b.transform);

            Note newNoteScript = newNote.GetComponent<Note>();
            newNoteScript.gm = gameObject.GetComponent<SecondWeLiveWeLoveWeLie>();
            newNoteScript.duration = 32f * (float)secondsPerBeat;

            // 👇 key fix: set delay relative to current timer
            newNoteScript.delay = (float)timer + 4f * (float)secondsPerBeat; 
            newNoteScript.realNote = isReal;
        }
    }


    private IEnumerator OpenBriefcase(GameObject pivot, bool isReal)
    {
        float openAngle = isReal ? 105f : 90f; // real ones open wider
        float duration = 0.5f;
        float elapsed = 0f;
        Quaternion startRot = pivot.transform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(openAngle, 0f, 0f);

        while (elapsed < duration)
        {
            pivot.transform.rotation = Quaternion.Slerp(startRot, endRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        pivot.transform.rotation = endRot;
    }



    public void GenerateNotes() {
        if (madeNotes) return;
        madeNotes = true;
        if (hard) {
            this.notes = new List<string> {
            "64,2,0",
            "80,1,0",
            "-96,0,-1",
            "112,-1,0",
            "128,-3,0",
            "144,-2,1",
            "152,-1,1",
            "-160,0,-1",
            "168,2,1",
            "176,1,1",

            "200,0,0",
            "-208,0,-1",
            "216,0,-2",
            "-220,0,-1",
            "224,0,0",
            "-228,0,1",
            "232,0,2",
            "-236,0,1",
            "240,0,0",
            "-244,0,-1",
            "248,0,-2",

            "256,1,0,120",
            "272,0,0,120",
            "288,1,0,120",
            "296,0,1,120",
            "304,1,1,120",

            // Drop
            "320,0,0,120",
            "324,0,1,120",
            "326,-1,1,120",
            "328,-1,0,120",
            "330,0,0,120",
            "332,1,0,120",

            "336,1,1,120",
            "338,1,0.8,120",
            "339,1,0.6,120",
            "340,1,0.4,120",
            "342,1,0,120",
            "344,-1,0,120",
            "346,0,0,120",
            "348,0,-1,120",

            "352,-2,0,120",
            "354,-1,0,120",
            "356,0,0,120",
            "358,1,0,120",
            "360,2,0,120",
            "362,2,1,120",
            "364,1,1,120",

            "368,0,0,120",
            "370,-1,0,120",
            "372,-1,-1,120",
            "374,0,-1,120",
            "376,1,-1,120",
            "378,1,0,120",
            "380,1,1,120",
            "382,0,1,120",

            "384,1,0,120",
            "388,0,0,120",
            "390,-0.5,0,120",
            "392,-0.5,-1,120",
            "396,-0.6,-1,120",
            "397,-0.7,-1,120",
            "398,-0.8,-1,120",
            "399,-0.9,-1,120",
            "400,-1,-1,120",
            "404,0,1,120",
            "406,1,1,120",
            "408,1,0,120",

            "416,0,0,120",
            "420,-1,0,120",
            "422,-1,-1,120",
            "424,0,-1,120",
            "428,1,-1,120",
            "429,1,0,120",
            "430,1,1,120",
            "431,0,1,120",
            "432,0,0,120",
            "436,-1,0,120",
            "438,-1,1,120",
            "440,0,2,120",
            };

            notes.AddRange(SonicBlasterPattern(448, 0f, 0f, 0f));
            notes.AddRange(SonicBlasterPattern(480, 0f, 0f, 0f));
            notes.AddRange(SpectrePattern(512, 0f, 0f, 0f));
            notes.AddRange(SpectrePattern(544, 0f, 0f, 0f));

            int[] times = {
                576,
                582,
                584,
                588,
                590,
                596,
                598,
                600,
                604,
            };

            // notes = notes.Concat(randomScatter(times,3)).ToArray();
            solveMaze(times, maze);

            // foreach (string n in notes) {
            //     string[] parts = n.Split(',');
            //     if (parts.Length < 3) continue;

            //     if (float.TryParse(parts[0], out float duration)) {
            //         float.TryParse(parts[1], out float x_pos);
            //         float.TryParse(parts[2], out float y_pos);
            //         // float.TryParse(parts[3], out float z_pos);
            //         GameObject newNote = Instantiate(note);
            //         newNote.transform.localPosition = player.transform.localPosition + new Vector3(x_pos, y_pos, (Mathf.Abs(duration) - 64f) * 10f + 205f);
            //         newNote.transform.localScale = new Vector3(1f, 1f, 1f);
            //         Note newNoteScript = newNote.GetComponent<Note>();
            //         newNoteScript.gm = gameObject.GetComponent<SecondWeLiveWeLoveWeLie>();
            //         newNoteScript.duration = 16f * (float) secondsPerBeat;
            //         newNoteScript.delay = Mathf.Abs(duration * (float) secondsPerBeat) - 8f * (float) secondsPerBeat;
            //         // newNoteScript.delay = Mathf.Abs(duration * (float) secondsPerBeat);
            //         newNoteScript.realNote = (duration > 0);
            //     }
            // }
        } else {
            this.notes = new List<string> {
            "64,2,0",
            "80,1,0",
            "-96,0,-1",
            "112,-1,0",
            "128,-3,0",
            "144,-2,1",
            "152,-1,1",
            "-160,0,-1",
            "168,2,1",
            "176,1,1",

            "192,0,0",
            "208,0,-1",
            "224,0,1",
            "240,0,-1",

            "256,1,0,120",
            "272,0,0,120",
            "288,1,0,120",
            "296,0,1,120",
            "304,1,1,120",

            // Drop
            "320,0,0,120",
            // "324,0,1,120",
            // "326,-1,1,120",
            "328,-1,0,120",
            // "330,0,0,120",
            // "332,1,0,120",

            "336,1,1,120",
            // "338,1,0.8,120",
            // "339,1,0.6,120",
            // "340,1,0.4,120",
            // "342,1,0,120",
            // "344,-1,0,120",
            // "346,0,0,120",
            // "348,0,-1,120",

            "352,-2,0,120",
            // "354,-1,0,120",
            // "356,0,0,120",
            // "358,1,0,120",
            "360,2,0,120",
            // "362,2,1,120",
            // "364,1,1,120",

            "368,0,0,120",
            // "370,-1,0,120",
            // "372,-1,-1,120",
            // "374,0,-1,120",
            // "376,1,-1,120",
            // "378,1,0,120",
            // "380,1,1,120",
            // "382,0,1,120",

            "384,1,0,120",
            // "388,0,0,120",
            // "390,-0.5,0,120",
            "392,-0.5,-1,120",
            // "396,-0.6,-1,120",
            // "397,-0.7,-1,120",
            // "398,-0.8,-1,120",
            // "399,-0.9,-1,120",
            "400,-1,-1,120",
            // "404,0,1,120",
            // "406,1,1,120",
            "408,1,0,120",

            "416,0,0,120",
            "420,-1,0,120",
            // "422,-1,-1,120",
            "424,0,-1,120",
            "428,1,-1,120",
            // "429,1,0,120",
            // "430,1,1,120",
            // "431,0,1,120",
            "432,0,0,120",
            // "436,-1,0,120",
            // "438,-1,1,120",
            // "440,0,2,120",
            };

            notes.AddRange(SonicBlasterPattern(448, 0f, 0f, 0f));
            notes.AddRange(SonicBlasterPattern(480, 0f, 0f, 0f));
            notes.AddRange(SpectrePattern(512, 0f, 0f, 0f));

            // this.notes = notes;

            // notes = notes.Concat(SpectrePattern(544, 0f, 0f, 0f)).ToArray();

            // int[] times = {
            //     576 + 8,
            //     582 + 8,
            //     584 + 8,
            //     588 + 8,
            //     590 + 8,
            //     596 + 8,
            //     598 + 8,
            //     600 + 8,
            //     604 + 8,
            // };
            int[] times = {
                576,
                582,
                584,
                588,
                590,
                596,
                598,
                600,
                604,
            };

            // notes = notes.Concat(randomScatter(times,3)).ToArray();
            solveMaze(times, maze);

            // foreach (string n in notes) {
            //     string[] parts = n.Split(',');
            //     if (parts.Length < 3) continue;

            //     if (float.TryParse(parts[0], out float duration)) {
            //         float.TryParse(parts[1], out float x_pos);
            //         float.TryParse(parts[2], out float y_pos);
            //         // float.TryParse(parts[3], out float z_pos);
            //         GameObject newNote = Instantiate(note);
            //         newNote.transform.localPosition = player.transform.localPosition + new Vector3(3f * x_pos, 3f * y_pos, (Mathf.Abs(duration) - 64f) * 10f + 205f);
            //         newNote.transform.localScale = new Vector3(3f, 3f, 1f);
            //         Note newNoteScript = newNote.GetComponent<Note>();
            //         newNoteScript.gm = gameObject.GetComponent<SecondWeLiveWeLoveWeLie>();
            //         newNoteScript.duration = 16f * (float) secondsPerBeat;
            //         newNoteScript.delay = Mathf.Abs(duration * (float) secondsPerBeat) - 8f * (float) secondsPerBeat;
            //         // newNoteScript.delay = Mathf.Abs(duration * (float) secondsPerBeat);
            //         newNoteScript.realNote = (duration > 0);
            //     }
            // }
        }
        madeNotes = true;
    }

    public void startGameButton() {
        startCanvas.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        gameCanvas.SetActive(true);
        loadingAudio.SetActive(false);
        gameAudio.SetActive(true);
        gameActive = true;
        timer = 0f;
    }

    public void Fail() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PlayerPrefs.SetInt("PreviousLevel", 11);
        gameActive = false;
        StartCoroutine(LoadFailScene());
        // transitionScript.ToFail();
    }

    public void addScore(int scoreToAdd) {
        score += scoreToAdd;
    }

    public IEnumerator LoadFailScene() {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(9); // Change when we have the actual scene
    }
}
