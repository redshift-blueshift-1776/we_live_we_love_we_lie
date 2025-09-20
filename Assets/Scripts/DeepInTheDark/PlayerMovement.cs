using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject body;
    private float sensitivityX;
    private float sensitivityY;

    private const float g = 9.8f;
    private Vector3 acceleration;
    private Vector3 velocity;

    private const float jumpHeight = 1.5f;

    
    private const float walkingAcceleration = 20f;
    private const float runningAcceleration = 30f;

    private const float groundedFrictionCoefficient = 0.99f;
    private const float airborneFrictionCoefficient = 0.991f;

    private const float minSpeed = 0.01f;

    /*
     * there is already a terminal velocity without maxSpeed just from
     * acceleration and friction, but reaching terminal velocity takes
     * too long, so capping it at a lower speed makes it feel more fluid
     * */
    private const float maxSpeed = 30f;


    private float yaw;
    private float pitch;

    //the maximum angled slope the player can walk up (in degrees)
    private const float maxSlopeAngle = 60f;

    private const float rotationSpeed = 5f;

    //min distance between body's upward vector and average normal vector before rotation occurs
    private const float minRotationThreshold = 0.01f;

    //stores current ground contact colliders and their normals
    private Dictionary<Collider, List<Vector3>> groundContactPoints;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("START GAME");
        Cursor.lockState = CursorLockMode.Locked;
        // Hide the hardware cursor
        Cursor.visible = false;

        sensitivityX = 2.5f;
        sensitivityY = 2.0f;

        acceleration = new Vector3(0, -g, 0);
        velocity = new Vector3();


        yaw = 0f;
        pitch = 0f;

        groundContactPoints = new Dictionary<Collider, List<Vector3>>();
    }

    // Update is called once per frame
    void Update()
    {
        movePlayer();
        rotateCamera();
        handleJump();
    }

    private void FixedUpdate()
    {
        rotatePlayer();
    }

    private void rotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivityX;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY;

        yaw += mouseX;
        pitch -= mouseY; //inverted
        pitch = Mathf.Clamp(pitch, -90f, 90f); //clamp it between two values

        player.transform.rotation = Quaternion.Euler(0, yaw, 0);
        playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0, 0);
    }

    private void rotatePlayer()
    {
        Vector3 averageNormal = AverageVector(groundContactPoints.Values.ToList().SelectMany(l => l).ToList());
        //Debug.Log(averageNormal);
        if (groundContactPoints.Count > 0)
        {
            //if (Vector3.Distance(body.transform.up, averageNormal) < minRotationThreshold)
            //{
            //    body.transform.up = averageNormal;
            //    return;
            //}
            body.transform.up = Vector3.Slerp(body.transform.up, averageNormal, rotationSpeed * Time.deltaTime);
        } 
    }


    private void movePlayer()
    {
        Vector3 forward = player.transform.forward;
        
        Vector3 inputAcceleration = Vector3.zero;

        //only check for WASD input if grounded
        if (Input.GetKey(KeyCode.W))
        {
            inputAcceleration += forward;
        }

        if (Input.GetKey(KeyCode.A))
        {
            inputAcceleration += Quaternion.Euler(0, -90, 0) * forward;
        }

        if (Input.GetKey(KeyCode.S))
        {
            inputAcceleration -= forward;
        }

        if (Input.GetKey(KeyCode.D))
        {
            inputAcceleration += Quaternion.Euler(0, 90, 0) * forward;
        }

        inputAcceleration.Normalize();

        //if the player is running
        if (Input.GetKey(KeyCode.LeftShift))
        {
            inputAcceleration *= runningAcceleration;
        } else
        {
            inputAcceleration *= walkingAcceleration;
        }

        //update velocities
        if (groundContactPoints.Count == 0)
        {
            //airborne, apply friction after
            velocity.y += acceleration.y * Time.deltaTime;
        }
        else
        {
            //grounded, apply friction after
            velocity.y = 0;
        }
        velocity += inputAcceleration * Time.deltaTime;

        // Clamp horizontal velocity
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);

        if (horizontalVelocity.magnitude > maxSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
            velocity.x = horizontalVelocity.x;
            velocity.z = horizontalVelocity.z;
        }
       
        player.transform.position += (groundContactPoints.Count > 0 ? Vector3.ProjectOnPlane(velocity, body.transform.up) : velocity) * Time.deltaTime;


        //apply friction now

        Vector3 frictionVector = new Vector3(0, 1f, 0);

        if (groundContactPoints.Count == 0)
        {
            frictionVector.x = airborneFrictionCoefficient;
            frictionVector.z = airborneFrictionCoefficient;
        } else
        {
            frictionVector.x = groundedFrictionCoefficient;
            frictionVector.z = groundedFrictionCoefficient;
        }

        velocity.Scale(frictionVector);
        if (Mathf.Abs(velocity.x) < minSpeed)
        {
            velocity.x = 0;
        }

        if (Mathf.Abs(velocity.z) < minSpeed)
        {
            velocity.z = 0;
        }
    }

    private void handleJump()
    {
        if (Input.GetKey(KeyCode.Space) && groundContactPoints.Count > 0)
        {
            groundContactPoints.Clear();
            velocity.y = Mathf.Sqrt(2 * g * jumpHeight);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("entered " + collision.gameObject.name);
        //Debug.Log(collision.contacts);
        foreach (ContactPoint contact in collision.contacts)
        {
            //Debug.Log(groundContactPoints);
            if (contact.normal.y >= Mathf.Cos(maxSlopeAngle * Mathf.Deg2Rad))
            {
                if (!groundContactPoints.ContainsKey(collision.collider))
                {
                    groundContactPoints.Add(collision.collider, new List<Vector3> { });
                }
                groundContactPoints[collision.collider].Add(contact.normal);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        //Debug.Log("left " + collision.gameObject.name);
        //Debug.Log(collision.contacts);
        groundContactPoints.Remove(collision.collider);
    }



    // Averages the components of all vectors and returns a normalized vector
    private Vector3 AverageVector(List<Vector3> list)
    {
        Vector3 averageVector = new Vector3();
        foreach (Vector3 v in list)
        {
            averageVector.x += v.x;
            averageVector.y += v.y;
            averageVector.z += v.z;
        }

        averageVector /= list.Count;
        averageVector.Normalize();
        
        return averageVector;
    }
}
