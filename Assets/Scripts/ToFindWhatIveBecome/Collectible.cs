using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class Collectible : MonoBehaviour
{
    public int id;
    [SerializeField] public GameObject gameManager;
    public ToFindWhatIveBecome gm;

    [SerializeField] public GameObject collectSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(true);
        gm = gameManager.GetComponent<ToFindWhatIveBecome>();
        collectSound.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 90f * Time.deltaTime, 0);
    }

    public IEnumerator collect() {
        int oldUsingAlternate = gm.usingAlternate;
        float duration = 2f;
        float elapsed = 0f;
        gm.CollectItem(id);
        Vector3 oldPosition = transform.position + new Vector3(0,0,0);
        Vector3 targetPosition = transform.position + new Vector3(0,100,0);
        while (elapsed < duration) {
            float t = elapsed / duration;

            transform.position = Vector3.Lerp(oldPosition, targetPosition, t);

            elapsed += Time.deltaTime;
            yield return null;
        }
        // Destroy(gameObject);
        if (!gm.endless) {
            gameObject.SetActive(false);
        } else if (gm.usingAlternate == oldUsingAlternate) {
            transform.position = transform.position + new Vector3(0,-1000,0);
            collectSound.SetActive(false);
        }
    }

    public void Interact() {
        Debug.Log("Interacting");
        collectSound.SetActive(true);
        StartCoroutine(collect());
    }
}
