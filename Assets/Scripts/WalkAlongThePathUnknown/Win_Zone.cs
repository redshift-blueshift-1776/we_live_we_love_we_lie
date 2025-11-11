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
            int sceneID = SceneManager.GetActiveScene().buildIndex;
            if (sceneID == 3) {
                SceneManager.LoadScene(15);
            } else if (sceneID == 16) {
                SceneManager.LoadScene(4);
            } else if (sceneID == 24) {
                SceneManager.LoadScene(1);
            } else {
                SceneManager.LoadScene(0);
            }
        }
    }
}