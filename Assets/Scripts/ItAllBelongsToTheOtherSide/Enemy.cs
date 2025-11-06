using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
public class Enemy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private NavMeshAgent enemy;
    [SerializeField] private GameObject player;

    [SerializeField] private GameObject wanderNodes;
    [SerializeField] private GameObject navMeshJumps;
    HashSet<Vector3> wanderLocations = new HashSet<Vector3>();
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
    [SerializeField] private float jumpHeight = 2.0f;
    void Start()
    {
        foreach (Transform child in wanderNodes.transform)
        {
            wanderLocations.Add(child.position);
        }

        currentState = EnemyState.None;
        //TransitionToState(EnemyState.Wander);
        enemy.autoTraverseOffMeshLink = false;
        enemy.SetDestination(player.transform.position);

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
    }

    private void handleMeshLink()
    {
        if (enemy.isOnOffMeshLink && !isJumping)
        {
            OffMeshLinkData linkData = enemy.currentOffMeshLinkData;
            Vector3 startPos = enemy.transform.position;
            Vector3 endPos = linkData.endPos;
            StartCoroutine(PerformJump(startPos, endPos, true));
        }
    }

    private IEnumerator PerformJump(Vector3 startPos, Vector3 endPos, bool meshLinkJump)
    {
        isJumping = true;
        Debug.Log("start jumping!");
        // Get link data


        // Disable NavMesh auto-movement during jump
        enemy.updatePosition = false;

        float elapsed = 0;
        const float g = -9.8f;

        Vector3 velocity = new Vector3(0, Mathf.Sqrt(-2 * g * jumpHeight), 0);
        Vector3 acceleration = new Vector3(0, g, 0);
        float dy = endPos.y - startPos.y;
        float t1 = velocity.y / -g; //time until peak of jump
        float t2 = Mathf.Sqrt(2 * dy / g);
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

        Debug.Log("Jump completed!");
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
