using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Weapon : MonoBehaviour
{
    private float timeSinceAttack = Mathf.Infinity;

    [SerializeField] private Player7 player7;
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private Animator knifeAnimator;
    [SerializeField] private AnimationClip knifeAttackAnimation;
    public WeaponInfo weaponInfo;

    [SerializeField] private GameObject primaryWeaponContainer;
    [SerializeField] private GameObject secondaryWeaponContainer;

    [SerializeField] private GameObject bulletHolePrefab;

    #region Weapons
    [Header("Knife")]
    [SerializeField] private GameObject knife;

    [Header("Pistols")]
    [SerializeField] private GameObject USPS;
    [SerializeField] private GameObject Glock18;
    [SerializeField] private GameObject P250;
    [SerializeField] private GameObject P2000;
    [SerializeField] private GameObject DualBerettas;
    [SerializeField] private GameObject FiveSeveN;
    [SerializeField] private GameObject Tec9;
    [SerializeField] private GameObject CZ75Auto;
    [SerializeField] private GameObject DesertEagle;
    [SerializeField] private GameObject R8Revolver;

    [Header("Shotguns")]
    [SerializeField] private GameObject MAG7;
    [SerializeField] private GameObject Nova;
    [SerializeField] private GameObject SawedOff;
    [SerializeField] private GameObject XM1014;

    [Header("SMGs")]
    [SerializeField] private GameObject MAC10;
    [SerializeField] private GameObject MP5SD;
    [SerializeField] private GameObject MP7;
    [SerializeField] private GameObject MP9;
    [SerializeField] private GameObject PPBizon;
    [SerializeField] private GameObject P90;
    [SerializeField] private GameObject UMP45;

    [Header("Assault Rifles")]
    [SerializeField] private GameObject AK47;
    [SerializeField] private GameObject AUG;
    [SerializeField] private GameObject FAMAS;
    [SerializeField] private GameObject GalilAR;
    [SerializeField] private GameObject M4A1S;
    [SerializeField] private GameObject M4A4;
    [SerializeField] private GameObject SG553;

    [Header("Sniper Rifles")]
    [SerializeField] private GameObject AWP;
    [SerializeField] private GameObject G3SG1;
    [SerializeField] private GameObject SCAR20;
    [SerializeField] private GameObject SSG08;

    [Header("Machine Guns")]
    [SerializeField] private GameObject M249;
    [SerializeField] private GameObject Negev;
    #endregion

    #region UI
    [Header("UI")]
    [SerializeField] private Image primaryWeaponBorder;
    [SerializeField] private TMP_Text primaryWeaponText;

    [SerializeField] private Image secondaryWeaponBorder;
    [SerializeField] private TMP_Text secondaryWeaponText;

    [SerializeField] private Image knifeWeaponBorder;
    #endregion
    private int weaponIndex = 3;
    private string primaryWeapon = "";
    private string secondaryWeapon = "";

    private string primaryWeaponCategory = "";

    void Start()
    {
        playDrawWeaponSound();
    }

    // Update is called once per frame
    void Update()
    {
        displayWeaponModel();

        if (!player7.getIsInWeaponShop())
        {
            handleShoot();
            handleKnifeAttack();
        }
        timeSinceAttack += Time.deltaTime;
        int prevWeaponIndex = weaponIndex;
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (primaryWeapon != "")
            {
                weaponIndex = 1;
            }
        } else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (secondaryWeapon != "") 
            {
                weaponIndex = 2;
            }
        } else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            weaponIndex = 3;
        }

        if (prevWeaponIndex != weaponIndex)
        {
            playDrawWeaponSound();
        }

    }

    private void displayWeaponModel()
    {
        if (weaponIndex == 1)
        {
            primaryWeaponContainer.SetActive(true);
            secondaryWeaponContainer.SetActive(false);

            primaryWeaponBorder.color = Color.yellow;
            secondaryWeaponBorder.color = Color.white;
            knifeWeaponBorder.color = Color.white;

            knife.SetActive(false);
        } else if (weaponIndex == 2)
        {
            primaryWeaponContainer.SetActive(false);
            secondaryWeaponContainer.SetActive(true);

            primaryWeaponBorder.color = Color.white;
            secondaryWeaponBorder.color = Color.yellow;
            knifeWeaponBorder.color = Color.white;

            knife.SetActive(false);
        } else if (weaponIndex == 3)
        {
            primaryWeaponContainer.SetActive(false);
            secondaryWeaponContainer.SetActive(false);

            primaryWeaponBorder.color = Color.white;
            secondaryWeaponBorder.color = Color.white;
            knifeWeaponBorder.color = Color.yellow;

            knife.SetActive(true);
        }
    }

    private void handleShoot()
    {
        if (weaponIndex == 3)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hitData;
            playShootWeaponSound();
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hitData, 1000f))
            {
                GameObject objectHit = hitData.collider.gameObject;
                Debug.Log(hitData.normal);
                GameObject newBulletHole = Instantiate(bulletHolePrefab);
                newBulletHole.transform.position = hitData.point;
                newBulletHole.transform.rotation = Quaternion.LookRotation(hitData.normal);
                newBulletHole.transform.rotation *= Quaternion.Euler(90, 0, 0);
                newBulletHole.transform.SetParent(objectHit.transform);
                StartCoroutine(fadeBulletHole(newBulletHole));
            }
        }
    }

    private void playDrawWeaponSound()
    {
        if (weaponIndex == 2) {
            if (secondaryWeapon == "USP-S")
            {
                audioManager.playSound("USPSDraw");
            }
            else
            {
                audioManager.playSound("DefaultPistolDraw");
            }
        } else if (weaponIndex == 3)
        {
            audioManager.playSound("KnifeDraw");
        } else
        {
            //primary weapon
            if (primaryWeapon == "AWP")
            {
                audioManager.playSound("AWPDraw");
            }
            else if (primaryWeapon == "SSG 08")
            {
                audioManager.playSound("SSG08Draw");
            }
            else
            {
                switch (primaryWeaponCategory)
                {
                    case "Shotgun":
                        audioManager.playSound("DefaultShotgunDraw");
                        break;
                    case "SMG":
                        audioManager.playSound("DefaultSMGDraw");
                        break;
                    case "AssaultRifle":
                        audioManager.playSound("DefaultARDraw");
                        break;
                    case "SniperRifle":
                        audioManager.playSound("SCAR20Draw");
                        break;
                    case "MachineGun":
                        audioManager.playSound("DefaultMGDraw");
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void playShootWeaponSound()
    {
        if (weaponIndex == 2)
        {
            if (secondaryWeapon == "USP-S")
            {
                audioManager.playSound("USPSShoot");
            }
            else
            {
                audioManager.playSound("DefaultPistolShoot");
            }
        }
        else if (weaponIndex == 3)
        {
            audioManager.playSound("KnifeSlash");
        }
        else
        {
            //primary weapon
            if (primaryWeapon == "AWP")
            {
                audioManager.playSound("AWPShoot");
            }
            else if (primaryWeapon == "SSG 08")
            {
                audioManager.playSound("SSG08Shoot");
            }
            else
            {
                switch (primaryWeaponCategory)
                {
                    case "Shotgun":
                        audioManager.playSound("DefaultShotgunShoot");
                        break;
                    case "SMG":
                        audioManager.playSound("DefaultSMGShoot");
                        break;
                    case "AssaultRifle":
                        audioManager.playSound("DefaultARShoot");
                        break;
                    case "SniperRifle":
                        audioManager.playSound("SCAR20Shoot");
                        break;
                    case "MachineGun":
                        audioManager.playSound("DefaultMGShoot");
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private const float bulletDecayTime = 5f;
    private IEnumerator fadeBulletHole(GameObject bulletHole)
    {
        float t = 0;
        Color color = bulletHole.GetComponent<MeshRenderer>().material.color;
        while (t < bulletDecayTime)
        {
            float normalizedT = t / bulletDecayTime;

            bulletHole.GetComponent<MeshRenderer>().material.color = new Color(color.r, color.g, color.b, 1 - normalizedT);

            t += Time.deltaTime;
            yield return null;
        }
        Destroy(bulletHole);
        yield return null;
    }

    private void handleKnifeAttack()
    {
        if (weaponIndex != 3)
        {
            return;
        }
        if (Input.GetMouseButton(0))
        {
            if (!knifeAnimator.GetCurrentAnimatorStateInfo(0).IsName("KnifeAttack")
                && timeSinceAttack >= weaponInfo.getWeaponAttackCooldown("Knife"))
            {
                knifeAnimator.Play("KnifeAttack", 0, 0f);
                playShootWeaponSound();
                timeSinceAttack = 0f;
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {

        }
    }

    private void disableAllPrimary()
    {
        MAG7.SetActive(false);
        Nova.SetActive(false);
        SawedOff.SetActive(false);
        XM1014.SetActive(false);

        MAC10.SetActive(false);
        MP5SD.SetActive(false);
        MP7.SetActive(false);
        MP9.SetActive(false);
        PPBizon.SetActive(false);
        P90.SetActive(false);
        UMP45.SetActive(false);

        AK47.SetActive(false);
        AUG.SetActive(false);
        FAMAS.SetActive(false);
        GalilAR.SetActive(false);
        M4A1S.SetActive(false);
        M4A4.SetActive(false);
        SG553.SetActive(false);

        AWP.SetActive(false);
        G3SG1.SetActive(false);
        SCAR20.SetActive(false);
        SSG08.SetActive(false);

        M249.SetActive(false);
        Negev.SetActive(false);
    }

    private void disableAllSecondary()
    {
        USPS.SetActive(false);
        Glock18.SetActive(false);
        P250.SetActive(false);
        P2000.SetActive(false);
        DualBerettas.SetActive(false);
        FiveSeveN.SetActive(false);
        Tec9.SetActive(false);
        CZ75Auto.SetActive(false);
        DesertEagle.SetActive(false);
        R8Revolver.SetActive(false);
    }

    public void updatePrimary(string primary)
    {
        primaryWeapon = primary;
        weaponIndex = 1;
        primaryWeaponText.text = primary;

        disableAllPrimary();
        switch (primaryWeapon)
        {
            case "MAG7":
                MAG7.SetActive(true);
                primaryWeaponCategory = "Shotgun";
                break;
            case "Nova":
                Nova.SetActive(true);
                primaryWeaponCategory = "Shotgun";
                break;
            case "Sawed-Off":
                SawedOff.SetActive(true);
                primaryWeaponCategory = "Shotgun";
                break;
            case "XM1014":
                XM1014.SetActive(true);
                primaryWeaponCategory = "Shotgun";
                break;
            case "MAC-10":
                MAC10.SetActive(true);
                primaryWeaponCategory = "SMG";
                break;
            case "MP5-SD":
                MP5SD.SetActive(true);
                primaryWeaponCategory = "SMG";
                break;
            case "MP7":
                MP7.SetActive(true);
                primaryWeaponCategory = "SMG";
                break;
            case "MP9":
                MP9.SetActive(true);
                primaryWeaponCategory = "SMG";
                break;
            case "PP-Bizon":
                PPBizon.SetActive(true);
                primaryWeaponCategory = "SMG";
                break;
            case "P90":
                P90.SetActive(true);
                primaryWeaponCategory = "SMG";
                break;
            case "UMP-45":
                UMP45.SetActive(true);
                primaryWeaponCategory = "SMG";
                break;
            case "AK-47":
                AK47.SetActive(true);
                primaryWeaponCategory = "AssaultRifle";
                break;
            case "AUG":
                AUG.SetActive(true);
                primaryWeaponCategory = "AssaultRifle";
                break;
            case "FAMAS":
                FAMAS.SetActive(true);
                primaryWeaponCategory = "AssaultRifle";
                break;
            case "Galil AR":
                GalilAR.SetActive(true);
                primaryWeaponCategory = "AssaultRifle";
                break;
            case "M4A1-S":
                M4A1S.SetActive(true);
                primaryWeaponCategory = "AssaultRifle";
                break;
            case "M4A4":
                M4A4.SetActive(true);
                primaryWeaponCategory = "AssaultRifle";
                break;
            case "SG 553":
                SG553.SetActive(true);
                primaryWeaponCategory = "AssaultRifle";
                break;
            case "AWP":
                AWP.SetActive(true);
                primaryWeaponCategory = "SniperRifle";
                break;
            case "G3SG1":
                G3SG1.SetActive(true);
                primaryWeaponCategory = "SniperRifle";
                break;
            case "SCAR-20":
                SCAR20.SetActive(true);
                primaryWeaponCategory = "SniperRifle";
                break;
            case "SSG 08":
                SSG08.SetActive(true);
                primaryWeaponCategory = "SniperRifle";
                break;
            case "M249":
                M249.SetActive(true);
                primaryWeaponCategory = "MachineGun";
                break;
            case "Negev":
                Negev.SetActive(true);
                primaryWeaponCategory = "MachineGun";
                break;
            default:
                MAG7.SetActive(true);
                break;
        }
        playDrawWeaponSound();
    }

    public void updateSecondary(string secondary)
    {
        secondaryWeapon = secondary;
        weaponIndex = 2;
        secondaryWeaponText.text = secondary;

        disableAllSecondary();
        switch (secondaryWeapon)
        {
            case "USP-S":
                USPS.SetActive(true);
                break;
            case "Glock-18":
                Glock18.SetActive(true);
                break;
            case "P250":
                P250.SetActive(true);
                break;
            case "P2000":
                P2000.SetActive(true);
                break;
            case "Dual Berettas":
                DualBerettas.SetActive(true);
                break;
            case "Five-SeveN":
                FiveSeveN.SetActive(true);
                break;
            case "Tec-9":
                Tec9.SetActive(true);
                break;
            case "CZ75-Auto":
                CZ75Auto.SetActive(true);
                break;
            case "Desert Eagle":
                DesertEagle.SetActive(true);
                break;
            case "R8 Revolver":
                R8Revolver.SetActive(true);
                break;
        }
        playDrawWeaponSound();
    }

}
