using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Player_Movement_Open_Exploration : MonoBehaviour
{
    [SerializeField] public GameObject mainCamera;
    private CharacterController controller;

    private Vector3 playerVelocity = new(0,0,0);
    private bool groundedPlayer;

    private float jumpHeight = 2.0f;
    private float gravityValue = -16f;

    private float jumpVelocity;

    //private float playerMass = 120;

    public static float mouseSensitivity = 1;


    private float interactDistance = 5f;

    private float defaultFieldOfView;
    private float fieldOfViewMultiplier = 1.18f;
    private float fastFieldOfView;


    private readonly KeyCode runKey = KeyCode.LeftShift;
    private readonly KeyCode pushKey = KeyCode.Mouse0;
    private readonly KeyCode pullKey = KeyCode.Mouse1;

    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject bigCrosshair;

    [SerializeField] public GameObject bodySlamSound;

    public float timeSinceLastKick;
    private float timeSinceGrounded;
    private float timeSinceLanded;

    public bool movementLocked;

    public bool autoBunnyHopping = false;

    private Vector3 initialPosition = Vector3.zero;
    private Quaternion initialRotation = Quaternion.identity;

    [SerializeField] private GameObject leftLeg;
    [SerializeField] private GameObject rightLeg;
    [SerializeField] private GameObject leftArm;
    [SerializeField] private GameObject rightArm;

    private void Awake()
    {
        initialPosition = gameObject.transform.position;
        initialRotation = gameObject.transform.rotation;
    }

    private void Start()
    {
        jumpVelocity = Mathf.Sqrt(-2 * gravityValue * jumpHeight);
        controller = gameObject.GetComponent<CharacterController>();
        // set the skin width appropriately according to Unity documentation: https://docs.unity3d.com/Manual/class-CharacterController.html
        controller.skinWidth = 0.1f * controller.radius;
        defaultFieldOfView = Camera.main.fieldOfView;
        fastFieldOfView = defaultFieldOfView * fieldOfViewMultiplier;
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        crosshair.SetActive(true);
        bigCrosshair.SetActive(false);
        mainCamera.SetActive(true);
        bodySlamSound.SetActive(true);
        timeSinceLastKick = 0;
        timeSinceGrounded = 0;
        timeSinceLanded = 0;
        // Application.targetFrameRate = 6;
    }

    void Update()
    {
        if (!controller.enabled || movementLocked) {
            return;
        }
        groundedPlayer = isGrounded();

        jumpHelper();

        interactRaycast();
        rotationHelper();
        timeSinceLastKick += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.R)) {
            respawn();
        }

    }

    private void FixedUpdate()
    {
        movePlayer();
    }

    private void respawn()
    {
        controller.enabled = false;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        controller.enabled = true;
        playerVelocity = Vector3.zero;
    }

    private void movePlayer()
    {
        horizontalMovementHelper();
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private float jumpBufferTime = 0.3f;
    private float jumpBufferCounter = 0f;
    private bool hasJumpedThisLanding = false;
    void jumpHelper() {
        if (groundedPlayer) {
            //needs a little bit of downward velocity to prevent slowly sinking into the ground
            //and also allows proper grounded detection
            //if (playerVelocity.y < 0)
            //{
            //    playerVelocity.y = 0f;
            //}

            hasJumpedThisLanding = false;
        }

        if (Input.GetKeyDown(KeyCode.Space) || (autoBunnyHopping && Input.GetKey(KeyCode.Space)))
        {
            jumpBufferCounter = jumpBufferTime;
        }

        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // conditions for jumping include coyote time and jump buffering (search up for more details)
        const float coyoteTime = 0.05f;
        if (timeSinceGrounded <= coyoteTime && jumpBufferCounter > 0 && !hasJumpedThisLanding) {
            playerVelocity.y = jumpVelocity;
            timeSinceGrounded = coyoteTime + Time.deltaTime;
            jumpBufferCounter = 0f;
            consecutiveBhops += 1;
            hasJumpedThisLanding = true;
        }

        if (!groundedPlayer)
        {
            playerVelocity.y += gravityValue * Time.deltaTime;
        }
    }
    private Vector3 getBottomPos()
    {
        return controller.transform.position + controller.center - new Vector3(0, gameObject.transform.localScale.y * controller.height / 2, 0);
    }

    private bool isGrounded()
    {
        float sphereRadius = controller.radius * 0.99f;
        Vector3 bottomPos = getBottomPos() + 1.001f * sphereRadius * Vector3.up;
        bool grounded = Physics.SphereCast(new Ray(bottomPos, Vector3.down), sphereRadius, 1.002f * sphereRadius);
        timeSinceGrounded = grounded ? 0f : timeSinceGrounded + Time.deltaTime;
        timeSinceLanded += Time.deltaTime;
        if (!groundedPlayer && grounded)
        {
            playerVelocity.y = 0f;
            timeSinceLanded = 0f;
        }
        return grounded;
    }

    private const float baseAcceleration = 80f;
    private const float airAccelerationFactor = 5f / baseAcceleration;
    private const float maxAirSpeed = 200f;
    private const float maxRunningSpeed = 21f;
    private const float maxWalkingSpeed = 5f;
    private const float minSpeedThreshold = 0.1f;
    private const float frictionFactor = 0.9f;
    private int consecutiveBhops = 0;
    private const float bhopsRequired = 1;
    void horizontalMovementHelper() {
        //simplified friction

        float diffFOV = math.abs(fastFieldOfView - defaultFieldOfView);
        Vector3 inputDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            inputDirection += transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputDirection -= transform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputDirection -= transform.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputDirection += transform.right;
        }

        if (inputDirection == Vector3.zero)
        {
            leftLeg.transform.rotation = Quaternion.Euler(0, 0, 0);
            rightLeg.transform.rotation = Quaternion.Euler(0, 0, 0);
            leftArm.transform.rotation = Quaternion.Euler(0, 0, 0);
            rightArm.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            leftLeg.transform.rotation = Quaternion.Euler(Mathf.Sin(Time.time * 17f) * 60f, 0, 0);
            rightLeg.transform.rotation = Quaternion.Euler(-Mathf.Sin(Time.time * 17f) * 60f, 0, 0);
            leftArm.transform.rotation = Quaternion.Euler(-Mathf.Sin(Time.time * 17f) * 60f, 0, 0);
            rightArm.transform.rotation = Quaternion.Euler(Mathf.Sin(Time.time * 17f) * 60f, 0, 0);
        }
        else
        {
            leftLeg.transform.rotation = Quaternion.Euler(Mathf.Sin(Time.time * 5f) * 45f, 0, 0);
            rightLeg.transform.rotation = Quaternion.Euler(-Mathf.Sin(Time.time * 5f) * 45f, 0, 0);
            leftArm.transform.rotation = Quaternion.Euler(-Mathf.Sin(Time.time * 5f) * 45f, 0, 0);
            rightArm.transform.rotation = Quaternion.Euler(Mathf.Sin(Time.time * 5f) * 45f, 0, 0);
        }

        inputDirection = Vector3.Normalize(inputDirection);

        bool isRunning = false;
        //running
        Vector3 playerAcceleration = inputDirection * baseAcceleration;
        if (Input.GetKey(runKey) && !Input.GetKey(KeyCode.S)) {
            isRunning = true;
            playerAcceleration *= 1.5f;
            //do not change the fov when holding shift alone
            if (inputDirection.magnitude > 0)
            {
                Camera.main.fieldOfView = Mathf.MoveTowards(Camera.main.fieldOfView, fastFieldOfView, diffFOV * Time.deltaTime / 0.25f);
            }
        } else {
            Camera.main.fieldOfView = Mathf.MoveTowards(Camera.main.fieldOfView, defaultFieldOfView, diffFOV * Time.deltaTime / 0.25f);
        }
        
        //set horizontal acceleration to 0 when crossing speed threshold and grounded for more than 10 frames
        Vector3 horizontalVelocity = new Vector3(playerVelocity.x, 0, playerVelocity.z);
        float maxGroundSpeed = isRunning ? maxRunningSpeed : maxWalkingSpeed;
        if (groundedPlayer && timeSinceLanded > 10 * Time.deltaTime)
        {
            consecutiveBhops = 0;
            if (horizontalVelocity.magnitude > 1)
            {
                horizontalVelocity *= frictionFactor;
            } else if (inputDirection.magnitude == 0)
            {
                //much faster deceleration at slower speeds

                horizontalVelocity.x = Mathf.Sign(horizontalVelocity.x) * Mathf.Abs(Mathf.Pow(horizontalVelocity.x, 5f));
                horizontalVelocity.z = Mathf.Sign(horizontalVelocity.z) * Mathf.Abs(Mathf.Pow(horizontalVelocity.z, 5f));
            }
            if (horizontalVelocity.magnitude > maxGroundSpeed)
            {
                horizontalVelocity = Vector3.Normalize(horizontalVelocity) * maxGroundSpeed;
            }
        } else {
            if (horizontalVelocity.magnitude > 0.1f)
            {
                Vector3 velocityDir = horizontalVelocity.normalized;

                // decompose acceleration into parallel and perpendicular components
                float parallelComponent = Vector3.Dot(playerAcceleration, velocityDir);
                Vector3 parallelAccel = velocityDir * parallelComponent;
                Vector3 perpAccel = horizontalVelocity.magnitude > 10f ? playerAcceleration - parallelAccel : Vector3.zero;

                // allow full perpendicular control (strafing)
                // while limiting forward acceleration when over max speed
                if (horizontalVelocity.magnitude > maxGroundSpeed && parallelComponent > 0)
                {
                    parallelAccel *= consecutiveBhops >= bhopsRequired ? airAccelerationFactor : 0f;
                }

                playerAcceleration = parallelAccel + perpAccel;
            }
            else if (horizontalVelocity.magnitude > maxGroundSpeed)
            {
                // when starting from near-zero, just reduce all acceleration
                playerAcceleration *= airAccelerationFactor;
            }

            // hard cap at max air speed
            if (horizontalVelocity.magnitude > maxAirSpeed)
            {
                horizontalVelocity = Vector3.Normalize(horizontalVelocity) * maxAirSpeed;
            } else if (consecutiveBhops < bhopsRequired)
            {
                horizontalVelocity = Vector3.Normalize(horizontalVelocity) * maxGroundSpeed;
            }
        }
        
        //allows player to come to a complete stop when not holding anything
        if (horizontalVelocity.magnitude < minSpeedThreshold && inputDirection.magnitude == 0)
        {
            horizontalVelocity = Vector3.zero;
        }

        //counter strafing
        if (Vector3.Dot(horizontalVelocity, inputDirection) < -Mathf.Sin(46f * Mathf.Deg2Rad) && horizontalVelocity.magnitude < maxGroundSpeed / 2f)
        {
            horizontalVelocity *= 0.5f;
            playerAcceleration = Vector3.zero;
        }
        playerVelocity.x = horizontalVelocity.x;
        playerVelocity.z = horizontalVelocity.z;
        playerVelocity += playerAcceleration * Time.deltaTime;
    }


    void rotationHelper() {
        // Rotates the camera and character object
        float rotX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float rotY = -Input.GetAxis("Mouse Y") * mouseSensitivity;
        gameObject.transform.Rotate(0, rotX, 0);
        Camera.main.transform.Rotate(rotY, 0, 0);
        if (Camera.main.transform.localEulerAngles.y == 180 && Camera.main.transform.localEulerAngles.z == 180) {
            float diffBetweenUpDir = Mathf.Abs(270 - Camera.main.transform.localEulerAngles.x);
            float diffBetweenDownDir = Mathf.Abs(90 - Camera.main.transform.localEulerAngles.x);
            if (diffBetweenDownDir <= diffBetweenUpDir) {
                Camera.main.transform.localEulerAngles = new Vector3(90, 0, 0);
            } else {
                Camera.main.transform.localEulerAngles = new Vector3(270, 0, 0);
            }
        }
        gameObject.transform.Rotate(0, rotX, 0);
    }

    private Door currentDoor = null;
    private Open_Exploration_Collectible currentCollectible = null;

    void interactRaycast()
    {
        RaycastHit hit;
        Vector3 origin = Camera.main.transform.position;
        Vector3 dir = Camera.main.transform.forward;
        Door newDoor = null;
        Open_Exploration_Collectible collectible = null;

        float radius = 0.05f;
        if (Physics.SphereCast(origin, radius, dir, out hit, interactDistance))
        {
            newDoor = hit.collider.GetComponent<Door>();
            collectible = hit.collider.GetComponent<Open_Exploration_Collectible>();
        }

        // Only update state if changed
        if (newDoor != currentDoor)
        {
            currentDoor = newDoor;

            if (currentDoor != null)
            {
                crosshair.SetActive(false);
                bigCrosshair.SetActive(true);
            }
            else
            {
                crosshair.SetActive(true);
                bigCrosshair.SetActive(false);
            }
        }

        // Only interact if we currently have a valid target
        if (currentDoor != null)
        {
            if (Input.GetKeyDown(pushKey))
            {
                currentDoor.Interact(); // or InteractPush()
            }
            else if (Input.GetKeyDown(pullKey))
            {
                currentDoor.Interact(); // or InteractPull()
            }
        }

        // Only update state if changed
        if (collectible != currentCollectible)
        {
            currentCollectible = collectible;

            if (currentCollectible != null)
            {
                crosshair.SetActive(false);
                bigCrosshair.SetActive(true);
            }
            else
            {
                crosshair.SetActive(true);
                bigCrosshair.SetActive(false);
            }
        }

        // Only interact if we currently have a valid target
        if (currentCollectible != null)
        {
            if (Input.GetKeyDown(pushKey))
            {
                currentCollectible.Interact(); // or InteractPush()
            }
            else if (Input.GetKeyDown(pullKey))
            {
                currentCollectible.Interact(); // or InteractPull()
            }
        }
    }

    // void OnTriggerEnter(Collider hit) {
    //     Debug.Log(hit.gameObject.name);
    //     if (hit.gameObject.name.Contains("Bullet")) {
    //         MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
    //         if (meshRenderer != null)
    //         {
    //             meshRenderer.enabled = false; // Disables the Mesh Renderer
    //         }
    //         mainCamera.SetActive(false);
    //         altCamera.SetActive(true);
    //         fakeBody.SetActive(true);
    //         // gm.Fail();
    //     }
    // }

    public void SetMouseSensitivity(float sensitivity) {
        mouseSensitivity = sensitivity;
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity); // Save to PlayerPrefs
    }

    void OnControllerColliderHit(ControllerColliderHit hit) {
        if (hit.rigidbody != null) {
            if (timeSinceLastKick >= 1f) {
                if (Input.GetKey(runKey)) {
                    Vector3 horizontalDir = new Vector3(hit.moveDirection.x, 0.1f, hit.moveDirection.z);
                    hit.rigidbody.AddForce(horizontalDir * 300000 * Time.fixedDeltaTime);
                } else {
                    Vector3 horizontalDir = new Vector3(hit.moveDirection.x, 0.01f, hit.moveDirection.z);
                    hit.rigidbody.AddForce(horizontalDir * 60000 * Time.fixedDeltaTime);
                }
                bodySlamSound.GetComponent<AudioSource>().Play();
                timeSinceLastKick = 0f;
            } else
            {
                if (Input.GetKey(runKey)) {
                    Vector3 horizontalDir = new Vector3(hit.moveDirection.x, 0.1f, hit.moveDirection.z);
                    hit.rigidbody.AddForce(horizontalDir * 30000 * Time.fixedDeltaTime);
                } else {
                    Vector3 horizontalDir = new Vector3(hit.moveDirection.x, 0.01f, hit.moveDirection.z);
                    hit.rigidbody.AddForce(horizontalDir * 6000 * Time.fixedDeltaTime);
                }
                bodySlamSound.GetComponent<AudioSource>().Play();
                timeSinceLastKick = 0f;
            }
        }
    }
}
