using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance;

    public GameObject npcVisual;
    public Transform player;

    public List<NPC_Open_Exploration> nearbyNPCs = new();
    private NPC_Open_Exploration currentNPC;

    private bool inConversation = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (!inConversation)
            SelectClosestNPC();

        npcVisual.SetActive(currentNPC != null && !inConversation);

        if (!inConversation && currentNPC != null && Input.GetMouseButtonDown(1))
        {
            StartConversation(currentNPC);
        }

        if (inConversation && Input.GetKeyDown(KeyCode.E))
        {
            EndConversation(currentNPC);
        }
    }


    public void RegisterNPC(NPC_Open_Exploration npc)
    {
        if (!nearbyNPCs.Contains(npc))
            nearbyNPCs.Add(npc);
    }

    private List<NPC_Open_Exploration> toRemove = new();

    public void UnregisterNPC(NPC_Open_Exploration npc)
    {
        if (currentNPC == npc)
        {
            if (inConversation)
                EndConversation(npc);

            currentNPC = null;
        }
        toRemove.Add(npc);
    }

    void LateUpdate()
    {
        foreach (var npc in toRemove)
            nearbyNPCs.Remove(npc);

        toRemove.Clear();
    }


    void SelectClosestNPC()
    {
        float closestDist = float.MaxValue;
        NPC_Open_Exploration closest = null;

        // iterate over a copy
        foreach (var npc in nearbyNPCs.ToArray())
        {
            if (npc == null) continue;

            float dist = Vector3.Distance(player.position, npc.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = npc;
            }
        }

        currentNPC = closest;
    }

    void StartConversation(NPC_Open_Exploration npc)
    {
        if (inConversation || npc.talkingTo) return;

        inConversation = true;
        npc.StartConversationWrapper();
    }


    void EndConversation(NPC_Open_Exploration npc)
    {
        if (!inConversation || npc == null) return;

        inConversation = false;
        npc.endConversation();
    }

}