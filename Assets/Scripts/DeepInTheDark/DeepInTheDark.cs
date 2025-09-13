using UnityEngine;
using TMPro;

public class DeepInTheDark : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject Player;

    private float sensitivity;

    private bool gameActive;

    public float timer;

    [SerializeField] private TMP_Text timerText;


    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject UICanvas;
    [SerializeField] private GameObject EndScreenCanvas;
    [SerializeField] private GameObject winText;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        // Hide the hardware cursor
        Cursor.visible = false;

        sensitivity = 1f;
        gameActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        updatePlayerPosition();
        updatePlayerRotation();
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

    private void updatePlayerRotation()
    {
        Player.transform.Rotate(new Vector3(0, 2.5f * Input.GetAxis("Mouse X") * sensitivity, 0));
    }


    private void updatePlayerPosition()
    {
        Vector3 forward = Player.transform.forward;

        Vector3 velocity = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W))
        {
            velocity += forward;
        }

        if (Input.GetKey(KeyCode.A))
        {
            velocity += Quaternion.Euler(0, -90, 0) * forward;
        }

        if (Input.GetKey(KeyCode.S))
        {
            velocity -= forward;
        }

        if (Input.GetKey(KeyCode.D))
        {
            velocity += Quaternion.Euler(0, 90, 0) * forward;
        }

        Vector3.Normalize(velocity);

        //if the player is running
        if (Input.GetKey(KeyCode.LeftShift))
        {
            velocity *= 2f;
        }

        Player.transform.position += velocity * Time.deltaTime;
    }
}
