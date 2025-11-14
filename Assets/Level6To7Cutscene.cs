using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class Level6To7Cutscene : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    void Start()
    {
        
    }

    void Update()
    {
    }

    public void goToLevel7()
    {
        SceneManager.LoadScene("Level 7");
    }
    
}
