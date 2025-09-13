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
    private CharacterController controller;

    private Vector3 playerVelocity = new(0,0,0);
    private bool groundedPlayer;
    public static float basePlayerSpeed = 5.0f;

    public static float speedUp = 2.5f;

    // time to run from standstill
    private static float timeToRun = 2;

    private float playerSpeed = 0;

    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;

    private float jumpVelocity;

    //private float playerMass = 120;

    public static float mouseSensitivity = 1;


    private float interactDistance = 5f;


    private float maxSpeed;

    private float defaultFieldOfView;
    private float fieldOfViewMultiplier = 1.18f;
    private float fastFieldOfView;


    private readonly KeyCode runKey = KeyCode.LeftShift;
    private readonly KeyCode failKey = KeyCode.M;
    private readonly KeyCode pushKey = KeyCode.Mouse0;
    private readonly KeyCode pullKey = KeyCode.Mouse1;

    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject bigCrosshair;

    [SerializeField] WheelCollider frontRightWheel;
    [SerializeField] WheelCollider frontLeftWheel;
    [SerializeField] WheelCollider backRightWheel;
    [SerializeField] WheelCollider backLeftWheel;

    public float acceleration = 500f;
    public float brakingForce = 300f;
    public float maxTurnAngle = 15f;

    private float currentAcceleration = 0f;
    private float currentBrakingForce = 0f;
    private float currentTurnAngle = 0f;


    private void Awake()
    {
        gm = gameManager.GetComponent<ToFindWhatIveBecome>();
        jumpVelocity = Mathf.Sqrt(-2 * gravityValue * jumpHeight);
        controller = gameObject.GetComponent<CharacterController>();
        // set the skin width appropriately according to Unity documentation: https://docs.unity3d.com/Manual/class-CharacterController.html
        controller.skinWidth = 0.1f * controller.radius;
        maxSpeed = Player_Movement.basePlayerSpeed * Player_Movement.speedUp;
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);
        crosshair.SetActive(true);
        bigCrosshair.SetActive(false);
        defaultFieldOfView = Camera.main.fieldOfView;
        fastFieldOfView = defaultFieldOfView * fieldOfViewMultiplier;
    }

    void Update()
    {
        if (gm.gameActive) {
            // modify player velocity
            jumpHelper();
            // horizontalMovementHelper();
            // move player
            controller.Move(playerVelocity * Time.deltaTime);
            interactRaycast();
            // rotationHelper();
        }
    }

    private void FixedUpdate()
    {
        int vert = 0;
        int horiz = 0;
        if (Input.GetKey(KeyCode.W)) {
            vert = 1;
        }
        if (Input.GetKey(KeyCode.S)) {
            vert = -1;
        }
        if (Input.GetKey(KeyCode.D)) {
            horiz = 1;
        }
        if (Input.GetKey(KeyCode.A)) {
            horiz = -1;
        }
        //currentAcceleration = -acceleration * Input.GetAxis("Vertical");
        currentAcceleration = -acceleration * vert;
        if (Input.GetKey(KeyCode.Space))
        {
            currentBrakingForce = brakingForce;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentAcceleration *= 2f;
        }
        else currentBrakingForce = 0;

        frontRightWheel.motorTorque = currentAcceleration;
        frontLeftWheel.motorTorque = currentAcceleration;

        frontRightWheel.brakeTorque = currentBrakingForce;
        frontLeftWheel.brakeTorque = currentBrakingForce;
        backLeftWheel.brakeTorque = currentBrakingForce;
        backRightWheel.brakeTorque = currentBrakingForce;


        //currentTurnAngle = maxTurnAngle * Input.GetAxis("Horizontal");
        currentTurnAngle = maxTurnAngle * horiz;
        frontLeftWheel.steerAngle = currentTurnAngle;
        frontRightWheel.steerAngle = currentTurnAngle;
    }


    void jumpHelper() {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0) {
            playerVelocity.y = 0f;
        }

        // Changes the height position of the player..
        if (Input.GetKeyDown(KeyCode.Space) && groundedPlayer) {
            playerVelocity.y += jumpVelocity;
        }
        playerVelocity.y += gravityValue * Time.deltaTime;
    }


    void horizontalMovementHelper() {
        playerVelocity.x = 0;
        playerVelocity.z = 0;

        float diffFOV = math.abs(fastFieldOfView - defaultFieldOfView);

        float hSpeed = 0.0f;
        float vSpeed = 0.0f;

        if (Input.GetKey(KeyCode.W))
        {
            vSpeed += 1.0f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            vSpeed -= 1.0f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            hSpeed -= 1.0f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            hSpeed += 1.0f;
        }

        if (Input.GetKey(runKey) && vSpeed > 0) {
            playerSpeed = Mathf.MoveTowards(playerSpeed, maxSpeed, maxSpeed * Time.deltaTime / timeToRun);
            Camera.main.fieldOfView = Mathf.MoveTowards(Camera.main.fieldOfView, fastFieldOfView, diffFOV * Time.deltaTime / timeToRun);
        } else {
            playerSpeed = Mathf.MoveTowards(playerSpeed, basePlayerSpeed, maxSpeed * Time.deltaTime / timeToRun);
            Camera.main.fieldOfView = Mathf.MoveTowards(Camera.main.fieldOfView, defaultFieldOfView, diffFOV * Time.deltaTime / timeToRun);
        }
        if (Input.GetKey(failKey)) {
            Cursor.lockState = Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene("MoveInMenu");
        }
        playerVelocity += Vector3.Normalize(gameObject.transform.right * hSpeed + gameObject.transform.forward * vSpeed) * playerSpeed;
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

    void interactRaycast() {
        RaycastHit hit;
        Vector3 origin = Camera.main.transform.position;
        Vector3 dir = Camera.main.transform.forward;
        if (Physics.Raycast(origin, dir, out hit, interactDistance)) {
            Wall interactableObject = hit.collider.gameObject.GetComponent<Wall>();
            
            if (interactableObject != null) {
                // TODO: ADD PUSH/PULL INDICATOR TO HUD!!!!!
                crosshair.SetActive(false);
                bigCrosshair.SetActive(true);
                Debug.Log("Raycase");
                if (Input.GetKeyDown(pushKey)) {
                    interactableObject.Interact();
                } else if (Input.GetKeyDown(pullKey)) {
                    interactableObject.Interact();
                }
            } else {
                crosshair.SetActive(true);
                bigCrosshair.SetActive(false);
            }
        } else {
            crosshair.SetActive(true);
            bigCrosshair.SetActive(false);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit) {
        
    }

    public void SetMouseSensitivity(float sensitivity) {
        mouseSensitivity = sensitivity;
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity); // Save to PlayerPrefs
    }
}
