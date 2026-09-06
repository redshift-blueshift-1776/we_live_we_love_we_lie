using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelEditorMenu : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject pagePrefab;
    public GameObject songEntryPrefab;

    [Header("Page Settings")]
    public RectTransform pageContainer;
    public float slideDuration = 0.52f;
    public float pageWidth = 1920f;
    public int songsPerPage = 4; // Distinct from SelectMapMenu (5)

    [Header("Buttons")]
    public GameObject previousButton;
    public GameObject nextButton;

    [Header("Header")]
    public TMP_Text headerText;

    [Header("Audio Preview")]
    public AudioSource previewSource;

    private List<GameObject> pages = new List<GameObject>();
    private SongDataSO[] songs;

    private int currentPage = 0;
    private bool isSliding = false;
    public int numPages;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (headerText != null) headerText.text = "SONG SELECT — CREATE NEW MAP";

        if (previewSource == null)
        {
            previewSource = gameObject.AddComponent<AudioSource>();
            previewSource.playOnAwake = false;
            previewSource.loop = false;
        }

        songs = Resources.LoadAll<SongDataSO>("Songs");
        // Sort alphabetically -> distinct from Custom Map Select (newest first)
        Array.Sort(songs, (a, b) => string.Compare(a.songName, b.songName, StringComparison.Ordinal));

        if (songs.Length == 0)
            Debug.LogWarning("LevelEditorMenu: No SongDataSO found in Resources/Songs. Add ScriptableObjects there.");

        GeneratePages();
        InitPagePositions();
        UpdatePageButtons();
    }

    private void GeneratePages()
    {
        foreach (var p in pages) Destroy(p);
        pages.Clear();

        numPages = Mathf.CeilToInt((float)songs.Length / songsPerPage);

        for (int p = 0; p < numPages; p++)
        {
            GameObject page = Instantiate(pagePrefab, pageContainer);
            pages.Add(page);

            int start = p * songsPerPage;
            int end = Mathf.Min(start + songsPerPage, songs.Length);

            for (int i = start; i < end; i++)
            {
                GameObject entry = Instantiate(songEntryPrefab, page.transform);
                PopulateSongEntry(entry, songs[i]);
            }
        }
    }

    private void PopulateSongEntry(GameObject obj, SongDataSO data)
    {
        var ui = obj.GetComponent<SongEntryUI>();
        if (ui == null)
        {
            Debug.LogWarning($"LevelEditorMenu: {obj.name} missing SongEntryUI");
            return;
        }

        ui.nameText.text = data.songName;
        ui.bpmText.text = $"{data.bpm} BPM";
        ui.genreText.text = data.genre;
        ui.timeSigText.text = data.timeSignature;
        ui.lengthText.text = $"{data.approxLengthInSeconds}s";

        if (ui.coverArtImage != null)
        {
            if (data.coverArt != null)
            {
                ui.coverArtImage.sprite = data.coverArt;
                ui.coverArtImage.enabled = true;
            }
            else
            {
                ui.coverArtImage.enabled = false;
            }
        }

        // Distinct labeling vs SelectMapMenu (PLAY/EDIT): here Preview + Create
        SetButtonLabel(ui.previewButton, "PREVIEW");
        SetButtonLabel(ui.selectButton, "CREATE");

        ui.previewButton.onClick.RemoveAllListeners();
        ui.selectButton.onClick.RemoveAllListeners();

        ui.previewButton.onClick.AddListener(() => PlayPreview(data));
        ui.selectButton.onClick.AddListener(() => SelectSong(data));
    }

    private void SetButtonLabel(Button btn, string label)
    {
        if (btn == null) return;
        var tmp = btn.GetComponentInChildren<TMP_Text>();
        if (tmp != null) tmp.text = label;
    }

    private void PlayPreview(SongDataSO song)
    {
        if (song == null || song.audioClip == null)
        {
            Debug.Log($"LevelEditorMenu Preview: '{song?.songName}' has no audioClip");
            return;
        }
        if (previewSource == null) return;
        if (previewSource.isPlaying) previewSource.Stop();
        previewSource.clip = song.audioClip;
        previewSource.time = Mathf.Clamp(song.previewStartTime, 0f, Mathf.Max(0f, song.audioClip.length - 0.1f));
        previewSource.Play();
        Debug.Log($"LevelEditorMenu Previewing: {song.songName} @ {song.previewStartTime}s");
    }

    private void SelectSong(SongDataSO song)
    {
        if (previewSource != null && previewSource.isPlaying) previewSource.Stop();

        PlayerPrefs.SetString("SelectedSong", song.songName);
        // Clear any previously selected *map* path so LevelEditorScene knows this is a NEW map creation
        PlayerPrefs.DeleteKey("SelectedMapPath");
        PlayerPrefs.DeleteKey("SelectedMapFile");
        // Hint for creation path: store source bpm in case LevelEditorScene needs it for new map
        PlayerPrefs.SetInt("SelectedSongBPM", song.bpm);
        Debug.Log($"LevelEditorMenu: CREATE new map for '{song.songName}' ({song.bpm} BPM) -> loading Level Editor");
        SceneManager.LoadScene("Level Editor");
    }

    // Alternative entry point if you wire a second button for SimpleMapMaker
    public void SelectSongSimple(SongDataSO song)
    {
        PlayerPrefs.SetString("SelectedSong", song.songName);
        PlayerPrefs.DeleteKey("SelectedMapPath");
        PlayerPrefs.SetInt("SelectedSongBPM", song.bpm);
        SceneManager.LoadScene("SimpleMapMaker");
    }

    private void InitPagePositions()
    {
        for (int i = 0; i < pages.Count; i++)
        {
            RectTransform rt = pages[i].GetComponent<RectTransform>();
            if (rt == null) continue;
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
        if (previousButton != null) previousButton.SetActive(currentPage > 0);
        if (nextButton != null) nextButton.SetActive(currentPage < pages.Count - 1);
    }

    private void UpdatePageButtons()
    {
        if (previousButton != null) previousButton.SetActive(currentPage > 0);
        if (nextButton != null) nextButton.SetActive(currentPage < pages.Count - 1);
    }

    void OnDestroy()
    {
        if (previewSource != null && previewSource.isPlaying) previewSource.Stop();
    }
}
