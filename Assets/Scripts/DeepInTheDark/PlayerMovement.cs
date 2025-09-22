using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject body;

    public float initialYaw;
    public float initialPitch;

    private Rigidbody playerRigidBody;

    private float sensitivityX;
    private float sensitivityY;

    private const float g = 9.8f;
    private Vector3 acceleration;
    private Vector3 velocity;

    private const float jumpHeight = 1.5f;


    private const float frictionCoefficient = 0.97f;

    private const float minSpeed = 0.001f;

    private float walkingAcceleration = 0.05f;
    private float runningAcceleration = 0.1f;

    private float maxWalkingVelocity = 5f;
    private float maxRunningVelocity = 8f;

    private float yaw;
    private float pitch;

    private bool userInput;

    //the maximum angled slope the player can walk up (in degrees)
    private const float maxSlopeAngle = 60f;

    private const float rotationSpeed = 5f;

    //stores current ground contact colliders and their normals
    private Dictionary<Collider, List<Vector3>> groundContactPoints;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        playerRigidBody = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        // Hide the hardware cursor
        Cursor.visible = false;

        sensitivityX = 2.5f;
        sensitivityY = 2.0f;

        acceleration = new Vector3(0, -g, 0);
        velocity = new Vector3();

        yaw = initialYaw;
        pitch = initialPitch;

        userInput = false;

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
            body.transform.up = Vector3.Slerp(body.transform.up, averageNormal, rotationSpeed * Time.deltaTime);
        } 
    }


    private void movePlayer()
    {
        bool isRunning = false;
        Vector3 forward = player.transform.forward;

        if (Input.GetKey(KeyCode.LeftShift)) {
            isRunning = true;
        }


        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A)
            || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            userInput = true;
            Vector3 inputVelocity = Vector3.zero;

            //only check for WASD input if grounded
            if (Input.GetKey(KeyCode.W))
            {
                inputVelocity += forward;
            }

            if (Input.GetKey(KeyCode.A))
            {
                inputVelocity -= player.transform.right;
            }

            if (Input.GetKey(KeyCode.S))
            {
                inputVelocity -= forward;
            }

            if (Input.GetKey(KeyCode.D))
            {
                inputVelocity += player.transform.right;
            }

            inputVelocity.Normalize();
            inputVelocity *= isRunning ? runningAcceleration : walkingAcceleration;

            //inputAcceleration should have no y component
            velocity += inputVelocity;
        } else
        {
            userInput = false;
        }

            //scale velocity to max
            Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);

        if (isRunning && horizontalVelocity.magnitude > maxRunningVelocity) {
            horizontalVelocity.Normalize();
            horizontalVelocity *= maxRunningVelocity;
        } else if (!isRunning && horizontalVelocity.magnitude > maxWalkingVelocity)
        {
            horizontalVelocity.Normalize();
            horizontalVelocity *= maxWalkingVelocity;
        }

        velocity.x = horizontalVelocity.x;
        velocity.z = horizontalVelocity.z;
        

        //update vertical velocity
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
        //Debug.Log(inputAcceleration * Time.deltaTime);

        //avoids teleporting into wall issue
        playerRigidBody.MovePosition(playerRigidBody.position + (groundContactPoints.Count > 0 ? Vector3.ProjectOnPlane(velocity, body.transform.up) : velocity) * Time.deltaTime);
        //player.transform.position += (groundContactPoints.Count > 0 ? Vector3.ProjectOnPlane(velocity, body.transform.up) : velocity) * Time.deltaTime;


        //only apply friction when the player stops giving input (WASD)
        if (!userInput)
        {
            StartCoroutine(stopSliding());
            Vector3 frictionVector = new Vector3(0, 1f, 0);

            frictionVector.x = frictionCoefficient;
            frictionVector.z = frictionCoefficient;

            velocity.Scale(frictionVector);
        }
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

    public void haltMovement()
    {
        velocity = Vector3.zero;
    }

    private IEnumerator stopSliding()
    {
        yield return new WaitForSeconds(3f);
        if (!userInput)
        {
            velocity.x = 0;
            velocity.z = 0;
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

    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal != Vector3.up)
            {
                if (Mathf.Abs(contact.normal.x) > Mathf.Sin(maxSlopeAngle * Mathf.Deg2Rad))
                {
                    velocity.x = 0;
                }
                if (Mathf.Abs(contact.normal.z) > Mathf.Sin(maxSlopeAngle * Mathf.Deg2Rad))
                {
                    velocity.z = 0;
                }
            }

        }
    }

    private void OnCollisionExit(Collision collision)
    {
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
