using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

public class MainMenu : MonoBehaviour
{
    [SerializeField] public float timeBetween = 0.1f;
    [SerializeField] public GameObject pillar;
    [SerializeField] public GameObject transition;

    [SerializeField] public GameObject weLive;
    [SerializeField] public GameObject weLove;
    [SerializeField] public GameObject weLie;

    [SerializeField] public TMP_Text weLiveText;
    [SerializeField] public TMP_Text weLoveText;
    [SerializeField] public TMP_Text weLieText;

    private RectTransform textRectTransform;
    private Vector2 startAnchoredPos;
    private RectTransform textRectTransform2;
    private Vector2 startAnchoredPos2;
    private RectTransform textRectTransform3;
    private Vector2 startAnchoredPos3;
        
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textRectTransform = weLive.GetComponent<RectTransform>();
        startAnchoredPos = textRectTransform.anchoredPosition;
        textRectTransform2 = weLove.GetComponent<RectTransform>();
        startAnchoredPos2 = textRectTransform2.anchoredPosition;
        textRectTransform3 = weLie.GetComponent<RectTransform>();
        startAnchoredPos3 = textRectTransform3.anchoredPosition;
        StartCoroutine(MenuLoop());
        StartCoroutine(DoTextStuff());
        
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        int usePostProcessing = PlayerPrefs.GetInt("useVisualEffects", 0);
        if (usePostProcessing == 0) {
            UniversalAdditionalCameraData cameraData = Camera.main.GetUniversalAdditionalCameraData();
            cameraData.renderPostProcessing = false;
        } else {
            UniversalAdditionalCameraData cameraData = Camera.main.GetUniversalAdditionalCameraData();
            cameraData.renderPostProcessing = true;
        }
    }

    public IEnumerator DoTextStuff() {
        float elapsed = 0f;
        while (true) {
            textRectTransform.anchoredPosition = startAnchoredPos + new Vector2(0, 20f * Mathf.Sin(elapsed));
            weLiveText.color = new Color(weLiveText.color.r, weLiveText.color.g, weLiveText.color.b, 0.9f + 0.1f * Mathf.Cos(elapsed));
            textRectTransform2.anchoredPosition = startAnchoredPos2 + new Vector2(0, 20f * Mathf.Sin(elapsed / 1.5f));
            weLoveText.color = new Color(weLoveText.color.r, weLoveText.color.g, weLoveText.color.b, 0.9f + 0.1f * Mathf.Cos(elapsed / 1.5f));
            textRectTransform3.anchoredPosition = startAnchoredPos3 + new Vector2(0, 20f * Mathf.Sin(elapsed / 2f));
            weLieText.color = new Color(weLieText.color.r, weLieText.color.g, weLieText.color.b, 0.9f + 0.1f * Mathf.Cos(elapsed / 2f));
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator MenuLoop() {
        while (true) {
            MakeGrid();
            yield return new WaitForSeconds(16f);
        }
    }

    public void MakeGrid() {
        // Make a square grid of size gridSize with squares of side squareSize
        // clear old children if rerun
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Center grid at (0,0)
        for (int x = 0; x < 20; x++)
        {
            for (int z = 0; z < 50; z++)
            {
                GameObject square = Instantiate(pillar);
                square.transform.SetParent(transform);

                square.transform.localScale = new Vector3(1f, 1f, 1f);

                float worldX = x * 20 - 190;
                float worldZ = z * 20;

                square.transform.localPosition = new Vector3(worldX, 0f, worldZ);
                EliminationPillar ep = square.GetComponent<EliminationPillar>();
                ep.transition = transition;
                ep.eliminate = false;
                ep.drop = true;
                ep.delay = 2f + (x + z) * timeBetween;
            }
        }
    }
}
