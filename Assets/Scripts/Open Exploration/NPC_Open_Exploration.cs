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
    public GameObject mainCamera;

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
        mainCamera = GameObject.Find("Player_Open_Exploration/Main Camera");

        // Go to Meetinghouse
        if (levelID == 0) {
            if (npcName == "Jeff") {
                npcd.lyricsText = new string[] {
                    "l:",
                    "l:Hello Jeff.",
                    "r:Hi Wade.",
                    "l:Have you heard about Spectre Games?",
                    "r:Yeah, but I'm probably not going to do it.",
                    "r:Professor Donald James is across the street if you want to speak to him, though...",
                    "r:Apparently they need 1000 people...",
                    "r:95% of the contestants will probably get eliminated in the first two rounds.",
                    "r:"
                };
                npcd.rightName = "Jeff Johnson";
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
                    "r:Also, Lan Attis ranked One Large Waffle above Planetary Platformer.",
                    "r:I don't know how Donald James hasn't exploded yet.",
                    "r:"
                };
                npcd.rightName = "Colin Norton";
            }

            if (npcName == "Jessica") {
                npcd.lyricsText = new string[] {
                    "l:",
                    "l:Hello Jessica.",
                    "r:Yo.",
                    "l:Have you heard about Spectre Games?",
                    "r:I'm literally praying that I win so I can get into the game dev program.",
                    "l:...",
                    "r:You know, I applied to Pen You Never City as a game dev major.",
                    "r:But I didn't get it, so I had to do computer science instead.",
                    "r:I've been praying every day for an opportunity to join... and the prayers have been answered.",
                    "l:Ok... Also, didn't this Meetinghouse have benches?",
                    "r:Bruh, you're trying to get back into the game dev program, and you worry about the benches?",
                    "r:I'm sure Donald James just took them for the game or something.",
                    "r:Also, why does everyone keep calling this place Pen City now?",
                    "r:No one talks like that in real life.",
                    "r:"
                };
                npcd.rightName = "Jessica She";
            }

            if (npcName == "Patrick") {
                npcd.lyricsText = new string[] {
                    "l:",
                    "l:Hi Patrick.",
                    "r:What's up Wade.",
                    "l:You signing up for Spectre Games?",
                    "r:Yeah.",
                    "r:I heard the final challenge is supposed to be some giant rhythm game.",
                    "r:Which is funny because none of these developers know how to make rhythm game editors.",
                    "r:"
                };
                npcd.rightName = "Patrick Farmer";
            }
        }

        // Go to F150
        if (levelID == 1) {
            if (npcName == "Jessica") {
                npcd.lyricsText = new string[] {
                    "l:",
                    "l:Hello Jessica.",
                    "r:Yo.",
                    "l:I just signed up for Spectre Games.",
                    "r:Makes sense.",
                    "l:...",
                    "r:I mean, Lan Attis put Love Sees Differences as the best game here...",
                    "r:I personally think Luca is better because of the music, art, and message, but...",
                    "r:At least Love Sees Differences actually released.",
                    "r:Half the games in this universe exist only in dialogue boxes.",
                    "r:"
                };
                npcd.rightName = "Jessica She";
            }

            if (npcName == "Jacob") {
                npcd.lyricsText = new string[] {
                    "l:",
                    "l:Hello Jacob.",
                    "r:Hello Wade.",
                    "l:Are you doing Spectre Games?",
                    "r:Yes. I mean, this is the program everyone at this school wants to join.",
                    "r:Well, maybe not everyone, but you get the point.",
                    "r:Honestly, I just hope there isn't another Alien Party situation.",
                    "l:Alien Party?",
                    "r:You know. The median game.",
                    "r:Where everyone spends twenty hours optimizing a strategy just to lose to someone named aintNoWay.",
                    "r:"
                };
                npcd.rightName = "Jacob Martin";
            }

            if (npcName == "Mary") {
                npcd.lyricsText = new string[] {
                    "l:",
                    "l:Hello Mary.",
                    "r:Hi Wade.",
                    "l:Are you excited for Spectre Games?",
                    "r:Kind of.",
                    "r:I'm more worried about the social part.",
                    "r:Every time I walk through campus someone starts doing the Stickity Stack dance.",
                    "r:And then five other people join in like it's some kind of ritual.",
                    "r:"
                };
                npcd.rightName = "Mary Kuang";
            }
        }

        // Go to The Circle
        if (levelID == 2) {
            if (npcName == "Jeff") {
                npcd.lyricsText = new string[] {
                    "l:",
                    "l:Hey, how did I get out here?",
                    "l:Aren't these walls supposed to be solid?",
                    "r:They are...",
                    "r:But they say that words have no wings, but they can fly thousands of miles.",
                    "l:So I flew through the wall?",
                    "r:If you want to interpret it like that, you can...",
                    "r:I guess there's just some power of speaking to people...",
                    "l:No one is going to believe that this is an intentional game mechanic...",
                    "r:It's fine.",
                    "r:If the players complain, just call it symbolic.",
                    "r:"
                };
                npcd.rightName = "Jeff Johnson";
            }

            if (npcName == "Emma") {
                npcd.lyricsText = new string[] {
                    "l:",
                    "l:Hi Emma.",
                    "r:Hi Wade!",
                    "l:How are you feeling about Spectre Games?",
                    "r:Oh, that first task was easy...",
                    "r:I can't believe that 724 people got eliminated.",
                    "r:I saw someone crying because they spent three weeks making lore for their character.",
                    "r:And then they lost in round one to semicircle and a triangle.",
                    "r:"
                };
                npcd.rightName = "Emma Rose";
            }

            if (npcName == "Rahima") {
                npcd.lyricsText = new string[] {
                    "l:",
                    "l:Hello Rahima.",
                    "r:Hey Wade.",
                    "l:How's Spectre Games going?",
                    "r:Pretty well.",
                    "r:Though I still don't understand the Religion of Common Sense building.",
                    "l:Why?",
                    "r:Because every time someone explains it, it somehow makes less sense.",
                    "r:"
                };
                npcd.rightName = "Rahima Sherazi";
            }
        }

        // Find Lan Attis, Open Exploration 3, between 3 and 4.
        if (levelID == 3) {
            if (npcName == "Colin") {
                npcd.lyricsText = new string[] {
                    "l:",
                    "l:Hello Colin.",
                    "r:Hi Wade.",
                    "r:Only 32 contestants left now.",
                    "r:Honestly, most of them were never winning anyway.",
                    "l:That's harsh.",
                    "r:This entire competition is harsh.",
                    "r:Donald James literally dropped 724 people from pillars.",
                    "r:"
                };
                npcd.rightName = "Colin Norton";
            }

            if (npcName == "Mario") {
                npcd.lyricsText = new string[] {
                    "l:",
                    "l:Hello Mario.",
                    "r:Hey Wade.",
                    "l:You seem calm.",
                    "r:That's because I already accepted that this universe runs on nonsense.",
                    "r:People phase through walls.",
                    "r:There are random dance mobs.",
                    "r:And half the textures on campus have aliasing issues.",
                    "r:"
                };
                npcd.rightName = "Mario Carideo";
            }

            if (npcName == "Mack") {
                npcd.lyricsText = new string[] {
                    "l:",
                    "l:Hello Mack.",
                    "r:Hey Wade.",
                    "r:"
                };
                npcd.rightName = "Mack Aroni";
            }
        }

        // After level 4, going down to the library from behind Main
        if (levelID == 4) {
            if (npcName == "Colin") {
                npcd.lyricsText = new string[] {
                    "l:",
                    "l:Hello Colin.",
                    "r:Hi Wade.",
                    "r:That last challenge was so easy...",
                    "r:Donald James better get some harder challenges...",
                    "r:I hope he eliminates a lot of the competition...",
                    "r:Because that'll make it easier for me to win!",
                    "r:Also, if I lose to some random rhythm game level, I'm uninstalling uninstall.exe.",
                    "r:"
                };
                npcd.rightName = "Colin Norton";
            }

            if (npcName == "Luba") {
                npcd.lyricsText = new string[] {
                    "l:",
                    "l:Hello Luba.",
                    "r:Hi Wade.",
                    "l:Only sixteen contestants left now.",
                    "r:Eight after tonight, probably.",
                    "r:Honestly, I think Donald James enjoys making elimination speeches more than making games.",
                    "r:"
                };
                npcd.rightName = "Luba Novikova";
            }

            if (npcName == "IY-zak") {
                npcd.lyricsText = new string[] {
                    "l:",
                    "l:Hello... uh...",
                    "r:IY-zak Wiyt.",
                    "l:Right.",
                    "r:You almost pronounced it correctly.",
                    "l:So how are you feeling about Spectre Games?",
                    "r:I spent six hours studying Alien Party strategies.",
                    "r:Then I realized this competition has absolutely nothing to do with Alien Party.",
                    "r:So naturally, I'm more confident than ever.",
                    "r:"
                };
                npcd.rightName = "IY-zak Wiyt";
            }
        }

        // Before level 6, go from the front of Main to the side of Main.
        if (levelID == 5) {
            if (npcName == "Jeff") {
                npcd.lyricsText = new string[] {
                    "l:",
                    "l:Jeff, why are you here?",
                    "r:I live here now.",
                    "l:What?",
                    "r:Every Open Exploration level needs at least one NPC to explain the weird lore.",
                    "r:Otherwise players will think the developers forgot to finish the story.",
                    "l:Did they?",
                    "r:Probably.",
                    "r:"
                };
                npcd.rightName = "Jeff Johnson";
            }
        }

        // Before level 8, go from inside Drayton to the top floor of the library
        // Can see much more of the world
        if (levelID == 6) {
            if (npcName == "Jeff") {
                npcd.lyricsText = new string[] {
                    "l:",
                    "l:Jeff.",
                    "r:Wade.",
                    "l:Why is everyone dancing near Stickity Stack?",
                    "r:No one knows.",
                    "r:One person started doing the dance ironically.",
                    "r:Then someone added music.",
                    "r:Now it's a campus tradition.",
                    "r:The entire economy of this city is held together by breadsticks and earworms.",
                    "r:"
                };
                npcd.rightName = "Jeff Johnson";
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FreezePlayer()
    {
        if (player.TryGetComponent<CharacterController>(out var cc))
        {
            cc.enabled = false;
        }

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
    
    public void endConversation()
    {
        if (!talkingTo) {
            return;
        }

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
        if (player.GetComponent<Player_Movement_Open_Exploration>().vehicle
            != Player_Movement_Open_Exploration.OpenExplorationVehicle.Walking)
        {
            return;
        }
        if (conversationRoutine != null)
        {
            StopCoroutine(conversationRoutine);
        }  

        conversationRoutine = StartCoroutine(StartConversation());
    }


    public IEnumerator StartConversation() {
        if (talkingTo)
        {
            yield break;
        }
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
        Vector3 targetPosition = transform.position + transform.forward * -4.20f + new Vector3(0, 3f, 0);
        Quaternion startRotation = player.transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(transform.forward);
        Quaternion startRotation2 = mainCamera.transform.rotation;
        Quaternion targetRotation2 = Quaternion.LookRotation(transform.forward);

        float elapsed = 0f;
        float duration = 2f;
        while (elapsed < duration) {
            float tx = elapsed / duration;
            float t = tx * tx;
            player.transform.SetPositionAndRotation(Vector3.Lerp(startPosition, targetPosition, t),
                                                    Quaternion.Slerp(startRotation, targetRotation, t));
            mainCamera.transform.rotation = Quaternion.Slerp(startRotation2, targetRotation2, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        player.transform.SetPositionAndRotation(targetPosition, targetRotation);
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
        if (!other.CompareTag("Player")) {
            return;
        }
        if (talkingTo)
        {
            return;
        }

        playerInRange = false;
        interactionManager.UnregisterNPC(this);
    }

}
