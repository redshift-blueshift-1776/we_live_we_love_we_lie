using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class WeaponInfo : MonoBehaviour
{

    private Dictionary<string, float> weaponAttackDamage = new Dictionary<string, float>()
    {
        {"Knife", 40},

        //pistols
        {"USP-S", 35},
        {"Glock-18", 30},
        {"P250", 38},
        {"P2000", 35},
        {"Dual Berettas", 38},
        {"Tec-9", 33},
        {"CZ75-Auto", 31},
        {"Desert Eagle", 53},
        {"R8 Revolver", 86},

        //shotguns
        {"MAG-7", 30},
        {"Nova", 26},
        {"Sawed-Off", 32},
        {"XM-1014", 20},

        //SMGs
        {"MAC-10", 29},
        {"MP5-SD", 27},
        {"MP7", 29},
        {"MP9", 26},
        {"PP-Bizon", 27},
        {"P90", 26},
        {"UMP-45", 35},

        //assault rifles
        {"AK47", 36},
        {"AUG", 28},
        {"FAMAS", 30},
        {"Galil AR", 30},
        {"M4A1S", 38},
        {"M4A4", 33},
        {"SG 553", 30},

        //snipers
        {"AWP", 115},
        {"G3SG1", 80},
        {"SCAR-20", 80},
        {"SSG 08", 88},

        //machine guns
        {"M249", 32},
        {"Negev", 35},
    };

    private const float weaponHeadshotMultiplier = 4.0f;

    //fire rate is in shots per minute; convert to period (s)
    private Dictionary<string, float> weaponFireRate = new Dictionary<string, float>()
    {
        {"Knife", 150},

        //pistols
        {"USP-S", 352.94f},
        {"Glock-18", 400},
        {"P250", 400},
        {"P2000", 352.94f},
        {"Dual Berettas", 500},
        {"Tec-9", 500},
        {"CZ75-Auto", 600},
        {"Desert Eagle", 266.67f},
        {"R8 Revolver", 120},

        //shotguns
        {"MAG-7", 70.59f},
        {"Nova", 68.18f},
        {"Sawed-Off", 70.59f},
        {"XM-1014", 171.43f},

        //SMGs
        {"MAC-10", 800},
        {"MP5-SD", 750},
        {"MP7", 750},
        {"MP9", 857.14f},
        {"PP-Bizon", 750},
        {"P90", 857.14f},
        {"UMP-45", 666.67f},

        //assault rifles
        {"AK47", 600},
        {"AUG", 600},
        {"FAMAS", 666.67f},
        {"Galil AR", 666.67f},
        {"M4A1S", 600},
        {"M4A4", 666.67f},
        {"SG 553", 545.45f},

        //snipers
        {"AWP", 41.24f},
        {"G3SG1", 240},
        {"SCAR-20", 240},
        {"SSG 08", 48},

        //machine guns
        {"M249", 750},
        {"Negev", 800},
    };

    private Dictionary<string, float> weaponMagazineSize = new Dictionary<string, float>()
    {
        //pistols
        {"USP-S", 12},
        {"Glock-18", 20},
        {"P250", 13},
        {"P2000", 13},
        {"Dual Berettas", 30},
        {"Tec-9", 18},
        {"CZ75-Auto", 12},
        {"Desert Eagle", 7},
        {"R8 Revolver", 8},

        //shotguns
        {"MAG-7", 5},
        {"Nova", 8},
        {"Sawed-Off", 7},
        {"XM-1014", 7},

        //SMGs
        {"MAC-10", 30},
        {"MP5-SD", 30},
        {"MP7", 30},
        {"MP9", 30},
        {"PP-Bizon", 64},
        {"P90", 50},
        {"UMP-45", 25},

        //assault rifles
        {"AK47", 30},
        {"AUG", 30},
        {"FAMAS", 25},
        {"Galil AR", 35},
        {"M4A1S", 20},
        {"M4A4", 30},
        {"SG 553", 30},

        //snipers
        {"AWP", 5},
        {"G3SG1", 20},
        {"SCAR-20", 20},
        {"SSG 08", 10},

        //machine guns
        {"M249", 100},
        {"Negev", 150},
    };

    private Dictionary<string, float> weaponAttackCooldown = new Dictionary<string, float>()
    {
        {"Knife", 0.5f},
        {"Pistol", 0.25f},
        {"AssaultRifle", 0.2f},
        {"Sniper", 3f},
        {"SMG", 0.1f},
        {"MG", 0.1f}
    };


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float getWeaponDamage(string name)
    {
        return weaponAttackDamage.GetValueOrDefault(name, 0f);
    }

    public float getWeaponAttackCooldown(string name)
    {
        return weaponAttackCooldown.GetValueOrDefault(name, Mathf.Infinity);
    }

    public float convertRPMToPeriod(float rpm)
    {
        return 60 / rpm;
    }
}
