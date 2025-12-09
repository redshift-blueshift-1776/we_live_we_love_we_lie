using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameRow : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text rankText;
    public TMP_Text titleText;
    // public TMP_Text authorText;
    public RawImage icon;

    public void SetData(string gameString, Texture img, int rank)
    {
        string[] parts = gameString.Split(':');
        string title = parts.Length > 0 ? parts[0] : "UNKNOWN";
        // string authors = parts.Length > 1 ? parts[1] : "";

        rankText.text = $"#{rank}";
        titleText.text = title;
        // authorText.text = authors;
        icon.texture = img;
    }
}