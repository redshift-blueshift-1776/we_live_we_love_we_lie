using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; 

public class Player_Movement_Open_Exploration : MonoBehaviour
{
    public enum OpenExplorationVehicle {Walking, Scooter, F150};
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

    public OpenExplorationVehicle vehicle;

    [Header("Scooter Stats")]
    public Vector3 velocityScooter;
    public float gravityScooter = 32f;
    [SerializeField] private float acceleration = 1000f; // Acceleration rate
    [SerializeField] private float maxSpeed = 420f; // Maximum speed
    [SerializeField] private float brakeForce = 666f; // Braking power
    [SerializeField] private float friction = 0.98f; // Simulated drag
    [SerializeField] private float speedMultiplier = 20f; // Speed boost with Shift

    public float currentSpeed = 0f;
    public float groundCheckDistance = 1f;
    public LayerMask groundMask;
    public Vector3 lastPosition;
    [SerializeField] private bool isGroundedScooter;

    [Header("Scooter Rotation Settings")]
    public float rotationSpeed = 360f;   // degrees per second
    public float manualRotationSpeed = 30f;   // degrees per second

    [Header("Scooter Smoothing")]
    public float normalLerpSpeed = 10f;  // how quickly ground normal smooths

    private Vector3 smoothedNormal = Vector3.up;

    private Vector3 checkStartPosition;
    private float checkStartTime;

    private Camera _cachedMainCamera;
    private Camera CachedMainCamera
    {
        get
        {
            if (_cachedMainCamera == null)
            {
                _cachedMainCamera = Camera.main;
            }
            return _cachedMainCamera;
        }
    }

    private void Awake()
    {
        vehicle = OpenExplorationVehicle.Walking;
        initialPosition = gameObject.transform.position;
        initialRotation = gameObject.transform.rotation;
    }

    private void Start()
    {
        jumpVelocity = Mathf.Sqrt(-2 * gravityValue * jumpHeight);
        controller = gameObject.GetComponent<CharacterController>();
        // set the skin width appropriately according to Unity documentation: https://docs.unity3d.com/Manual/class-CharacterController.html
        controller.skinWidth = 0.1f * controller.radius;
        var camInit = CachedMainCamera;
        if (camInit != null)
        {
            defaultFieldOfView = camInit.fieldOfView;
        }
        else
        {
            defaultFieldOfView = 60f;
        }
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
        if (vehicle == OpenExplorationVehicle.Walking)
        {
            if (!controller.enabled || movementLocked) {
                return;
            }
            groundedPlayer = IsGrounded();

            jumpHelper();

            InteractRaycast();
            RotationHelper();
            timeSinceLastKick += Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.R)) {
                Respawn();
            }
        }
        else if (vehicle == OpenExplorationVehicle.Scooter)
        {
            crosshair.SetActive(false);
            bigCrosshair.SetActive(false);
            if (!controller.enabled || movementLocked) {
                return;
            }
            isGroundedScooter = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, ~0);
            if (controller.isGrounded) {
                isGroundedScooter = true;
            }
            // if (transform.position.y <= 1) {
            //     isGroundedScooter = true;
            // }

            if (isGroundedScooter && velocityScooter.y < 0)
            {
                velocityScooter.y = -0.5f; // Small offset to keep grounded
            }
            else
            {
                velocityScooter.y -= 1 * gravityScooter * Time.deltaTime;
            }

            HandleMovementScooter();
            RotationHelperScooter();
            AlignWithGroundScooter();
            Vector3 move = Vector3.zero;
            move.y = velocityScooter.y * Time.deltaTime; // Apply gravity
            if (controller.enabled)
            {
                controller.Move(move);
            }

            if (Input.GetKeyDown(KeyCode.R)) {
                Respawn();
            }

            if (Input.GetKeyDown(KeyCode.E)) {
                currentScooter.Dismount(gameObject);
            }

            if (MobileSuperCheat.Instance != null)
            {
                if (MobileSuperCheat.Instance.mobileSuperCheat
                    && MobileSuperCheat.Instance.interactPressed)
                {
                    currentScooter.Dismount(gameObject);
                    MobileSuperCheat.Instance.interactPressed = false;
                }
            }
        }

    }

    private void FixedUpdate()
    {
        if (vehicle == OpenExplorationVehicle.Walking)
        {
            MovePlayer();
        }
    }

    private void Respawn()
    {
        controller.enabled = false;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        controller.enabled = true;
        playerVelocity = Vector3.zero;
        // vehicle = OpenExplorationVehicle.Walking;
    }

    // ============================
    // WALKING MOVEMENT FUNCTIONS
    // ============================

    private void MovePlayer()
    {
        HorizontalMovementHelper();
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

        bool didJump = false;

        if (Input.GetKeyDown(KeyCode.Space)
            || (autoBunnyHopping && Input.GetKey(KeyCode.Space))
            || (MobileSuperCheat.Instance != null && MobileSuperCheat.Instance.jumpPressed))
        {
            jumpBufferCounter = jumpBufferTime;
            didJump = true;
        }

        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // conditions for jumping include coyote time and jump buffering
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

        // Consume the mobile input
        if (MobileSuperCheat.Instance != null) {
            if (MobileSuperCheat.Instance.jumpPressed && didJump)
            {
                Debug.Log("Consuming a jump");
                MobileSuperCheat.Instance.jumpPressed = false;
            }
        }
    }
    private Vector3 GetBottomPos()
    {
        return controller.transform.position + controller.center - new Vector3(0, gameObject.transform.localScale.y * controller.height / 2, 0);
    }

    private bool IsGrounded()
    {
        float sphereRadius = controller.radius * 0.99f;
        Vector3 bottomPos = GetBottomPos() + 1.001f * sphereRadius * Vector3.up;
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

    void ApplyRotation(Transform part, float targetX)
    {
        Quaternion target =
            Quaternion.Euler(targetX, 0, 0);

        part.localRotation =
            Quaternion.Lerp(
                part.localRotation,
                target,
                Time.deltaTime * 8f
            );
    }
    void HorizontalMovementHelper() {
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

        if (MobileSuperCheat.Instance != null)
        {
            if (MobileSuperCheat.Instance.mobileSuperCheat)
            {
                inputDirection += MobileSuperCheat.Instance.horizontal * transform.right;
                inputDirection += MobileSuperCheat.Instance.vertical * transform.forward;
            }
        }

        if (inputDirection == Vector3.zero)
        {
            ApplyRotation(leftLeg.transform, 0);
            ApplyRotation(rightLeg.transform, 0);
            ApplyRotation(leftArm.transform, 0);
            ApplyRotation(rightArm.transform, 0);
        }
        else
        {
            bool running = Input.GetKey(KeyCode.LeftShift);
            if (MobileSuperCheat.Instance != null)
            {
                if (MobileSuperCheat.Instance.mobileSuperCheat)
                {
                    running = true;
                }
            }
            float runSpeed = 12f;
            float runAngle = 60f;
            float walkSpeed = 5f;
            float walkAngle = 45f;
            float animSpeed = running ? runSpeed : walkSpeed;
            float angle = running ? runAngle : walkAngle;

            float swing =
                Mathf.Sin(Time.time * animSpeed) * angle;

            ApplyRotation(leftLeg.transform, swing);
            ApplyRotation(rightLeg.transform, -swing);
            ApplyRotation(leftArm.transform, -swing);
            ApplyRotation(rightArm.transform, swing);
        }

        inputDirection = Vector3.Normalize(inputDirection);

        bool isRunning = false;
        //running
        Vector3 playerAcceleration = inputDirection * baseAcceleration;
        if ((Input.GetKey(runKey) && !Input.GetKey(KeyCode.S)) || (MobileSuperCheat.Instance != null && MobileSuperCheat.Instance.mobileSuperCheat)) {
            isRunning = true;
            playerAcceleration *= 1.5f;
            //do not change the fov when holding shift alone
            if (inputDirection.magnitude > 0)
            {
                var camFov = CachedMainCamera;
                if (camFov != null)
                    camFov.fieldOfView = Mathf.MoveTowards(camFov.fieldOfView, fastFieldOfView, diffFOV * Time.deltaTime / 0.25f);
            }
        } else {
            var camFov2 = CachedMainCamera;
            if (camFov2 != null)
                camFov2.fieldOfView = Mathf.MoveTowards(camFov2.fieldOfView, defaultFieldOfView, diffFOV * Time.deltaTime / 0.25f);
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


    void RotationHelper() {
        // Rotates the camera and character object
        float rotX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float rotY = -Input.GetAxis("Mouse Y") * mouseSensitivity;

        if (MobileSuperCheat.Instance != null && MobileSuperCheat.Instance.mobileSuperCheat)
        {
            rotX = MobileSuperCheat.Instance.lookX;
            rotY = -MobileSuperCheat.Instance.lookY;
        }

        gameObject.transform.Rotate(0, rotX, 0);
        var cam = CachedMainCamera;
        if (cam == null) return;
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

    private Door currentDoor = null;
    private Open_Exploration_Collectible currentCollectible = null;

    public Open_Exploration_Scooter currentScooter = null;

    void InteractRaycast()
    {
        RaycastHit hit;
        var cam = CachedMainCamera;
        if (cam == null) return;
        Vector3 origin = cam.transform.position;
        Vector3 dir = cam.transform.forward;
        Door newDoor = null;
        Open_Exploration_Collectible collectible = null;
        Open_Exploration_Scooter scooter = null;
        bool didInteract = false;

        float radius = 0.05f;
        if (Physics.SphereCast(origin, radius, dir, out hit, interactDistance))
        {
            newDoor = hit.collider.GetComponent<Door>();
            collectible = hit.collider.GetComponent<Open_Exploration_Collectible>();
            scooter = hit.collider.GetComponent<Open_Exploration_Scooter>();
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
            if (MobileSuperCheat.Instance != null)
            {
                if (MobileSuperCheat.Instance.mobileSuperCheat)
                {
                    if (MobileSuperCheat.Instance.interactPressed)
                    {
                        currentDoor.Interact();
                        MobileSuperCheat.Instance.interactPressed = false;
                        didInteract = true;
                    }
                }
                else
                {
                    if (Input.GetKeyDown(pushKey))
                    {
                        currentDoor.Interact();
                        didInteract = true;
                    }
                    else if (Input.GetKeyDown(pullKey))
                    {
                        currentDoor.Interact();
                        didInteract = true;
                    }
                }
            }
            else
            {
                if (Input.GetKeyDown(pushKey))
                {
                    currentDoor.Interact();
                }
                else if (Input.GetKeyDown(pullKey))
                {
                    currentDoor.Interact();
                }
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
            if (MobileSuperCheat.Instance != null)
            {
                if (MobileSuperCheat.Instance.mobileSuperCheat
                    && MobileSuperCheat.Instance.interactPressed)
                {
                    currentCollectible.Interact();
                    MobileSuperCheat.Instance.interactPressed = false;
                    didInteract = true;
                }
            }

            if (!didInteract)
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

        // Only update state if changed
        if (scooter != currentScooter)
        {
            currentScooter = scooter;

            if (currentScooter != null)
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
        if (currentScooter != null)
        {
            if (MobileSuperCheat.Instance != null)
            {
                if (MobileSuperCheat.Instance.mobileSuperCheat
                    && MobileSuperCheat.Instance.interactPressed)
                {
                    currentScooter.Mount(gameObject);
                    MobileSuperCheat.Instance.interactPressed = false;
                    didInteract = true;
                }
            }

            if (!didInteract)
            {
                if (Input.GetKeyDown(pushKey))
                {
                    currentScooter.Mount(gameObject); // or InteractPush()
                }
                else if (Input.GetKeyDown(pullKey))
                {
                    currentScooter.Mount(gameObject); // or InteractPull()
                }
            }
        }
    }

    public void SetMouseSensitivity(float sensitivity) {
        mouseSensitivity = sensitivity;
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity); // Save to PlayerPrefs
    }

    void OnControllerColliderHit(ControllerColliderHit hit) {
        if (hit.rigidbody != null) {
            if (timeSinceLastKick >= 1f) {
                if (Input.GetKey(runKey) || (MobileSuperCheat.Instance != null && MobileSuperCheat.Instance.mobileSuperCheat)) {
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
                if (Input.GetKey(runKey) || (MobileSuperCheat.Instance != null && MobileSuperCheat.Instance.mobileSuperCheat)) {
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

    // ==========================
    // SCOOTER MOVEMENT FUNCTIONS
    // ==========================
    void HandleMovementScooter()
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

        if (MobileSuperCheat.Instance != null)
        {
            if (MobileSuperCheat.Instance.mobileSuperCheat)
            {
                moveInput += MobileSuperCheat.Instance.vertical;
                isBoosting = true;
            }
        }

        float speedFactor = isBoosting ? speedMultiplier : 1f;
        
        if (isGroundedScooter) {
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

        // Apply movement
        Vector3 movement = currentSpeed * Time.deltaTime * transform.forward;
        if (controller.enabled)
        {
            controller.Move(movement);
        }

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

    void AlignWithGroundScooter()
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
                if (!hit.collider.gameObject.name.Contains("Building")) {
                    seeARamp = true;
                    isGroundedScooter = true;
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
            if (!Input.GetKey(KeyCode.B)) {
                if (Input.GetKey(KeyCode.Space)) {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, transform.eulerAngles.y, 0), Time.deltaTime / 2f);
                } else {
                    // Reset rotation when not on a ramp
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, transform.eulerAngles.y, 0), Time.deltaTime);
                }
            }
        }
    }

    void RotationHelperScooter() {
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

        if (MobileSuperCheat.Instance != null && MobileSuperCheat.Instance.mobileSuperCheat)
        {
            rotX = MobileSuperCheat.Instance.lookX;
            rotY = -MobileSuperCheat.Instance.lookY;
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
}
