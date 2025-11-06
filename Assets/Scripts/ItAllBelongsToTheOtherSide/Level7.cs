using UnityEngine;

public class Level7 : MonoBehaviour
{
    [SerializeField] private GameObject titleScreenCanvas;
    [SerializeField] private Player7 playerScript;
    [SerializeField] private GameObject localEnemyPrefab;

    [SerializeField] private GameObject wanderNodes;
    [SerializeField] private GameObject navMeshJumps;
    [SerializeField] private GameObject playerRaycastNodes;
    [SerializeField] private GameObject deathmatchSpawnNodes;

    private bool gameStarted = false;
    private string gamemode = "";
    void Start()
    {
        titleScreenCanvas.SetActive(true);
        activateNodes();
        makeAllNodesInvisible();

        //Instantiate(localEnemyPrefab);
    }

    private void Update()
    {
        handleGame();
    }

    private void handleGame()
    {
        switch (gamemode)
        {
            case "Deathmatch":
                break;
            case "Defusal":
                break;
            case "Zombie Apocalypse":
                break;
            default:
                break;
        }
    }

    private void activateNodes()
    {
        wanderNodes.SetActive(true);
        navMeshJumps.SetActive(true);
        playerRaycastNodes.SetActive(true);
        playerRaycastNodes.SetActive(true);
    }

    private void makeAllNodesInvisible()
    {
        foreach (Transform child in wanderNodes.transform)
        {
            child.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        makeNodesInvisible(navMeshJumps);
        makeNodesInvisible(playerRaycastNodes);
        makeNodesInvisible(deathmatchSpawnNodes);
    }

    private void makeNodesInvisible(GameObject nodes)
    {
        foreach (MeshRenderer node in nodes.GetComponentsInChildren<MeshRenderer>())
        {
            if (node != null)
            {
                node.enabled = false;
            }
        }
    }

    public bool getGameStarted()
    {
        return gameStarted;
    }
    public void setGamemode(string gamemode)
    {
        this.gamemode = gamemode;
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
