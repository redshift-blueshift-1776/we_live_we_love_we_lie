using UnityEngine;

public class Universal_Manager : MonoBehaviour
{
    [SerializeField] public bool debug;

    [Header("Statistics Needed")]
    [SerializeField] public int numLevels; // Story Mode
    [SerializeField] public int numHardLevels;
    [SerializeField] public int numNonInfiniteLevels;

    [Header("Unlocked Levels")]
    public bool[] unlockedEasy;
    public bool[] unlockedHard;
    public bool[] unlockedEndless;
    

    [Header("Overall Achievements")]
    public bool beatStoryMode;
    public bool beatStoryModeWithoutFailing;
    public bool[] beatHardLevels;
    public bool[] beatNonInfiniteLevels;

    [Header("Speedrun Achievements")]
    public bool level1Speedrun;
    public bool level2Speedrun;
    public bool level3Speedrun;
    public bool level4Speedrun;
    public bool level5Speedrun;
    public bool level5Speedrun2;
    public bool level6Speedrun;
    public bool level7Speedrun;

    [Header("Level 1 Achievements")]
    public bool level1Layer20;
    public bool level1Layer20noWSUpDown;
    public bool level1Layer20DLeft;
    public bool level1Layer50;
    public bool level1Layer100;

    [Header("Level 2 Achievements")]
    public bool level2fiveSecondsAirtime;
    public bool level2iteration5;
    public bool level2iteration10;

    public static Universal_Manager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist this instance
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        unlockedEasy = new bool[8] {
            true, true, true, true, true, true, true, true
        };
        unlockedHard = new bool[8];
        unlockedEndless = new bool[8];

        level1Layer20 = (PlayerPrefs.GetInt("level1Layer20", 0) == 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (debug) {
            unlockedHard = new bool[8] {
                true, true, true, true, true, true, true, true
            };
            unlockedEndless = new bool[8] {
                true, true, true, true, true, true, true, true
            };
        } else {
            for (int i = 1; i <= numLevels; i++) {
                unlockedHard[i - 1] = (PlayerPrefs.GetInt("unlockedHard" + i, 0) == 1);
                unlockedEndless[i - 1] = (PlayerPrefs.GetInt("unlockedEndless" + i, 0) == 1);
            }
        }
    }
}
