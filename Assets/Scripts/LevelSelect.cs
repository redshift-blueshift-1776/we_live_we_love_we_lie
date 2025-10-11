using UnityEngine;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] public GameObject[][] levelButtons;
    [SerializeField] public GameObject currentBackground;
    [SerializeField] public GameObject leftBackground;
    [SerializeField] public GameObject rightBackground;

    public bool[][] unlocked;

    public int currentPage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 3; j++) {
                if (i == 0) {
                    levelButtons[i][j].SetActive(true);
                    levelButtons[i][j].transform.position = new Vector3(0, 0 - 100 * j, 0);
                }
                levelButtons[i][j].SetActive(unlocked[i][j]);
            }
        }
        currentPage = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
