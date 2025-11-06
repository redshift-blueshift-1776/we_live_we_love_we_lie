using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;

public class WeaponInfo : MonoBehaviour
{
    private HashSet<string> weaponNames = new HashSet<string>()
    {
        "USP-S",
        "Glock-18",
        "P250",
        "P2000",
        "Dual Berettas",
        "Tec-9",
        "CZ75-Auto",
        "Desert Eagle",
        "R8 Revolver",

        "MAG-7",
        "Nova",
        "Sawed-Off",
        "XM1014",
        "MAC-10",
        "MP5-SD",
        "MP7",
        "MP9",
        "PP-Bizon",
        "P90",
        "UMP-45",
        "AK-47",
        "AUG",
        "FAMAS",
        "Galil AR",
        "M4A1-S",
        "M4A4",
        "SG 553",
        "AWP",
        "G3SG1",
        "SCAR-20",
        "SSG 08",
        "M249",
        "Negev"
    };

    private HashSet<string> primaryWeaponNames = new HashSet<string>()
    {
        "MAG-7",
        "Nova",
        "Sawed-Off",
        "XM1014",
        "MAC-10",
        "MP5-SD",
        "MP7",
        "MP9",
        "PP-Bizon",
        "P90",
        "UMP-45",
        "AK-47",
        "AUG",
        "FAMAS",
        "Galil AR",
        "M4A1-S",
        "M4A4",
        "SG 553",
        "AWP",
        "G3SG1",
        "SCAR-20",
        "SSG 08",
        "M249",
        "Negev"
    };

    private HashSet<string> secondaryWeaponNames = new HashSet<string>()
    {
        "USP-S",
        "Glock-18",
        "P250",
        "P2000",
        "Dual Berettas",
        "Tec-9",
        "CZ75-Auto",
        "Desert Eagle",
        "R8 Revolver"
    };

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
        {"XM1014", 20},

        //SMGs
        {"MAC-10", 29},
        {"MP5-SD", 27},
        {"MP7", 29},
        {"MP9", 26},
        {"PP-Bizon", 27},
        {"P90", 26},
        {"UMP-45", 35},

        //assault rifles
        {"AK-47", 36},
        {"AUG", 28},
        {"FAMAS", 30},
        {"Galil AR", 30},
        {"M4A1-S", 38},
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

    //range in unity units
    private Dictionary<string, float> weaponRange = new Dictionary<string, float>()
    {
        {"Knife", 1},

        //pistols
        {"USP-S", 50},
        {"Glock-18", 50},
        {"P250", 50},
        {"P2000", 50},
        {"Dual Berettas", 35},
        {"Tec-9", 50},
        {"CZ75-Auto", 25},
        {"Desert Eagle", 100},
        {"R8 Revolver", 150},

        //shotguns
        {"MAG-7", 20},
        {"Nova", 20},
        {"Sawed-Off", 20},
        {"XM1014", 20},

        //SMGs
        {"MAC-10", 35},
        {"MP5-SD", 35},
        {"MP7", 35},
        {"MP9", 35},
        {"PP-Bizon", 35},
        {"P90", 35},
        {"UMP-45", 35},

        //assault rifles
        {"AK-47", 80},
        {"AUG", 80},
        {"FAMAS", 80},
        {"Galil AR", 80},
        {"M4A1-S", 80},
        {"M4A4", 80},
        {"SG 553", 80},

        //snipers
        {"AWP", 200},
        {"G3SG1", 50},
        {"SCAR-20", 50},
        {"SSG 08", 300},

        //machine guns
        {"M249", 120},
        {"Negev", 150},
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
        {"XM1014", 171.43f},

        //SMGs
        {"MAC-10", 800},
        {"MP5-SD", 750},
        {"MP7", 750},
        {"MP9", 857.14f},
        {"PP-Bizon", 750},
        {"P90", 857.14f},
        {"UMP-45", 666.67f},

        //assault rifles
        {"AK-47", 600},
        {"AUG", 600},
        {"FAMAS", 666.67f},
        {"Galil AR", 666.67f},
        {"M4A1-S", 600},
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
        {"XM1014", 7},

        //SMGs
        {"MAC-10", 30},
        {"MP5-SD", 30},
        {"MP7", 30},
        {"MP9", 30},
        {"PP-Bizon", 64},
        {"P90", 50},
        {"UMP-45", 25},

        //assault rifles
        {"AK-47", 30},
        {"AUG", 30},
        {"FAMAS", 25},
        {"Galil AR", 35},
        {"M4A1-S", 20},
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

    private Dictionary<string, float> weaponTotalAmmo = new Dictionary<string, float>()
    {
        //pistols
        {"USP-S", 24},
        {"Glock-18", 120},
        {"P250", 26},
        {"P2000", 52},
        {"Dual Berettas", 120},
        {"Tec-9", 90},
        {"CZ75-Auto", 12},
        {"Desert Eagle", 35},
        {"R8 Revolver", 8},

        //shotguns
        {"MAG-7", 32},
        {"Nova", 32},
        {"Sawed-Off", 32},
        {"XM1014", 32},

        //SMGs
        {"MAC-10", 100},
        {"MP5-SD", 120},
        {"MP7", 120},
        {"MP9", 120},
        {"PP-Bizon", 120},
        {"P90", 100},
        {"UMP-45", 100},

        //assault rifles
        {"AK-47", 90},
        {"AUG", 90},
        {"FAMAS", 90},
        {"Galil AR", 90},
        {"M4A1-S", 80},
        {"M4A4", 90},
        {"SG 553", 90},

        //snipers
        {"AWP", 30},
        {"G3SG1", 90},
        {"SCAR-20", 90},
        {"SSG 08", 90},

        //machine guns
        {"M249", 200},
        {"Negev", 300},
    };

    private Dictionary<string, float> weaponMobility = new Dictionary<string, float>()
    {
        {"Knife", 250},
        //pistols
        {"USP-S", 240},
        {"Glock-18", 240},
        {"P250", 240},
        {"P2000", 240},
        {"Dual Berettas", 240},
        {"Tec-9", 240},
        {"CZ75-Auto", 240},
        {"Desert Eagle", 230},
        {"R8 Revolver", 180},

        //shotguns
        {"MAG-7", 225},
        {"Nova", 220},
        {"Sawed-Off", 210},
        {"XM1014", 215},

        //SMGs
        {"MAC-10", 240},
        {"MP5-SD", 235},
        {"MP7", 220},
        {"MP9", 240},
        {"PP-Bizon", 240},
        {"P90", 230},
        {"UMP-45", 230},

        //assault rifles
        {"AK-47", 215},
        {"AUG", 220},
        {"AUG Scoped", 150},
        {"FAMAS", 220},
        {"Galil AR", 215},
        {"M4A1-S", 225},
        {"M4A4", 225},
        {"SG 553", 210},
        {"SG 553 Scoped", 150},

        //snipers
        {"AWP", 200},
        {"AWP Scoped", 120},
        {"G3SG1", 215},
        {"G3SG1 Scoped", 120},
        {"SCAR-20", 215},
        {"SCAR-20 Scoped", 120},
        {"SSG 08", 230},
        {"SSG 08 Scoped", 230},

        //machine guns
        {"M249", 195},
        {"Negev", 150},
    };

    //1 if true, 0 if false
    private Dictionary<string, float> weaponHoldToShoot = new Dictionary<string, float>()
    {
        {"Knife", 1},
        //pistols
        {"USP-S", 0},
        {"Glock-18", 0},
        {"P250", 0},
        {"P2000", 0},
        {"Dual Berettas", 0},
        {"Tec-9", 0},
        {"CZ75-Auto", 1},
        {"Desert Eagle", 0},
        {"R8 Revolver", 1},

        //shotguns
        {"MAG-7", 0},
        {"Nova", 0},
        {"Sawed-Off", 0},
        {"XM1014", 1},

        //SMGs
        {"MAC-10", 1},
        {"MP5-SD", 1},
        {"MP7", 1},
        {"MP9", 1},
        {"PP-Bizon", 1},
        {"P90", 1},
        {"UMP-45", 1},

        //assault rifles
        {"AK-47", 1},
        {"AUG", 1},
        {"FAMAS", 1},
        {"Galil AR", 1},
        {"M4A1-S", 1},
        {"M4A4", 1},
        {"SG 553", 1},

        //snipers
        {"AWP", 0},
        {"G3SG1", 1},
        {"SCAR-20", 1},
        {"SSG 08", 0},

        //machine guns
        {"M249", 1},
        {"Negev", 1},
    };

    //max angle weapon shots deviate in
    private Dictionary<string, float> weaponBaseInaccuracy = new Dictionary<string, float>()
    {
        {"Knife", 0},
        //pistols
        {"USP-S", 1},
        {"Glock-18", 1},
        {"P250", 1},
        {"P2000", 1},
        {"Dual Berettas", 1},
        {"Tec-9", 1},
        {"CZ75-Auto", 3},
        {"Desert Eagle", 1},
        {"R8 Revolver", 1},

        //shotguns
        {"MAG-7", 5},
        {"Nova", 3},
        {"Sawed-Off", 4},
        {"XM1014", 3},

        //SMGs
        {"MAC-10", 2},
        {"MP5-SD", 2},
        {"MP7", 2},
        {"MP9", 2},
        {"PP-Bizon", 2},
        {"P90", 2},
        {"UMP-45", 2},

        //assault rifles
        {"AK-47", 0.5f},
        {"AUG", 3},
        {"AUG Scoped", 0.5f},
        {"FAMAS", 1},
        {"Galil AR", 1},
        {"M4A1-S", 0.5f},
        {"M4A4", 1.5f},
        {"SG 553", 3},
        {"SG 553 Scoped", 0.5f},

        //snipers
        {"AWP", 20},
        {"AWP Scoped", 0},
        {"G3SG1", 20},
        {"G3SG1 Scoped", 0},
        {"SCAR-20", 20},
        {"SCAR-20 Scoped", 0},
        {"SSG 08", 20},
        {"SSG 08 Scoped", 0},

        //machine guns
        {"M249", 2},
        {"Negev", 2.5f},
    };

    private HashSet<string> scopedWeapons = new HashSet<string> {
        "AUG",
        "SG 553",
        "AWP",
        "G3SG1",
        "SCAR-20",
        "SSG 08"
    };

    public Dictionary<string, float> getWeaponStats(string weaponName, bool scoped)
    {
        if (!isWeaponScoped(weaponName))
        {
            scoped = false;
        }

        Dictionary<string, float> stats = new Dictionary<string, float>();
        stats.Add("damage", weaponAttackDamage.GetValueOrDefault(weaponName));
        stats.Add("range", weaponRange.GetValueOrDefault(weaponName));
        stats.Add("headshotMultiplier", weaponHeadshotMultiplier);
        stats.Add("fireCooldown", convertRPMToPeriod(weaponFireRate.GetValueOrDefault(weaponName)));
        stats.Add("magazineSize", weaponMagazineSize.GetValueOrDefault(weaponName));
        stats.Add("totalAmmo", weaponTotalAmmo.GetValueOrDefault(weaponName));
        stats.Add("mobility", weaponMobility.GetValueOrDefault(weaponName + (scoped ? " Scoped" : "")));
        stats.Add("holdToShoot", weaponHoldToShoot.GetValueOrDefault(weaponName));
        stats.Add("baseInaccuracy", weaponBaseInaccuracy.GetValueOrDefault(weaponName + (scoped ? " Scoped" : "")));

        return stats;
    }
    
    public HashSet<string> getScopedWeapons()
    {
        return scopedWeapons;
    }

    public bool isWeaponScoped(string weaponName)
    {
        return scopedWeapons.Contains(weaponName);
    }

    public HashSet<string> getWeaponNames()
    {
        return weaponNames;
    }

    public HashSet<string> getPrimaryWeaponNames()
    {
        return primaryWeaponNames;
    }
    
    public HashSet<string> getSecondaryWeaponNames()
    {
        return secondaryWeaponNames;
    }

    public string getRandomWeapon()
    {
        return new List<string>(weaponNames)[Random.Range(0, weaponNames.Count)];
    }

    public string getRandomPrimaryWeapon()
    {
        return new List<string>(primaryWeaponNames)[Random.Range(0, primaryWeaponNames.Count)];
    }

    public string getRandomSecondaryWeapon()
    {
        return new List<string>(secondaryWeaponNames)[Random.Range(0, secondaryWeaponNames.Count)];
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float convertRPMToPeriod(float rpm)
    {
        return 60 / rpm;
    }
}
