using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class NPC_Open_Exploration : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public GameObject iclsd;
    [SerializeField] public NPC_Dialogue npcd;
    public GameObject player;

    public bool talkingTo;

    [Header("Info")]
    [SerializeField] public string npcName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        npcd = iclsd.GetComponent<NPC_Dialogue>();
        player = GameObject.Find("Player_Open_Exploration");
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < 10f) {
            GameObject foundObject = GameObject.Find("Player_Open_Exploration/Canvas/NPC_Visual");
            if (foundObject != null) {
                // Debug.Log("Found Visual");
                foundObject.SetActive(!talkingTo);
            } else {
                Debug.Log("No Visual");
            }
            if (Input.GetKeyDown(KeyCode.Mouse1)) {
                talkingTo = true;
                FreezePlayer();
                StartCoroutine(startConversation());
            }
        } else {
            GameObject foundObject = GameObject.Find("Player_Open_Exploration/Canvas/NPC_Visual");
            if (foundObject != null) {
                // Debug.Log("Found Visual");
                foundObject.SetActive(false);
            }
            talkingTo = false;
        }
        if (Input.GetKeyDown(KeyCode.E) && talkingTo) {
            endConversation();
        }
    }

    private void FreezePlayer()
    {
        player.GetComponent<CharacterController>().enabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void UnfreezePlayer()
    {
        player.GetComponent<CharacterController>().enabled = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void endConversation() {
        UnfreezePlayer();
        Debug.Log("Unfreeze player");
        talkingTo = false;
        GameObject foundObject = GameObject.Find("Player_Open_Exploration/Canvas/RawImage");
        if (foundObject != null) {
            // Debug.Log("Found Visual");
            foundObject.SetActive(true);
        } else {
            Debug.Log("No Visual");
        }
    }

    public IEnumerator startConversation() {
        GameObject foundObject = GameObject.Find("Player_Open_Exploration/Canvas/RawImage");
        if (foundObject != null) {
            // Debug.Log("Found Visual");
            foundObject.SetActive(false);
        } else {
            Debug.Log("No Visual");
        }
        foundObject = GameObject.Find("Player_Open_Exploration/Canvas/RawImage (1)");
        if (foundObject != null) {
            // Debug.Log("Found Visual");
            foundObject.SetActive(false);
        } else {
            Debug.Log("No Visual");
        }
        Vector3 startPosition = player.transform.position + new Vector3(0,0,0);
        Vector3 targetPosition = transform.position + transform.forward * -5f + new Vector3(0, 2.5f, 0);
        Quaternion startRotation = player.transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(transform.forward);

        float elapsed = 0f;
        float duration = 2f;
        while (elapsed < duration) {
            float tx = elapsed / duration;
            float t = tx * tx;
            player.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            player.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        player.transform.position = targetPosition;
        player.transform.rotation = targetRotation;
        npcd.showLyrics();
        yield return null;
    }
}
