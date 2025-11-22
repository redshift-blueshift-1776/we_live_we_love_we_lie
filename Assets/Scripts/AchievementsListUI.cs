using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AchievementsListUI : MonoBehaviour
{
    public Transform content;
    public GameObject achievementRowPrefab;

    void Start()
    {
        var U = Universal_Manager.Instance;

        AddHeader("Story Mode");
        for (int i = 0; i < U.beatStoryModeLevels.Length; i++)
        {
            AddAchievement(
                "Beat Level " + (i+1),
                "Complete the level in Story Mode.",
                U.beatStoryModeLevels[i]
            );
        }

        // AddHeader("Speedrun Achievements");
        // AddAchievement("Level 1 Speedrun", "Beat Level 1 within the time limit.", U.level1Speedrun);
        // AddAchievement("Level 2 Speedrun", "Beat Level 2 within the time limit.", U.level2Speedrun);

        AddHeader("Level 1 Achievements");
        AddAchievement("Reach Layer 20", "", U.level1Layer20);
        AddAchievement("Reach Layer 50", "", U.level1Layer50);
        AddAchievement("Reach Layer 100", "", U.level1Layer100);

        AddHeader("Level 2 Achievements");
        AddAchievement("Reach Iteration 5", "", U.level2iteration5);
        AddAchievement("Reach Iteration 10", "", U.level2iteration10);

    }

    void AddHeader(string title)
    {
        var header = Instantiate(achievementRowPrefab, content);
        header.transform.GetChild(0).GetComponent<Image>().enabled = false;
        header.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = title;
        header.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "";
    }

    void AddAchievement(string title, string description, bool achieved)
    {
        var row = Instantiate(achievementRowPrefab, content);
        var icon = row.transform.GetChild(0).GetComponent<Image>();
        var t = row.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        var d = row.transform.GetChild(2).GetComponent<TextMeshProUGUI>();

        icon.color = achieved ? Color.green : Color.gray;
        icon.GetComponentInChildren<TMP_Text>().text = achieved ? "Y" : "N";

        t.text = title;
        t.color = achieved ? Color.white : Color.gray;
        d.text = description;
    }
}
