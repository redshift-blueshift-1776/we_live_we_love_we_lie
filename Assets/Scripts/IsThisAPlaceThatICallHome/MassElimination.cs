using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class MassElimination : MonoBehaviour
{
    [SerializeField] public float timeBetween = 0.1f;
    [SerializeField] public GameObject pillar;
    [SerializeField] public GameObject transition;

    [SerializeField] public GameObject camera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MakeGrid();
        StartCoroutine(MoveCamera());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator MoveCamera() {
        yield return new WaitForSeconds(3f);
        float duration = 13f;
        float elapsed = 0f;
        Quaternion rotationStart = camera.transform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(0, 0, 0);
        while (elapsed < duration) {
            camera.transform.position += new Vector3(0, -1.5f * Time.deltaTime, 200 * Time.deltaTime);
            camera.transform.localRotation = Quaternion.Slerp(rotationStart, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(2);
    }

    public void MakeGrid() {
        // Make a square grid of size gridSize with squares of side squareSize
        // clear old children if rerun
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Center grid at (0,0)
        for (int x = 0; x < 5; x++)
        {
            for (int z = 0; z < 145; z++)
            {
                GameObject square = Instantiate(pillar);
                square.transform.SetParent(transform);

                square.transform.localScale = new Vector3(1f, 1f, 1f);

                float worldX = x * 20 - 40;
                float worldZ = z * 20;

                square.transform.localPosition = new Vector3(worldX, 0f, worldZ);

                EliminationPillar ep = square.GetComponent<EliminationPillar>();
                ep.transition = transition;
                ep.eliminate = false;
                ep.drop = true;
                ep.delay = 2f + (Mathf.Abs(x - 2) + z) / 10f;
            }
        }
    }
}
