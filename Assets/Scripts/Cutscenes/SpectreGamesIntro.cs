using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class SpectreGamesIntro : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    [SerializeField] public TMP_Text quoteText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        quoteText.text = "";
        // StartCoroutine(DoIntro());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator DoLine(string line) {
        float startAlpha = 1f;
        float targetAlpha = 0f;
        quoteText.color = new Color(quoteText.color.r, quoteText.color.g, quoteText.color.b, 1f);
        yield return new WaitForSeconds(0.1f);
        float elapsed = 0f;
        float duration = Mathf.Max(2f, line.Length / 21f);
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

        duration = 1f;
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

    public IEnumerator DoIntro(){
        string[] lines = {
            "To my left is a Five MILLION Dollar ticket...",
            "The most valuable ticket prize in game show history...",
            "And competing for this spot in the game dev program...",
            "Are these ONE THOUSAND Students!",

        };

        foreach (string line in lines)
        {
            yield return StartCoroutine(DoLine(line));
        }
        
        goToNextLevel();
    }

    public void OnSkipPressed() {
        goToNextLevel();
    }

    public void StartIntro() {
        StartCoroutine(DoIntro());
    }

    public void goToNextLevel()
    {
        SceneManager.LoadScene("Intro to Level 1");
    }
    
}
