using UnityEngine;

public class KeyWinZone : MonoBehaviour
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
        if(col.gameObject.name == "Key") {
            gm.nextBoard();
        }
    }
}
