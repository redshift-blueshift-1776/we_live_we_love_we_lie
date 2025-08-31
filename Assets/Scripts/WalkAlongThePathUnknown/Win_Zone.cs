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
        if(col.gameObject == player) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            int sceneID = SceneManager.GetActiveScene().buildIndex;
            if (sceneID == 1) {
                SceneManager.LoadScene(0);
            }
            if (sceneID == 3) {
                SceneManager.LoadScene(0);
            }
            if (sceneID == 5) {
                SceneManager.LoadScene(7);
            }
            if (sceneID == 7) {
                SceneManager.LoadScene(9);
            }
            if (sceneID == 9) {
                SceneManager.LoadScene(11);
            }
            if (sceneID == 11) {
                SceneManager.LoadScene(13);
            }
            if (sceneID == 11) {
                SceneManager.LoadScene(13);
            }
            if (sceneID == 13) {
                SceneManager.LoadScene(15);
            }
            if (sceneID == 15) {
                SceneManager.LoadScene(17);
            }
            if (sceneID == 17) {
                SceneManager.LoadScene(19);
            }
        }
    }
}