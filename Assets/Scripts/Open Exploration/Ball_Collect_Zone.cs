using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class Ball_Collect_Zone : MonoBehaviour
{
    //[SerializeField]
    //Game level;
    [SerializeField] public int id;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator collectVisualization() {
        GameObject foundObject = GameObject.Find("Player_Open_Exploration/Canvas/Ball_Collected_Visual");
        if (foundObject != null) {
            Debug.Log("Found Visual");
            foundObject.SetActive(true);
            float duration = 2f;
            float elapsed = 0f;
            Vector3 oldPosition = foundObject.transform.localPosition + new Vector3(0,0,0);
            TMP_Text visualText = foundObject.GetComponentInChildren<TMP_Text>();
            visualText.text = "Ball Challenge\nCompleted!";
            while (elapsed < duration) {
                float t = elapsed / duration;

                foundObject.transform.localPosition = oldPosition + new Vector3(50 * Mathf.Sin(5 * t) + (t - 0.5f) * 1920, 0, 0);
                foundObject.transform.localRotation = Quaternion.Euler(0, 0, -45 * Mathf.Sin(5 * t));

                elapsed += Time.deltaTime;
                yield return null;
            }
            foundObject.transform.localPosition = new Vector3(0, 0, 0);
            foundObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
            foundObject.SetActive(false);
        } else {
            Debug.Log("No Visual");
        }
        yield return null;
    }

    void OnTriggerEnter(Collider col) {
        Debug.Log("collided");
        if(col.gameObject.tag == "OpenExplorationBall") {
            GameObject foundObject2 = GameObject.Find("Universal_Manager");
            if (foundObject2 != null) {
                Debug.Log("Found Universal_Manager");
                Universal_Manager um = foundObject2.GetComponent<Universal_Manager>();
                PlayerPrefs.SetInt("openExplorationBallChallenges" + id, 1);
                um.openExplorationBallChallenges[id] = true;
            } else {
                Debug.Log("No Universal_Manager");
            }
            StartCoroutine(collectVisualization());
        }
    }
}