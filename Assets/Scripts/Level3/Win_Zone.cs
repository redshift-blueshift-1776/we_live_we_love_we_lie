using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Win_Zone : MonoBehaviour
{
    //[SerializeField]
    //Game level;

    [SerializeField]
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col) {
        Debug.Log("collided");
        if(col.gameObject == player) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            int nextSceneIndex = 0;
            int sceneID = SceneManager.GetActiveScene().buildIndex;
            if (sceneID == 3) {
                GameObject foundObject2 = GameObject.Find("Universal_Manager");
                if (foundObject2 != null) {
                    Debug.Log("Found Universal_Manager");
                    Universal_Manager um = foundObject2.GetComponent<Universal_Manager>();
                    um.beatStoryModeLevels[2] = true;
                    um.unlockedHard[2] = true;
                    PlayerPrefs.SetInt("beatStoryModeLevels3", 1);
                    PlayerPrefs.SetInt("unlockedHard3", 1);
                } else {
                    Debug.Log("No Universal_Manager");
                }
                GameObject foundObject3 = GameObject.Find("StoryMode");
                // Check if the foundObject is not null
                if (foundObject3 != null)
                {
                    Debug.Log("GameObject '" + "StoryMode" + "' found in the scene.");
                }
                else
                {
                    SceneManager.LoadScene(0); // Not in story mode, goes back to the menu page
                } 
                nextSceneIndex = 41;
                // SceneManager.LoadScene(36);
            } else if (sceneID == 16) {
                nextSceneIndex = 0;
                // SceneManager.LoadScene(0);
            } else if (sceneID == 22) {
                GameObject foundObject2 = GameObject.Find("Universal_Manager");
                if (foundObject2 != null) {
                    Debug.Log("Found Universal_Manager");
                    Universal_Manager um = foundObject2.GetComponent<Universal_Manager>();
                    um.beatHardLevels[2] = true;
                    PlayerPrefs.SetInt("beatHardLevels3", 1);
                } else {
                    Debug.Log("No Universal_Manager");
                }
                nextSceneIndex = 0;
                // SceneManager.LoadScene(0);
            } else if (sceneID == 24) {
                nextSceneIndex = 25;
                // SceneManager.LoadScene(25);
            } else if (sceneID == 27) {
                nextSceneIndex = 29;
                // SceneManager.LoadScene(29);
            } else if (sceneID == 32) {
                nextSceneIndex = 33;
                // SceneManager.LoadScene(33);
            } else if (sceneID == 36) {
                nextSceneIndex = 38;
                // SceneManager.LoadScene(15);
            } else if (sceneID == 39) {
                nextSceneIndex = 20;
            } else if (sceneID == 42) {
                nextSceneIndex = 43;
            } else if (sceneID == 45) {
                nextSceneIndex = 46;
            } else {
                nextSceneIndex = 0;
                // SceneManager.LoadScene(0);
            }
            GameObject foundObject = GameObject.Find("TransitionCanvas");
            // Check if the foundObject is not null
            if (foundObject != null) {
                Transition t = foundObject.GetComponent<Transition>();
                t.ToSpecified(nextSceneIndex);
            } else {
                SceneManager.LoadScene(nextSceneIndex);
            }
            
        }
    }
}