using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
public class Enemy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Level7 levelScript;

    [SerializeField] private NavMeshAgent enemy;
    [SerializeField] private GameObject player;
    [SerializeField] private Player7 playerScript;
    [SerializeField] private PlayerMovement7 playerMovementScript;
    [SerializeField] private Weapon weaponScript;
    [SerializeField] private GameObject playerHead;
    [SerializeField] private GameObject playerBody;
    [SerializeField] private GameObject eye;

    [SerializeField] private GameObject wanderNodes;
    [SerializeField] private GameObject navMeshJumps;
    [SerializeField] private GameObject playerRaycastNodes;

    [SerializeField] private AudioManager bodySounds;
    [SerializeField] private AudioManager shootSounds;

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
    [SerializeField] private GameObject muzzlePosition;
    [SerializeField] private GameObject bulletHolePrefab;
    [SerializeField] private GameObject bulletTracerPrefab;
    [SerializeField] private string currWeapon = "";
    private string weaponCategory = "";
    private Dictionary<string, float> weaponStats = new Dictionary<string, float>();
    private float baseInaccuracy;
    private float range;
    private float damage;
    private float fireCooldown = Mathf.Infinity;

    //[Header("Bot Stats")]
    private float fov = 200f;
    private float hearingDistance = 15f;
    private float reactionTime = 0.5f;
    private float fireCooldownMult = 1.25f;
    private float chanceOfAimingForHead = 0.0f;
    private float chanceOfAimingForBody = 0.0f;
    private float acceptableChanceOfHitting = 0.4f;
    private float defaultMoveSpeed = 6.0f;
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
    private EnemyState currentState = EnemyState.None;
    private float stateTimer = 0f;

    private bool isJumping = false;
    private const float defaultJumpHeight = 2.0f;


    HashSet<Vector3> wanderLocations = new HashSet<Vector3>();

    private bool dead = false;
    void Start()
    {
    }

    public void Initialize(int difficulty)
    {
        bodySounds.setAll3D(50);
        shootSounds.setAll3D(200);
        foreach (Transform child in wanderNodes.transform)
        {
            wanderLocations.Add(child.position);
        }
        enemy.autoTraverseOffMeshLink = false;

        if (difficulty == 7)
        {
            currWeapon = "SSG 08";
        }

        if (currWeapon.Equals(""))
        {
            currWeapon = weaponInfo.getRandomWeapon();
        }
        updateWeapon(currWeapon);
        setDifficulty(difficulty);

        StartCoroutine(handleFootstepSounds());
        TransitionToState(EnemyState.Wander);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == EnemyState.Dead) {
            return;
        }

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
            case EnemyState.Dead:
                break;
            default:
                break;
        }

        handleMeshLink();
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

    public void takeDamage(float damage, bool head)
    {
        health -= damage;
        if (health < 0)
        {
            TransitionToState(EnemyState.Dead);
        } else if (currentState == EnemyState.Wander)
        {
            TransitionToState(EnemyState.Chase);
        }

        if (head)
        {
            bodySounds.playSound("headshot");
        } else
        {
            bodySounds.playSound("bodyshot");
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

            float angleToTarget = Vector3.Angle(eye.transform.forward, direction);
            if (angleToTarget > fov / 2f)
            {
                continue;
            }

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

    private Vector3 canSeeHead()
    {
        foreach (Transform transform in playerRaycastNodes.GetComponentsInChildren<Transform>())
        {
            Vector3 eyePos = eye.transform.position;
            Vector3 nodePos = transform.position;
            Vector3 direction = nodePos - eyePos;

            RaycastHit hitData;
            if (Physics.Raycast(eyePos, direction, out hitData, Mathf.Infinity))
            {
                if (hitData.collider.gameObject.name.Equals("Head"))
                {
                    return direction;
                }
            }
        }
        return Vector3.zero;
    }

    private Vector3 canSeeTorso()
    {
        foreach (Transform transform in playerRaycastNodes.GetComponentsInChildren<Transform>())
        {
            Vector3 eyePos = eye.transform.position;
            Vector3 nodePos = transform.position;
            Vector3 direction = nodePos - eyePos;

            RaycastHit hitData;
            if (Physics.Raycast(eyePos, direction, out hitData, Mathf.Infinity))
            {
                if (hitData.collider.gameObject.name.Equals("Torso"))
                {
                    return direction;
                }
            }
        }
        return Vector3.zero;
    }

    private float chanceOfHitting(float distance)
    {
        if (range < distance)
        {
            return 0;
        }

        if (baseInaccuracy == 0)
        {
            return 1;
        }

        //area that bullet could potentially hit
        float largeArea = Mathf.PI * Mathf.Pow(distance * Mathf.Tan(baseInaccuracy * Mathf.Deg2Rad), 2);
        float headArea = Mathf.PI * Mathf.Pow(0.3f, 2);
        return Mathf.Min(1, headArea / largeArea);
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
        baseInaccuracy = weaponInfo.getWeaponStats(weapon, weaponInfo.isWeaponScoped(weapon)).GetValueOrDefault("baseInaccuracy", 0);
        range = weaponStats.GetValueOrDefault("range", 0);
        damage = weaponStats.GetValueOrDefault("damage", 0);
        fireCooldown = weaponStats.GetValueOrDefault("fireCooldown", Mathf.Infinity);

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
                weaponCategory = "Pistol";
                break;
            case "Glock-18":
                Glock18.SetActive(true);
                weaponCategory = "Pistol";
                break;
            case "P250":
                P250.SetActive(true);
                weaponCategory = "Pistol";
                break;
            case "P2000":
                P2000.SetActive(true);
                weaponCategory = "Pistol";
                break;
            case "Dual Berettas":
                DualBerettas.SetActive(true);
                weaponCategory = "Pistol";
                break;
            case "Five-SeveN":
                FiveSeveN.SetActive(true);
                weaponCategory = "Pistol";
                break;
            case "Tec-9":
                Tec9.SetActive(true);
                weaponCategory = "Pistol";
                break;
            case "CZ75-Auto":
                CZ75Auto.SetActive(true);
                weaponCategory = "Pistol";
                break;
            case "Desert Eagle":
                DesertEagle.SetActive(true);
                weaponCategory = "Pistol";
                break;
            case "R8 Revolver":
                R8Revolver.SetActive(true);
                weaponCategory = "Pistol";
                break;
            default:
                R8Revolver.SetActive(true);
                weaponCategory = "Pistol";
                break;
        }
    }

    private void shoot(Vector3 direction)
    {
        float bullets = weaponCategory.Equals("Shotgun") ? 8 : 1;

        playShootWeaponSound();
        for (int i = 0; i < bullets; i++)
        {
            Quaternion randomInaccuracy = Quaternion.Euler(
                Random.value * 2 * baseInaccuracy - baseInaccuracy,
                Random.value * 2 * baseInaccuracy - baseInaccuracy,
                Random.value * 2 * baseInaccuracy - baseInaccuracy
            );
            RaycastHit hitData;
            Vector3 shootDirection = (randomInaccuracy * direction).normalized;
            Vector3 origin = eye.transform.position;
            Vector3 endPoint = origin + shootDirection * range;
            if (Physics.Raycast(origin, shootDirection, out hitData, range))
            {
                endPoint = hitData.point;

                GameObject objectHit = hitData.collider.gameObject;

                if (objectHit.CompareTag("Player"))
                {
                    float headshotMult = weaponInfo.getWeaponStats(currWeapon, false).GetValueOrDefault("headshotMultiplier", 0);
                    playerScript.takeDamage(gameObject, damage * (objectHit.name.Equals("Head") ? headshotMult : 1), objectHit.name.Equals("Head"));
                } else
                {
                    GameObject newBulletHole = Instantiate(bulletHolePrefab);
                    newBulletHole.transform.position = hitData.point;
                    newBulletHole.transform.rotation = Quaternion.LookRotation(hitData.normal);
                    newBulletHole.transform.rotation *= Quaternion.Euler(90, 0, 0);
                    newBulletHole.transform.SetParent(objectHit.transform);
                    StartCoroutine(fadeBulletHole(newBulletHole));
                }
            }

            GameObject tracer = Instantiate(bulletTracerPrefab);
            BulletTracer tracerScript = tracer.GetComponent<BulletTracer>();
            float tracerLife = 0.12f;
            tracerScript.Initialize(muzzlePosition.transform.position, endPoint, tracerLife);
        }
    }

    private void playShootWeaponSound()
    {
            if (currWeapon == "USP-S")
            {
                shootSounds.playSound("USPSShoot");
            } else if (currWeapon == "AWP")
            {
                shootSounds.playSound("AWPShoot");
            }
            else if (currWeapon == "SSG 08")
            {
                shootSounds.playSound("SSG08Shoot");
            }
            else
            {
                switch (weaponCategory)
                {
                    case "Shotgun":
                        shootSounds.playSound("DefaultShotgunShoot");
                        break;
                    case "SMG":
                        shootSounds.playSound("DefaultSMGShoot");
                        break;
                    case "AssaultRifle":
                        shootSounds.playSound("DefaultARShoot");
                        break;
                    case "SniperRifle":
                        shootSounds.playSound("SCAR20Shoot");
                        break;
                    case "MachineGun":
                        shootSounds.playSound("DefaultMGShoot");
                        break;
                    case "Pistol":
                        shootSounds.playSound("DefaultPistolShoot");
                        break;
                }
            }
    }

    private Vector3 GetGroundedPlayerPosition()
    {
        RaycastHit hit;
        Vector3 playerPos = player.transform.position;

        // Raycast down from player position
        if (Physics.Raycast(playerPos, Vector3.down, out hit, 10f))
        {
            return hit.point;
        }

        // Fallback to player position if raycast fails
        return playerPos;
    }

    private const float bulletDecayTime = 5f;
    private IEnumerator fadeBulletHole(GameObject bulletHole)
    {
        if (bulletHole == null)
        {
            yield break;
        }
        float t = 0;
        MeshRenderer renderer = bulletHole.GetComponent<MeshRenderer>();
        if (renderer == null)
        {
            yield break;
        }

        Color color = renderer.material.color;
        while (t < bulletDecayTime)
        {
            if (bulletHole == null || renderer == null)
            {
                yield break;
            }
            float normalizedT = t / bulletDecayTime;

            renderer.material.color = new Color(color.r, color.g, color.b, 1 - normalizedT);

            t += Time.deltaTime;
            yield return null;
        }
        Destroy(bulletHole);
        yield return null;
    }

    Vector3 currTarget;
    private const float minDistanceToWanderNodeThreshold = 5.0f;
    
    private void HandleWanderState()
    {
        if (currWanderLocations.Count == 0)
        {
            initializeWanderLocations();
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position); 
        if (canSeePlayer() || 
            ((playerMovementScript.getIsMakingSprintingNoise() || weaponScript.getIsMakingNoise()) 
                && distanceToPlayer <= hearingDistance)
            )
        {
            if (chanceOfHitting(distanceToPlayer) >= acceptableChanceOfHitting)
            {
                TransitionToState(EnemyState.Attack);
            } else
            {
                TransitionToState(EnemyState.Chase);
            }
            return;
        }

        if (Vector3.Distance(transform.position, currTarget) < minDistanceToWanderNodeThreshold)
        {
            currTarget = currWanderLocations.Pop();
        }

        enemy.SetDestination(currTarget);
    }

    private void HandlePatrolState()
    {

    }

    private void HandleChaseState()
    {
        float minDistanceToPlayerThreshold = range * 0.75f;
        float maxChaseTime = 10f;

        if (stateTimer > maxChaseTime)
        {
            TransitionToState(EnemyState.Wander);
        }

        //always chase the player while it can see the player
        if (canSeePlayer())
        {
            stateTimer = 0;
        }

        float distance = Vector3.Distance(player.transform.position, transform.position);

        if (canSeePlayer() && chanceOfHitting(distance) > acceptableChanceOfHitting)
        {
            TransitionToState(EnemyState.Attack);
            return;
        }
        enemy.SetDestination(GetGroundedPlayerPosition());
    }

    private void HandleAttackState()
    {
        float minDistanceToPlayerThreshold = range * 0.7f;
        float distance = Vector3.Distance(player.transform.position, transform.position);

        if (!canSeePlayer() || chanceOfHitting(distance) < acceptableChanceOfHitting * 0.6f)
        {
            TransitionToState(EnemyState.Chase);
            return;
        }

        if (distance > minDistanceToPlayerThreshold)
        {
            enemy.SetDestination(GetGroundedPlayerPosition());
        } else
        {
            enemy.SetDestination(transform.position);
        }

        //rotate bot to face player
        Vector3 directionToPlayer = player.transform.position - transform.position;
        directionToPlayer.y = 0; // keep rotation on horizontal plane only
        if (directionToPlayer != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = targetRotation;
        }

        //shoot
        if (stateTimer >= fireCooldown * fireCooldownMult)
        {
            Vector3 headDirection = canSeeHead();
            Vector3 torsoDirection = canSeeTorso();
            float chance = Random.Range(0f, 1f);
            if (chance < chanceOfAimingForHead && headDirection != Vector3.zero)
            {
                shoot(Vector3.Lerp(headDirection, playerHead.transform.position - eye.transform.position, 0.1f));
            }
            else if (chance - chanceOfAimingForHead < chanceOfAimingForBody && torsoDirection != Vector3.zero)
            {
                shoot(Vector3.Lerp(torsoDirection, playerBody.transform.position - eye.transform.position, 0.1f));
            } else
            {
                int randomDirection = chance < 0.5f ? 1 : -1;
                shoot((randomDirection == 1 ? torsoDirection : headDirection) + Random.Range(0f, 0.4f) * randomDirection * Vector3.Cross(torsoDirection, Vector3.up));
            }
                stateTimer = 0;
        }
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
                enemy.updateRotation = true;
                initializeWanderLocations();
                enemy.SetDestination(currTarget);
                enemy.speed = defaultMoveSpeed;
                break;
            case EnemyState.Chase:
                enemy.updateRotation = true;
                enemy.SetDestination(GetGroundedPlayerPosition());
                enemy.speed = 2.0f * defaultMoveSpeed;
                break;
            case EnemyState.Attack:
                enemy.updateRotation = false;
                stateTimer = -reactionTime;
                enemy.SetDestination(transform.position);
                enemy.speed = 1.5f * defaultMoveSpeed;
                break;
            case EnemyState.Dead:
                if (!dead)
                {
                    dead = true;
                    StartCoroutine(handleDeath());
                }
                break;
            default:
                break;
        }
    }

    private void OnStateExit(EnemyState state)
    {
        if (state == EnemyState.None)
        {
            return;
        }

        enemy.isStopped = true;
        enemy.ResetPath();
        enemy.SetDestination(enemy.transform.position);
        enemy.isStopped = false;
        switch (state) {
            case EnemyState.Wander:
                break;
            case EnemyState.Chase:
                break;
            case EnemyState.Attack:
                break;
            case EnemyState.Dead:
                break;
            default:
                break;
        }
    }


    private IEnumerator handleFootstepSounds()
    {
        float t = 0;
        while (true)
        {
            if (t > 0.75f && enemy.velocity.magnitude > 0.1f && !isJumping)
            {
                bodySounds.playSound("footsteps");
                t = 0;
            }
            t += Time.deltaTime;
            yield return null;
        }
    }


    private const float dyingTime = 1f;
    private IEnumerator handleDeath()
    {
        levelScript.updateEnemyCount(-1);
        bodySounds.playSound("death");
        MeshRenderer[] childRenderers = GetComponentsInChildren<MeshRenderer>();
        Color originalColor = childRenderers[0].material.color;
        float r = originalColor.r;
        float g = originalColor.g;
        float b = originalColor.b;

        float t = 0;
        float endTime = Mathf.Max(dyingTime, bodySounds.getLength("death"));
        while (t < endTime)
        {
            t += Time.deltaTime;

            float fraction = t / endTime;
            foreach (MeshRenderer rend in childRenderers)
            {
                rend.material.color = new Color(r, g, b, 1 - fraction);
            }
            yield return null;
        }
        Destroy(gameObject);
        yield return null;
    }

    //0 is the easiest
    private void setDifficulty(int difficulty)
    {
        switch (difficulty)
        {
            //story
            //Bots move like snails. They are fully deaf and have extreme tunnel vision. Their reaction time and attack speed is slower than a sloth's, and they always miss.
            case 0:
                fov = 60f;
                hearingDistance = 0f;
                reactionTime = 2.0f;
                fireCooldownMult = 3f;
                chanceOfAimingForHead = 0.0f;
                chanceOfAimingForBody = 0.0f;
                acceptableChanceOfHitting = 0.4f;
                defaultMoveSpeed = 4.0f;
                break;
            //baby
            //These bots move like babies. They are moderately deaf and have tunnel vision. Their reaction time and attack speed is like a newborn's, and we'd be able to travel to other stars before they could land a hit on you.
            case 1:
                fov = 90f;
                hearingDistance = 5f;
                reactionTime = 1.0f;
                fireCooldownMult = 2f;
                chanceOfAimingForHead = 0.05f;
                chanceOfAimingForBody = 0.1f;
                acceptableChanceOfHitting = 0.4f;
                defaultMoveSpeed = 6.0f;
                break;
            //easy
            //The bots are obese. They have slight hearing issues and have slight tunnel vision. Their reaction time and attack speed is sluggish and that of a casual gamer with casual aim.
            case 2:
                fov = 150f;
                hearingDistance = 10f;
                reactionTime = 0.75f;
                fireCooldownMult = 1.5f;
                chanceOfAimingForHead = 0.1f;
                chanceOfAimingForBody = 0.2f;
                acceptableChanceOfHitting = 0.4f;
                defaultMoveSpeed = 7.0f;
                break;
            //normal
            //These bots are your average well-rounded teenager with a slow 100-meter dash of 20 seconds, average hearing, sub-par field of vision, slightly slow reaction time, normal-speed clicking, and bad aim.
            case 3:
                fov = 180f;
                hearingDistance = 15f;
                reactionTime = 0.50f;
                fireCooldownMult = 1.5f;
                chanceOfAimingForHead = 0.3f;
                chanceOfAimingForBody = 0.3f;
                acceptableChanceOfHitting = 0.4f;
                defaultMoveSpeed = 8.0f;
                break;
            //hard
            //Tryhard bots that run fast. They have a field of vision akin to a human's with great hearing, average reaction time, fast clicking, and okay aim.
            case 4:
                fov = 210f;
                hearingDistance = 25f;
                reactionTime = 0.40f;
                fireCooldownMult = 1.25f;
                chanceOfAimingForHead = 0.5f;
                chanceOfAimingForBody = 0.3f;
                acceptableChanceOfHitting = 0.4f;
                defaultMoveSpeed = 10.0f;
                break;
            //expert
            //Be careful because these bots are olympic sprinters that can see up to 45 degrees behind them. Their hearing is comparable to a bat's, and they have a gamer's reaction time. They shoot almost as fast as possible, and their aim is so good they will always hit you.
            case 5:
                fov = 270f;
                hearingDistance = 35f;
                reactionTime = 0.25f;
                fireCooldownMult = 1.1f;
                chanceOfAimingForHead = 0.5f;
                chanceOfAimingForBody = 0.5f;
                acceptableChanceOfHitting = 0.4f;
                defaultMoveSpeed = 15.0f;
                break;
            //pro
            //You'd better give up because these bots play at the level of professional e-sports. They can run at the speed that an olympic biker cycles. Their only blind spot is directly behind them, and they have hearing comparable to a sonar, and they have a superhuman reaction time. They shoot as fast as the weapon is physically able to, and their aim is so great you'd flip 4 heads in a row before they hit your body instead of your head.
            case 6:
                fov = 330f;
                hearingDistance = 100f;
                reactionTime = 0.1f;
                fireCooldownMult = 1.0f;
                chanceOfAimingForHead = 0.9f;
                chanceOfAimingForBody = 0.1f;
                acceptableChanceOfHitting = 0.4f;
                defaultMoveSpeed = 20.0f;
                break;
            //aimbot
            //These bots come from a superintelligent alien species that can see with a 360-degree field of vision. They break special relativity by moving at the speed of light. They can hear you from the other edge of the unobservable universe. Their reaction time is instantaneous (faster than light). They felt the mechanical restrictions of weapons were too much, so they physically throw bullets at you with a frequency comparable to vibration of atoms. They are cruel beings, so they will always headshot you.
            case 7:
                fov = 359.99f;
                hearingDistance = 1000f;
                reactionTime = 0f;
                fireCooldownMult = 0.01f;
                chanceOfAimingForHead = 1f;
                chanceOfAimingForBody = 0f;
                acceptableChanceOfHitting = 0.6f;
                defaultMoveSpeed = 100.0f;
                break;
        }
    }
}

