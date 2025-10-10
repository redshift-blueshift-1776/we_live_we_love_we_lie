using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameSettings : MonoBehaviour
{
    public GameObject settings;
    public GameObject homePage;
    public GameObject mouseSettingsPage;

    public Button mouseSettingsButton;

    private bool inSettings;
    private Stack<string> settingStack;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inSettings = false;
        settingStack = new Stack<string>();
        Debug.Log(mouseSettingsButton != null ? "Mouse Settings Button found" : "Mouse Settings Button is NULL!");

        mouseSettingsButton.onClick.AddListener(() =>
        {
            settingStack.Push("Mouse");
            Debug.Log("Pushed Mouse; stack count = " + settingStack.Count);
        });


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!inSettings)
            {
                settingStack.Push("Home");
            } else
            {
                settingStack.Pop();
            }
        }
        setPage();
    }

    private void disableAllPages()
    {
        homePage.SetActive(false);
        mouseSettingsPage.SetActive(false);
    }

    private void setPage()
    {
        if (settingStack.Count == 0)
        {
            inSettings = false;
            Time.timeScale = 1f;
            AudioListener.pause = false;
        } else
        {
            inSettings = true;
            Time.timeScale = 0f;
            AudioListener.pause = true;

            string page = settingStack.Peek();
            switch (page)
            {
                case "Home":
                    disableAllPages();
                    homePage.SetActive(true);
                    break;
                case "Mouse":
                    disableAllPages();
                    mouseSettingsPage.SetActive(true);
                    break;
            }
        }
        settings.SetActive(inSettings);
    }

    public bool isInSettings()
    {
        return inSettings;
    }
}
