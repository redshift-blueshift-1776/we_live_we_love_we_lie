using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class Player
{
    public string Name { get; private set; }
    private List<Card> cards = new List<Card>();

    public Player(string name)
    {
        Name = name;
    }

    public void AddCard(Card c)
    {
        if (c != null)
        {
            cards.Add(c);
        }
    }

    public IEnumerable<Card> GetCards()
    {
        return cards;
    }

    public CardColor GetCardColor()
    {
        if (cards.Count == 0)
        {
            Debug.LogWarning($"{Name} has no cards!");
            return CardColor.Black; // default/fallback
        }

        // Example: check if *any* card is red
        // (you can adjust this logic depending on win condition)
        return cards[0].Color;
    }
}

public class FirstWeLiveWeLoveWeLie : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] public bool endless;
    [SerializeField] private int numPlayers = 12;
    [SerializeField] private int numRed = 6;
    [SerializeField] private int numBlack = 6;

    [Header("Card Slots (UI Prefabs in Scene)")]
    [SerializeField] private List<CardView> cardSlots;

    [Header("UI References")]
    [SerializeField] private GameObject ChoiceCanvas; // Steal/No Steal
    [SerializeField] private GameObject UICanvas;     // General UI
    [SerializeField] private GameObject SeatSelectCanvas;
    [SerializeField] private GameObject CutsceneCanvas;
    [SerializeField] private GameObject DialogueCanvas;
    [SerializeField] private Slider SeatSlider;
    [SerializeField] private TMP_Text SeatText;
    [SerializeField] private TMP_Text redLeft;
    [SerializeField] private TMP_Text blackLeft;
    [SerializeField] private GameObject stealOrNoStealObject;
    [SerializeField] private TMP_Text stealOrNoSteal;
    [SerializeField] private GameObject yourCard;
    [SerializeField] private GameObject yourCardReal;
    [SerializeField] private GameObject cardInBriefcaseSafe;
    [SerializeField] private GameObject cardInBriefcaseEliminate;
    [SerializeField] public GameObject skipButton;

    [SerializeField] private TMP_Text opponentText;

    [SerializeField] private GameObject cutsceneManager;
    private CutsceneManager cm;

    [SerializeField] private GameObject dialogueManager;
    private DialogueManager dm;

    [SerializeField] public Transform PlayerTo;
    [SerializeField] public Transform PlayerFrom;
    [SerializeField] public GameObject player;

    [SerializeField] public bool bayesian;
    [SerializeField] public float probTruthIfSafe = 0.6f;
    [SerializeField] public float probTruthIfEliminate = 0.2f;

    private DeckManager deckManager;
    private List<Player> players = new List<Player>();

    private int humanPlayerIndex = -1; // decided at runtime
    private int currentDefenseIndex = 0;
    private int currentOffenseIndex = 1;

    private int redsRemaining;
    private int blacksRemaining;

    public Card defenseCard; // card currently being contested
    private int cardIndex = 0; // which slot to fill next

    private bool roundActive = false;
    private bool turnActive = false;
    private Action queuedTurn = null;

    private bool showSkipButton = false;

    public bool aiLied;

    public static FirstWeLiveWeLoveWeLie Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // At start, only show seat select
        stealOrNoStealObject.SetActive(false);
        SeatSelectCanvas.SetActive(true);
        UICanvas.SetActive(false);
        ChoiceCanvas.SetActive(false);
        CutsceneCanvas.SetActive(false);
        DialogueCanvas.SetActive(false);
        yourCard.SetActive(false);
        yourCardReal.SetActive(false);

        SeatSlider.minValue = 0;
        SeatSlider.maxValue = numPlayers - 1;
        SeatSlider.value = 0;
        UpdateSeatText();

        cm = cutsceneManager.GetComponent<CutsceneManager>();
        dm = dialogueManager.GetComponent<DialogueManager>();
        showSkipButton = false;
        cm.cutsceneCamera.SetActive(false);
        cm.mainCamera.SetActive(true);
        cardInBriefcaseEliminate.SetActive(false);
        cardInBriefcaseSafe.SetActive(false);
        probTruthIfEliminate = PlayerPrefs.GetFloat("probTruthIfEliminate", 0.2f);
        probTruthIfSafe = PlayerPrefs.GetFloat("probTruthIfSafe", 0.6f);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.M)) {
            SceneManager.LoadScene(0);
        }
        if (UICanvas.activeSelf) {
            redLeft.text = $"Eliminates Left: {redsRemaining}";
            blackLeft.text = $"Safes Left: {blacksRemaining}";
        }
        skipButton.SetActive(showSkipButton);
    }

    public void OnSeatSliderChanged()
    {
        UpdateSeatText();
    }

    void UpdateSeatText()
    {
        int seat = Mathf.RoundToInt(SeatSlider.value);
        SeatText.text = $"Seat: {seat + 1}";
    }

    public void ConfirmSeatChoice()
    {
        humanPlayerIndex = Mathf.RoundToInt(SeatSlider.value);

        SeatSelectCanvas.SetActive(false);
        UICanvas.SetActive(true);

        InitializeGame();
        StartRound();
    }

    void InitializeGame()
    {
        deckManager = new DeckManager(numRed, numBlack);
        players.Clear();

        for (int i = 0; i < numPlayers; i++)
        {
            players.Add(new Player(i == humanPlayerIndex ? "YOU" : $"Player {i + 1}"));
        }

        redsRemaining = numRed;
        blacksRemaining = numBlack;

        currentDefenseIndex = 0;
        currentOffenseIndex = 1;
        cardIndex = 0;
    }

    void StartRound()
    {
        if (roundActive) return;  // Prevent overlapping rounds
        roundActive = true;
        stealOrNoStealObject.SetActive(false);

        if ((humanPlayerIndex == currentOffenseIndex) || (humanPlayerIndex == currentDefenseIndex)) {
            player.transform.position = new Vector3(PlayerTo.position.x, PlayerTo.position.y, PlayerTo.position.z);
        } else {
            player.transform.position = new Vector3(PlayerFrom.position.x, PlayerFrom.position.y, PlayerFrom.position.z);
        }

        Debug.Log($"Offense: {currentOffenseIndex + 1}, Defense: {currentDefenseIndex + 1}");
        yourCard.SetActive(false);

        defenseCard = deckManager.DrawCard();
        if (defenseCard == null)
        {
            Debug.Log("No more cards. Game Over.");
            roundActive = false;
            return;
        }

        Debug.Log($"{players[currentDefenseIndex].Name} drew a hidden card.");

        if (currentOffenseIndex == humanPlayerIndex)
        {
            Debug.Log("Your turn as OFFENSE! Steal or No Steal?");
            ShowPlayerChoiceUI(defenseCard);
        }
        else if (currentDefenseIndex == humanPlayerIndex)
        {
            Debug.Log("Your turn as DEFENSE. Choose dialogue to influence offense.");
            yourCard.SetActive(true);
            RawImage cardImage = yourCard.GetComponent<RawImage>();
            cardImage.color = (defenseCard.Color == CardColor.Red) ? Color.red : Color.green;

            string[] options = { "It's eliminate...", "It's safe...", "Your move, but risky..." };
            dm.ShowDialogueOptions(options, choice =>
            {
                if (turnActive) return; // Ignore if a turn is already being processed

                float stealChance = 0.5f;
                if (bayesian) {
                    if (choice == 0) {
                        // Player says eliminated
                        float baseProbRed = redsRemaining / (redsRemaining + blacksRemaining + 0f);
                        float baseProbBlack = blacksRemaining / (redsRemaining + blacksRemaining + 0f);

                        float probRedTruth = baseProbRed * probTruthIfEliminate;
                        float probBlackLie = baseProbBlack * (1 - probTruthIfSafe);

                        float impliedRedProb = probRedTruth / (probRedTruth + probBlackLie);

                        Debug.Log("probRedTruth: " + probRedTruth);
                        Debug.Log("probBlackLie: " + probBlackLie);
                        Debug.Log("impliedRedProb: " + impliedRedProb);

                        if (probRedTruth > probBlackLie) {
                            stealChance -= 0.25f;
                        } else {
                            stealChance += 0.25f;
                        }
                        Debug.Log("stealChance: " + stealChance);

                        if (defenseCard.Color == CardColor.Red) {
                            // Told the truth
                            probTruthIfEliminate = (probTruthIfEliminate + 1f) / 2f;
                            PlayerPrefs.SetFloat("probTruthIfEliminate", probTruthIfEliminate);
                        } else {
                            // Lied
                            probTruthIfSafe = (probTruthIfSafe + 0f) / 2f;
                            PlayerPrefs.SetFloat("probTruthIfSafe", probTruthIfSafe);
                        }
                    } else {
                        // Player says safe
                        float baseProbRed = redsRemaining / (redsRemaining + blacksRemaining + 0f);
                        float baseProbBlack = blacksRemaining / (redsRemaining + blacksRemaining + 0f);

                        float probRedLie = baseProbRed * (1 - probTruthIfEliminate);
                        float probBlackTruth = baseProbBlack * probTruthIfSafe;

                        float impliedRedProb = probRedLie / (probRedLie + probBlackTruth);

                        Debug.Log("probRedLie: " + probRedLie);
                        Debug.Log("probBlackTruth: " + probBlackTruth);
                        Debug.Log("impliedRedProb: " + impliedRedProb);

                        if (probRedLie > probBlackTruth) {
                            stealChance -= 0.25f;
                        } else {
                            stealChance += 0.25f;
                        }
                        Debug.Log("stealChance: " + stealChance);

                        if (defenseCard.Color == CardColor.Red) {
                            // Lied
                            probTruthIfEliminate = (probTruthIfEliminate + 0f) / 2f;
                            PlayerPrefs.SetFloat("probTruthIfEliminate", probTruthIfEliminate);
                        } else {
                            // Told the truth
                            probTruthIfSafe = (probTruthIfSafe + 1f) / 2f;
                            PlayerPrefs.SetFloat("probTruthIfSafe", probTruthIfSafe);
                        }
                    }
                } else {
                    // if (choice == 0) stealChance -= 0.3f;
                    // if (choice == 1) stealChance += 0.3f;

                    // Made this deterministic
                    if (choice == 0) stealChance = 0f;
                    if (choice == 1) stealChance = 1f;
                }

                bool aiSteals = UnityEngine.Random.value < stealChance;
                ResolveTurn(aiSteals, isHuman: false);
            });
        }
        else
        {
            AutoResolveAI();
        }
    }

    void ShowPlayerChoiceUI(Card defenseCard)
    {
        // UICanvas.SetActive(false);
        ChoiceCanvas.SetActive(true);
        if (defenseCard.Color == CardColor.Red) {
            bool aiLies = UnityEngine.Random.value < 0.25f; // placeholder strategy
            if (aiLies) {
                opponentText.text = "It's safe.";
            } else {
                opponentText.text = "It's not safe.";
            }
            aiLied = aiLies;
        } else {
            bool aiLies = UnityEngine.Random.value < 0.25f; // placeholder strategy
            if (aiLies) {
                opponentText.text = "It's not safe.";
            } else {
                opponentText.text = "It's safe.";
            }
            aiLied = aiLies;
        }
    }

    public void OnStealButton()
    {
        if (!turnActive) ResolveTurn(true, true);
    }

    public void OnNoStealButton()
    {
        if (!turnActive) ResolveTurn(false, true);
    }

    void AutoResolveAI()
    {
        bool aiSteals = UnityEngine.Random.value > 0.5f; // placeholder strategy
        ResolveTurn(aiSteals, isHuman: false);
    }

    void ResolveTurn(bool steal, bool isHuman)
    {
        if (turnActive)
        {
            // Put things into a queue so it doesn't blow up
            queuedTurn = () => ResolveTurn(steal, isHuman);
            Debug.Log("Turn queued while a cutscene is active.");
            return;
        }

        turnActive = true;

        // CutsceneType type = isHuman
        //     ? (steal ? CutsceneType.HumanSteal : CutsceneType.HumanPass)
        //     : (steal ? CutsceneType.AISteal : CutsceneType.AIPass);
        CutsceneType type = CutsceneType.HumanDefendSteal;
        if (isHuman && steal) {
            type = CutsceneType.HumanSteal;
        } else if (isHuman && !steal) {
            type = CutsceneType.HumanPass;
        } else if ((humanPlayerIndex == currentDefenseIndex) && steal) {
            type = CutsceneType.HumanDefendSteal;
        } else if ((humanPlayerIndex == currentDefenseIndex) && !steal) {
            type = CutsceneType.HumanDefendPass;
        } else if (!isHuman && steal) {
            type = CutsceneType.AISteal;
        } else if (!isHuman && !steal) {
            type = CutsceneType.AIPass;
        }

        if (steal) {
            stealOrNoStealObject.SetActive(true);
            stealOrNoSteal.text = "STEAL!!!";
        } else {
            stealOrNoStealObject.SetActive(true);
            stealOrNoSteal.text = "NO STEAL!!!";
        }

        if (isHuman)
        {
            UICanvas.SetActive(true);
            ChoiceCanvas.SetActive(false);
        }

        yourCard.SetActive(false);

        cardInBriefcaseEliminate.SetActive(defenseCard.Color == CardColor.Red);
        cardInBriefcaseSafe.SetActive(defenseCard.Color != CardColor.Red);

        cm.PlayCutscene(type, () =>
        {
            // Assign card and update indexes
            if (steal)
            {
                Debug.Log($"{players[currentOffenseIndex].Name} steals!");
                AssignCard(players[currentOffenseIndex], defenseCard);
                currentOffenseIndex = (currentOffenseIndex + 1) % numPlayers; 
            }
            else
            {
                Debug.Log($"{players[currentOffenseIndex].Name} passes.");
                AssignCard(players[currentDefenseIndex], defenseCard);
                int temp = currentOffenseIndex;
                currentOffenseIndex = (currentOffenseIndex + 1) % numPlayers;
                currentDefenseIndex = temp;
            }

            // Check end conditions
            if (redsRemaining == 0 || blacksRemaining == 0)
            {
                CheckEndConditions();
            }
            else
            {
                roundActive = false;
                StartRound(); // Start next round
            }

            turnActive = false;

            // Execute queued turn if any
            if (queuedTurn != null)
            {
                var queued = queuedTurn;
                queuedTurn = null;
                queued.Invoke();
            }
        });
    }


    void AssignCard(Player p, Card c)
    {
        p.AddCard(c);

        if (cardIndex < cardSlots.Count)
        {
            cardSlots[cardIndex].SetCard(c);
            cardIndex++;
        }

        if (c.Color == CardColor.Red) {
            redsRemaining--;
        } else {
            blacksRemaining--;
        }

        Debug.Log($"{p.Name} reveals {c.Color}! ({redsRemaining} red, {blacksRemaining} black left)");
        if (p.Name == "YOU") {
            yourCardReal.SetActive(true);
            RawImage cardImage = yourCardReal.GetComponent<RawImage>();
            cardImage.color = (c.Color == CardColor.Red) ? Color.red : Color.green;
            showSkipButton = true;
        }
    }

    void CheckEndConditions()
    {
        if (redsRemaining == 0 || blacksRemaining == 0)
        {
            Debug.Log("One color exhausted! Assigning remaining cards...");
            while (deckManager.CardsRemaining > 0)
            {
                Card leftover = deckManager.DrawCard();
                if (leftover == null)
                {
                    Debug.LogError("DrawCard returned null while CardsRemaining > 0!");
                    break;
                }

                int targetIndex = (currentDefenseIndex + 1) % numPlayers;
                players[targetIndex].AddCard(leftover);
                Debug.Log($"{players[targetIndex].Name} gets leftover {leftover.Color}");
            }
            Debug.Log("Game Over.");
            CardColor finalColor = players[humanPlayerIndex].GetCardColor();
            Debug.Log($"Human's last card color: {finalColor}");

            if (finalColor == CardColor.Red)
            {
                if (bayesian) {
                    PlayerPrefs.SetInt("PreviousLevel", 4);
                } else {
                    PlayerPrefs.SetInt("PreviousLevel", 15);
                }
                SceneManager.LoadScene(9);
            }
            else
            {
                // if (bayesian) {
                //     SceneManager.LoadScene(6);
                // } else {
                //     SceneManager.LoadScene(19);
                // }
                GameObject foundObject = GameObject.Find("StoryMode");

                // Check if the foundObject is not null
                if (foundObject != null)
                {
                    Debug.Log("GameObject '" + "StoryMode" + "' found in the scene.");
                    SceneManager.LoadScene(20);
                }
                else
                {
                    SceneManager.LoadScene(0); // Not in story mode, goes back to the menu page
                } 
            }
        }
    }

    public void SkipToEnd() {
        CardColor finalColor = players[humanPlayerIndex].GetCardColor();
        if (finalColor == CardColor.Red)
        {
            if (bayesian) {
                PlayerPrefs.SetInt("PreviousLevel", 4);
            } else {
                PlayerPrefs.SetInt("PreviousLevel", 15);
            }
            SceneManager.LoadScene(9);
        }
        else
        {
            GameObject foundObject = GameObject.Find("StoryMode");

            // Check if the foundObject is not null
            if (foundObject != null)
            {
                Debug.Log("GameObject '" + "StoryMode" + "' found in the scene.");
                SceneManager.LoadScene(20);
            }
            else
            {
                SceneManager.LoadScene(0); // Not in story mode, goes back to the menu page
            } 
        }
    }

    // Called from CardView buttons (optional for future: AI or human could pick specific card slot)
    public void SelectCard(CardView cv)
    {
        Debug.Log($"Card selected: {cv.CardData.Color}");
    }
}