using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class EliminationPillar : MonoBehaviour
{
    [SerializeField] private GameObject leftHinge;
    [SerializeField] private GameObject rightHinge;
    [SerializeField] public GameObject transition;
    private Transition transitionScript;

    [SerializeField] public float delay = 2f;
    [SerializeField] public bool eliminate;
    [SerializeField] public bool drop;
    [SerializeField] public Material originalMaterial;
    [SerializeField] public Material eliminateMaterial;
    [SerializeField] public GameObject[] glowingParts;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (GameObject part in glowingParts) {
            part.GetComponent<MeshRenderer>().material = originalMaterial;
        }
        transitionScript = transition.GetComponent<Transition>();
        if (drop) {
            StartCoroutine(OpenTrapDoor());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator OpenTrapDoor() {
        float duration = 1f;
        float elapsed = 0f;
        Quaternion leftStart = leftHinge.transform.localRotation;
        Quaternion rightStart = rightHinge.transform.localRotation;
        Quaternion targetRotationLeft = Quaternion.Euler(0, 0, -90);
        Quaternion targetRotationRight = Quaternion.Euler(0, 0, 90);
        yield return new WaitForSeconds(delay);
        foreach (GameObject part in glowingParts) {
            part.GetComponent<MeshRenderer>().material = eliminateMaterial;
        }
        while (elapsed < duration) {
            float t = elapsed / duration;

            leftHinge.transform.localRotation = Quaternion.Slerp(leftStart, targetRotationLeft, t);
            rightHinge.transform.localRotation = Quaternion.Slerp(rightStart, targetRotationRight, t);

            elapsed += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(3f);
        if (eliminate) {
            transitionScript.ToFail();
        }
    }
}
