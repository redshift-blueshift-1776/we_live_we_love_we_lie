using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    private Color selectedColor = new Color(1, 1, 0);
    ColorBlock defaultColorBlock = new ColorBlock();
    ColorBlock selectedColorBlock = new ColorBlock();

    [SerializeField] private Weapon weaponScript;

    #region Weapon Categories
    [Header("Weapon Categories")]
    [SerializeField] private GameObject pistols;
    [SerializeField] private GameObject shotguns;
    [SerializeField] private GameObject subMachineGuns;
    [SerializeField] private GameObject assaultRifles;
    [SerializeField] private GameObject sniperRifles;
    [SerializeField] private GameObject machineGuns;
    #endregion

    #region Weapon Category Buttons
    [Header("Weapon Category Buttons")]
    [SerializeField] private Button pistolsButton;
    [SerializeField] private Button shotgunsButton;
    [SerializeField] private Button subMachineGunsButton;
    [SerializeField] private Button assaultRiflesButton;
    [SerializeField] private Button sniperRiflesButton;
    [SerializeField] private Button machineGunsButton;
    #endregion

    #region Pistols
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
    #endregion

    #region Shotguns
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
    #endregion

    #region SMGs
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
    #endregion

    #region Assault Rifles
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
    #endregion

    #region Sniper Rifles
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
    #endregion

    #region Machine Guns
    [Header("Machine Guns")]
    [Header("M249")]
    [SerializeField] private Button M249Button;
    [SerializeField] private GameObject M249ButtonBorder;
    [SerializeField] private GameObject M249ButtonBackground;

    [Header("Negev")]
    [SerializeField] private Button NegevButton;
    [SerializeField] private GameObject NegevButtonBorder;
    [SerializeField] private GameObject NegevButtonBackground;
    #endregion

    private string selectedCategory = "Pistols";
    private string selectedPrimary = "";
    private string selectedSecondary = "";
    void Start()
    {
        defaultColorBlock = pistolsButton.colors;
        selectedColorBlock = pistolsButton.colors;

        selectedColorBlock.normalColor = selectedColor;
        selectedColorBlock.highlightedColor = selectedColor;
        selectedColorBlock.pressedColor = selectedColor;
        selectedColorBlock.selectedColor = selectedColor;
        displayWeapons();

        disableAllPrimaryWeaponUI();
        disableAllSecondaryWeaponUI();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void disableAllCategoryUI()
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

    private void disableAllPrimaryWeaponUI()
    {
        MAG7ButtonBorder.SetActive(false);
        NovaButtonBorder.SetActive(false);
        SawedOffButtonBorder.SetActive(false);
        XM1014ButtonBorder.SetActive(false);

        MAC10ButtonBorder.SetActive(false);
        MP5SDButtonBorder.SetActive(false);
        MP7ButtonBorder.SetActive(false);
        MP9ButtonBorder.SetActive(false);
        PPBizonButtonBorder.SetActive(false);
        P90ButtonBorder.SetActive(false);
        UMP45ButtonBorder.SetActive(false);

        AK47ButtonBorder.SetActive(false);
        AUGButtonBorder.SetActive(false);
        FAMASButtonBorder.SetActive(false);
        GalilARButtonBorder.SetActive(false);
        M4A1SButtonBorder.SetActive(false);
        M4A4ButtonBorder.SetActive(false);
        SG553ButtonBorder.SetActive(false);

        AWPButtonBorder.SetActive(false);
        G3SG1ButtonBorder.SetActive(false);
        SCAR20ButtonBorder.SetActive(false);
        SSG08ButtonBorder.SetActive(false);

        M249ButtonBorder.SetActive(false);
        NegevButtonBorder.SetActive(false);
    }

    private void disableAllSecondaryWeaponUI()
    {
        USPSButtonBorder.SetActive(false);
        Glock18ButtonBorder.SetActive(false);
        P250ButtonBorder.SetActive(false);
        P2000ButtonBorder.SetActive(false);
        DualBerettasButtonBorder.SetActive(false);
        FiveSeveNButtonBorder.SetActive(false);
        Tec9ButtonBorder.SetActive(false);
        CZ75AutoButtonBorder.SetActive(false);
        DesertEagleButtonBorder.SetActive(false);
        R8RevolverButtonBorder.SetActive(false);
    }

    private void displayWeapons()
    {
        disableAllCategoryUI();
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

    private void displaySelectedPrimary()
    {
        disableAllPrimaryWeaponUI();
        switch (selectedPrimary)
        {
            case "MAG-7":
                MAG7ButtonBorder.SetActive(true);
                break;
            case "Nova":
                NovaButtonBorder.SetActive(true);
                break;
            case "Sawed-Off":
                SawedOffButtonBorder.SetActive(true);
                break;
            case "XM1014":
                XM1014ButtonBorder.SetActive(true);
                break;
            case "MAC-10":
                MAC10ButtonBorder.SetActive(true);
                break;
            case "MP5-SD":
                MP5SDButtonBorder.SetActive(true);
                break;
            case "MP7":
                MP7ButtonBorder.SetActive(true);
                break;
            case "MP9":
                MP9ButtonBorder.SetActive(true);
                break;
            case "PP-Bizon":
                PPBizonButtonBorder.SetActive(true);
                break;
            case "P90":
                P90ButtonBorder.SetActive(true);
                break;
            case "UMP-45":
                UMP45ButtonBorder.SetActive(true);
                break;
            case "AK-47":
                AK47ButtonBorder.SetActive(true);
                break;
            case "AUG":
                AUGButtonBorder.SetActive(true);
                break;
            case "FAMAS":
                FAMASButtonBorder.SetActive(true);
                break;
            case "Galil AR":
                GalilARButtonBorder.SetActive(true);
                break;
            case "M4A1-S":
                M4A1SButtonBorder.SetActive(true);
                break;
            case "M4A4":
                M4A4ButtonBorder.SetActive(true);
                break;
            case "SG 553":
                SG553ButtonBorder.SetActive(true);
                break;
            case "AWP":
                AWPButtonBorder.SetActive(true);
                break;
            case "G3SG1":
                G3SG1ButtonBorder.SetActive(true);
                break;
            case "SCAR-20":
                SCAR20ButtonBorder.SetActive(true);
                break;
            case "SSG 08":
                SSG08ButtonBorder.SetActive(true);
                break;
            default:
                MAG7ButtonBorder.SetActive(true);
                break;
        }

        weaponScript.updatePrimary(selectedPrimary);
    }

    private void displaySelectedSecondary()
    {
        disableAllSecondaryWeaponUI();
        switch (selectedSecondary)
        {
            case "USP-S":
                USPSButtonBorder.SetActive(true);
                break;
            case "Glock-18":
                Glock18ButtonBorder.SetActive(true);
                break;
            case "P250":
                P250ButtonBorder.SetActive(true);
                break;
            case "P2000":
                P2000ButtonBorder.SetActive(true);
                break;
            case "Dual Berettas":
                DualBerettasButtonBorder.SetActive(true);
                break;
            case "Five-SeveN":
                FiveSeveNButtonBorder.SetActive(true);
                break;
            case "Tec-9":
                Tec9ButtonBorder.SetActive(true);
                break;
            case "CZ75-Auto":
                CZ75AutoButtonBorder.SetActive(true);
                break;
            case "Desert Eagle":
                DesertEagleButtonBorder.SetActive(true);
                break;
            case "R8 Revolver":
                R8RevolverButtonBorder.SetActive(true);
                break;
        }

        weaponScript.updateSecondary(selectedSecondary);
    }

    public void setSelectedCategory(string category)
    {
        selectedCategory = category;
        displayWeapons();
    }

    public void setSelectedPrimary(string weapon)
    {
        selectedPrimary = weapon;
        displaySelectedPrimary();
    }

    public void setSelectedSecondary(string weapon)
    {
        selectedSecondary = weapon;
        displaySelectedSecondary();
    }
}
