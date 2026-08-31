using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class SelectMapMenu : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject pagePrefab;
    public GameObject songEntryPrefab;

    [Header("Page Settings")]
    public RectTransform pageContainer;
    public float slideDuration = 0.5f;
    public float pageWidth = 1920f;
    public int songsPerPage = 4;

    [Header("Buttons")]
    public GameObject previousButton;
    public GameObject nextButton;

    private List<GameObject> pages = new List<GameObject>();
    private SongDataSO[] songs;

    private int currentPage = 0;
    private bool isSliding = false;

    public int numPages;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Load song data automatically
        songs = Resources.LoadAll<SongDataSO>("Songs");

        GeneratePages();
        InitPagePositions();
        UpdatePageButtons();
    }

    private void GeneratePages()
    {
        numPages = Mathf.CeilToInt((float)songs.Length / songsPerPage);

        for (int p = 0; p < numPages; p++)
        {
            GameObject page = Instantiate(pagePrefab, pageContainer);
            pages.Add(page);

            // Fill page with song entries
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

        ui.nameText.text = data.songName;
        ui.bpmText.text = $"{data.bpm} BPM";
        ui.genreText.text = data.genre;
        ui.timeSigText.text = data.timeSignature;
        ui.lengthText.text = $"{data.approxLengthInSeconds}s";

        if (ui.coverArtImage != null && data.coverArt != null) {
            ui.coverArtImage.sprite = data.coverArt;
        }

        ui.previewButton.onClick.AddListener(() =>
        {
            PlayPreview(data);
        });

        ui.selectButton.onClick.AddListener(() =>
        {
            SelectSong(data);
        });
    }

    private void PlayPreview(SongDataSO song)
    {
        Debug.Log($"Previewing: {song.songName}");
    }

    private void SelectSong(SongDataSO song)
    {
        PlayerPrefs.SetString("SelectedSong", song.songName);
        Debug.Log($"Selected: {song.songName}");
        SceneManager.LoadScene("Level Editor");
    }

    // Initialize page positions
    private void InitPagePositions()
    {
        for (int i = 0; i < pages.Count; i++)
        {
            RectTransform rt = pages[i].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2((i - currentPage) * pageWidth, 0);
            pages[i].SetActive(i == currentPage);
        }
    }

    public void OnNextPage()
    {
        if (isSliding || currentPage >= pages.Count - 1) return;
        StartCoroutine(SlideToPage(currentPage + 1));
    }

    public void OnPrevPage()
    {
        if (isSliding || currentPage <= 0) return;
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

        // Snap to exact positions
        for (int i = 0; i < pages.Count; i++)
        {
            RectTransform rt = pages[i].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2((i - newPage) * pageWidth, 0);
            // Only keep visible ones active
            pages[i].SetActive(i == newPage || i == newPage + 1 || i == newPage - 1);
        }

        currentPage = newPage;
        isSliding = false;
        previousButton.SetActive(currentPage > 0);
        nextButton.SetActive(currentPage < pages.Count - 1);
    }

    private void UpdatePageButtons()
    {
        previousButton.SetActive(currentPage > 0);
        nextButton.SetActive(currentPage < pages.Count - 1);
    }
}
