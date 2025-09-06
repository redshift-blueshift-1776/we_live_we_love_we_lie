using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public bool breakable = false;
    [SerializeField] public GameObject gameManager;
    public WalkAlongThePathUnknown gm;
    private Material originalMaterial;
    [SerializeField] public Material highlightMaterial;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gm = gameManager.GetComponent<WalkAlongThePathUnknown>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator moveDown() {
        float duration = 2f;
        float elapsed = 0f;
        Vector3 oldPosition = transform.position + new Vector3(0,0,0);
        Vector3 targetPosition = transform.position + new Vector3(0,-100,0);
        while (elapsed < duration) {
            float t = elapsed / duration;

            transform.position = Vector3.Lerp(oldPosition, targetPosition, t);

            elapsed += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }

    public void Interact() {
        Debug.Log("Interacting");
        if (breakable) {
            Debug.Log("Breakable");
            StartCoroutine(moveDown());
            gm.wallBreak();
        } else {
            Debug.Log("Not Breakable");
            gm.soundAlarm();
            gm.wallFail();
        }
    }
}
