using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

public class Player_Movement : MonoBehaviour
{
    [SerializeField] public GameObject gameManager;

    [SerializeField] public GameObject mainCamera;
    [SerializeField] public GameObject altCamera;
    [SerializeField] public GameObject fakeBody;
    public WalkAlongThePathUnknown gm;
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
    private readonly KeyCode pushKey = KeyCode.Mouse0;
    private readonly KeyCode pullKey = KeyCode.Mouse1;

    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject bigCrosshair;

    private Camera _cachedMainCamera;
    private Camera CachedMainCamera
    {
        get
        {
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


    private void Start()
    {
        gm = gameManager.GetComponent<WalkAlongThePathUnknown>();
        jumpVelocity = Mathf.Sqrt(-2 * gravityValue * jumpHeight);
        controller = gameObject.GetComponent<CharacterController>();
        
        controller.skinWidth = 0.1f * controller.radius;
        maxSpeed = basePlayerSpeed * speedUp;
        var mainCamInit = CachedMainCamera;
        if (mainCamInit != null)
        {
            defaultFieldOfView = mainCamInit.fieldOfView;
        }
        else
        {
            defaultFieldOfView = 60f;
        }
        
        fastFieldOfView = defaultFieldOfView * fieldOfViewMultiplier;
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);
        if (crosshair != null) crosshair.SetActive(true);
        if (bigCrosshair != null) bigCrosshair.SetActive(false);
        if (mainCamera != null) mainCamera.SetActive(true);
        if (altCamera != null) altCamera.SetActive(false);
        if (fakeBody != null) fakeBody.SetActive(false);
        int usePostProcessing = PlayerPrefs.GetInt("useVisualEffects", 0);
        var camPP = CachedMainCamera;
        if (camPP != null)
        {
            var cameraData = camPP.GetComponent<UniversalAdditionalCameraData>();
            if (cameraData == null) cameraData = camPP.GetUniversalAdditionalCameraData();
            if (cameraData != null)
            {
                cameraData.renderPostProcessing = (usePostProcessing != 0);
            }
        }
    }

    void Update()
    {
        if (gm.gameActive) {
            // modify player velocity
            jumpHelper();
            horizontalMovementHelper();
            // move player
            controller.Move(playerVelocity * Time.deltaTime);
            interactRaycast();
            rotationHelper();
        }
    }


    // void jumpHelper() {
    //     groundedPlayer = controller.isGrounded;
    //     if (groundedPlayer && playerVelocity.y < 0) {
    //         playerVelocity.y = 0f;
    //     }

    //     // Changes the height position of the player..
    //     if (Input.GetKeyDown(KeyCode.Space) && groundedPlayer) {
    //         playerVelocity.y += jumpVelocity;
    //     }

    //     if (MobileSuperCheat.Instance != null)
    //     {
    //         if (MobileSuperCheat.Instance.mobileSuperCheat)
    //         {
    //             if (MobileSuperCheat.Instance.jumpPressed && groundedPlayer) {
    //                 playerVelocity.y += jumpVelocity;
    //             }
    //         }
    //     }

    //     playerVelocity.y += gravityValue * Time.deltaTime;
    // }

    void jumpHelper() {
        groundedPlayer = controller.isGrounded;

        if (groundedPlayer && playerVelocity.y < 0) {
            playerVelocity.y = 0f;
        }

        bool jumpInput = Input.GetKeyDown(KeyCode.Space);

        if (MobileSuperCheat.Instance != null &&
            MobileSuperCheat.Instance.mobileSuperCheat)
        {
            jumpInput |= MobileSuperCheat.Instance.jumpPressed;
        }
        bool didJump = false;

        if (jumpInput) {
            if (groundedPlayer)
            {
                playerVelocity.y += jumpVelocity;
                didJump = true;
            }
            else
            {
                Debug.Log("!groundedPlayer");
            }
            
        }

        // Consume the mobile input
        if (MobileSuperCheat.Instance != null) {
            if (MobileSuperCheat.Instance.jumpPressed && didJump)
            {
                Debug.Log("Consuming a jump");
                MobileSuperCheat.Instance.jumpPressed = false;
            }
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

        if (MobileSuperCheat.Instance != null)
        {
            if (MobileSuperCheat.Instance.mobileSuperCheat)
            {
                hSpeed += MobileSuperCheat.Instance.horizontal;
                vSpeed += MobileSuperCheat.Instance.vertical;
            }
        }

        if ((Input.GetKey(runKey) && vSpeed > 0) || (MobileSuperCheat.Instance != null && MobileSuperCheat.Instance.mobileSuperCheat)) {
            playerSpeed = Mathf.MoveTowards(playerSpeed, maxSpeed, maxSpeed * Time.deltaTime / timeToRun);
            var camFov = CachedMainCamera;
            if (camFov != null)
                camFov.fieldOfView = Mathf.MoveTowards(camFov.fieldOfView, fastFieldOfView, diffFOV * Time.deltaTime / timeToRun);
        } else {
            playerSpeed = Mathf.MoveTowards(playerSpeed, basePlayerSpeed, maxSpeed * Time.deltaTime / timeToRun);
            var camFov2 = CachedMainCamera;
            if (camFov2 != null)
                camFov2.fieldOfView = Mathf.MoveTowards(camFov2.fieldOfView, defaultFieldOfView, diffFOV * Time.deltaTime / timeToRun);
        }
        playerVelocity += Vector3.Normalize(gameObject.transform.right * hSpeed + gameObject.transform.forward * vSpeed) * playerSpeed;
    }


    void rotationHelper() {
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

    void interactRaycast() {
        RaycastHit hit;
        var cam = CachedMainCamera;
        if (cam == null) return;
        Vector3 origin = cam.transform.position;
        Vector3 dir = cam.transform.forward;
        float radius = 0.05f;
        if (Physics.SphereCast(origin, radius, dir, out hit, interactDistance)) {
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
                } else if (MobileSuperCheat.Instance != null) {
                    if (MobileSuperCheat.Instance.mobileSuperCheat)
                    {
                        if (MobileSuperCheat.Instance.interactPressed)
                        {
                            interactableObject.Interact();
                            MobileSuperCheat.Instance.interactPressed = false;
                        }
                    }
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

    void OnTriggerEnter(Collider hit) {
        if (hit.gameObject.name.Contains("Bullet")) {
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.enabled = false; // Disables the Mesh Renderer
            }
            mainCamera.SetActive(false);
            altCamera.SetActive(true);
            fakeBody.SetActive(true);
            gm.Fail();
        }
    }

    public void SetMouseSensitivity(float sensitivity) {
        mouseSensitivity = sensitivity;
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity); // Save to PlayerPrefs
    }
}
