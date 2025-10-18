using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class WeaponInfo : MonoBehaviour
{

    private Dictionary<string, float> weaponAttackDamage = new Dictionary<string, float>()
    {
        {"Knife", 40},
        {"Pistol", 8},
        {"AssaultRifle", 20},
        {"Sniper", 95},
        {"SMG", 15},
        {"MG", 20}
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
}
