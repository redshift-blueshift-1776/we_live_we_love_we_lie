using System.Collections.Generic;
using UnityEngine;

public class Player5 : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject camera;
    private float sensitivity;

    private float g;
    private float velocityY;
    private float jumpHeight;

    //for the ground, does not count walls
    private HashSet<Collider> groundContactPoints;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("START GAME");
        Cursor.lockState = CursorLockMode.Locked;
        // Hide the hardware cursor
        Cursor.visible = false;

        sensitivity = 1f;

        g = 9.8f;
        velocityY = 0;
        jumpHeight = 2f;

        groundContactPoints = new HashSet<Collider>();
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
        camera.transform.Rotate(new Vector3(-2.5f * Input.GetAxis("Mouse Y") * sensitivity, 0, 0));
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

        if (groundContactPoints.Count == 0)
        {
            player.transform.position += new Vector3(0, velocityY, 0) * Time.deltaTime;
            velocityY -= g * Time.deltaTime;
        } else
        {
            velocityY = 0;
        }
    }

    private void handleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && groundContactPoints.Count > 0)
        {
            groundContactPoints.Clear();
            velocityY = Mathf.Sqrt(2 * g * jumpHeight);
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
                groundContactPoints.Add(collision.collider);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        //Debug.Log(collision.contacts);
        groundContactPoints.Remove(collision.collider);
    }
}
