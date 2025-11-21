using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

public class IsThisAPlaceThatICallHome : MonoBehaviour
{
    [SerializeField] public bool endless;
    public float timer;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text layerText;

    [Header("Canvasses")]
    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject UICanvas;
    [SerializeField] private GameObject EndScreenCanvas;
    [SerializeField] private GameObject winText;
    [SerializeField] private GameObject skipButton;

    [Header("Audio")]
    [SerializeField] public GameObject loadingAudio;
    [SerializeField] public GameObject gameAudio;

    [SerializeField] private GameObject blockManger;
    private BlockManager bm;

    [SerializeField] private Transform camTransform;   // drag your camera here
    [SerializeField] private float cameraMoveSpeed = 2f; // how fast to ease
    [SerializeField] private float cameraYOffset = 3f;  // how much per layer
    private Coroutine camMoveRoutine;

    public bool gameActive;
    public bool gameDone;

    [SerializeField] private GameObject transition;
    private Transition transitionScript;

    [SerializeField] private GameObject callableLyricsSyncDisplay;
    private CallableLyricsSyncDisplay clsd;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bm = blockManger.GetComponent<BlockManager>();
        transitionScript = transition.GetComponent<Transition>();
        clsd = callableLyricsSyncDisplay.GetComponent<CallableLyricsSyncDisplay>();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        startCanvas.SetActive(true);
        UICanvas.SetActive(false);
        EndScreenCanvas.SetActive(false);
        loadingAudio.SetActive(true);
        gameAudio.SetActive(false);
        gameActive = false;
        gameDone = false;

        int usePostProcessing = PlayerPrefs.GetInt("useVisualEffects", 0);
        if (usePostProcessing == 0) {
            UniversalAdditionalCameraData cameraData = Camera.main.GetUniversalAdditionalCameraData();
            cameraData.renderPostProcessing = false;
        } else {
            UniversalAdditionalCameraData cameraData = Camera.main.GetUniversalAdditionalCameraData();
            cameraData.renderPostProcessing = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.M)) {
            SceneManager.LoadScene(0);
        }
        if (gameActive) {
            if (endless) {
                timerText.text = $"Time: {Mathf.Floor(timer)}";
                timer += Time.deltaTime;
                layerText.text = $"Layer: {bm.currentLayer}";

                if (bm.currentLayer >= 20) {
                    GameObject foundObject = GameObject.Find("Universal_Manager");
                    // Check if the foundObject is not null
                    if (foundObject != null) {
                        Debug.Log("Found Universal_Manager");
                        Universal_Manager um = foundObject.GetComponent<Universal_Manager>();
                        um.level1Layer20 = true;
                        PlayerPrefs.SetInt("level1Layer20", 1);
                    } else {
                        Debug.Log("No Universal_Manager");
                    }
                }
            } else {
                timerText.text = $"Time Remaining: {120 - Mathf.Floor(timer)}";
                timer += Time.deltaTime;
                layerText.text = $"Layer: {bm.currentLayer}";
                if ((timer > 120f) && bm.currentLayer >= bm.numLayers) {
                    StartCoroutine(EndGame());
                }
                if ((timer < 90f) && gameDone) {
                    StartCoroutine(SkipToEnd());
                }
                if ((timer > 120f) && bm.currentLayer < bm.numLayers) {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    Scene currentScene = SceneManager.GetActiveScene();
                    PlayerPrefs.SetInt("PreviousLevel", currentScene.buildIndex);
                    // SceneManager.LoadScene(0); // Change to loss scene once made.
                    transitionScript.ToFail();
                }
            }
        }
    }

    public void StartGame() {
        gameActive = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        startCanvas.SetActive(false);
        UICanvas.SetActive(true);
        EndScreenCanvas.SetActive(false);
        loadingAudio.SetActive(false);
        gameAudio.SetActive(true);
        skipButton.SetActive(false);
        bm.SpawnNextBlock();
    }

    public void MoveCameraUp(int newLayer)
    {
        // target height based on current layer
        float targetY = 47 + newLayer * cameraYOffset;

        // stop previous move if it’s still going
        if (camMoveRoutine != null)
            StopCoroutine(camMoveRoutine);

        camMoveRoutine = StartCoroutine(MoveCameraRoutine(targetY));
    }

    private IEnumerator MoveCameraRoutine(float targetY)
    {
        Vector3 startPos = camTransform.position;
        Vector3 targetPos = new Vector3(startPos.x, targetY, startPos.z);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * cameraMoveSpeed;
            float easedT = Mathf.SmoothStep(0, 1, t); // easing
            camTransform.position = Vector3.Lerp(startPos, targetPos, easedT);
            yield return null;
        }

        camTransform.position = targetPos; // snap at the end
    }

    private IEnumerator SkipToEnd() {
        yield return new WaitForSeconds(3f);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        skipButton.SetActive(true);
    }

    public void SkipRestOfGame() {
        gameActive = false;
        StartCoroutine(EndGame());
    }

    private IEnumerator EndGame() {
        gameActive = false;
        startCanvas.SetActive(false);
        UICanvas.SetActive(false);
        EndScreenCanvas.SetActive(true);
        winText.SetActive(false);
        Vector3 startPos = camTransform.position;
        Vector3 targetPos = new Vector3(camTransform.position.x,
            camTransform.position.y + 50, camTransform.position.z - 100);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 5f;
            float easedT = Mathf.SmoothStep(0, 1, t); // easing
            camTransform.position = Vector3.Lerp(startPos, targetPos, easedT);
            yield return null;
        }

        camTransform.position = targetPos; // snap at the end

        winText.SetActive(true);
        yield return new WaitForSeconds(3f);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameObject foundObject2 = GameObject.Find("Universal_Manager");
        if (foundObject2 != null) {
            Debug.Log("Found Universal_Manager");
            Universal_Manager um = foundObject2.GetComponent<Universal_Manager>();
            um.beatStoryModeLevels[0] = true;
            um.unlockedHard[0] = true;
            PlayerPrefs.SetInt("beatStoryModeLevels1", 1);
            PlayerPrefs.SetInt("unlockedHard1", 1);

            int sceneID = SceneManager.GetActiveScene().buildIndex;
            if (sceneID != 1) {
                um.beatHardLevels[0] = true;
                PlayerPrefs.SetInt("beatHardLevels1", 1);
            }
        } else {
            Debug.Log("No Universal_Manager");
        }
        GameObject foundObject = GameObject.Find("StoryMode");

        // Check if the foundObject is not null
        if (foundObject != null)
        {
            Debug.Log("GameObject '" + "StoryMode" + "' found in the scene.");
            SceneManager.LoadScene(10); // Change to 2 once To Find What I've Become is made.
        }
        else
        {
            SceneManager.LoadScene(0); // Not in story mode, goes back to the menu page
        } 
    }


}
