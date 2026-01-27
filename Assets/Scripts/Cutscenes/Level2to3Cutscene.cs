using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using System.Collections.Generic;
using System.Collections;

public class Level2to3Cutscene : MonoBehaviour
{
    [SerializeField] public PlayableDirector director;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(startCutscene());
    }

    public IEnumerator startCutscene() {
        yield return new WaitForSeconds(0.5f);
        director.time = 0;
    director.Play();

    // Force timeline evaluation to align with audio DSP
    director.playableGraph.GetRootPlayable(0)
        .SetSpeed(1);

    director.initialTime = 0;
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
