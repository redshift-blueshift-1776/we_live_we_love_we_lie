using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Ending_Cutscene : MonoBehaviour
{
    [SerializeField] public TMP_Text quoteText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject foundObject = GameObject.Find("Universal_Manager");
        // Check if the foundObject is not null
        if (foundObject != null) {
            Debug.Log("Found Universal_Manager");
            Universal_Manager um = foundObject.GetComponent<Universal_Manager>();
            um.beatStoryMode = true;
            PlayerPrefs.SetInt("beatStoryMode", 1);
            for (int i = 1; i <= 8; i++) {
                PlayerPrefs.GetInt("unlockedEndless" + i, 1);
            }
        } else {
            Debug.Log("No Universal_Manager");
        }

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
        float duration = Mathf.Max(2f, line.Length / 13f);
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
"It was the best thing in the world…",
"We made great games…",
"We made fun games…",
"We even made strange games…",
"Everything was going well…",
"Until disaster struck…",
"A game jam came, and my game, Love Sees Differences, was hated by everyone.",
"They called me a monster for making that.",
"Donald James had no choice…",
"He had to kick me out of the game dev program.",
"It was very painful getting kicked out.",
"I cried, and cried.",
"I knew that I was missing something…",
"Something important to me…",
"Something I lost on a boat trip…",
"So I went to Atlantis to find the root of my inspiration.",
"A toy, given to me by a game dev that I idolize.",
"A toy, given to me by Lan.",
"A toy, given to me when I was young.",
"It was my inspiration, and I had to get it back.",
"And once I did, my game dev skill was back to my prime.",
"I remade Love Sees Differences.",
"And the fans all loved it.",
"They never saw a game as bold.",
"My confidence was back with the remastered game.",
"I did another game jam with Allen, and we did well.",
"But at the end of the day, the thing I wanted the most, more than anything…",
"Was to get back into the game dev program…",
"And after eight tough challenges…",
"I'm back…",
"It's been a journey…",
"And even though I may have been the monster…",
"It's good to be back in the game dev program.",

        };

        foreach (string line in lines)
        {
            yield return StartCoroutine(DoLine(line));
        }
        
        SceneManager.LoadScene(17);
    }
}
