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

    // Start is called before the first frame update
    void Start()
    {
        transitionSound.SetActive(false);
        failSound.SetActive(false);
        currentCoroutine = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator LoadLevel1() {
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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(23);

    }

    public void ToLevel1() {
        if (currentCoroutine == null) {
            currentCoroutine = StartCoroutine(LoadLevel1());
        }
    }

    public IEnumerator LoadLevelSelect() {
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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(8);

    }

    public void ToLevelSelect() {
        if (currentCoroutine == null) {
            currentCoroutine = StartCoroutine(LoadLevelSelect());
        }
    }

    public IEnumerator LoadMenu() {
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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(0);

    }

    public void ToMenu() {
        if (currentCoroutine == null) {
            currentCoroutine = StartCoroutine(LoadMenu());
        }
    }

    public IEnumerator LoadPrevious() {
        int n = PlayerPrefs.GetInt("PreviousLevel", 1);
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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(n);

    }

    public void ToPrevious() {
        if (currentCoroutine == null) {
            currentCoroutine = StartCoroutine(LoadPrevious());
        }
    }

    public IEnumerator LoadFail() {
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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(5);

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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(n);

    }

    public void ToSpecified(int sceneNum) {
        if (currentCoroutine == null) {
            currentCoroutine = StartCoroutine(LoadSpecified(sceneNum));
        }
    }
}
