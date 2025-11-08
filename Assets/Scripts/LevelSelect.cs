using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    [Header("Page Setup")]
    [SerializeField] public GameObject[] pages; // 8 pages, each with its own buttons and background
    [SerializeField] public RectTransform pageContainer; // parent holding all pages
    [SerializeField] public float slideDuration = 0.5f;
    [SerializeField] public float pageWidth = 1920f;
    [SerializeField] public GameObject previousButton;
    [SerializeField] public GameObject nextButton;

    [Header("Unlocked Data")]
    [SerializeField] public bool[] unlockedEasy;
    [SerializeField] public bool[] unlockedHard;
    [SerializeField] public bool[] unlockedEndless;

    private int currentPage = 0;
    private bool isSliding = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // currentPage = 0;

        // // Safety checks — prevent null/short arrays
        // if (easyButtons == null || easyButtons.Length < 8 ||
        //     hardButtons == null || hardButtons.Length < 8 ||
        //     endlessButtons == null || endlessButtons.Length < 8)
        // {
        //     Debug.LogError("LevelSelectManager: One or more button arrays are not properly assigned in the Inspector!");
        //     return;
        // }

        // if (unlockedEasy == null || unlockedEasy.Length < 8)
        //     unlockedEasy = new bool[8];
        // if (unlockedHard == null || unlockedHard.Length < 8)
        //     unlockedHard = new bool[8];
        // if (unlockedEndless == null || unlockedEndless.Length < 8)
        //     unlockedEndless = new bool[8];

        // for (int i = 0; i < 8; i++)
        // {
        //     easyButtons[i].SetActive(true);
        //     hardButtons[i].SetActive(true);
        //     endlessButtons[i].SetActive(true);

        //     easyButtons[i].transform.localPosition = new Vector3(-420, -420, 0);
        //     hardButtons[i].transform.localPosition = new Vector3(0, -420, 0);
        //     endlessButtons[i].transform.localPosition = new Vector3(420, -420, 0);

        //     easyButtons[i].SetActive(unlockedEasy[i]);
        //     hardButtons[i].SetActive(unlockedHard[i]);
        //     endlessButtons[i].SetActive(unlockedEndless[i]);
        // }

        // easyButtons[0].SetActive(true);
        // Initialize all pages' positions and visibility
        for (int i = 0; i < pages.Length; i++)
        {
            RectTransform rt = pages[i].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2((i - currentPage) * pageWidth, 0);
            pages[i].SetActive(i == currentPage || i == currentPage + 1 || i == currentPage - 1);
        }

        UpdateButtonsForPage(currentPage);
        if (currentPage > 0) {
            UpdateButtonsForPage(currentPage - 1);
        }
        if (currentPage < 7) {
            UpdateButtonsForPage(currentPage + 1);
        }
        previousButton.SetActive(currentPage > 0);
        nextButton.SetActive(currentPage < pages.Length - 1);
    }

    public void OnNextPage()
    {
        if (isSliding || currentPage >= pages.Length - 1) return;
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

        // Cache start positions for all active pages
        Vector2[] startPositions = new Vector2[pages.Length];
        for (int i = 0; i < pages.Length; i++)
        {
            RectTransform rt = pages[i].GetComponent<RectTransform>();
            startPositions[i] = rt.anchoredPosition;
            // Pre-activate neighbors so they slide in smoothly
            if (i == newPage || i == currentPage)
                pages[i].SetActive(true);
        }

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / slideDuration);

            for (int i = 0; i < pages.Length; i++)
            {
                RectTransform rt = pages[i].GetComponent<RectTransform>();
                rt.anchoredPosition = Vector2.Lerp(startPositions[i],
                    startPositions[i] - new Vector2(direction * pageWidth, 0), t);
            }

            yield return null;
        }

        // Snap to exact positions
        for (int i = 0; i < pages.Length; i++)
        {
            RectTransform rt = pages[i].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2((i - newPage) * pageWidth, 0);
            // Only keep visible ones active
            pages[i].SetActive(i == newPage || i == newPage + 1 || i == newPage - 1);
        }

        currentPage = newPage;
        UpdateButtonsForPage(currentPage);
        if (currentPage > 0) {
            UpdateButtonsForPage(currentPage - 1);
        }
        if (currentPage < 7) {
            UpdateButtonsForPage(currentPage + 1);
        }
        isSliding = false;
        previousButton.SetActive(currentPage > 0);
        nextButton.SetActive(currentPage < pages.Length - 1);
    }

    void UpdateButtonsForPage(int pageIndex)
    {
        // Example logic: enable/disable based on unlock arrays
        Transform easyBtn = pages[pageIndex].transform.Find("EasyButton");
        Transform hardBtn = pages[pageIndex].transform.Find("HardButton");
        Transform endlessBtn = pages[pageIndex].transform.Find("EndlessButton");

        if (easyBtn != null) easyBtn.gameObject.SetActive(unlockedEasy[pageIndex]);
        if (hardBtn != null) hardBtn.gameObject.SetActive(unlockedHard[pageIndex]);
        if (endlessBtn != null) endlessBtn.gameObject.SetActive(unlockedEndless[pageIndex]);
    }
}
