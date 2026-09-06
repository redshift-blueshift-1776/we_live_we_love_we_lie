using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;

// THIS ONE IS USED TO SELECT AN EXISTING MAP (custom JSON)
// To select a song to CREATE a new map, use LevelEditorMenu.cs
public class SelectMapMenu : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject pagePrefab;
    public GameObject songEntryPrefab; // Reuses Song Entry UI prefab but displays map data

    [Header("Page Settings")]
    public RectTransform pageContainer;
    public float slideDuration = 0.45f;
    public float pageWidth = 1920f;
    public int songsPerPage = 5; // Distinct from LevelEditorMenu (4) -> smaller cards for maps

    [Header("Buttons")]
    public GameObject previousButton;
    public GameObject nextButton;

    [Header("Header / Empty State")]
    public TMP_Text headerText;
    public TMP_Text emptyStateText;

    [Header("Audio Preview")]
    public AudioSource previewSource;

    private List<GameObject> pages = new List<GameObject>();
    private List<SimpleMapData> maps = new List<SimpleMapData>();
    private List<string> mapFilePaths = new List<string>();
    private Dictionary<string, SongDataSO> songLookup = new Dictionary<string, SongDataSO>();

    private int currentPage = 0;
    private bool isSliding = false;
    public int numPages;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (previewSource == null)
        {
            previewSource = gameObject.AddComponent<AudioSource>();
            previewSource.playOnAwake = false;
        }

        if (headerText != null) {
            headerText.text = "CUSTOM MAPS — SELECT TO PLAY";
        }

        // Build song lookup for cover art / preview clip
        var allSongs = Resources.LoadAll<SongDataSO>("Songs");
        foreach (var s in allSongs) if (!songLookup.ContainsKey(s.songName)) songLookup[s.songName] = s;

        LoadCustomMaps();
        GeneratePages();
        InitPagePositions();
        UpdatePageButtons();
    }

    private void LoadCustomMaps()
    {
        maps.Clear();
        mapFilePaths.Clear();

        try
        {
            string[] jsonFiles = Directory.GetFiles(Application.persistentDataPath, "*.json");
            // Sort by last write time, newest first
            Array.Sort(jsonFiles, (a, b) => File.GetLastWriteTime(b).CompareTo(File.GetLastWriteTime(a)));

            foreach (string file in jsonFiles)
            {
                try
                {
                    string json = File.ReadAllText(file);
                    SimpleMapData m = JsonUtility.FromJson<SimpleMapData>(json);
                    if (m != null && !string.IsNullOrEmpty(m.songName))
                    {
                        maps.Add(m);
                        mapFilePaths.Add(file);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"SelectMapMenu: failed to parse {file}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"SelectMapMenu: persistentDataPath scan failed: {ex.Message}");
        }

        if (maps.Count == 0 && emptyStateText != null)
        {
            emptyStateText.text = "No custom maps yet.\nGo to Song Select to create one!";
            emptyStateText.gameObject.SetActive(true);
        }
        else if (emptyStateText != null)
        {
            emptyStateText.gameObject.SetActive(false);
        }
    }

    private void GeneratePages()
    {
        // Clean previous
        foreach (var p in pages) Destroy(p);
        pages.Clear();

        if (maps.Count == 0)
        {
            numPages = 1;
            GameObject emptyPage = Instantiate(pagePrefab, pageContainer);
            pages.Add(emptyPage);
            // Show a big message inside the empty page
            var tmp = emptyPage.GetComponentInChildren<TMP_Text>();
            if (tmp != null) tmp.text = "No custom maps found.";
            return;
        }

        numPages = Mathf.CeilToInt((float)maps.Count / songsPerPage);

        for (int p = 0; p < numPages; p++)
        {
            GameObject page = Instantiate(pagePrefab, pageContainer);
            pages.Add(page);

            int start = p * songsPerPage;
            int end = Mathf.Min(start + songsPerPage, maps.Count);

            for (int i = start; i < end; i++)
            {
                GameObject entry = Instantiate(songEntryPrefab, page.transform);
                PopulateMapEntry(entry, maps[i], mapFilePaths[i]);
            }
        }
    }

    private void PopulateMapEntry(GameObject obj, SimpleMapData map, string filePath)
    {
        // Support both MapEntryUI and SongEntryUI on the same prefab
        var mapUI = obj.GetComponent<MapEntryUI>();
        var songUI = obj.GetComponent<SongEntryUI>();

        string displayName = Path.GetFileNameWithoutExtension(filePath);
        string songName = map.songName;
        SongDataSO songInfo = null;
        songLookup.TryGetValue(songName, out songInfo);

        int noteCount = map.notes != null ? map.notes.Count : 0;
        string trimmedName = songName.Length > 18 ? songName.Substring(0, 18) + "…" : songName;
        string bumpInfo = $"{noteCount} notes";

        if (mapUI != null)
        {
            mapUI.nameText.text = trimmedName;
            mapUI.bpmText.text = $"{map.bpm} BPM";
            mapUI.genreText.text = map.mapType; // e.g. "simple"
            mapUI.timeSigText.text = bumpInfo;
            mapUI.lengthText.text = $"{displayName}";
            if (mapUI.coverArtImage != null)
            {
                if (songInfo != null && songInfo.coverArt != null) mapUI.coverArtImage.sprite = songInfo.coverArt;
                mapUI.coverArtImage.enabled = (songInfo != null && songInfo.coverArt != null);
            }
            // Relabel buttons.
            SetButtonLabel(mapUI.previewButton, "EDIT");
            SetButtonLabel(mapUI.selectButton, "PLAY");
            mapUI.previewButton.onClick.RemoveAllListeners();
            mapUI.selectButton.onClick.RemoveAllListeners();
            mapUI.previewButton.onClick.AddListener(() => EditMap(map, filePath));
            mapUI.selectButton.onClick.AddListener(() => PlayMap(map, filePath));
        }
        else if (songUI != null)
        {
            songUI.nameText.text = trimmedName;
            songUI.bpmText.text = $"{map.bpm} BPM";
            songUI.genreText.text = map.mapType;
            songUI.timeSigText.text = bumpInfo;
            songUI.lengthText.text = displayName;
            if (songUI.coverArtImage != null)
            {
                if (songInfo != null && songInfo.coverArt != null) songUI.coverArtImage.sprite = songInfo.coverArt;
                songUI.coverArtImage.enabled = (songInfo != null && songInfo.coverArt != null);
            }
            SetButtonLabel(songUI.previewButton, "EDIT");
            SetButtonLabel(songUI.selectButton, "PLAY");
            songUI.previewButton.onClick.RemoveAllListeners();
            songUI.selectButton.onClick.RemoveAllListeners();
            songUI.previewButton.onClick.AddListener(() => EditMap(map, filePath));
            songUI.selectButton.onClick.AddListener(() => PlayMap(map, filePath));
        }
    }

    private void SetButtonLabel(Button btn, string label)
    {
        if (btn == null) {
            return;
        }
        var tmp = btn.GetComponentInChildren<TMP_Text>();
        if (tmp != null) {
            tmp.text = label;
        }
    }

    private void PlayPreviewForMap(SimpleMapData map)
    {
        if (previewSource == null) {
            return;
        }
        if (songLookup.TryGetValue(map.songName, out var song) && song.audioClip != null)
        {
            if (previewSource.isPlaying) {
                previewSource.Stop();
            }
            previewSource.clip = song.audioClip;
            previewSource.time = song.previewStartTime;
            previewSource.Play();
            Debug.Log($"Previewing map '{map.songName}' @ {song.previewStartTime}s");
        }
        else
        {
            Debug.Log($"Previewing: {map.songName} (no audio clip found)");
        }
    }

    private void PlayMap(SimpleMapData map, string filePath)
    {
        // ResolveCustomMapPath in SecondWeLiveWeLoveWeLie will scan by SelectedSong,
        // but also persist file path for debugging / direct load.
        // Store both for robustness.
        PlayerPrefs.SetString("SelectedSong", map.songName);
        PlayerPrefs.SetString("SelectedMapPath", filePath);
        PlayerPrefs.SetString("SelectedMapFile", Path.GetFileName(filePath));
        Debug.Log($"SelectMapMenu: PLAY map '{map.songName}' -> {filePath} -> loading Custom Levels");
        SceneManager.LoadScene("Custom Levels");
    }

    private void EditMap(SimpleMapData map, string filePath)
    {
        PlayerPrefs.SetString("SelectedSong", map.songName);
        PlayerPrefs.SetString("SelectedMapPath", filePath);
        PlayerPrefs.SetString("SelectedMapFile", Path.GetFileName(filePath));
        Debug.Log($"SelectMapMenu: EDIT map '{map.songName}' -> {filePath} -> loading Level Editor");
        SceneManager.LoadScene("Level Editor");
    }

    private void InitPagePositions()
    {
        for (int i = 0; i < pages.Count; i++)
        {
            RectTransform rt = pages[i].GetComponent<RectTransform>();
            if (rt == null) {
                continue;
            }
            rt.anchoredPosition = new Vector2((i - currentPage) * pageWidth, 0);
            pages[i].SetActive(i == currentPage);
        }
    }

    public void OnNextPage()
    {
        if (isSliding || currentPage >= pages.Count - 1) return;
        if (previewSource != null && previewSource.isPlaying) previewSource.Stop();
        StartCoroutine(SlideToPage(currentPage + 1));
    }

    public void OnPrevPage()
    {
        if (isSliding || currentPage <= 0) return;
        if (previewSource != null && previewSource.isPlaying) previewSource.Stop();
        StartCoroutine(SlideToPage(currentPage - 1));
    }

    IEnumerator SlideToPage(int newPage)
    {
        isSliding = true;
        float elapsed = 0f;
        float direction = Mathf.Sign(newPage - currentPage);

        Vector2[] startPositions = new Vector2[pages.Count];
        for (int i = 0; i < pages.Count; i++)
        {
            RectTransform rt = pages[i].GetComponent<RectTransform>();
            startPositions[i] = rt.anchoredPosition;
            if (i == newPage || i == currentPage)
                pages[i].SetActive(true);
        }

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / slideDuration);
            for (int i = 0; i < pages.Count; i++)
            {
                RectTransform rt = pages[i].GetComponent<RectTransform>();
                rt.anchoredPosition = Vector2.Lerp(startPositions[i],
                    startPositions[i] - new Vector2(direction * pageWidth, 0), t);
            }
            yield return null;
        }

        for (int i = 0; i < pages.Count; i++)
        {
            RectTransform rt = pages[i].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2((i - newPage) * pageWidth, 0);
            pages[i].SetActive(i == newPage || i == newPage + 1 || i == newPage - 1);
        }

        currentPage = newPage;
        isSliding = false;
        if (previousButton != null) {
            previousButton.SetActive(currentPage > 0);
        }
        if (nextButton != null) {
            nextButton.SetActive(currentPage < pages.Count - 1);
        }
    }

    private void UpdatePageButtons()
    {
        if (previousButton != null) {
            previousButton.SetActive(currentPage > 0);
        }
        if (nextButton != null) {
            nextButton.SetActive(currentPage < pages.Count - 1);
        }
    }

    void OnDestroy()
    {
        if (previewSource != null && previewSource.isPlaying) {
            previewSource.Stop();
        }
    }
}
