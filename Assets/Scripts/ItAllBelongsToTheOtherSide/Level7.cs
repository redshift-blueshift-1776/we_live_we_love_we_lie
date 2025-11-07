using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Level7 : MonoBehaviour
{
    [SerializeField] private GameObject titleScreenCanvas;
    [SerializeField] private GameObject settingsCanvas;
    [SerializeField] private GameObject player;
    CharacterController characterController;
    [SerializeField] private Player7 playerScript;
    [SerializeField] private GameObject localEnemyPrefab;

    [SerializeField] private GameObject wanderNodes;
    [SerializeField] private GameObject navMeshJumps;
    [SerializeField] private GameObject playerRaycastNodes;
    [SerializeField] private GameObject deathmatchSpawnNodes;

    private bool gameStarted = false;
    private string gamemode = "";
    private int currEnemies = 0;
    void Start()
    {
        settingsCanvas.GetComponent<GameSettings>().setOverrideCursor(true);
        settingsCanvas.SetActive(false);
        titleScreenCanvas.SetActive(true);

        obtainSpawnLocations();
        characterController = player.GetComponent<CharacterController>();
        
        activateNodes();
        makeAllNodesInvisible();

        StartCoroutine(handleGame());
        //Instantiate(localEnemyPrefab);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
    }

    HashSet<Vector3> deathmatchSpawnLocations = new HashSet<Vector3>();
    private void obtainSpawnLocations()
    {
        foreach (Transform child in deathmatchSpawnNodes.transform)
        {
            deathmatchSpawnLocations.Add(child.position);
        }
       
    }

    private IEnumerator handleGame()
    {
        while (true)
        {
            switch (gamemode)
            {
                case "Deathmatch":
                    if (gameStarted && currEnemies == 0)
                    {
                        Debug.Log("YOU WON!");
                        yield break;
                    }
                    break;
                case "Defusal":
                    break;
                case "Zombie Apocalypse":
                    break;
                default:
                    break;
            }
            yield return null;
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

    public void updateEnemyCount(int amount)
    {
        currEnemies += amount;
    }

    public void setGamemode(string gamemode)
    {
        Debug.Log("set gamemode!");
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
                initializeDeathmatchSpawnLocations();
                playerScript.Initialize();

                characterController.enabled = false;
                player.transform.position = currDeathmatchSpawnLocations.Pop() + Vector3.up;
                characterController.enabled = true;

                const int deathmatchBots = 5;
                for (int i = 0; i < deathmatchBots; i++)
                {
                    GameObject enemy = Instantiate(localEnemyPrefab);
                    Enemy enemyScript = enemy.GetComponent<Enemy>();

                    NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
                    agent.Warp(currDeathmatchSpawnLocations.Pop() + Vector3.up);

                    enemyScript.Initialize();
                    currEnemies++;
                }
                break;
            case "Defusal":
                break;
            case "Zombie Apocalypse":
                break;
            default:
                break;
        }
        gameStarted = true;
        titleScreenCanvas.SetActive(false);
        settingsCanvas.SetActive(true);
    }

    
    Stack<Vector3> currDeathmatchSpawnLocations = new Stack<Vector3>();
    private void initializeDeathmatchSpawnLocations()
    {
        currDeathmatchSpawnLocations.Clear();

        List<Vector3> shuffled = deathmatchSpawnLocations.OrderBy(x => Random.value).ToList();

        foreach (Vector3 location in shuffled)
        {
            currDeathmatchSpawnLocations.Push(location);
        }
    }

}
