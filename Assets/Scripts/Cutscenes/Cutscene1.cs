using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Cutscene1 : MonoBehaviour
{
    [SerializeField] public TMP_Text quoteText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        quoteText.text = "";
        StartCoroutine(DoCredits());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator DoLine(string line) {
        float startAlpha = 1f;
        float targetAlpha = 0f;
        quoteText.color = new Color(quoteText.color.r, quoteText.color.g, quoteText.color.b, 1f);
        yield return new WaitForSeconds(1f);
        float elapsed = 0f;
        float duration = 2f;
        while (elapsed < duration) {
            float t = elapsed / duration;

            string chars = line;
            int numChars = (int) (chars.Length * t);
            string charsToPut = chars.Substring(0, numChars);
            quoteText.text = charsToPut;
            elapsed += Time.deltaTime;
            yield return null;
        }
        quoteText.text = line;
        yield return new WaitForSeconds(0.5f);

        duration = 2f;
        elapsed = 0f;
        while (elapsed < duration) {
            float t = elapsed / duration;
            float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            quoteText.color = new Color(quoteText.color.r, quoteText.color.g, quoteText.color.b, currentAlpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
        quoteText.color = new Color(quoteText.color.r, quoteText.color.g, quoteText.color.b, targetAlpha);
        quoteText.text = "";
        yield return null;
    }

    public IEnumerator DoCredits(){
        string[] lines = {
            "I remember those days…",
"When I was in the game dev program…",
"There was the boat trip…",
"Where I lost something important to me…",
"When I was young, someone gave me a toy monster…",
"Only for the wind to blow it off the boat…",
"The boat was over Atlantis… so it's really lost…",
"My game development skills were never the same after that.",
"A game jam came, and my game, Love Sees Differences, was hated by everyone.",
"They called me a monster for making that.",
"Almost everyone in the game dev program called me a monster.",
"Even the program director, Professor Donald James, thought I was a monster for making that game.",
"And he rarely hates games.",
"The only person who didn't think I was a monster was my close friend, Allen Lively.",
"But even he couldn't convince Donald James to keep me in the program.",
"So now, all I can do is hope…",
"That the root of my inspiration is still in Atlantis…"

        };

        foreach (string line in lines)
        {
            yield return StartCoroutine(DoLine(line));
        }
        
        SceneManager.LoadScene(12);
    }
}
