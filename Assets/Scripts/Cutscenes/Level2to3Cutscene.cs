using UnityEngine;
using UnityEngine.SceneManagement;

public class Level2to3Cutscene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void goToLevel3()
    {
        SceneManager.LoadScene("Intro to Level 3");
    }
}
