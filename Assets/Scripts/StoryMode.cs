using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryMode : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.buildIndex == 0) {
            Destroy(this.gameObject);
        }
    }
}
