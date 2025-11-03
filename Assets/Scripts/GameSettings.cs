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
    private bool gameEnded;
    private Stack<string> settingStack;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inSettings = false;
        gameEnded = false;
        settingStack = new Stack<string>();

        mouseSettingsButton.onClick.AddListener(() =>
        {
            Debug.Log("mouse");
            settingStack.Push("Mouse");
        });
    }

    // Update is called once per frame
    void Update()
    {   
        if (gameEnded)
        {
            settingStack.Clear();
            inSettings = false;
            setPage();
            return;
        }

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

    public bool getGameEnded()
    {
        return gameEnded;
    }

    public void setGameEnded(bool b)
    {
        gameEnded = b;
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
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            AudioListener.pause = false;
        } else
        {
            inSettings = true;
            AudioListener.pause = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
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
