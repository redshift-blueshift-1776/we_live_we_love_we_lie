using UnityEngine;
using UnityEngine.AI;

public class NPC_Movement : MonoBehaviour
{
    public NavMeshAgent agent;

    [SerializeField] private GameObject locations;

    [SerializeField] private Transform[] wanderPoints;

    [SerializeField] private float waitTime = 4f;

    public int currentTarget;
    public float waitTimer;

    void Start()
    {
        locations.transform.SetParent(null);
        agent = GetComponent<NavMeshAgent>();
        PickNextPoint();
    }

    void Update()
    {
        if (agent.pathPending)
        {
            Debug.Log("agent.pathPending");
            return;
        }

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTime)
            {
                Debug.Log("waitTimer >= waitTime");
                PickNextPoint();
                waitTimer = 0;
            }
        }
    }

    void PickNextPoint()
    {
        if (wanderPoints.Length == 0)
        {
            Debug.Log("wanderPoints.Length == 0");
            return;
        }

        currentTarget = Random.Range(0, wanderPoints.Length);
        Debug.Log("currentTarget: " + currentTarget);

        agent.SetDestination(wanderPoints[currentTarget].position);
    }
}