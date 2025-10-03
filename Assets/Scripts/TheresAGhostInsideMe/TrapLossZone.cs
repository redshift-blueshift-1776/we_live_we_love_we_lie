using UnityEngine;

public class TrapLossZone : MonoBehaviour
{
    [SerializeField] public GameObject gameManager;
    public TheresAGhostInsideMe gm;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gm = gameManager.GetComponent<TheresAGhostInsideMe>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col) {
        Debug.Log(col.gameObject.name);
        if(col.gameObject.name.Contains("Trap")) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            gm.GameLose();
        }
    }
}
