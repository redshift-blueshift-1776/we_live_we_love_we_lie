using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] private Slider SeatSlider;
    [SerializeField] private TMP_Text SeatText;
    [SerializeField] private TMP_Text redLeft;
    [SerializeField] private TMP_Text blackLeft;

    private DeckManager deckManager;
    private List<Player> players = new List<Player>();

    private int humanPlayerIndex = -1; // decided at runtime
    private int currentDefenseIndex = 0;
    private int currentOffenseIndex = 1;

    private int redsRemaining;
    private int blacksRemaining;

    private Card defenseCard; // card currently being contested
    private int cardIndex = 0; // which slot to fill next

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

        SeatSlider.minValue = 0;
        SeatSlider.maxValue = numPlayers - 1;
        SeatSlider.value = 0;
        UpdateSeatText();
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
        defenseCard = deckManager.DrawCard();
        if (defenseCard == null)
        {
            Debug.Log("No more cards. Game Over.");
            return;
        }

        Debug.Log($"{players[currentDefenseIndex].Name} drew a hidden card.");

        if (currentOffenseIndex == humanPlayerIndex)
        {
            Debug.Log("Your turn as OFFENSE! Steal or No Steal?");
            ShowPlayerChoiceUI();
        }
        else if (currentDefenseIndex == humanPlayerIndex)
        {
            Debug.Log("Your turn as DEFENSE. Waiting to see if offense steals.");
            AutoResolveAI();
        }
        else
        {
            AutoResolveAI();
        }
    }

    void ShowPlayerChoiceUI()
    {
        UICanvas.SetActive(false);
        ChoiceCanvas.SetActive(true);
    }

    public void OnStealButton()
    {
        ResolveTurn(steal: true, isHuman: true);
    }

    public void OnNoStealButton()
    {
        ResolveTurn(steal: false, isHuman: true);
    }

    void AutoResolveAI()
    {
        bool aiSteals = Random.value > 0.5f; // placeholder strategy
        ResolveTurn(aiSteals, isHuman: false);
    }

    void ResolveTurn(bool steal, bool isHuman)
    {
        if (steal)
        {
            Debug.Log($"{players[currentOffenseIndex].Name} steals!");
            AssignCard(players[currentOffenseIndex], defenseCard);
            currentDefenseIndex = currentOffenseIndex;
            currentOffenseIndex = (currentDefenseIndex + 1) % numPlayers;
        }
        else
        {
            Debug.Log($"{players[currentOffenseIndex].Name} passes.");
            AssignCard(players[currentDefenseIndex], defenseCard);
            currentOffenseIndex = (currentOffenseIndex + 1) % numPlayers;
        }

        if (isHuman)
        {
            UICanvas.SetActive(true);
            ChoiceCanvas.SetActive(false);
        }

        CheckEndConditions();
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
        else
        {
            StartRound();
        }
    }

    // Called from CardView buttons (optional for future: AI or human could pick specific card slot)
    public void SelectCard(CardView cv)
    {
        Debug.Log($"Card selected: {cv.CardData.Color}");
    }
}