using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using System.Collections;

public class Level2to3Cutscene : MonoBehaviour
{
    [SerializeField] public PlayableDirector director;

    public static double dspStartTime;

    void Start()
    {
        StartCoroutine(StartCutsceneDSP());
    }

    IEnumerator StartCutsceneDSP()
    {
        yield return null;

        dspStartTime = AudioSettings.dspTime + 0.2;

        director.time = 0;
        director.initialTime = 0;

        director.Play();

        while (AudioSettings.dspTime < dspStartTime)
        {
            yield return null;
        }

        director.playableGraph.GetRootPlayable(0).SetSpeed(1);
    }

    public void goToLevel3()
    {
        SceneManager.LoadScene("Intro to Level 3");
    }

    public void goToDanceSequence()
    {
        SceneManager.LoadScene("DanceSequence");
    }
}