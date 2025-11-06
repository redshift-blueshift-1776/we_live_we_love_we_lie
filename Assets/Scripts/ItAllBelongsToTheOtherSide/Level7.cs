using UnityEngine;

public class Level7 : MonoBehaviour
{
    [SerializeField] private GameObject titleScreenCanvas;
    [SerializeField] private Player7 playerScript;
    [SerializeField] private GameObject localEnemyPrefab;

    [SerializeField] private GameObject wanderNodes;
    [SerializeField] private GameObject navMeshJumps;
    [SerializeField] private GameObject playerRaycastNodes;

    private bool gameStarted = false;
    void Start()
    {
        titleScreenCanvas.SetActive(true);
        activateNodes();
        makeNodesInvisible();

        //Instantiate(localEnemyPrefab);
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
    public void setGamemode(string gamemode)
    {
        switch (gamemode)
        {
            case "Deathmatch":
                //TO-DO
                // add nodes for spawn locations of player and bots
                // put normal enemy out of bounds
                // clone enemy
                // update positions accordingly
                // make sure to call instantiate for both player and each enemy clone
                break;
            case "Defusal":
                break;
            case "Zombie Apocalypse":
                break;
            default:
                break;
        }

        gameStarted = true;
        playerScript.Initialize();
        titleScreenCanvas.SetActive(false);
    }
}
