using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Death_Zone : MonoBehaviour
{
    [SerializeField] private GameObject transition;
    [SerializeField] private Transition transitionScript;
    // Start is called before the first frame update
    void Start()
    {
        transitionScript = transition.GetComponent<Transition>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col) {
        if(col.gameObject.name.Contains("Block")) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            // transitionScript.ToFail();
            // PlayerPrefs.SetInt("PreviousLevel", 1);
            Scene currentScene = SceneManager.GetActiveScene();
            PlayerPrefs.SetInt("PreviousLevel", currentScene.buildIndex);
            SceneManager.LoadScene(9); // Change once official scene is made
        }
        if(col.gameObject.name.Contains("Body")) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            // transitionScript.ToFail();
            // PlayerPrefs.SetInt("PreviousLevel", 1);
            Scene currentScene = SceneManager.GetActiveScene();
            PlayerPrefs.SetInt("PreviousLevel", currentScene.buildIndex);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene(9); // Change once official scene is made
        }
    }
}