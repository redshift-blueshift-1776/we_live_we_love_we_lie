using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
public class Enemy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private NavMeshAgent enemy;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject eye;

    [SerializeField] private GameObject wanderNodes;
    [SerializeField] private GameObject navMeshJumps;
    [SerializeField] private GameObject playerRaycastNodes;

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

    [Header("Weapon Info")]
    [SerializeField] private WeaponInfo weaponInfo;
    private string currWeapon = "";
    private string weaponCategory = "";
    private Dictionary<string, float> weaponStats = new Dictionary<string, float>();
    private float primaryBaseInaccuracy;

    public enum EnemyState
    {
        None,
        Wander,
        Patrol,
        Chase,
        Attack,
        Dead
    }

    float health = 100;
    private EnemyState currentState;
    private float stateTimer = 0f;

    private bool isJumping = false;
    private const float defaultJumpHeight = 2.0f;


    HashSet<Vector3> wanderLocations = new HashSet<Vector3>();
    void Start()
    {
        activateNodes();
        foreach (Transform child in wanderNodes.transform)
        {
            wanderLocations.Add(child.position);
        }

        enemy.autoTraverseOffMeshLink = false;
        currentState = EnemyState.None;
        TransitionToState(EnemyState.Wander);
        
        //enemy.SetDestination(player.transform.position);

        makeNodesInvisible();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == EnemyState.Dead) return;

        stateTimer += Time.deltaTime;

        // State machine logic
        switch (currentState)
        {
            case EnemyState.Wander:
                HandleWanderState();
                break;
            case EnemyState.Patrol:
                HandlePatrolState();
                break;
            case EnemyState.Chase:
                HandleChaseState();
                break;
            case EnemyState.Attack:
                HandleAttackState();
                break;
        }
        handleMeshLink();
    }

    private void activateNodes()
    {
        wanderNodes.SetActive(true);
        navMeshJumps.SetActive(true);
        playerRaycastNodes.SetActive(true);
    }

    private void makeNodesInvisible()
    {
        foreach (Transform child in wanderNodes.transform)
        {
            child.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }

        foreach(MeshRenderer meshRenderer in navMeshJumps.GetComponentsInChildren<MeshRenderer>())
        {
            if (meshRenderer != null)
            {
                meshRenderer.enabled = false;
            }
        }

        foreach (MeshRenderer meshRenderer in playerRaycastNodes.GetComponentsInChildren<MeshRenderer>())
        {
            if (meshRenderer != null)
            {
                meshRenderer.enabled = false;
            }
        }
    }

    private void handleMeshLink()
    {
        if (enemy.isOnOffMeshLink && !isJumping)
        {
            OffMeshLinkData linkData = enemy.currentOffMeshLinkData;
            Vector3 startPos = enemy.transform.position;
            Vector3 endPos = linkData.endPos;

            if (Vector3.Distance(startPos, enemy.transform.position) > 2.5f || endPos.y - startPos.y > defaultJumpHeight)
            {
                enemy.CompleteOffMeshLink();
                return;
            }
            StartCoroutine(PerformJump(startPos, endPos, true));
        }
    }

    private IEnumerator PerformJump(Vector3 startPos, Vector3 endPos, bool meshLinkJump, float jumpHeight = defaultJumpHeight)
    {
        isJumping = true;
        // Get link data


        // Disable NavMesh auto-movement during jump
        enemy.updatePosition = false;

        float elapsed = 0;
        const float g = -9.8f;

        Vector3 velocity = new Vector3(0, Mathf.Sqrt(-2 * g * jumpHeight), 0);
        Vector3 acceleration = new Vector3(0, g, 0);
        float t1 = velocity.y / -g; //time until peak of jump
        float dy = endPos.y - (startPos.y + jumpHeight);
        float t2 = Mathf.Sqrt(2 * Mathf.Abs(dy / g));
        float jumpDuration = t1 + t2;
        Debug.Log(t1 + " " + t2 + " " + jumpDuration);
        float vx = (endPos.x - startPos.x) / jumpDuration;
        float vz = (endPos.z - startPos.z) / jumpDuration;
        velocity.x = vx;
        velocity.z = vz;

        while (elapsed < jumpDuration)
        {
            elapsed += Time.deltaTime;

            Vector3 currentPos = enemy.transform.position;
            velocity += acceleration * Time.deltaTime;
            currentPos += velocity * Time.deltaTime;

            // move the agent
            enemy.transform.position = currentPos;

            yield return null;
        }

        // ensure agent is exactly at end position
        enemy.transform.position = endPos;

        // re-enable NavMesh auto-movement
        enemy.updatePosition = true;

        if (meshLinkJump) {
            enemy.CompleteOffMeshLink();
        }

        isJumping = false;
    }

    public void takeDamage(float damage)
    {
        health -= damage;
        if (health < 0)
        {
            Destroy(gameObject);
        }
    }

    Stack<Vector3> currWanderLocations = new Stack<Vector3>();
    private void initializeWanderLocations()
    {
        currWanderLocations.Clear();

        List<Vector3> shuffled = wanderLocations.OrderBy(x => Random.value).ToList();

        foreach (Vector3 location in shuffled)
        {
            currWanderLocations.Push(location);
        }

        currTarget = currWanderLocations.Pop();
    }

    private bool canSeePlayer()
    {
        foreach (Transform transform in playerRaycastNodes.GetComponentsInChildren<Transform>())
        {
            Vector3 eyePos = eye.transform.position;
            Vector3 nodePos = transform.position;
            Vector3 direction = nodePos - eyePos;

            RaycastHit hitData;
            if (Physics.Raycast(eyePos, direction, out hitData, Mathf.Infinity))
            {
                if (hitData.collider.gameObject.CompareTag("Player") || hitData.collider.gameObject.CompareTag("PlayerRaycastNode"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void disableAllWeapons()
    {
        knife.SetActive(false);

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

    public void updateWeapon(string weapon)
    {
        currWeapon = weapon;
        weaponStats = weaponInfo.getWeaponStats(weapon, false);
        primaryBaseInaccuracy = weaponStats.GetValueOrDefault("baseInaccuracy", 0);

        disableAllWeapons();
        switch (weapon)
        {
            case "MAG-7":
                MAG7.SetActive(true);
                weaponCategory = "Shotgun";
                break;
            case "Nova":
                Nova.SetActive(true);
                weaponCategory = "Shotgun";
                break;
            case "Sawed-Off":
                SawedOff.SetActive(true);
                weaponCategory = "Shotgun";
                break;
            case "XM1014":
                XM1014.SetActive(true);
                weaponCategory = "Shotgun";
                break;
            case "MAC-10":
                MAC10.SetActive(true);
                weaponCategory = "SMG";
                break;
            case "MP5-SD":
                MP5SD.SetActive(true);
                weaponCategory = "SMG";
                break;
            case "MP7":
                MP7.SetActive(true);
                weaponCategory = "SMG";
                break;
            case "MP9":
                MP9.SetActive(true);
                weaponCategory = "SMG";
                break;
            case "PP-Bizon":
                PPBizon.SetActive(true);
                weaponCategory = "SMG";
                break;
            case "P90":
                P90.SetActive(true);
                weaponCategory = "SMG";
                break;
            case "UMP-45":
                UMP45.SetActive(true);
                weaponCategory = "SMG";
                break;
            case "AK-47":
                AK47.SetActive(true);
                weaponCategory = "AssaultRifle";
                break;
            case "AUG":
                AUG.SetActive(true);
                weaponCategory = "AssaultRifle";
                break;
            case "FAMAS":
                FAMAS.SetActive(true);
                weaponCategory = "AssaultRifle";
                break;
            case "Galil AR":
                GalilAR.SetActive(true);
                weaponCategory = "AssaultRifle";
                break;
            case "M4A1-S":
                M4A1S.SetActive(true);
                weaponCategory = "AssaultRifle";
                break;
            case "M4A4":
                M4A4.SetActive(true);
                weaponCategory = "AssaultRifle";
                break;
            case "SG 553":
                SG553.SetActive(true);
                weaponCategory = "AssaultRifle";
                break;
            case "AWP":
                AWP.SetActive(true);
                weaponCategory = "SniperRifle";
                break;
            case "G3SG1":
                G3SG1.SetActive(true);
                weaponCategory = "SniperRifle";
                break;
            case "SCAR-20":
                SCAR20.SetActive(true);
                weaponCategory = "SniperRifle";
                break;
            case "SSG 08":
                SSG08.SetActive(true);
                weaponCategory = "SniperRifle";
                break;
            case "M249":
                M249.SetActive(true);
                weaponCategory = "MachineGun";
                break;
            case "Negev":
                Negev.SetActive(true);
                weaponCategory = "MachineGun";
                break;


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
            default:
                R8Revolver.SetActive(true);
                break;
        }
    }

    Vector3 currTarget;
    private const float minDistanceThreshold = 5.0f;
    private void HandleWanderState()
    {
        if (currWanderLocations.Count == 0)
        {
            initializeWanderLocations();
            return;
        }

        if (Vector3.Distance(transform.position, currTarget) < minDistanceThreshold)
        {
            currTarget = currWanderLocations.Pop();
        }

        enemy.SetDestination(currTarget);

        //if (canSeePlayer())
        //{
        //    TransitionToState(EnemyState.Chase);
        //}
    }

    private void HandlePatrolState()
    {

    }

    private void HandleChaseState()
    {

    }

    private void HandleAttackState()
    {

    }

    private void HandleDeadState()
    {

    }

    private void TransitionToState(EnemyState newState)
    {
        OnStateExit(currentState);
        currentState = newState;
        stateTimer = 0f;
        OnStateEnter(newState);
    }

    private void OnStateEnter(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Wander:
                initializeWanderLocations();
                break;
            default:
                break;
        }
    }

    private void OnStateExit(EnemyState state)
    {
        // Cleanup when leaving a state
    }
}
