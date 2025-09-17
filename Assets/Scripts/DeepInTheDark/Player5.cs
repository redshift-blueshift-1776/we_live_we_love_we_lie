using UnityEngine;

public class Player5 : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private float sensitivity;

    private bool canJump;

    private float g;
    private float velocityY;
    private float jumpHeight;

    //for the ground, does not count walls
    private ContactPoint lastContactPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("START GAME");
        player = gameObject;
        Cursor.lockState = CursorLockMode.Locked;
        // Hide the hardware cursor
        Cursor.visible = false;

        sensitivity = 1f;
        canJump = false;

        g = 9.8f;
        velocityY = 0;
        jumpHeight = 2f;
    }

    // Update is called once per frame
    void Update()
    {
        movePlayer();
        rotatePlayer();
        handleJump();
    }

    private void rotatePlayer()
    {
        player.transform.Rotate(new Vector3(0, 2.5f * Input.GetAxis("Mouse X") * sensitivity, 0));
    }


    private void movePlayer()
    {
        Vector3 forward = player.transform.forward;

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

        player.transform.position += velocity * Time.deltaTime;

        if (!canJump)
        {
            player.transform.position += new Vector3(0, velocityY, 0) * Time.deltaTime;
            velocityY -= g * Time.deltaTime;
        }
    }

    private void handleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            velocityY = Mathf.Sqrt(2 * g * jumpHeight);
            canJump = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.contacts);
        foreach (ContactPoint contact in collision.contacts)
        {
            //Debug.Log(contact.normal);
            if (contact.normal.y == 1f)
            {
                canJump = true;
                velocityY = 0;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("EXIT COLLISION");
        Debug.Log(collision.contacts);
        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.Log(contact);
            if (contact.normal.y == 1f)
            {
                canJump = false;
            }
        }
    }
}
