using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrailMaker : MonoBehaviour
{
    [SerializeField] public GameObject player;
    [SerializeField] public GameObject trail;
    [SerializeField] public float timeBetween = 0.1f;
    [SerializeField] public float scale = 1f;
    [SerializeField] public int numToKeep = 1000;

    public List<GameObject> trails;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(spawnTrails());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator spawnTrails() {
        while (true) {
            GameObject newTrail = Instantiate(trail);
            newTrail.transform.position = player.transform.position;
            newTrail.transform.localScale = new Vector3(scale, scale, scale);
            trails.Add(newTrail);
            if (trails.Count > numToKeep) {
                GameObject toDestroy = trails[0];
                trails.RemoveAt(0);
                Destroy(toDestroy);
            }
            yield return new WaitForSeconds(timeBetween);
        }
    }
}
