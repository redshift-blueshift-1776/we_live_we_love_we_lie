using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;

public class Television : MonoBehaviour
{
    [SerializeField] public int secondsBetween;

    [SerializeField] public GameObject tvCanvas;

    [SerializeField] public GameObject prefabForGameRow;

    [SerializeField] public GameObject prefabForMiddleGraphic;

    [SerializeField] public GameObject player;
    [SerializeField] public GameObject mainAudio;
    [SerializeField] public GameObject tvAudio;

    [SerializeField] public Texture[] gameImages;

    public string[] gamesInfoLi;
    public int gameRank;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gamesInfoLi = new string[] {
            "LSD:Wade Zunic",
            "Luca:Melora Oh, Mortem, Alessia Chou, and Lana Attis",
            "Doguns:Donald James and program",
            "Live Laugh Love:Lana Attis, Alessia Chou, Melora Oh",
            "BTD7:Wade Zunic and Allen Lively",
            "EL HL:Mortem, Sam Chang, and Worng Retipulo",
            "GvG4:Candice Long and Joseph Momah",
            "God Made Guns:Lana Attis",
            "One Large Waffle:Wade Zunic and Allen Lively",
            "Planetary Platformer:Donald James and program"
        };
        gameRank = 10;
    }

    // Update is called once per frame
    void Update()
    {
        // If the player is close enough to the TV, disable the mainAudio and play tvAudio.
        // This could also be done with a secondary hitbox.
        // Repeatedly call showNextGame every secondsBetween seconds if the player is close enough.
        bool playerClose = (
            Mathf.Abs(player.transform.position.x + 162.5f) <= 5f
        ) && (
            Mathf.Abs(player.transform.position.y - 5f) <= 5f
        ) && (
            Mathf.Abs(player.transform.position.z - 250f) <= 10f
        );

        if (playerClose)
        {
            mainAudio.SetActive(false);
            tvAudio.SetActive(true);

            if (!isRunning)
            {
                StartCoroutine(RevealLoop());
            }
        }
        else
        {
            mainAudio.SetActive(true);
            tvAudio.SetActive(false);
        }
    }

    bool isRunning = false;

    IEnumerator RevealLoop()
    {
        isRunning = true;

        while (gameRank > 0)
        {
            yield return showNextGame();
            yield return new WaitForSeconds(secondsBetween);
        }

        isRunning = false;
    }


    public IEnumerator showNextGame() {
        string gameToAdd = gamesInfoLi[gameRank - 1];
        Texture gameImage = gameImages[gameRank - 1];
        // First use the prefabForMiddleGraphic to display the game in the middle with the gameImage.
        GameObject mg = Instantiate(prefabForMiddleGraphic, tvCanvas.transform.Find("MiddleGraphicHolder"));
        mg.GetComponent<MiddleGraphic>().SetData(gameToAdd, gameImage, gameRank);
        mg.GetComponent<MiddleGraphic>().PlayReveal();

        yield return new WaitForSeconds(1.5f);
        // Then add the game to the list using prefabForGameRow. 5 rows, 2 columns.
        GameObject row = Instantiate(prefabForGameRow, tvCanvas.transform.Find("RankingsGrid"));
        row.GetComponent<GameRow>().SetData(gameToAdd, gameImage, gameRank);

        // Then subtract 1 from gameRank to go to the next game.
        gameRank--;
        yield return null;
    }
}
