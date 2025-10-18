using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject knife;
    private string activeWeapon = "Knife";
    private float timeSinceAttack = Mathf.Infinity;
    private float attackCooldown = 2f;

    [SerializeField] private Animator knifeAnimator;
    [SerializeField] private AnimationClip knifeAttackAnimation;

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


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        handleAttack();
        attackCooldown += Time.deltaTime;
    }

    private void handleAttack()
    {
        if (activeWeapon == "Knife")
        {
            if (Input.GetMouseButton(0))
            {
                if (!knifeAnimator.GetCurrentAnimatorStateInfo(0).IsName("KnifeAttack")
                    && attackCooldown >= weaponAttackCooldown.GetValueOrDefault("Knife", -1))
                {
                    knifeAnimator.Play("KnifeAttack", 0, 0f);
                    attackCooldown = 0f;
                }
            } else if (Input.GetMouseButtonDown(1))
            { 

            }
        }
    }
}
