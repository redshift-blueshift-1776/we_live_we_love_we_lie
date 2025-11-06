using UnityEngine;

public class Level7 : MonoBehaviour
{
    [SerializeField] private GameObject wanderNodes;
    [SerializeField] private GameObject navMeshJumps;
    [SerializeField] private GameObject playerRaycastNodes;

    private bool gameStarted = true;
    void Start()
    {
        activateNodes();
        makeNodesInvisible();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void activateNodes()
    {
        wanderNodes.SetActive(true);
        navMeshJumps.SetActive(true);
        playerRaycastNodes.SetActive(true);
    }

    private void makeNodesInvisible()
    {
        foreach (Transform child in wanderNodes.transform)
        {
            child.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }

        foreach (MeshRenderer meshRenderer in navMeshJumps.GetComponentsInChildren<MeshRenderer>())
        {
            if (meshRenderer != null)
            {
                meshRenderer.enabled = false;
            }
        }

        foreach (MeshRenderer meshRenderer in playerRaycastNodes.GetComponentsInChildren<MeshRenderer>())
        {
            if (meshRenderer != null)
            {
                meshRenderer.enabled = false;
            }
        }
    }

    public bool getGameStarted()
    {
        return gameStarted;
    }
}
