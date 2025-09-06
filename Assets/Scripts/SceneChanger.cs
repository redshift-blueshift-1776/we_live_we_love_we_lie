using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class SceneChanger : MonoBehaviour
{
    public static string lvl_name;
    public static TimeSpan lvl_time;

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MoveInMenu");
    }
    
    public void LoadTutorialScene()
    {
        SceneManager.LoadScene("Level 0");
    }

    public void LoadLevel1()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene("Level 2");
    }

    public void LoadLevelSelect()
    {
        SceneManager.LoadScene("Level Select");
    }

    public void LoadCredits()
    {
        SceneManager.LoadScene("MoveInCredits");
    }

    public void LoadHowToPlay()
    {
        SceneManager.LoadScene("MoveInHowToPlay");
    }

    public void LoadLevel3()
    {
        SceneManager.LoadScene("Level 3");
    }

    public void LoadLevel4()
    {
        SceneManager.LoadScene("Level 4");
    }

    public void LoadLevel5()
    {
        SceneManager.LoadScene("Level 5");
    }

    public void LoadLevel7()
    {
        SceneManager.LoadScene("Level 7");
    }

    public void LoadLevel6()
    {
        SceneManager.LoadScene("Level 6");
    }

    public void Settings()
    {
        SceneManager.LoadScene("MoveInSettings");
    }

    public void Secret()
    {
        SceneManager.LoadScene(30);
    }

    public void Secret2()
    {
        SceneManager.LoadScene(32);
    }

    public void LoadSpecifiedLevel() {
        SceneManager.LoadScene(lvl_name);
    }

    public void LoadPreviousLevel() {
        int n = PlayerPrefs.GetInt("PreviousLevel", 1);
        SceneManager.LoadScene(n);
    }

    public void LoadSceneByNumber(int num) {
        SceneManager.LoadScene(num);
    }
}
