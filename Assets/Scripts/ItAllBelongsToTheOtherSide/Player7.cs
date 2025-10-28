using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player7 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private float health = 60f;

    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private TMP_Text healthNumberText;
    [SerializeField] private Image healthBarFill;

    [SerializeField] private GameObject weaponShopCanvas;

    private bool isInWeaponShop = false;

    void Start()
    {
        weaponShopCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        healthNumberText.text = (Mathf.Ceil(health)).ToString();
        healthBarSlider.value = Mathf.Clamp(health, 0, 100);
        healthBarFill.color = Color.Lerp(Color.white, Color.red, 1 - health / 100);

        if (Input.GetKeyDown(KeyCode.B))
        {
            isInWeaponShop = !isInWeaponShop;
        }
        weaponShopCanvas.SetActive(isInWeaponShop);
        if (isInWeaponShop)
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
}
