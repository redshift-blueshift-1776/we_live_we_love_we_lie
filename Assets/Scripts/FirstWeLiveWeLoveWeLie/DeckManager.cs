using System.Collections.Generic;
using UnityEngine;

// Enum for clarity
public enum CardColor { Red, Black }

public class Card
{
    public CardColor Color { get; private set; }
    public int ID { get; private set; } // unique identifier for debugging

    public Card(CardColor color, int id)
    {
        Color = color;
        ID = id;
    }

    public override string ToString()
    {
        return $"{Color} (ID {ID})";
    }
}

public class DeckManager : MonoBehaviour
{
    private List<Card> deck = new List<Card>();
    private System.Random rng = new System.Random();

    public DeckManager(int numRed, int numBlack)
    {
        BuildDeck(numRed, numBlack);
        Shuffle();
    }

    private void BuildDeck(int numRed, int numBlack)
    {
        deck.Clear();
        int id = 0;
        for (int i = 0; i < numRed; i++) {
            deck.Add(new Card(CardColor.Red, id++));
        }
        for (int i = 0; i < numBlack; i++) {
            deck.Add(new Card(CardColor.Black, id++));
        }    
    }

    public void Shuffle()
    {
        // Fisher-Yates shuffle
        int n = deck.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Card value = deck[k];
            deck[k] = deck[n];
            deck[n] = value;
        }
    }

    public Card DrawCard()
    {
        if (deck.Count == 0) {
            return null;
        }
        Card c = deck[0];
        deck.RemoveAt(0);
        return c;
    }

    public int CardsRemaining => deck.Count;
}