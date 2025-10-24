using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    private Color selectedColor = new Color(1, 1, 0);
    ColorBlock defaultColorBlock = new ColorBlock();
    ColorBlock selectedColorBlock = new ColorBlock();
    [Header("Weapon Categories")]
    [SerializeField] private GameObject pistols;
    [SerializeField] private GameObject shotguns;
    [SerializeField] private GameObject subMachineGuns;
    [SerializeField] private GameObject assaultRifles;
    [SerializeField] private GameObject sniperRifles;
    [SerializeField] private GameObject machineGuns;

    [Header("Weapon Category Buttons")]
    [SerializeField] private Button pistolsButton;
    [SerializeField] private Button shotgunsButton;
    [SerializeField] private Button subMachineGunsButton;
    [SerializeField] private Button assaultRiflesButton;
    [SerializeField] private Button sniperRiflesButton;
    [SerializeField] private Button machineGunsButton;

    [Header("Pistols")]
    [Header("USP-S")]
    [SerializeField] private Button USPSButton;
    [SerializeField] private GameObject USPSButtonBorder;
    [SerializeField] private GameObject USPSButtonBackground;

    [Header("Glock-18")]
    [SerializeField] private Button Glock18Button;
    [SerializeField] private GameObject Glock18ButtonBorder;
    [SerializeField] private GameObject Glock18ButtonBackground;

    [Header("P250")]
    [SerializeField] private Button P250Button;
    [SerializeField] private GameObject P250ButtonBorder;
    [SerializeField] private GameObject P250ButtonBackground;

    [Header("P2000")]
    [SerializeField] private Button P2000Button;
    [SerializeField] private GameObject P2000ButtonBorder;
    [SerializeField] private GameObject P2000ButtonBackground;

    [Header("Dual Berettas")]
    [SerializeField] private Button DualBerettasButton;
    [SerializeField] private GameObject DualBerettasButtonBorder;
    [SerializeField] private GameObject DualBerettasButtonBackground;

    [Header("Five-SeveN")]
    [SerializeField] private Button FiveSeveNButton;
    [SerializeField] private GameObject FiveSeveNButtonBorder;
    [SerializeField] private GameObject FiveSeveNButtonBackground;

    [Header("Tec-9")]
    [SerializeField] private Button Tec9Button;
    [SerializeField] private GameObject Tec9ButtonBorder;
    [SerializeField] private GameObject Tec9ButtonBackground;

    [Header("CZ75-Auto")]
    [SerializeField] private Button CZ75AutoButton;
    [SerializeField] private GameObject CZ75AutoButtonBorder;
    [SerializeField] private GameObject CZ75AutoButtonBackground;

    [Header("Desert Eagle")]
    [SerializeField] private Button DesertEagleButton;
    [SerializeField] private GameObject DesertEagleButtonBorder;
    [SerializeField] private GameObject DesertEagleButtonBackground;

    [Header("R8 Revolver")]
    [SerializeField] private Button R8RevolverButton;
    [SerializeField] private GameObject R8RevolverButtonBorder;
    [SerializeField] private GameObject R8RevolverButtonBackground;

    [Header("Shotguns")]
    [Header("MAG-7")]
    [SerializeField] private Button MAG7Button;
    [SerializeField] private GameObject MAG7ButtonBorder;
    [SerializeField] private GameObject MAG7ButtonBackground;

    [Header("Nova")]
    [SerializeField] private Button NovaButton;
    [SerializeField] private GameObject NovaButtonBorder;
    [SerializeField] private GameObject NovaButtonBackground;

    [Header("Sawed-Off")]
    [SerializeField] private Button SawedOffButton;
    [SerializeField] private GameObject SawedOffButtonBorder;
    [SerializeField] private GameObject SawedOffButtonBackground;

    [Header("XM1014")]
    [SerializeField] private Button XM1014Button;
    [SerializeField] private GameObject XM1014ButtonBorder;
    [SerializeField] private GameObject XM1014ButtonBackground;

    [Header("MAC-10")]
    [SerializeField] private Button MAC10Button;
    [SerializeField] private GameObject MAC10ButtonBorder;
    [SerializeField] private GameObject MAC10ButtonBackground;

    [Header("MP5-SD")]
    [SerializeField] private Button MP5SDButton;
    [SerializeField] private GameObject MP5SDButtonBorder;
    [SerializeField] private GameObject MP5SDButtonBackground;

    [Header("MP7")]
    [SerializeField] private Button MP7Button;
    [SerializeField] private GameObject MP7ButtonBorder;
    [SerializeField] private GameObject MP7ButtonBackground;

    [Header("MP9")]
    [SerializeField] private Button MP9Button;
    [SerializeField] private GameObject MP9ButtonBorder;
    [SerializeField] private GameObject MP9ButtonBackground;

    [Header("PP-Bizon")]
    [SerializeField] private Button PPBizonButton;
    [SerializeField] private GameObject PPBizonButtonBorder;
    [SerializeField] private GameObject PPBizonButtonBackground;

    [Header("P90")]
    [SerializeField] private Button P90Button;
    [SerializeField] private GameObject P90ButtonBorder;
    [SerializeField] private GameObject P90ButtonBackground;

    [Header("UMP-45")]
    [SerializeField] private Button UMP45Button;
    [SerializeField] private GameObject UMP45ButtonBorder;
    [SerializeField] private GameObject UMP45ButtonBackground;

    [Header("AssaultRifles")]
    [Header("AK-47")]
    [SerializeField] private Button AK47Button;
    [SerializeField] private GameObject AK47ButtonBorder;
    [SerializeField] private GameObject AK47ButtonBackground;

    [Header("AUG")]
    [SerializeField] private Button AUGButton;
    [SerializeField] private GameObject AUGButtonBorder;
    [SerializeField] private GameObject AUGButtonBackground;

    [Header("FAMAS")]
    [SerializeField] private Button FAMASButton;
    [SerializeField] private GameObject FAMASButtonBorder;
    [SerializeField] private GameObject FAMASButtonBackground;

    [Header("Galil AR")]
    [SerializeField] private Button GalilARButton;
    [SerializeField] private GameObject GalilARButtonBorder;
    [SerializeField] private GameObject GalilARButtonBackground;

    [Header("M4A1-S")]
    [SerializeField] private Button M4A1SButton;
    [SerializeField] private GameObject M4A1SButtonBorder;
    [SerializeField] private GameObject M4A1SButtonBackground;

    [Header("M4A4")]
    [SerializeField] private Button M4A4Button;
    [SerializeField] private GameObject M4A4ButtonBorder;
    [SerializeField] private GameObject M4A4ButtonBackground;

    [Header("SG 553")]
    [SerializeField] private Button SG553Button;
    [SerializeField] private GameObject SG553ButtonBorder;
    [SerializeField] private GameObject SG553ButtonBackground;

    [Header("Sniper Rifles")]
    [Header("AWP")]
    [SerializeField] private Button AWPButton;
    [SerializeField] private GameObject AWPButtonBorder;
    [SerializeField] private GameObject AWPButtonBackground;

    [Header("G3SG1")]
    [SerializeField] private Button G3SG1Button;
    [SerializeField] private GameObject G3SG1ButtonBorder;
    [SerializeField] private GameObject G3SG1ButtonBackground;

    [Header("SCAR-20")]
    [SerializeField] private Button SCAR20Button;
    [SerializeField] private GameObject SCAR20ButtonBorder;
    [SerializeField] private GameObject SCAR20ButtonBackground;

    [Header("SSG 08")]
    [SerializeField] private Button SSG08Button;
    [SerializeField] private GameObject SSG08ButtonBorder;
    [SerializeField] private GameObject SSG08ButtonBackground;

    [Header("Machine Guns")]
    [Header("M249")]
    [SerializeField] private Button M249Button;
    [SerializeField] private GameObject M249ButtonBorder;
    [SerializeField] private GameObject M249ButtonBackground;

    [Header("Negev")]
    [SerializeField] private Button NegevButton;
    [SerializeField] private GameObject NegevButtonBorder;
    [SerializeField] private GameObject NegevButtonBackground;

    private string selectedCategory = "Pistols";
    void Start()
    {
        defaultColorBlock = pistolsButton.colors;
        selectedColorBlock = pistolsButton.colors;

        selectedColorBlock.normalColor = selectedColor;
        selectedColorBlock.highlightedColor = selectedColor;
        selectedColorBlock.pressedColor = selectedColor;
        selectedColorBlock.selectedColor = selectedColor;
        displayWeapons();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void disableAllPages()
    {
        pistols.SetActive(false);
        shotguns.SetActive(false);
        subMachineGuns.SetActive(false);
        assaultRifles.SetActive(false);
        sniperRifles.SetActive(false);
        machineGuns.SetActive(false);

        pistolsButton.colors = defaultColorBlock;
        shotgunsButton.colors = defaultColorBlock;
        subMachineGunsButton.colors = defaultColorBlock;
        assaultRiflesButton.colors = defaultColorBlock;
        sniperRiflesButton.colors = defaultColorBlock;
        machineGunsButton.colors = defaultColorBlock;
    }

    private void displayWeapons()
    {
        disableAllPages();
        switch (selectedCategory)
        {
            case "Pistols":
                pistols.SetActive(true);
                pistolsButton.colors = selectedColorBlock;
                break;
            case "Shotguns":
                shotguns.SetActive(true);
                shotgunsButton.colors = selectedColorBlock;
                break;
            case "SMGs":
                subMachineGuns.SetActive(true);
                subMachineGunsButton.colors = selectedColorBlock;
                break;
            case "AssaultRifles":
                assaultRifles.SetActive(true);
                assaultRiflesButton.colors = selectedColorBlock;
                break;
            case "SniperRifles":
                sniperRifles.SetActive(true);
                sniperRiflesButton.colors = selectedColorBlock;
                break;
            case "MachineGuns":
                machineGuns.SetActive(true);
                machineGunsButton.colors = selectedColorBlock;
                break;
            default:
                pistols.SetActive(true);
                pistolsButton.colors = selectedColorBlock;
                break;
        }
    }

    public void setSelectedCategory(string category)
    {
        selectedCategory = category;
        displayWeapons();
    }
}
