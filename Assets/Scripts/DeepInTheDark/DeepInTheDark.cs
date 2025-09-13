using UnityEngine;

public class DeepInTheDark : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject Player;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        // Hide the hardware cursor
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        updatePlayerPosition();
        updatePlayerRotation();
    }

    private void updatePlayerRotation()
    {
        Player.transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0));
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

        Player.transform.position += velocity * Time.deltaTime;
    }
}
