using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player7 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private float health = 100f;

    [SerializeField] private Level7 levelScript;

    [SerializeField] private GameObject UICanvas;
    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private TMP_Text healthNumberText;
    [SerializeField] private Image healthBarFill;
    

    [SerializeField] private GameObject weaponShopCanvas;
    [SerializeField] private GameSettings gameSettings;

    private bool isInWeaponShop = false;


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
        if (!levelScript.getGameStarted())
        {
            UICanvas.SetActive(false);
            return;
        } else
        {
            UICanvas.SetActive(true);
        }

        updateHealthDisplay();

        if (Input.GetKeyDown(KeyCode.B))
        {
            isInWeaponShop = !isInWeaponShop;
        }
        weaponShopCanvas.SetActive(isInWeaponShop);
        if (isInWeaponShop || gameSettings.isInSettings())
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        } else
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

    public void takeDamage(float damage)
    {
        health -= damage;
        updateHealthDisplay();
        if (health <= 0)
        {
            Debug.Log("DEAD!");
        }
    }
}
