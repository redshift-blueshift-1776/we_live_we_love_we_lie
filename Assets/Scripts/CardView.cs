using UnityEngine;

public class CardView : MonoBehaviour
{
    public Card CardData { get; private set; }
    private Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    public void SetCard(Card card)
    {
        CardData = card;
        gameObject.SetActive(true); // reveal in scene
        UpdateVisual();
    }

    void UpdateVisual()
    {
        if (CardData.Color == CardColor.Red)
            rend.material.color = Color.red;
        else
            rend.material.color = Color.black;
    }
}