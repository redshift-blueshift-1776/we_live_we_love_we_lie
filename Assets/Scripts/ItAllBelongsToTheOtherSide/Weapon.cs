using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject knife;
    private string activeWeapon = "Knife";
    private float timeSinceAttack = Mathf.Infinity;

    [SerializeField] private Player7 player7;
    [SerializeField] private Animator knifeAnimator;
    [SerializeField] private AnimationClip knifeAttackAnimation;
    public WeaponInfo weaponInfo;

    private Dictionary<string, float> weaponSpread = new Dictionary<string, float>()
    {
        
    };


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!player7.getIsInWeaponShop())
        {
            handleAttack();
        }
        timeSinceAttack += Time.deltaTime;
    }

    private void handleAttack()
    {
        if (activeWeapon == "Knife")
        {
            if (Input.GetMouseButton(0))
            {
                if (!knifeAnimator.GetCurrentAnimatorStateInfo(0).IsName("KnifeAttack")
                    && timeSinceAttack >= weaponInfo.getWeaponAttackCooldown("Knife"))
                {
                    knifeAnimator.Play("KnifeAttack", 0, 0f);
                    timeSinceAttack = 0f;
                }
            } else if (Input.GetMouseButtonDown(1))
            { 

            }
        }
    }
}
