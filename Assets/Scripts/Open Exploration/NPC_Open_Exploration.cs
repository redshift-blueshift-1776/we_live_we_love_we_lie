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
    private InteractionManager interactionManager;
    public GameObject player;

    public bool talkingTo;

    [Header("Info")]
    [SerializeField] public string npcName;
    [SerializeField] public int levelID;
    public float interactRange = 10f;
    public bool playerInRange;
    private Coroutine conversationRoutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interactionManager = InteractionManager.Instance;
        npcd = iclsd.GetComponent<NPC_Dialogue>();
        player = GameObject.Find("Player_Open_Exploration");

        if (levelID == 0) {
            if (npcName == "Jeff") {
                npcd.lyricsText = new string[] {
                    "l:",
                    "l:Hello Jeff.",
                    "r:Hi Wade.",
                    "l:Have you heard about Spectre Games?",
                    "r:Yeah, but I'm probably not going to do it.",
                    "r:Professor Donald James is across the street if you want to speak to him, though...",
                    "r:"
                };
            }
            if (npcName == "Colin") {
                npcd.lyricsText = new string[] {
                    "l:",
                    "l:Hello Colin.",
                    "r:Hi Wade.",
                    "l:Have you heard about Spectre Games?",
                    "r:Oh, I'm definitely doing that...",
                    "l:Really? I didn't know you were into game dev...",
                    "r:Well, I played Doguns and Planetary Platformer recently.",
                    "r:So now I really want to join the program.",
                    "r:And I'm sure I'm going to win.",
                    "r:"
                };
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FreezePlayer()
    {
        var cc = player.GetComponent<CharacterController>();
        if (cc != null)
            cc.enabled = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }


    private void UnfreezePlayer()
    {
        player.GetComponent<CharacterController>().enabled = true;
        player.GetComponent<Player_Movement_Open_Exploration>().movementLocked = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // public void endConversation() {
    //     UnfreezePlayer();
    //     Debug.Log("Unfreeze player");
    //     npcd.ClearLyrics();
    //     talkingTo = false;
    //     GameObject foundObject = GameObject.Find("Player_Open_Exploration/Canvas/RawImage");
    //     if (foundObject != null) {
    //         // Debug.Log("Found Visual");
    //         foundObject.SetActive(true);
    //     } else {
    //         Debug.Log("No Visual");
    //     }
    // }
    public void endConversation()
    {
        if (!talkingTo) return;

        talkingTo = false;

        if (conversationRoutine != null)
        {
            StopCoroutine(conversationRoutine);
            conversationRoutine = null;
        }

        UnfreezePlayer();
        npcd.ClearLyrics();
        GameObject foundObject = GameObject.Find("Player_Open_Exploration/Canvas/RawImage");
        if (foundObject != null) {
            // Debug.Log("Found Visual");
            foundObject.SetActive(true);
        } else {
            Debug.Log("No Visual");
        }
    }

    public void StartConversationWrapper()
    {
        if (conversationRoutine != null)
            StopCoroutine(conversationRoutine);

        conversationRoutine = StartCoroutine(startConversation());
    }


    public IEnumerator startConversation() {
        if (talkingTo) yield break;
        talkingTo = true;

        FreezePlayer();

        Debug.Log("START CONVERSATION " + npcName);
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
        Vector3 targetPosition = transform.position + transform.forward * -4.20f + new Vector3(0, 2.5f, 0);
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
        Debug.Log("Calling Show Lyrics");
        npcd.showLyrics();
        yield return null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            interactionManager.RegisterNPC(this);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (talkingTo) return; // IMPORTANT

        playerInRange = false;
        interactionManager.UnregisterNPC(this);
    }

}
