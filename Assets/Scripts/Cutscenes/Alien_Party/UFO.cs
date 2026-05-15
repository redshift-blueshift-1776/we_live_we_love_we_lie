using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class UFO : MonoBehaviour
{
    [Header("UFO Visuals")]
    [SerializeField] private GameObject ufoLight;
    [SerializeField] private Transform lightPivot;
    [SerializeField] private int numLights = 8;
    [SerializeField] private float lightPeriod = 1f;
    [SerializeField] private float rotationSpeed = 60f;
    [SerializeField] private GameObject partyBomb;
    [SerializeField] private Transform bombSpawnPoint;

    public List<GameObject> lights = new List<GameObject>();

    [Header("UFO Movements")]
    [SerializeField] public Transform startPosition;
    [SerializeField] public Transform endPosition;
    [SerializeField] public float flightDuration = 10f;
    [SerializeField] public float holdingTime = 2f;
    [SerializeField] private float wobble = 30f; // Tilt in degrees from (-wobble, wobble)
    [SerializeField] private float wobbleSpeed = 2f;

    [Header("Bomb")]
    [SerializeField] private float bombLowerDistance = 3f;
    [SerializeField] private float bombLowerDuration = 1.5f;
    [SerializeField] private float bombDropForce = 10f;

    public GameObject spawnedBomb;

    [Header("UFO Sounds")]
    [SerializeField] private AudioSource flightSound;
    [SerializeField] private AudioSource preDropSound;
    [SerializeField] private AudioSource dropSound;
    [SerializeField] private AudioSource explosionSound;

    [Header("Camera")]
    [SerializeField] public GameObject trackingCamera;
    public bool manualTrackCamera = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Spawn numLights lights
        for (int i = 0; i < numLights; i++)
        {
            GameObject newLight = Instantiate(ufoLight, lightPivot);

            float angle = i * 360f / numLights;

            
            newLight.transform.SetLocalPositionAndRotation(Quaternion.Euler(0, angle, 0) * Vector3.forward * 25f,
                Quaternion.Euler(0, angle, 0));
            newLight.transform.localScale = new Vector3(5, 1, 5);

            lights.Add(newLight);
        }

        transform.position = startPosition.position;

        StartCoroutine(FlyThenDrop());
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate the lights, flash the lights
        RotateLights();
        FlashLights();

        if (!manualTrackCamera)
        {
            trackingCamera.transform.SetParent(null);
        }
    }

    void RotateLights()
    {
        lightPivot.Rotate(rotationSpeed * Time.deltaTime * Vector3.up);
    }

    void FlashLights()
    {
        float t = Mathf.PingPong(Time.time / lightPeriod, 1f);

        foreach (GameObject lightObj in lights)
        {
            if (lightObj.TryGetComponent<Light>(out var l))
            {
                l.intensity = Mathf.Lerp(0.5f, 3f, t);
            }
        }
    }

    public IEnumerator FlyThenDrop()
    {
        if (flightSound != null)
        {
            flightSound.Play();
        }

        float elapsed = 0f;

        Vector3 initialPos = startPosition.position;
        Vector3 finalPos = endPosition.position;

        // Fly phase
        while (elapsed < flightDuration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / flightDuration;

            // Smooth movement
            transform.position = Vector3.Lerp(initialPos, finalPos, t);

            // UFO wobble
            float wobbleX = Mathf.Sin(Time.time * wobbleSpeed) * wobble;
            float wobbleZ = Mathf.Cos(Time.time * wobbleSpeed * 1.3f) * wobble;

            transform.rotation = Quaternion.Euler(wobbleX, t * 360f, wobbleZ);

            trackingCamera.transform.SetPositionAndRotation(new Vector3(-500, 100, 0) + transform.position,
                Quaternion.Euler(0, 90, 0));
            yield return null;
        }

        // Hold in place
        elapsed = 0f;
        Quaternion initialRotation = transform.rotation;

        while (elapsed < holdingTime)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / holdingTime;

            transform.rotation = Quaternion.Lerp(initialRotation, Quaternion.identity, t);

            trackingCamera.transform.SetPositionAndRotation(Vector3.Lerp(new Vector3(-500, 100, 0),
                new Vector3(-420, 67, 0), t) + transform.position, Quaternion.Euler(0, 90, 0));
            yield return null;
        }

        if (preDropSound != null)
        {
            preDropSound.Play();
        }

        // Spawn bomb
        spawnedBomb = Instantiate(
            partyBomb,
            bombSpawnPoint.position,
            Quaternion.identity
        );

        spawnedBomb.transform.localScale = new Vector3(100, 100, 100);

        // Lower bomb dramatically
        Vector3 bombStart = spawnedBomb.transform.position;
        Vector3 bombEnd = bombStart + Vector3.down * bombLowerDistance;

        elapsed = 0f;

        while (elapsed < bombLowerDuration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / bombLowerDuration;

            spawnedBomb.transform.position =
                Vector3.Lerp(bombStart, bombEnd, t);

            trackingCamera.transform.SetPositionAndRotation(Vector3.Lerp(new Vector3(-420, 67, 0),
                new Vector3(-320, 167, 0), t) + transform.position,
                Quaternion.Lerp(Quaternion.Euler(0, 90, 0), Quaternion.Euler(45, 90, 0), t));
            yield return null;
        }

        // Detach bomb
        
        if (spawnedBomb.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.down * bombDropForce;
        }

        if (dropSound != null)
        {
            dropSound.Play();
        }

        manualTrackCamera = false;
    }

    public void Explode()
    {
        if (explosionSound != null)
        {
            explosionSound.Play();
        }
    }
}