using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class Open_Exploration_Collectible : MonoBehaviour
{
    [SerializeField] public int id;

    [SerializeField] public GameObject collectSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(true);
        collectSound.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 90f * Time.deltaTime, 0);
    }

    public IEnumerator collect() {
        float duration = 2f;
        float elapsed = 0f;
        Vector3 oldPosition = transform.position + new Vector3(0,0,0);
        Vector3 targetPosition = transform.position + new Vector3(0,100,0);
        while (elapsed < duration) {
            float t = elapsed / duration;

            transform.position = Vector3.Lerp(oldPosition, targetPosition, t);

            elapsed += Time.deltaTime;
            yield return null;
        }
        // Destroy(gameObject);
        GameObject foundObject2 = GameObject.Find("Universal_Manager");
        if (foundObject2 != null) {
            Debug.Log("Found Universal_Manager");
            Universal_Manager um = foundObject2.GetComponent<Universal_Manager>();
            PlayerPrefs.SetInt("openExplorationCollectibles" + id, 1);
            um.openExplorationCollectibles[id] = true;
        } else {
            Debug.Log("No Universal_Manager");
        }
        gameObject.SetActive(false);
    }

    public void Interact() {
        Debug.Log("Interacting");
        collectSound.SetActive(false);
        collectSound.SetActive(true);
        StartCoroutine(collect());
    }
}
