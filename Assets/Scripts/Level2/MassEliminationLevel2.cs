using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

public class MassEliminationLevel2 : MonoBehaviour
{
    [SerializeField] public bool menu;
    [SerializeField] public bool audioBlend;
    [SerializeField] public AudioBlender ab;
    [SerializeField] public float timeBetween = 0.1f;
    [SerializeField] public GameObject pillar;
    [SerializeField] public GameObject transition;

    [SerializeField] public GameObject camera1;

    [SerializeField] public TMP_Text playersRemaining;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int usePostProcessing = PlayerPrefs.GetInt("useVisualEffects", 0);
            if (usePostProcessing == 0) {
                UniversalAdditionalCameraData cameraData = Camera.main.GetUniversalAdditionalCameraData();
                cameraData.renderPostProcessing = false;
            } else {
                UniversalAdditionalCameraData cameraData = Camera.main.GetUniversalAdditionalCameraData();
                cameraData.renderPostProcessing = true;
            }
            playersRemaining.text = "Players Remaining:\n276";
            MakeGrid();
            StartCoroutine(MoveCamera());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator MoveCamera() {
        yield return new WaitForSeconds(3f);
        float duration = 6.7f;
        float elapsed = 0f;
        Quaternion rotationStart = camera1.transform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(0, 0, 0);
        while (elapsed < duration) {
            camera1.transform.position += new Vector3(0, -1.69f * Time.deltaTime, 101 * Time.deltaTime);
            camera1.transform.localRotation = Quaternion.Slerp(rotationStart, targetRotation, elapsed / duration);
            playersRemaining.text = $"Players Remaining:\n{(int) Mathf.Lerp(276, 163, elapsed / duration)}";
            if (audioBlend) {
                ab.SetRatio(Mathf.Clamp01(elapsed / duration));
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(37);
    }

    public void MakeGrid() {
        // Make a square grid of size gridSize with squares of side squareSize
        // clear old children if rerun
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Center grid at (0,0)
        for (int x = 0; x < 3; x++)
        {
            for (int z = 0; z < 38; z++)
            {
                if (x == 1 || z != 0) {
                    GameObject square = Instantiate(pillar);
                    square.transform.SetParent(transform);

                    square.transform.localScale = new Vector3(1f, 1f, 1f);

                    float worldX = x * 20 - 20;
                    float worldZ = z * 20 + 20;

                    square.transform.localPosition = new Vector3(worldX, 0f, worldZ);
                    EliminationPillar ep = square.GetComponent<EliminationPillar>();
                    ep.transition = transition;
                    ep.eliminate = false;
                    ep.drop = true;
                    ep.delay = 2f + (Mathf.Abs(x - 1) + z) / 5f;
                }
            }
        }
    }
}
