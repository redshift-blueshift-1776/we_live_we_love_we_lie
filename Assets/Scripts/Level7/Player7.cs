using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player7 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    [SerializeField] private Level7 levelScript;

    [SerializeField] private GameObject UICanvas;
    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private TMP_Text healthNumberText;
    [SerializeField] private Image healthBarFill;
    

    [SerializeField] private GameObject weaponShopCanvas;
    [SerializeField] private GameSettings gameSettings;

    [SerializeField] private AudioManager audioManager;

    private bool isInWeaponShop = false;
    private float health = 100f;
    void Start()
    {
    }

    public void Initialize()
    {
        UICanvas.SetActive(true);
        weaponShopCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        updateHealthDisplay();

        if (!levelScript.getGameStarted())
        {
            UICanvas.SetActive(false);
            return;
        }
        else if (!levelScript.getPlayerDead())
        {
            UICanvas.SetActive(true);
        }

        if (levelScript.getInBuyPeriod() && Input.GetKeyDown(KeyCode.B))
        {
            isInWeaponShop = !isInWeaponShop;
            weaponShopCanvas.SetActive(isInWeaponShop);
        }
        if (!levelScript.getInBuyPeriod())
        {
            weaponShopCanvas.SetActive(false);
        }

        if (isInWeaponShop || gameSettings.isInSettings())
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    
    public bool getIsInWeaponShop()
    {
        return isInWeaponShop;
    }

    public void setIsInWeaponShop(bool b)
    {
        isInWeaponShop = b;
    }
    private void updateHealthDisplay()
    {
        healthNumberText.text = (Mathf.Max(0, Mathf.Ceil(health))).ToString();
        healthBarSlider.value = Mathf.Clamp(health, 0, 100);
        healthBarFill.color = Color.Lerp(Color.white, Color.red, 1 - Mathf.Max(0, health) / 100);
    }

    public void takeDamage(GameObject enemy, float damage, bool head)
    {
        health -= damage;
        updateHealthDisplay();
        if (health <= 0)
        {
            audioManager.playSound("death");
            levelScript.startDeathScene(enemy);
        }

        if (head)
        {
            audioManager.playSound("headshot");
        } else
        {
            audioManager.playSound("bodyshot");
        }
    }
}
