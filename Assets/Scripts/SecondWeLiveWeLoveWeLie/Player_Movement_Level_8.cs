using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_Movement_Level_8 : MonoBehaviour
{
    [SerializeField] public GameObject gameManager;
    public SecondWeLiveWeLoveWeLie gm;
    private CharacterController controller;

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


    private float maxSpeed;
    private readonly KeyCode failKey = KeyCode.M;
    private readonly KeyCode mainKey = KeyCode.Space;

    // [SerializeField] private GameObject crosshair;
    // [SerializeField] private GameObject bigCrosshair;

    [SerializeField] public GameObject laser;
    [SerializeField] public GameObject fakeLaser;
    [SerializeField] public GameObject mainCamera;


    private void Start()
    {
        gm = gameManager.GetComponent<SecondWeLiveWeLoveWeLie>();
        jumpVelocity = Mathf.Sqrt(-2 * gravityValue * jumpHeight);
        controller = gameObject.GetComponent<CharacterController>();
        // set the skin width appropriately according to Unity documentation: https://docs.unity3d.com/Manual/class-CharacterController.html
        controller.skinWidth = 0.1f * controller.radius;
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);
        // crosshair.SetActive(true);
        // bigCrosshair.SetActive(false);
        laser.SetActive(false);
        fakeLaser.SetActive(false);
    }

    void Update()
    {
        if (gm.gameActive) {
            interactRaycast();
            rotationHelper();
        }
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
        if (Input.GetKey(mainKey)) {
            laser.SetActive(true);
            fakeLaser.SetActive(false);
            laser.transform.rotation = mainCamera.transform.rotation;
            laser.transform.position = mainCamera.transform.position + new Vector3(0, -0.5f, 0);
        } else {
            laser.SetActive(false);
            fakeLaser.SetActive(true);
            fakeLaser.transform.rotation = mainCamera.transform.rotation;
            fakeLaser.transform.position = mainCamera.transform.position + new Vector3(0, -0.5f, 0);
        }
    }

    public void SetMouseSensitivity(float sensitivity) {
        mouseSensitivity = sensitivity;
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity); // Save to PlayerPrefs
    }
}
