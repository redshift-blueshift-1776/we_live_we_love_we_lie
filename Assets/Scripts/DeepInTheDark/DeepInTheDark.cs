using UnityEngine;
using TMPro;

public class DeepInTheDark : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject Player;
    private bool gameActive;
    public float timer;

    [SerializeField] private TMP_Text timerText;


    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject UICanvas;
    [SerializeField] private GameObject EndScreenCanvas;
    [SerializeField] private GameObject winText;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        updateTimer();
    }

    private void updateTimer()
    {
        if (gameActive)
        {
            timerText.text = $"Time Remaining: {120 - Mathf.Floor(timer)}";
            timer += Time.deltaTime;
            if (timer > 120f)
            {
                //StartCoroutine(EndGame());
            }
        }
    }
}
