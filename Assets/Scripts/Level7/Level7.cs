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
    [SerializeField] private GameSettings gameSettings;

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

    [SerializeField] private TMP_Text killCountText;

    [Header("Bot Difficulty Borders")]
    [SerializeField] private GameObject storyDifficultyBorder;
    [SerializeField] private GameObject babyDifficultyBorder;
    [SerializeField] private GameObject easyDifficultyBorder;
    [SerializeField] private GameObject normalDifficultyBorder;
    [SerializeField] private GameObject hardDifficultyBorder;
    [SerializeField] private GameObject expertDifficultyBorder;
    [SerializeField] private GameObject proDifficultyBorder;
    [SerializeField] private GameObject aimbotDifficultyBorder;


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
    private int killCount = 0;
    private bool playerDead = false;
    private bool inBuyPeriod = false;


    private float timerSeconds = 0;
    private float buyPeriod = 15f;
    private float roundTime = 180f;

    private int botDifficulty = 0;

    private bool timerActive = true;
    private bool endlessMode = false;


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
        setBotDifficulty(0);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        killCountText.text = $"Kills: {killCount}";
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

    const int deathmatchBots = 5;
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
                case "Deathmatch Endless":
                    //respawn enemies
                    while (currEnemies < deathmatchBots + (killCount / 2))
                    {
                        List<Vector3> locations = deathmatchSpawnLocations.ToList();
                        Vector3 randomLocation = locations[Random.Range(0, locations.Count)];

                        Vector3 directionToSpawn = randomLocation - player.transform.position;
                        RaycastHit hit;
                        //make sure new bot spawns somewhere where player can not see
                        while (Physics.Raycast(player.transform.position, directionToSpawn.normalized, out hit, directionToSpawn.magnitude))
                        {
                            if (hit.distance >= directionToSpawn.magnitude - 0.5f)
                            {
                                randomLocation = locations[Random.Range(0, locations.Count)];
                                directionToSpawn = randomLocation - player.transform.position;
                            }
                            else
                            {
                                break;
                            }
                        }

                        GameObject enemy = Instantiate(localEnemyPrefab);
                        Enemy enemyScript = enemy.GetComponent<Enemy>();

                        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
                        agent.Warp(randomLocation + Vector3.up);

                        int newDifficulty = botDifficulty + Mathf.Clamp(killCount / 5, 0, 6 - botDifficulty);
                        if (killCount > 50)
                        {
                            newDifficulty = 7;
                        }

                        enemyScript.Initialize(newDifficulty);
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
            yield return null;
        }
    }


    public void setGamemode(string gamemode)
    {
        this.gamemode = gamemode;
        
        switch (gamemode)
        {
            case "Deathmatch":
                buyPeriod = 15f;
                roundTime = 180f;
                endlessMode = false;

                initializeDeathmatchSpawnLocations();
                playerScript.Initialize();

                characterController.enabled = false;
                player.transform.position = currDeathmatchSpawnLocations.Pop() + Vector3.up;
                characterController.enabled = true;

                
                for (int i = 0; i < deathmatchBots; i++)
                {
                    GameObject enemy = Instantiate(localEnemyPrefab);
                    Enemy enemyScript = enemy.GetComponent<Enemy>();

                    NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
                    agent.Warp(currDeathmatchSpawnLocations.Pop() + Vector3.up);

                    enemyScript.Initialize(botDifficulty);
                    currEnemies++;
                }

                timer.SetActive(true);
                inBuyPeriod = true;
                timerSeconds = buyPeriod;
                break;
            case "Deathmatch Endless":
                buyPeriod = 15f;
                roundTime = Mathf.Infinity;
                endlessMode = true;

                initializeDeathmatchSpawnLocations();
                playerScript.Initialize();

                characterController.enabled = false;
                player.transform.position = currDeathmatchSpawnLocations.Pop() + Vector3.up;
                characterController.enabled = true;

                for (int i = 0; i < deathmatchBots; i++)
                {
                    GameObject enemy = Instantiate(localEnemyPrefab);
                    Enemy enemyScript = enemy.GetComponent<Enemy>();

                    NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
                    agent.Warp(currDeathmatchSpawnLocations.Pop() + Vector3.up);

                    enemyScript.Initialize(botDifficulty);
                    currEnemies++;
                }

                timer.SetActive(true);
                inBuyPeriod = true;
                timerSeconds = buyPeriod;

                //reset list
                initializeDeathmatchSpawnLocations();
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
        if (gameSettings.isInSettings())
        {
            Time.timeScale = 0;
        } else
        {
            Time.timeScale = 1;
        }

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
            }
            else
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
        if (!inBuyPeriod && endlessMode)
        {
            timerText.text = "";
        }

        if (timerActive && gameStarted && !playerDead && !gameSettings.isInSettings())
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
        if (amount < 0)
        {
            killCount -= amount;
        }
        currEnemies += amount;

        killCountText.text = $"Kills: {killCount}";
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
        Camera.main.fieldOfView = 90f;
        timerActive = false;
        playerDead = true;
        Time.timeScale = 0;
        weapons.SetActive(false);
        UICanvas.SetActive(false);
        yield return new WaitForSecondsRealtime(1f);

        float t = 0f;
        Quaternion startRotation = player.transform.rotation;
        Vector3 directionToEnemy = (enemy.transform.Find("Head").position - player.transform.Find("Body").Find("Head").position).normalized;
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
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            GameObject universalManagerObject = GameObject.Find("Universal_Manager");
            if (universalManagerObject != null)
            {
                Universal_Manager um = universalManagerObject.GetComponent<Universal_Manager>();
                um.beatStoryModeLevels[6] = true;
                um.unlockedHard[6] = true;
                PlayerPrefs.SetInt("beatStoryModeLevels7", 1);
                PlayerPrefs.SetInt("unlockedHard7", 1);
            }

            GameObject storyModeObject = GameObject.Find("StoryMode");

            if (storyModeObject != null)
            {
                SceneManager.LoadScene("Open Exploration Before Level 8");
            }
            else
            {
                SceneManager.LoadScene("Menu");
            }
        }
        else
        {
            Scene currentScene = SceneManager.GetActiveScene();
            PlayerPrefs.SetInt("PreviousLevel", currentScene.buildIndex);
            SceneManager.LoadScene(9);
        }
        yield return null;
    }

    public void setBotDifficulty(int difficulty)
    {
        botDifficulty = Mathf.Clamp(difficulty, 0, 7);
        updateBotDifficultyBorders(botDifficulty);
    }

    private void updateBotDifficultyBorders(int difficulty)
    {
        storyDifficultyBorder.SetActive(false);
        babyDifficultyBorder.SetActive(false);
        easyDifficultyBorder.SetActive(false);
        normalDifficultyBorder.SetActive(false);
        hardDifficultyBorder.SetActive(false);
        expertDifficultyBorder.SetActive(false);
        proDifficultyBorder.SetActive(false);
        aimbotDifficultyBorder.SetActive(false);

        switch (difficulty)
        {
            case 0:
                storyDifficultyBorder.SetActive(true);
                break;
            case 1:
                babyDifficultyBorder.SetActive(true);
                break;
            case 2:
                easyDifficultyBorder.SetActive(true);
                break;
            case 3:
                normalDifficultyBorder.SetActive(true);
                break;
            case 4:
                hardDifficultyBorder.SetActive(true);
                break;
            case 5:
                expertDifficultyBorder.SetActive(true);
                break;
            case 6:
                proDifficultyBorder.SetActive(true);
                break;
            case 7:
                aimbotDifficultyBorder.SetActive(true);
                break;
        }

    }

    public bool isEndless()
    {
        return endlessMode;
    }
}
