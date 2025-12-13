using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_Movement_Open_Exploration : MonoBehaviour
{
    [SerializeField] public GameObject mainCamera;
    private CharacterController controller;

    private Vector3 playerVelocity = new(0,0,0);
    private bool groundedPlayer;
    public static float basePlayerSpeed = 5.0f;

    public static float speedUp = 4.2f;

    // time to run from standstill
    private static float timeToRun = 6;

    public float playerSpeed = 0;

    private float jumpHeight = 2.0f;
    private float gravityValue = -16f;

    private float jumpVelocity;

    //private float playerMass = 120;

    public static float mouseSensitivity = 1;


    private float interactDistance = 5f;


    public float maxSpeed;

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


    private void Start()
    {
        jumpVelocity = Mathf.Sqrt(-2 * gravityValue * jumpHeight);
        controller = gameObject.GetComponent<CharacterController>();
        // set the skin width appropriately according to Unity documentation: https://docs.unity3d.com/Manual/class-CharacterController.html
        controller.skinWidth = 0.1f * controller.radius;
        maxSpeed = basePlayerSpeed * speedUp;
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
        // Application.targetFrameRate = 6;
    }

    void Update()
    {
        if (!controller.enabled) {
            return;
        }
        // modify player velocity
        jumpHelper();
        horizontalMovementHelper();
        // move player
        controller.Move(playerVelocity * Time.deltaTime);
        interactRaycast();
        rotationHelper();
        timeSinceLastKick += Time.deltaTime;
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
            playerSpeed = Mathf.MoveTowards(playerSpeed, basePlayerSpeed, 10 * maxSpeed * Time.deltaTime / timeToRun);
            Camera.main.fieldOfView = Mathf.MoveTowards(Camera.main.fieldOfView, defaultFieldOfView, diffFOV * Time.deltaTime / timeToRun);
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
            }
        }
    }
}
