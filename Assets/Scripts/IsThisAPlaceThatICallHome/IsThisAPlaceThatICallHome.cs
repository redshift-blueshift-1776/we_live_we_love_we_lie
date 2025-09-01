using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class IsThisAPlaceThatICallHome : MonoBehaviour
{
    public float timer;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text layerText;

    [Header("Canvasses")]
    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject UICanvas;
    [SerializeField] private GameObject EndScreenCanvas;
    [SerializeField] private GameObject winText;

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

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bm = blockManger.GetComponent<BlockManager>();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        startCanvas.SetActive(true);
        UICanvas.SetActive(false);
        EndScreenCanvas.SetActive(false);
        loadingAudio.SetActive(true);
        gameAudio.SetActive(false);
        gameActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameActive) {
            timerText.text = $"Time Remaining: {120 - Mathf.Floor(timer)}";
            timer += Time.deltaTime;
            layerText.text = $"Layer: {bm.currentLayer}";
            if ((timer > 120f) && bm.currentLayer >= 12) {
                StartCoroutine(EndGame());
            }
            if ((timer > 120f) && bm.currentLayer < 12) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                SceneManager.LoadScene(0); // Change to loss scene once made.
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

    private IEnumerator EndGame() {
        gameActive = false;
        startCanvas.SetActive(false);
        UICanvas.SetActive(false);
        EndScreenCanvas.SetActive(true);
        winText.SetActive(false);
        Vector3 startPos = camTransform.position;
        Vector3 targetPos = new Vector3(camTransform.position.x,
            camTransform.position.y, camTransform.position.z - 100);

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
        SceneManager.LoadScene(3); // Change to 2 once To Find What I've Become is made.
    }


}
