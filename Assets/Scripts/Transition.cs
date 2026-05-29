using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour
{
    [SerializeField] public GameObject rightWall;
    [SerializeField] public GameObject leftWall;
    [SerializeField] public GameObject topWall;
    [SerializeField] public GameObject bottomWall;
    [SerializeField] public GameObject transitionSound;
    [SerializeField] public GameObject failSound;
    [SerializeField] public List<GameObject> toDisable;

    private Coroutine currentCoroutine;

    public bool skipTransition = false;

    private Universal_Manager um;

    // Start is called before the first frame update
    void Start()
    {
        transitionSound.SetActive(false);
        failSound.SetActive(false);
        currentCoroutine = null;
        rightWall.SetActive(false);
        leftWall.SetActive(false);
        topWall.SetActive(false);
        bottomWall.SetActive(false);
        GameObject foundObject2 = GameObject.Find("Universal_Manager");
        if (foundObject2 != null) {
            um = foundObject2.GetComponent<Universal_Manager>();
        } else {
            Debug.Log("No Universal_Manager");
            skipTransition = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (um != null)
        {
            skipTransition = um.skipTransitions;
        }
    }

    // public IEnumerator LoadSceneTransition(int sceneIndex)
    // {
    //     Cursor.lockState = CursorLockMode.None;
    //     Cursor.visible = true;

    //     AsyncOperation asyncLoad =
    //         SceneManager.LoadSceneAsync(sceneIndex);

    //     asyncLoad.allowSceneActivation = false;

    //     if (!skipTransition)
    //     {
    //         yield return StartCoroutine(PlayTransitionAnimation());
    //     }

    //     while (asyncLoad.progress < 0.9f)
    //     {
    //         yield return null;
    //     }

    //     asyncLoad.allowSceneActivation = true;
    //     currentCoroutine = null;
    // }

    public IEnumerator LoadLevel1() {
        AsyncOperation asyncLoad =
            SceneManager.LoadSceneAsync(23);

        asyncLoad.allowSceneActivation = false;

        if (!skipTransition)
        {
            rightWall.SetActive(true);
            leftWall.SetActive(true);
            topWall.SetActive(true);
            bottomWall.SetActive(true);

            transitionSound.SetActive(true);
            foreach (GameObject g in toDisable) {
                g.SetActive(false);
            }
            yield return new WaitForSeconds(2f);
            float duration = 1f;
            float elapsed = 0f;
            Vector3 ogRWpos = new Vector3(rightWall.transform.localPosition.x, rightWall.transform.localPosition.y, rightWall.transform.localPosition.z);
            Vector3 ogLWpos = new Vector3(leftWall.transform.localPosition.x, leftWall.transform.localPosition.y, leftWall.transform.localPosition.z);
            Vector3 ogTWpos = new Vector3(topWall.transform.localPosition.x, topWall.transform.localPosition.y, topWall.transform.localPosition.z);
            Vector3 ogBWpos = new Vector3(bottomWall.transform.localPosition.x, bottomWall.transform.localPosition.y, bottomWall.transform.localPosition.z);
            while (elapsed < duration) {
                float t = elapsed / duration;
                rightWall.transform.localPosition = Vector3.Lerp(ogRWpos, ogRWpos / 2f, t * t * t);
                bottomWall.transform.localPosition = Vector3.Lerp(ogBWpos, ogBWpos / 2f, t * t * t);
                leftWall.transform.localPosition = Vector3.Lerp(ogLWpos, ogLWpos / 2f, t * t * t);
                topWall.transform.localPosition = Vector3.Lerp(ogTWpos, ogTWpos / 2f, t * t * t);
                elapsed += Time.deltaTime;
                yield return null;
            }
            elapsed = 0f;
            ogRWpos = new Vector3(rightWall.transform.localPosition.x, rightWall.transform.localPosition.y, rightWall.transform.localPosition.z);
            ogLWpos = new Vector3(leftWall.transform.localPosition.x, leftWall.transform.localPosition.y, leftWall.transform.localPosition.z);
            ogTWpos = new Vector3(topWall.transform.localPosition.x, topWall.transform.localPosition.y, topWall.transform.localPosition.z);
            ogBWpos = new Vector3(bottomWall.transform.localPosition.x, bottomWall.transform.localPosition.y, bottomWall.transform.localPosition.z);
            while (elapsed < duration) {
                float t = elapsed / duration;
                rightWall.transform.localPosition = Vector3.Lerp(ogRWpos, new Vector3(0f, 0f, 0f), t * t * t);
                bottomWall.transform.localPosition = Vector3.Lerp(ogBWpos, new Vector3(0f, 0f, 0f), t * t * t);
                topWall.transform.localPosition = Vector3.Lerp(ogTWpos, new Vector3(0f, 0f, 0f), t * t * t);
                leftWall.transform.localPosition = Vector3.Lerp(ogLWpos, new Vector3(0f, 0f, 0f), t * t * t);
                elapsed += Time.deltaTime;
                yield return null;
            }
            rightWall.transform.localPosition = new Vector3(0f, 0f, 0f);
            bottomWall.transform.localPosition = new Vector3(0f, 0f, 0f);
            leftWall.transform.localPosition = new Vector3(0f, 0f, 0f);
            topWall.transform.localPosition = new Vector3(0f, 0f, 0f);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        asyncLoad.allowSceneActivation = true;
        yield return null;
        currentCoroutine = null;
    }

    public void ToLevel1() {
        if (currentCoroutine == null) {
            currentCoroutine = StartCoroutine(LoadLevel1());
        }
    }

    public IEnumerator LoadLevelSelect() {
        AsyncOperation asyncLoad =
            SceneManager.LoadSceneAsync(8);

        asyncLoad.allowSceneActivation = false;

        if (!skipTransition)
        {
            rightWall.SetActive(true);
            leftWall.SetActive(true);
            topWall.SetActive(true);
            bottomWall.SetActive(true);

            transitionSound.SetActive(true);
            foreach (GameObject g in toDisable) {
                g.SetActive(false);
            }
            // yield return new WaitForSeconds(2f);
            float duration = 2f;
            float elapsed = 0f;
            Vector3 ogRWpos = new Vector3(rightWall.transform.localPosition.x, rightWall.transform.localPosition.y, rightWall.transform.localPosition.z);
            Vector3 ogLWpos = new Vector3(leftWall.transform.localPosition.x, leftWall.transform.localPosition.y, leftWall.transform.localPosition.z);
            Vector3 ogTWpos = new Vector3(topWall.transform.localPosition.x, topWall.transform.localPosition.y, topWall.transform.localPosition.z);
            Vector3 ogBWpos = new Vector3(bottomWall.transform.localPosition.x, bottomWall.transform.localPosition.y, bottomWall.transform.localPosition.z);
            while (elapsed < duration) {
                float t = elapsed / duration;
                rightWall.transform.localPosition = Vector3.Lerp(ogRWpos, new Vector3(0f, 0f, 0f), t * t * t * t);
                leftWall.transform.localPosition = Vector3.Lerp(ogLWpos, new Vector3(0f, 0f, 0f), t * t * t * t);
                topWall.transform.localPosition = Vector3.Lerp(ogTWpos, new Vector3(0f, 0f, 0f), t * t * t * t);
                bottomWall.transform.localPosition = Vector3.Lerp(ogBWpos, new Vector3(0f, 0f, 0f), t * t * t * t);
                elapsed += Time.deltaTime;
                yield return null;
            }
            rightWall.transform.localPosition = new Vector3(0f, 0f, 0f);
            leftWall.transform.localPosition = new Vector3(0f, 0f, 0f);
            topWall.transform.localPosition = new Vector3(0f, 0f, 0f);
            bottomWall.transform.localPosition = new Vector3(0f, 0f, 0f);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        asyncLoad.allowSceneActivation = true;
        yield return null;
        currentCoroutine = null;
    }

    public void ToLevelSelect() {
        if (currentCoroutine == null) {
            currentCoroutine = StartCoroutine(LoadLevelSelect());
        }
    }

    public IEnumerator LoadMenu() {
        AsyncOperation asyncLoad =
            SceneManager.LoadSceneAsync(0);

        asyncLoad.allowSceneActivation = false;

        if (!skipTransition)
        {
            rightWall.SetActive(true);
            leftWall.SetActive(true);
            topWall.SetActive(true);
            bottomWall.SetActive(true);

            transitionSound.SetActive(true);
            foreach (GameObject g in toDisable) {
                g.SetActive(false);
            }
            // yield return new WaitForSeconds(2f);
            float duration = 2f;
            float elapsed = 0f;
            Vector3 ogRWpos = new Vector3(rightWall.transform.localPosition.x, rightWall.transform.localPosition.y, rightWall.transform.localPosition.z);
            Vector3 ogLWpos = new Vector3(leftWall.transform.localPosition.x, leftWall.transform.localPosition.y, leftWall.transform.localPosition.z);
            Vector3 ogTWpos = new Vector3(topWall.transform.localPosition.x, topWall.transform.localPosition.y, topWall.transform.localPosition.z);
            Vector3 ogBWpos = new Vector3(bottomWall.transform.localPosition.x, bottomWall.transform.localPosition.y, bottomWall.transform.localPosition.z);
            while (elapsed < duration) {
                float t = elapsed / duration;
                rightWall.transform.localPosition = Vector3.Lerp(ogRWpos, new Vector3(0f, 0f, 0f), t);
                leftWall.transform.localPosition = Vector3.Lerp(ogLWpos, new Vector3(0f, 0f, 0f), t);
                topWall.transform.localPosition = Vector3.Lerp(ogTWpos, new Vector3(0f, 0f, 0f), t);
                bottomWall.transform.localPosition = Vector3.Lerp(ogBWpos, new Vector3(0f, 0f, 0f), t);
                elapsed += Time.deltaTime;
                yield return null;
            }
            rightWall.transform.localPosition = new Vector3(0f, 0f, 0f);
            leftWall.transform.localPosition = new Vector3(0f, 0f, 0f);
            topWall.transform.localPosition = new Vector3(0f, 0f, 0f);
            bottomWall.transform.localPosition = new Vector3(0f, 0f, 0f);
        }
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        asyncLoad.allowSceneActivation = true;
        yield return null;
        currentCoroutine = null;
    }

    public void ToMenu() {
        if (currentCoroutine == null) {
            currentCoroutine = StartCoroutine(LoadMenu());
        }
    }

    public IEnumerator LoadPrevious() {
        int n = PlayerPrefs.GetInt("PreviousLevel", 0);

        AsyncOperation asyncLoad =
            SceneManager.LoadSceneAsync(n);

        asyncLoad.allowSceneActivation = false;

        if (!skipTransition)
        {
            rightWall.SetActive(true);
            leftWall.SetActive(true);
            topWall.SetActive(true);
            bottomWall.SetActive(true);
            
            transitionSound.SetActive(true);
            foreach (GameObject g in toDisable) {
                g.SetActive(false);
            }
            yield return new WaitForSeconds(2f);
            float duration = 1f;
            float elapsed = 0f;
            Vector3 ogRWpos = new Vector3(rightWall.transform.localPosition.x, rightWall.transform.localPosition.y, rightWall.transform.localPosition.z);
            Vector3 ogLWpos = new Vector3(leftWall.transform.localPosition.x, leftWall.transform.localPosition.y, leftWall.transform.localPosition.z);
            Vector3 ogTWpos = new Vector3(topWall.transform.localPosition.x, topWall.transform.localPosition.y, topWall.transform.localPosition.z);
            Vector3 ogBWpos = new Vector3(bottomWall.transform.localPosition.x, bottomWall.transform.localPosition.y, bottomWall.transform.localPosition.z);
            while (elapsed < duration) {
                float t = elapsed / duration;
                // rightWall.transform.localPosition = Vector3.Lerp(ogRWpos, ogRWpos / 3f, t * t * t);
                bottomWall.transform.localPosition = Vector3.Lerp(ogBWpos, ogBWpos / 1.5f, t * t * t);
                // leftWall.transform.localPosition = Vector3.Lerp(ogLWpos, ogLWpos / 3f, t * t * t);
                topWall.transform.localPosition = Vector3.Lerp(ogTWpos, ogTWpos / 1.5f, t * t * t);
                elapsed += Time.deltaTime;
                yield return null;
            }
            elapsed = 0f;
            ogRWpos = new Vector3(rightWall.transform.localPosition.x, rightWall.transform.localPosition.y, rightWall.transform.localPosition.z);
            ogLWpos = new Vector3(leftWall.transform.localPosition.x, leftWall.transform.localPosition.y, leftWall.transform.localPosition.z);
            ogTWpos = new Vector3(topWall.transform.localPosition.x, topWall.transform.localPosition.y, topWall.transform.localPosition.z);
            ogBWpos = new Vector3(bottomWall.transform.localPosition.x, bottomWall.transform.localPosition.y, bottomWall.transform.localPosition.z);
            while (elapsed < duration) {
                float t = elapsed / duration;
                rightWall.transform.localPosition = Vector3.Lerp(ogRWpos, new Vector3(0f, 0f, 0f), t * t * t);
                // bottomWall.transform.localPosition = Vector3.Lerp(ogBWpos, new Vector3(0f, 0f, 0f), t * t * t);
                // topWall.transform.localPosition = Vector3.Lerp(ogTWpos, new Vector3(0f, 0f, 0f), t * t * t);
                leftWall.transform.localPosition = Vector3.Lerp(ogLWpos, new Vector3(0f, 0f, 0f), t * t * t);
                elapsed += Time.deltaTime;
                yield return null;
            }
            rightWall.transform.localPosition = new Vector3(0f, 0f, 0f);
            bottomWall.transform.localPosition = new Vector3(0f, 0f, 0f);
            leftWall.transform.localPosition = new Vector3(0f, 0f, 0f);
            topWall.transform.localPosition = new Vector3(0f, 0f, 0f);
        }
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        asyncLoad.allowSceneActivation = true;
        yield return null;
        currentCoroutine = null;

    }

    public void ToPrevious() {
        if (currentCoroutine == null) {
            currentCoroutine = StartCoroutine(LoadPrevious());
        }
    }

    public IEnumerator LoadFail() {
        AsyncOperation asyncLoad =
            SceneManager.LoadSceneAsync(5);

        asyncLoad.allowSceneActivation = false;

        if (!skipTransition)
        {
            rightWall.SetActive(true);
            leftWall.SetActive(true);
            topWall.SetActive(true);
            bottomWall.SetActive(true);

            failSound.SetActive(true);
            foreach (GameObject g in toDisable) {
                g.SetActive(false);
            }
            yield return new WaitForSeconds(0.5f);
            float duration = 3f;
            float elapsed = 0f;
            Vector3 ogRWpos = new Vector3(rightWall.transform.localPosition.x, rightWall.transform.localPosition.y, rightWall.transform.localPosition.z);
            Vector3 ogLWpos = new Vector3(leftWall.transform.localPosition.x, leftWall.transform.localPosition.y, leftWall.transform.localPosition.z);
            Vector3 ogTWpos = new Vector3(topWall.transform.localPosition.x, topWall.transform.localPosition.y, topWall.transform.localPosition.z);
            Vector3 ogBWpos = new Vector3(bottomWall.transform.localPosition.x, bottomWall.transform.localPosition.y, bottomWall.transform.localPosition.z);
            while (elapsed < duration) {
                float t = elapsed / duration;
                rightWall.transform.localPosition = Vector3.Lerp(ogRWpos, new Vector3(0f, 0f, 0f), t * t * t * t);
                leftWall.transform.localPosition = Vector3.Lerp(ogLWpos, new Vector3(0f, 0f, 0f), t * t * t * t);
                topWall.transform.localPosition = Vector3.Lerp(ogTWpos, new Vector3(0f, 0f, 0f), t * t * t * t);
                bottomWall.transform.localPosition = Vector3.Lerp(ogBWpos, new Vector3(0f, 0f, 0f), t * t * t * t);
                elapsed += Time.deltaTime;
                yield return null;
            }
            rightWall.transform.localPosition = new Vector3(0f, 0f, 0f);
            leftWall.transform.localPosition = new Vector3(0f, 0f, 0f);
            topWall.transform.localPosition = new Vector3(0f, 0f, 0f);
            bottomWall.transform.localPosition = new Vector3(0f, 0f, 0f);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        asyncLoad.allowSceneActivation = true;
        yield return null;
        currentCoroutine = null;

    }

    public void ToFail() {
        if (currentCoroutine == null) {
            // Scene currentScene = SceneManager.GetActiveScene();
            // PlayerPrefs.SetInt("PreviousLevel", currentScene.buildIndex);
            currentCoroutine = StartCoroutine(LoadFail());
        }
    }

    public IEnumerator LoadSpecified(int sceneNum) {
        int n = sceneNum;

        AsyncOperation asyncLoad =
            SceneManager.LoadSceneAsync(n);

        asyncLoad.allowSceneActivation = false;

        if (!skipTransition)
        {
            rightWall.SetActive(true);
            leftWall.SetActive(true);
            topWall.SetActive(true);
            bottomWall.SetActive(true);

            transitionSound.SetActive(true);
            foreach (GameObject g in toDisable) {
                g.SetActive(false);
            }
            yield return new WaitForSeconds(2f);
            float duration = 1f;
            float elapsed = 0f;
            Vector3 ogRWpos = new Vector3(rightWall.transform.localPosition.x, rightWall.transform.localPosition.y, rightWall.transform.localPosition.z);
            Vector3 ogLWpos = new Vector3(leftWall.transform.localPosition.x, leftWall.transform.localPosition.y, leftWall.transform.localPosition.z);
            Vector3 ogTWpos = new Vector3(topWall.transform.localPosition.x, topWall.transform.localPosition.y, topWall.transform.localPosition.z);
            Vector3 ogBWpos = new Vector3(bottomWall.transform.localPosition.x, bottomWall.transform.localPosition.y, bottomWall.transform.localPosition.z);
            while (elapsed < duration) {
                float t = elapsed / duration;
                rightWall.transform.localPosition = Vector3.Lerp(ogRWpos, ogRWpos / 1.5f, t * t * t);
                // bottomWall.transform.localPosition = Vector3.Lerp(ogBWpos, ogBWpos / 1.5f, t * t * t);
                leftWall.transform.localPosition = Vector3.Lerp(ogLWpos, ogLWpos / 1.5f, t * t * t);
                // topWall.transform.localPosition = Vector3.Lerp(ogTWpos, ogTWpos / 1.5f, t * t * t);
                elapsed += Time.deltaTime;
                yield return null;
            }
            elapsed = 0f;
            ogRWpos = new Vector3(rightWall.transform.localPosition.x, rightWall.transform.localPosition.y, rightWall.transform.localPosition.z);
            ogLWpos = new Vector3(leftWall.transform.localPosition.x, leftWall.transform.localPosition.y, leftWall.transform.localPosition.z);
            ogTWpos = new Vector3(topWall.transform.localPosition.x, topWall.transform.localPosition.y, topWall.transform.localPosition.z);
            ogBWpos = new Vector3(bottomWall.transform.localPosition.x, bottomWall.transform.localPosition.y, bottomWall.transform.localPosition.z);
            while (elapsed < duration) {
                float t = elapsed / duration;
                // rightWall.transform.localPosition = Vector3.Lerp(ogRWpos, new Vector3(0f, 0f, 0f), t * t * t);
                bottomWall.transform.localPosition = Vector3.Lerp(ogBWpos, new Vector3(0f, 0f, 0f), t * t * t);
                topWall.transform.localPosition = Vector3.Lerp(ogTWpos, new Vector3(0f, 0f, 0f), t * t * t);
                // leftWall.transform.localPosition = Vector3.Lerp(ogLWpos, new Vector3(0f, 0f, 0f), t * t * t);
                elapsed += Time.deltaTime;
                yield return null;
            }
            rightWall.transform.localPosition = new Vector3(0f, 0f, 0f);
            bottomWall.transform.localPosition = new Vector3(0f, 0f, 0f);
            leftWall.transform.localPosition = new Vector3(0f, 0f, 0f);
            topWall.transform.localPosition = new Vector3(0f, 0f, 0f);
        }
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        asyncLoad.allowSceneActivation = true;
        yield return null;
        currentCoroutine = null;
    }

    private int difficulty = 0;
    public void setDifficulty(int difficulty)
    {
        this.difficulty = difficulty;
    }

    public void ToSpecified(int sceneNum) {
        if (currentCoroutine == null) {

            //level 6 difficulties
            if (sceneNum == 7)
            {
                GameObject level6difficulty = new GameObject($"Level6Difficulty{difficulty}");
                level6difficulty.tag = "DifficultyInfo";
                DontDestroyOnLoad(level6difficulty);
            }
            currentCoroutine = StartCoroutine(LoadSpecified(sceneNum));
        }
    }
}
