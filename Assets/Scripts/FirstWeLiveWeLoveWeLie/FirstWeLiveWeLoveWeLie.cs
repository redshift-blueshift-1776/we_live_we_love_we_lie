using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;

public class Player
{
    public string Name { get; private set; }
    public List<Card> CollectedCards { get; private set; } = new List<Card>();

    public Player(string name)
    {
        Name = name;
    }

    public void AddCard(Card c)
    {
        if (c != null) {
            CollectedCards.Add(c);
        }
    }
}

public class FirstWeLiveWeLoveWeLie : MonoBehaviour
{
    [Header("Game Settings")]
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
    [SerializeField] private TMP_Text playersLeft;

    [SerializeField] private TMP_Text opponentText;

    [SerializeField] private GameObject cutsceneManager;
    private CutsceneManager cm;

    [SerializeField] private GameObject dialogueManager;
    private DialogueManager dm;

    [SerializeField] private GameObject yourCard;

    private DeckManager deckManager;
    private List<Player> players = new List<Player>();

    private int humanPlayerIndex = -1; // decided at runtime
    private int currentDefenseIndex = 0;
    private int currentOffenseIndex = 1;

    private int redsRemaining;
    private int blacksRemaining;

    private Card defenseCard; // card currently being contested
    private int cardIndex = 0; // which slot to fill next

    private bool roundActive = false;
    private bool turnActive = false;
    private Action queuedTurn = null;

    public static FirstWeLiveWeLoveWeLie Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // At start, only show seat select
        SeatSelectCanvas.SetActive(true);
        UICanvas.SetActive(false);
        ChoiceCanvas.SetActive(false);
        CutsceneCanvas.SetActive(false);
        DialogueCanvas.SetActive(false);
        yourCard.SetActive(false);

        SeatSlider.minValue = 0;
        SeatSlider.maxValue = numPlayers - 1;
        SeatSlider.value = 0;
        UpdateSeatText();

        cm = cutsceneManager.GetComponent<CutsceneManager>();
        dm = dialogueManager.GetComponent<DialogueManager>();
    }

    void Update()
    {
        if (UICanvas.activeSelf) {
            redLeft.text = $"Red Left: {redsRemaining}";
            blackLeft.text = $"Black Left: {blacksRemaining}";
        }
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
            cardImage.color = (defenseCard.Color == CardColor.Red) ? Color.red : Color.black;

            string[] options = { "Trust me, pass!", "You'll regret passing!", "Your move, but risky..." };
            dm.ShowDialogueOptions(options, choice =>
            {
                if (turnActive) return; // Ignore if a turn is already being processed

                float stealChance = 0.5f;
                if (choice == 0) stealChance -= 0.2f;
                if (choice == 1) stealChance += 0.2f;

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
            bool aiLies = UnityEngine.Random.value < 0.2f; // placeholder strategy
            if (aiLies) {
                opponentText.text = "It's safe.";
            } else {
                opponentText.text = "It's not safe.";
            }
        } else {
            bool aiLies = UnityEngine.Random.value < 0.2f; // placeholder strategy
            if (aiLies) {
                opponentText.text = "It's not safe.";
            } else {
                opponentText.text = "It's safe.";
            }
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

        CutsceneType type = isHuman
            ? (steal ? CutsceneType.HumanSteal : CutsceneType.HumanPass)
            : (steal ? CutsceneType.AISteal : CutsceneType.AIPass);

        cm.PlayCutscene(type, () =>
        {
            // Assign card and update indexes
            if (steal)
            {
                Debug.Log($"{players[currentOffenseIndex].Name} steals!");
                AssignCard(players[currentOffenseIndex], defenseCard);
                if (!isHuman) {
                    int temp = currentOffenseIndex;
                    currentOffenseIndex = (currentOffenseIndex + 1) % numPlayers;
                    currentDefenseIndex = temp;
                } else {
                   currentOffenseIndex = (currentOffenseIndex + 1) % numPlayers; 
                }
            }
            else
            {
                Debug.Log($"{players[currentOffenseIndex].Name} passes.");
                AssignCard(players[currentDefenseIndex], defenseCard);
                if (isHuman) {
                    int temp = currentOffenseIndex;
                    currentOffenseIndex = (currentOffenseIndex + 1) % numPlayers;
                    currentDefenseIndex = temp;
                } else {
                   currentOffenseIndex = (currentOffenseIndex + 1) % numPlayers; 
                }
            }
            

            if (isHuman)
            {
                UICanvas.SetActive(true);
                ChoiceCanvas.SetActive(false);
            }

            // Check end conditions
            if (redsRemaining == 0 || blacksRemaining == 0)
            {
                Debug.Log("One color exhausted! Assigning remaining cards...");
                while (deckManager.CardsRemaining > 0)
                {
                    Card leftover = deckManager.DrawCard();
                    int targetIndex = (currentDefenseIndex + 1) % numPlayers;
                    players[targetIndex].AddCard(leftover);
                    Debug.Log($"{players[targetIndex].Name} gets leftover {leftover.Color}");
                }
                Debug.Log("Game Over.");
                roundActive = false;
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
    }

    void CheckEndConditions()
    {
        if (redsRemaining == 0 || blacksRemaining == 0)
        {
            Debug.Log("One color exhausted! Assigning remaining cards...");
            while (deckManager.CardsRemaining > 0)
            {
                Card leftover = deckManager.DrawCard();
                int targetIndex = (currentDefenseIndex + 1) % numPlayers;
                players[targetIndex].AddCard(leftover);
                Debug.Log($"{players[targetIndex].Name} gets leftover {leftover.Color}");
            }
            Debug.Log("Game Over.");
        }
    }

    // Called from CardView buttons (optional for future: AI or human could pick specific card slot)
    public void SelectCard(CardView cv)
    {
        Debug.Log($"Card selected: {cv.CardData.Color}");
    }
}