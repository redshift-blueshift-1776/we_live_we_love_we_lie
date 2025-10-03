using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_Movement_Level_2 : MonoBehaviour
{
    [SerializeField] public GameObject gameManager;
    public ToFindWhatIveBecome gm;
    // private CharacterController controller;

    private Vector3 playerVelocity = new(0,0,0);
    private bool groundedPlayer;
    public static float basePlayerSpeed = 5.0f;

    public static float speedUp = 2.5f;

    // time to run from standstill

    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;

    private float jumpVelocity;

    //private float playerMass = 120;

    public static float mouseSensitivity = 1;


    private float interactDistance = 5f;

    private float defaultFieldOfView;
    private float fieldOfViewMultiplier = 1.18f;
    private float fastFieldOfView;
    
    private readonly KeyCode pushKey = KeyCode.Mouse0;
    private readonly KeyCode pullKey = KeyCode.Mouse1;

    [Header("Speed Stats")]
    [SerializeField] private float acceleration = 10f; // Acceleration rate
    [SerializeField] private float maxSpeed = 200f; // Maximum speed
    [SerializeField] private float brakeForce = 20f; // Braking power
    [SerializeField] private float friction = 0.98f; // Simulated drag
    [SerializeField] private float speedMultiplier = 2f; // Speed boost with Shift

    public float currentSpeed = 0f;
    // private Rigidbody rb;

    private CharacterController controller;
    private BoxCollider boxCollider;
    public LayerMask obstacleMask; // Set this in the inspector to only include walls

    private Vector3 velocity;
    public float gravity = 32f;
    public float groundCheckDistance = 1f;
    public LayerMask groundMask;
    public Vector3 lastPosition;
    [SerializeField] private bool isGrounded;

    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject bigCrosshair;

    [Header("Rotation Settings")]
    public float rotationSpeed = 360f;   // degrees per second

    [Header("Smoothing")]
    public float normalLerpSpeed = 10f;  // how quickly ground normal smooths

    private Vector3 smoothedNormal = Vector3.up;

    private Vector3 checkStartPosition;
    private float checkStartTime;


    private void Awake()
    {
        gm = gameManager.GetComponent<ToFindWhatIveBecome>();
        jumpVelocity = Mathf.Sqrt(-2 * gravityValue * jumpHeight);
        // controller = gameObject.GetComponent<CharacterController>();
        // set the skin width appropriately according to Unity documentation: https://docs.unity3d.com/Manual/class-CharacterController.html
        // controller.skinWidth = 0.1f * controller.radius;
        // maxSpeed = Player_Movement.basePlayerSpeed * Player_Movement.speedUp;
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);
        crosshair.SetActive(true);
        bigCrosshair.SetActive(false);
        defaultFieldOfView = Camera.main.fieldOfView;
        fastFieldOfView = defaultFieldOfView * fieldOfViewMultiplier;
        //rb = GetComponent<Rigidbody>();
        //rb.freezeRotation = true; // Prevent the ambulance from tipping over
        controller = GetComponent<CharacterController>();
        boxCollider = GetComponent<BoxCollider>();
        controller.height = 6.5f;
        controller.center = new Vector3(0, 2f, 0);
        controller.slopeLimit = 50f;
        lastPosition = new Vector3(transform.position.x, 0, transform.position.z);

    }

    void Update()
    {
        if (gm.gameActive) {
            // modify player velocity
            // horizontalMovementHelper();
            // move player
            // controller.Move(playerVelocity * Time.deltaTime);
            interactRaycast();
            rotationHelper();
        }
        if (gm.gameActive) {
            // Gravity Handling
            isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, ~0);
            if (controller.isGrounded) {
                isGrounded = true;
            }
            if (transform.position.y <= 1) {
                isGrounded = true;
            }

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = 0f; // Small offset to keep grounded
            }
            else
            {
                velocity.y -= 5 * gravity * Time.deltaTime;
            }

            HandleMovement();
            AlignWithGround();

            // Move the scooter
            //Vector3 move = transform.forward * currentSpeed * Time.deltaTime;
            Vector3 move = new Vector3(0, 0, 0);
            move.y = velocity.y * Time.deltaTime; // Apply gravity

            controller.Move(move);
        }
    }


    void AlignWithGround()
    {
        // Raycast straight down from the center
        RaycastHit[] hits = Physics.RaycastAll(transform.position + Vector3.up, Vector3.down, groundCheckDistance * 20f);
        bool seeARamp = false;
        foreach (RaycastHit hit in hits)
        { 
            Debug.Log(hit.collider.gameObject.name);
            if (!hit.collider.transform.IsChildOf(transform)) // ignore self
            {
                Debug.Log("Hit normal: " + hit.normal);
                // Debug.DrawRay(hit.point, hit.normal * 30f, Color.green);
                if (hit.collider.gameObject.name.Contains("Ramp")) {
                    seeARamp = true;
                    isGrounded = true;
                    Vector3 groundNormal = hit.normal;

                    // Use cross-product to build a stable forward
                    // This ensures we get tilt along ramps, not just flat up
                    Vector3 forward = Vector3.Cross(transform.right, groundNormal).normalized;

                    // Build the target rotation
                    Quaternion targetRotation = Quaternion.LookRotation(forward, groundNormal);

                    // Smooth rotation into place
                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation,
                        targetRotation,
                        rotationSpeed * Time.deltaTime
                    );
                }
            }
        }
        if (!seeARamp) {
            // Reset rotation when not on a ramp
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, transform.eulerAngles.y, 0), Time.deltaTime);
        }
    }

    void HandleMovement()
    {
        float moveInput = 0f;

        // Forward and backward movement
        if (Input.GetKey(KeyCode.W))
            moveInput = 1f;
        if (Input.GetKey(KeyCode.S))
            moveInput = -1f;

        // Boost logic
        bool isBoosting = Input.GetKey(KeyCode.LeftShift);
        float speedFactor = isBoosting ? speedMultiplier : 1f;

        
        if (isGrounded) {
            currentSpeed *= speedFactor;
            currentSpeed += moveInput * acceleration * Time.deltaTime;
            // Apply friction
            currentSpeed *= friction;
        }
        // Accelerate and decelerate
        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);

        // Braking
        if (Input.GetKey(KeyCode.S) && currentSpeed > 0)
            currentSpeed -= brakeForce * Time.deltaTime;

        if (Mathf.Abs(currentSpeed) < 0.05f) currentSpeed = 0f;

        // Wall detection with Raycast
        Vector3 moveDirection = transform.forward.normalized;
        float rayLength = 1.0f + Mathf.Abs(currentSpeed * Time.deltaTime) / 5f; // look ahead
        RaycastHit hit;

        if (Physics.Raycast(transform.position, moveDirection, out hit, rayLength))
        {
            if (!hit.collider.isTrigger && !hit.collider.gameObject.name.Contains("Ramp")) // ignore triggers
            {
                Debug.Log("Wall detected: " + hit.collider.name);

                // Stop or slow down when close
                currentSpeed = Mathf.Lerp(currentSpeed, 0f, 0.5f);
            }
        }

        // Apply movement
        Vector3 movement = transform.forward * currentSpeed * Time.deltaTime;

        controller.Move(movement);

        // Velocity check
        float elapsed = Time.time - checkStartTime;
        if (elapsed >= 0.05f) // check every 0.1 seconds
        {
            float actualDistance = Vector3.Distance(checkStartPosition, transform.position);
            float expectedDistance = Mathf.Abs(currentSpeed) * elapsed;

            // If scooter barely moved compared to expected distance, wall hit
            if (actualDistance < expectedDistance * 0.25f) // tolerance factor
            {
                Debug.Log("Likely hit a wall");
                currentSpeed = 0f;
            }

            // Reset for next interval
            checkStartPosition = transform.position;
            checkStartTime = Time.time;
        }

        lastPosition = new Vector3(transform.position.x, 0, transform.position.z);
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

    public void SetMouseSensitivity(float sensitivity) {
        mouseSensitivity = sensitivity;
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity); // Save to PlayerPrefs
    }

    void OnTriggerEnter(Collider hit) {
        Debug.Log(hit.gameObject.name);
        if (hit.gameObject.name.Contains("Bad Car")) {
            gm.GameLose();
        }
    }

    void interactRaycast() {
        Vector3 origin = Camera.main.transform.position;
        Vector3 dir = Camera.main.transform.forward;
        RaycastHit[] hits = Physics.RaycastAll(origin, dir, interactDistance);
        if (hits.Length > 0) {
            bool foundSomething = false;
            foreach (RaycastHit hit in hits) {
                Collectible interactableObject = hit.collider.gameObject.GetComponent<Collectible>();
                if (interactableObject != null) {
                    foundSomething = true;
                    if (Input.GetKeyDown(pushKey)) {
                        interactableObject.Interact();
                    } else if (Input.GetKeyDown(pullKey)) {
                        interactableObject.Interact();
                    }
                }
            }
            if (foundSomething) {
                crosshair.SetActive(false);
                bigCrosshair.SetActive(true);
                Debug.Log("Raycase");
            } else {
                crosshair.SetActive(true);
                bigCrosshair.SetActive(false);
            }
        } else {
            crosshair.SetActive(true);
            bigCrosshair.SetActive(false);
        }
    }
}
