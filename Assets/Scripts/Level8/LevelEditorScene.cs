using UnityEngine;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelEditorScene : MonoBehaviour
{
    [Header("References")]
    public Camera editorCamera;
    public GameObject notePrefab;

    [Header("UI")]
    public GameObject xTextBackground;
    public GameObject yTextBackground;
    public GameObject beatTextBackground;
    public TMP_Text xText;
    public TMP_Text yText;
    public TMP_Text beatText;

    [Header("Editor Settings")]
    public float zScale = 10f;
    public float xyScale = 3f;
    public float moveSpeed = 15f;
    public float fastSpeed = 40f;
    public float lookSensitivity = 2f;

    [Header("Map Data")]
    public string songName;
    public SimpleMapData loadedMap;

    private List<EditorNote> editorNotes = new List<EditorNote>();
    private EditorNote selectedNote;

    private Vector3 lastMousePos;

    private bool draggingXY = false;
    private bool draggingZ = false;

    void Start()
    {
        if (editorCamera == null)
        {
            editorCamera = Camera.main;
        }

        LoadMap();
        SpawnEditorNotes();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            SaveMap();
        }
        HandleFreecam();
        HandleSelection();
        HandleDragging();
        HandleUI();
    }

    public string[] jsonfilen;

    // Loading the map
    void LoadMap()
    {
        songName = PlayerPrefs.GetString("SelectedSong", "UNKNOWN");

        string[] jsonFiles = Directory.GetFiles(Application.persistentDataPath, "*.json");
        jsonfilen = jsonFiles;
        foreach (string file in jsonFiles)
        {
            string json = File.ReadAllText(file);
            loadedMap = JsonUtility.FromJson<SimpleMapData>(json);
            Debug.Log("Loaded map: " + file);
            if (loadedMap.songName == songName)
            {
                Debug.Log("Found Song: " + file);
                return;
            }
        }

        Debug.LogError("No map found for song: " + songName);
    }

    void SpawnEditorNotes()
    {
        foreach (NoteData n in loadedMap.notes)
        {
            GameObject note = Instantiate(notePrefab);
            EditorNote en = note.AddComponent<EditorNote>();

            en.beat = n.beat;
            en.x = n.x;
            en.y = n.y;

            Vector3 pos = new Vector3(
                n.x * xyScale,
                n.y * xyScale,
                n.beat * zScale
            );

            note.transform.position = pos;
            editorNotes.Add(en);
        }
    }

    // UI
    void HandleUI() {
        if (selectedNote == null) {
            return;
        }
        if (draggingXY) {
            xTextBackground.SetActive(true);
            yTextBackground.SetActive(true);

            xText.text = "x: " + selectedNote.x;
            yText.text = "y: " + selectedNote.y;
        } else {
            xTextBackground.SetActive(false);
            yTextBackground.SetActive(false);
        }
        if (draggingZ) {
            beatTextBackground.SetActive(true);

            beatText.text = "Beat: " + selectedNote.beat;
        } else {
            beatTextBackground.SetActive(false);
        }
    }


    // Camera
    void HandleFreecam()
    {
        float mx = Input.GetAxis("Mouse X") * lookSensitivity;
        float my = Input.GetAxis("Mouse Y") * lookSensitivity;

        editorCamera.transform.eulerAngles += new Vector3(-my, mx, 0);

        // Movement
        Vector3 dir = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) dir += editorCamera.transform.forward;
        if (Input.GetKey(KeyCode.S)) dir -= editorCamera.transform.forward;
        if (Input.GetKey(KeyCode.A)) dir -= editorCamera.transform.right;
        if (Input.GetKey(KeyCode.D)) dir += editorCamera.transform.right;
        if (Input.GetKey(KeyCode.E)) dir += editorCamera.transform.up;
        if (Input.GetKey(KeyCode.Q)) dir -= editorCamera.transform.up;

        float speed = Input.GetKey(KeyCode.LeftShift) ? fastSpeed : moveSpeed;

        editorCamera.transform.position += dir * speed * Time.deltaTime;
    }


    // Note Selection
    void HandleSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = editorCamera.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2, 0));

            if (Physics.Raycast(ray, out RaycastHit hit, 500f))
            {
                EditorNote maybeNote = hit.collider.GetComponent<EditorNote>();
                if (maybeNote != null)
                {
                    SelectNote(maybeNote);
                }
                else
                {
                    DeselectNote();
                }
            }
            else
            {
                DeselectNote();
            }
        }
    }

    void SelectNote(EditorNote n)
    {
        if (selectedNote != null)
            selectedNote.SetSelected(false);

        selectedNote = n;
        selectedNote.SetSelected(true);
    }

    void DeselectNote()
    {
        if (selectedNote != null)
            selectedNote.SetSelected(false);

        selectedNote = null;
    }


    // Note Editing
    void HandleDragging()
    {
        if (selectedNote == null)
            return;

        if (Input.GetKeyDown(KeyCode.X))
            draggingXY = true;

        if (Input.GetKeyDown(KeyCode.Z))
            draggingZ = true;

        if (Input.GetKeyUp(KeyCode.X))
            draggingXY = false;

        if (Input.GetKeyUp(KeyCode.Z))
            draggingZ = false;

        if (draggingXY)
            DragXY();

        if (draggingZ)
            DragZ();
    }

    void DragXY()
    {
        float dx = Input.GetAxis("Mouse X") * 0.1f;
        float dy = Input.GetAxis("Mouse Y") * 0.1f;

        selectedNote.x += dx;
        selectedNote.y += dy;

        selectedNote.transform.position = new Vector3(
            selectedNote.x * xyScale,
            selectedNote.y * xyScale,
            selectedNote.beat * zScale
        );
    }

    void DragZ()
    {
        float dz = Input.GetAxis("Mouse Y") * 0.05f;

        float beatFloat = selectedNote.beat + dz * 16f;

        // Snap to 1/16 note
        int snapped16 = Mathf.RoundToInt(beatFloat * 4f);
        // float snappedBeats = snapped16 / 4f;
        int snappedBeats = snapped16;

        selectedNote.beat = snappedBeats;

        selectedNote.transform.position = new Vector3(
            selectedNote.x * xyScale,
            selectedNote.y * xyScale,
            selectedNote.beat * zScale
        );
    }


    // Save
    public void SaveMap()
    {
        SimpleMapData newMap = new()
        {
            mapType = loadedMap.mapType,
            bpm = loadedMap.bpm,
            msPerSixteenth = loadedMap.msPerSixteenth,
            songName = loadedMap.songName
        };

        foreach (EditorNote e in editorNotes)
        {
            int beatInt16 = Mathf.RoundToInt(e.beat * 4f);
            // float beatClean = beatInt16 / 4f;

            newMap.notes.Add(new NoteData(beatInt16, e.x, e.y));
        }

        string json = JsonUtility.ToJson(newMap, true);

        string fileName = loadedMap.songName + "_EDITED_" +
                          System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".json";

        string path = Path.Combine(Application.persistentDataPath, fileName);

        File.WriteAllText(path, json);

        Debug.Log("Saved edited map to: " + path);
        SceneManager.LoadScene("Menu");
    }
}
