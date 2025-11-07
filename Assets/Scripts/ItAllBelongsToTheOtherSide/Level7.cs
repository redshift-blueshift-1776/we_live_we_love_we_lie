using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Level7 : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject titleScreenCanvas;
    [SerializeField] private GameObject settingsCanvas;
    [SerializeField] private GameObject UICanvas;
    [SerializeField] private GameObject timer;
    [SerializeField] private Image whiteScreen;
    [SerializeField] private GameObject buyPeriodScreen;
    [SerializeField] private GameObject buyWeaponTooltip;

    [SerializeField] private GameObject primaryWeaponName;
    [SerializeField] private GameObject secondaryWeaponName;
    [SerializeField] private GameObject primaryWeaponAmmoText;
    [SerializeField] private GameObject secondaryWeaponAmmoText;

    [Header("Game Objects")]
    [SerializeField] private GameObject player;
    CharacterController characterController;
    [SerializeField] private GameObject weapons;

    [SerializeField] private Player7 playerScript;
    [SerializeField] private Weapon weaponScript;

    [SerializeField] private GameObject localEnemyPrefab;

    [Header("Nodes")]
    [SerializeField] private GameObject wanderNodes;
    [SerializeField] private GameObject navMeshJumps;
    [SerializeField] private GameObject playerRaycastNodes;
    [SerializeField] private GameObject deathmatchSpawnNodes;

    
    private bool gameStarted = false;
    private string gamemode = "";
    private int currEnemies = 0;
    private bool playerDead = false;
    private bool inBuyPeriod = false;


    private float timerSeconds = 0;
    private float buyPeriod = 15f;
    private float roundTime = 180f;

    private bool timerActive = true;

    private Coroutine currFadeoutCoroutine = null;
    [SerializeField] private TMP_Text timerText;
    void Start()
    {
        settingsCanvas.GetComponent<GameSettings>().setOverrideCursor(true);
        settingsCanvas.SetActive(false);
        timer.SetActive(false);
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
        handleTime();

        if (inBuyPeriod || !gameStarted || playerDead)
        {
            Time.timeScale = 0f;
        } else
        {
            Time.timeScale = 1f;
        }

        if (inBuyPeriod)
        {
            buyWeaponTooltip.SetActive(true);
        } else
        {
            buyWeaponTooltip.SetActive(false);

        }
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
                        timerActive = false;
                        if (currFadeoutCoroutine == null)
                        {
                            StartCoroutine(fadeoutCoroutine(true, 5f));
                        }
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


    public void setGamemode(string gamemode)
    {
        this.gamemode = gamemode;
        switch (gamemode)
        {
            case "Deathmatch":
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

                timer.SetActive(true);
                inBuyPeriod = true;
                timerSeconds = buyPeriod;
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

    private void handleTime()
    {
        if (timerSeconds < 0)
        {
            if (inBuyPeriod)
            {
                if (!Application.isFocused)
                {
                    UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
                }

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                buyPeriodScreen.SetActive(false);
                inBuyPeriod = false;
                timerSeconds = roundTime;
                
                if (weaponScript.getPrimaryWeapon().Equals(""))
                {
                    primaryWeaponName.SetActive(false);
                    primaryWeaponAmmoText.SetActive(false);
                }

                if (weaponScript.getSecondaryWeapon().Equals(""))
                {
                    secondaryWeaponName.SetActive(false);
                    secondaryWeaponAmmoText.SetActive(false);
                }
            } else
            {
                timer.transform.localScale = new Vector3(2, 2, 1);
                timerText.color = Color.red;
                Time.timeScale = 0;
                playerDead = true;
                if (currFadeoutCoroutine == null)
                {
                    currFadeoutCoroutine = StartCoroutine(fadeoutCoroutine(false, 5.0f));
                }
            }
        }

        timerText.text = convertSecondsToText(timerSeconds);

        if (timerActive && gameStarted && !playerDead)
        {
            timerSeconds -= Time.unscaledDeltaTime;
        }
    }

    private string convertSecondsToText(float totalSeconds)
    {
        totalSeconds = Mathf.Max(0, totalSeconds);
        int minutes = (int)(totalSeconds / 60);
        int seconds = (int)(totalSeconds % 60);
        return $"{minutes:D2}:{seconds:D2}";
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

    public bool getPlayerDead()
    {
        return playerDead;
    }

    public bool getInBuyPeriod()
    {
        return inBuyPeriod;
    }

    public void updateEnemyCount(int amount)
    {
        currEnemies += amount;
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

    public void startDeathScene(GameObject enemy)
    {
        StartCoroutine(startDeathSceneCoroutine(enemy));
    }
    private IEnumerator startDeathSceneCoroutine(GameObject enemy)
    {
        timerActive = false;
        playerDead = true;
        Time.timeScale = 0;
        weapons.SetActive(false);
        UICanvas.SetActive(false);
        yield return new WaitForSecondsRealtime(1f);

        float t = 0f;
        Quaternion startRotation = player.transform.rotation;
        Vector3 directionToEnemy = (enemy.transform.Find("Head").position - player.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy);

        float angle = Quaternion.Angle(player.transform.rotation, targetRotation);
        float rotationSpeed = 45f; 
        float turningTime = angle / rotationSpeed;

        while (t < turningTime)
        {
            t += Time.unscaledDeltaTime; 
            float fraction = Mathf.Clamp01(t / turningTime);

            float easedFraction = Mathf.SmoothStep(0f, 1f, fraction);

            player.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, easedFraction);

            yield return null;
        }
        player.transform.rotation = targetRotation;

        yield return new WaitForSecondsRealtime(1f);
        StartCoroutine(fadeoutCoroutine(false, 3.0f));
        yield return null;
    }

    private IEnumerator fadeoutCoroutine(bool wonGame, float time)
    {

        Color color = whiteScreen.color;
        float r = color.r;
        float g = color.g;
        float b = color.b;

        float t = 0f;
        while (t < time)
        {
            t += Time.unscaledDeltaTime;
            float fraction = Mathf.Clamp01(t / time);
            whiteScreen.color = Color.Lerp(color, new Color(r, g, b, fraction), fraction);
            yield return null;
        }

        Time.timeScale = 1;
        currFadeoutCoroutine = null;

        if (wonGame)
        {
            SceneManager.LoadScene("SecondWeLiveWeLoveWeLie");
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        yield return null;
    }
}
