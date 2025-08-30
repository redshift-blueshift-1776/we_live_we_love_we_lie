using UnityEngine;

public class WalkAlongThePathUnknown : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject maze;

    [Header("Canvasses")]
    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject gameCanvas;

    [Header("Audio")]
    [SerializeField] public GameObject loadingAudio;
    [SerializeField] public GameObject gameAudio;

    public bool gameActive;
    public float timer;

    public bool hitRealWall;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startCanvas.SetActive(true);
        gameCanvas.SetActive(false);
        loadingAudio.SetActive(true);
        gameAudio.SetActive(false);
        gameActive = false;
        timer = 0f;
        hitRealWall = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startGame() {
        
    }
}
