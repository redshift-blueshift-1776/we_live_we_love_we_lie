using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

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
    public float manualRotationSpeed = 30f;   // degrees per second

    [Header("Smoothing")]
    public float normalLerpSpeed = 10f;  // how quickly ground normal smooths

    private Vector3 smoothedNormal = Vector3.up;

    private Vector3 checkStartPosition;
    private float checkStartTime;

    [Header("Grapple")]
    [SerializeField] private float grappleSpeed = 420f;
    private enum GrappleState { Idle, Extending, Pulling }
    private GrappleState grappleState = GrappleState.Idle;
    public bool grappleUnlocked;
    private float grappleDistance = 1000f;
    private Vector3 grapplePoint;
    private GameObject ropeObject;
    public float currentRopeLength = 0f;
    [SerializeField] public bool banGrapple;
    public bool usedGrapple;

    [Header("Particles")]
    [SerializeField] public GameObject particleObjectRain;

    // Assign in inspector or create procedurally
    public GameObject ropePrefab; // a thin cylinder scaled to (1,1,1)

    private Camera _cachedMainCamera;
    private Camera CachedMainCamera
    {
        get
        {
            // Re-acquire if cached is null, inactive, or no longer the tagged MainCamera
            // (Level 2 swaps between cam1/cam2 via SetActive, so the cached menu cam
            // becomes inactive after startGameButton)
            if (_cachedMainCamera == null || !_cachedMainCamera.gameObject.activeInHierarchy)
                _cachedMainCamera = Camera.main;
            else
            {
                var currentMain = Camera.main;
                if (currentMain != null && currentMain != _cachedMainCamera)
                    _cachedMainCamera = currentMain;
            }
            return _cachedMainCamera;
        }
    }


    private void Awake()
    {
        GameObject foundObject2 = GameObject.Find("Universal_Manager");
        if (foundObject2 != null) {
            Debug.Log("Found Universal_Manager");
            Universal_Manager um = foundObject2.GetComponent<Universal_Manager>();
            grappleUnlocked = um.beatStoryMode;
        } else {
            Debug.Log("No Universal_Manager");
        }
        if (banGrapple) {
            grappleUnlocked = false;
        }
        if (gameManager == null)
        {
            // Fallback: try to find the manager in scene if prefab override was lost during Unity upgrade
            gameManager = GameObject.Find("ToFindWhatIveBecome");
            if (gameManager == null) gameManager = GameObject.Find("GameManager");
            if (gameManager == null)
            {
                var found = FindFirstObjectByType<ToFindWhatIveBecome>();
                if (found != null) gameManager = found.gameObject;
            }
            if (gameManager == null)
                Debug.LogWarning("Player_Movement_Level_2: gameManager reference missing and not found in scene. Interaction and win conditions will fail.");
        }
        if (gameManager != null)
            gm = gameManager.GetComponent<ToFindWhatIveBecome>();
        if (gm == null)
        {
            var found = FindFirstObjectByType<ToFindWhatIveBecome>();
            if (found != null) gm = found;
            if (gm == null) Debug.LogWarning("Player_Movement_Level_2: ToFindWhatIveBecome not found.");
        }
        // Fallback for crosshair refs if prefab overrides lost
        if (crosshair == null) crosshair = GameObject.Find("Crosshair");
        if (bigCrosshair == null) bigCrosshair = GameObject.Find("BigCrosshair");
        if (crosshair == null) Debug.LogWarning("Player_Movement_Level_2: crosshair not assigned.");
        if (bigCrosshair == null) Debug.LogWarning("Player_Movement_Level_2: bigCrosshair not assigned.");
        jumpVelocity = Mathf.Sqrt(-2 * gravityValue * jumpHeight);
        // controller = gameObject.GetComponent<CharacterController>();
        // set the skin width appropriately according to Unity documentation: https://docs.unity3d.com/Manual/class-CharacterController.html
        // controller.skinWidth = 0.1f * controller.radius;
        // maxSpeed = Player_Movement.basePlayerSpeed * Player_Movement.speedUp;
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);
        if (crosshair != null) {
            crosshair.SetActive(true);
        }
        if (bigCrosshair != null) {
            bigCrosshair.SetActive(false);
        }
        var camInit = CachedMainCamera;
        if (camInit != null)
        {
            defaultFieldOfView = camInit.fieldOfView;
        }
        else
        {
            defaultFieldOfView = 60f;
        }
        int usePostProcessing = PlayerPrefs.GetInt("useVisualEffects", 0);
        var camPP = CachedMainCamera;
        if (camPP != null)
        {
            var cameraData = camPP.GetComponent<UniversalAdditionalCameraData>();
            if (cameraData == null) {
                cameraData = camPP.GetUniversalAdditionalCameraData();
            }
            if (cameraData != null) {
                cameraData.renderPostProcessing = (usePostProcessing != 0);
            }
        }
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
        // Defensive: allow looking even if gm is null (common after scene upgrade losing prefab overrides)
        bool isGameActive = gm != null ? gm.gameActive : true;
        int usePostProcessing = PlayerPrefs.GetInt("useVisualEffects", 1);
        if (particleObjectRain != null) {
            if (usePostProcessing == 0) {
                // Low Detail Mode
                particleObjectRain.SetActive(false);
            } else {
                particleObjectRain.SetActive(true);
            }
        }
        if (isGameActive) {
            // modify player velocity
            // horizontalMovementHelper();
            // move player
            // controller.Move(playerVelocity * Time.deltaTime);
            interactRaycast();
            rotationHelper();
            HandleGrapple();
        }
        if (isGameActive) {
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
            Vector3 move = new(0, 0, 0)
            {
                y = velocity.y * Time.deltaTime // Apply gravity
            };

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
            // Debug.Log(hit.collider.gameObject.name);
            if (!hit.collider.transform.IsChildOf(transform)) // ignore self
            {
                // Debug.Log("Hit normal: " + hit.normal);
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
            if (!Input.GetKey(KeyCode.B) || banGrapple) {
                if (Input.GetKey(KeyCode.Space)) {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, transform.eulerAngles.y, 0), Time.deltaTime / 2f);
                } else {
                    // Reset rotation when not on a ramp
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, transform.eulerAngles.y, 0), Time.deltaTime);
                }
            }
        }
    }

    void HandleMovement()
    {
        float moveInput = 0f;

        // Forward and backward movement
        if (Input.GetKey(KeyCode.W))
        {
            moveInput = 1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveInput = -1f;
        } 

        // Boost logic
        bool isBoosting = Input.GetKey(KeyCode.LeftShift);
        float speedFactor = isBoosting ? speedMultiplier : 1f;

        
        if (isGrounded) {
            // currentSpeed *= speedFactor;
            currentSpeed += moveInput * acceleration * Time.deltaTime * speedFactor;
            // Apply friction
            float referenceFPS = 120f;

            float frictionPerSecond = Mathf.Pow(friction, referenceFPS);
            currentSpeed *= Mathf.Pow(frictionPerSecond, Time.deltaTime);
            // currentSpeed *= friction;
        }
        // Accelerate and decelerate
        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);

        // Braking
        if (Input.GetKey(KeyCode.S) && currentSpeed > 0)
        {
            currentSpeed -= brakeForce * Time.deltaTime;
        }

        if (Mathf.Abs(currentSpeed) < 0.05f) {
            currentSpeed = 0f;
        }

        // Wall detection with Raycast
        // Vector3 moveDirection = transform.forward.normalized;
        // float rayLength = 1.0f + Mathf.Abs(currentSpeed * Time.deltaTime) / 5f; // look ahead
        // RaycastHit hit;

        // if (Physics.Raycast(transform.position, moveDirection, out hit, rayLength))
        // {
        //     if (!hit.collider.isTrigger && !hit.collider.gameObject.name.Contains("Ramp")) // ignore triggers
        //     {
        //         Debug.Log("Wall detected: " + hit.collider.name);

        //         // Stop or slow down when close
        //         currentSpeed = Mathf.Lerp(currentSpeed, 0f, 0.5f);
        //     }
        // }

        // Apply movement
        Vector3 movement = currentSpeed * Time.deltaTime * transform.forward;
        // velocity = movement;
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

        if (Input.GetKey(KeyCode.A)) {
            if (Input.GetKey(KeyCode.S)) {
                rotX = manualRotationSpeed * Time.deltaTime;
            } else {
                rotX = -manualRotationSpeed * Time.deltaTime;
            }
        }
        if (Input.GetKey(KeyCode.D)) {
            if (Input.GetKey(KeyCode.S)) {
                rotX = -manualRotationSpeed * Time.deltaTime;
            } else {
                rotX = manualRotationSpeed * Time.deltaTime;
            }
        }

        gameObject.transform.Rotate(0, rotX, 0);
        var cam = CachedMainCamera;
        if (cam == null) {
            return;
        }
        cam.transform.Rotate(rotY, 0, 0);
        if (cam.transform.localEulerAngles.y == 180 && cam.transform.localEulerAngles.z == 180) {
            float diffBetweenUpDir = Mathf.Abs(270 - cam.transform.localEulerAngles.x);
            float diffBetweenDownDir = Mathf.Abs(90 - cam.transform.localEulerAngles.x);
            if (diffBetweenDownDir <= diffBetweenUpDir) {
                cam.transform.localEulerAngles = new Vector3(90, 0, 0);
            } else {
                cam.transform.localEulerAngles = new Vector3(270, 0, 0);
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
        RaycastHit hit;
        var cam = CachedMainCamera;
        if (cam == null) {
            return;
        }
        Vector3 origin = cam.transform.position;
        Vector3 dir = cam.transform.forward;
        float radius = 0.05f;
        if (Physics.SphereCast(origin, radius, dir, out hit, interactDistance)) {
            bool foundSomething = false;
            Collectible interactableObject = hit.collider.gameObject.GetComponent<Collectible>();
            if (interactableObject != null) {
                foundSomething = true;
                // Debug.Log("Raycase");
                if (Input.GetKeyDown(pushKey)) {
                    interactableObject.Interact();
                }
            }
            if (foundSomething) {
                if (crosshair != null) crosshair.SetActive(false);
                if (bigCrosshair != null) bigCrosshair.SetActive(true);
            } else {
                if (crosshair != null) crosshair.SetActive(true);
                if (bigCrosshair != null) bigCrosshair.SetActive(false);
            }
        } else {
            if (crosshair != null) crosshair.SetActive(true);
            if (bigCrosshair != null) bigCrosshair.SetActive(false);
        }
        if (Physics.Raycast(origin, dir, out hit, grappleDistance)) {
            // Debug.Log("Raycase Grapple");
            if (crosshair != null)
            {
                if (grappleUnlocked && !banGrapple) {
                    crosshair.transform.localRotation = Quaternion.Euler(0, 0, 45);
                } else {
                    crosshair.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
            }
        } else {
            if (crosshair != null) crosshair.transform.localRotation = Quaternion.Euler(0, 0, 0);
            if (crosshair != null) crosshair.SetActive(true);
            if (bigCrosshair != null) bigCrosshair.SetActive(false);
        }
    }

    void HandleGrapple()
    {
        if (!grappleUnlocked) {
            return;
        }

        // Fire grapple
        if (Input.GetKeyDown(pullKey) && grappleState == GrappleState.Idle) {
            TryStartGrapple();
        }

        // Cancel grapple
        if (Input.GetKeyUp(pullKey)) {
            CancelGrapple();
        }

        // Advance rope
        if (grappleState == GrappleState.Extending) {
            UpdateRopeExtension();
        }

        // Pull player
        if (grappleState == GrappleState.Pulling) {
            usedGrapple = true;
            UpdateGrapplePull();
        }
    }

    void TryStartGrapple()
    {
        RaycastHit hit;
        var cam = CachedMainCamera;
        if (cam == null) {
            return;
        }
        Vector3 origin = cam.transform.position;
        Vector3 dir = cam.transform.forward;

        if (Physics.Raycast(origin, dir, out hit, grappleDistance))
        {
            grapplePoint = hit.point;
            StartRope(origin + new Vector3(1, -1, 0), grapplePoint);
            grappleState = GrappleState.Extending;
        }
    }

    void StartRope(Vector3 start, Vector3 end)
    {
        ropeObject = Instantiate(ropePrefab);
        ropeObject.transform.position = start;
        ropeObject.transform.LookAt(end);
        currentRopeLength = 0f;
    }

    void UpdateRopeExtension()
    {
        currentRopeLength += grappleSpeed * Time.deltaTime;

        var cam = CachedMainCamera;
        if (cam == null) {
            return;
        }
        float totalDistance = Vector3.Distance(cam.transform.position + new Vector3(1, -1, 0), grapplePoint);

        ropeObject.transform.localScale = new Vector3(
            0.5f,
            0.5f,
            currentRopeLength / 2f
        );

        ropeObject.transform.position = cam.transform.position + new Vector3(1, -1, 0) + 
            ropeObject.transform.forward * (currentRopeLength / 2f);

        // Reached point, switch to pulling
        if (currentRopeLength >= totalDistance) {
            grappleState = GrappleState.Pulling;
        }
    }

    void UpdateGrapplePull()
    {
        Vector3 direction = (grapplePoint - transform.position).normalized;

        controller.Move(grappleSpeed * Time.deltaTime * direction);

        // close enough, auto-cancel
        if (Vector3.Distance(transform.position, grapplePoint) < 1.5f)
        {
            CancelGrapple();
        }
    }

    void CancelGrapple()
    {
        grappleState = GrappleState.Idle;

        if (ropeObject != null)
            Destroy(ropeObject);

        ropeObject = null;
    }
}
