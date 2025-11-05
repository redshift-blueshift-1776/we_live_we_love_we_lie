using UnityEngine;
using UnityEngine.AI;
public class Enemy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private NavMeshAgent enemy;
    [SerializeField] private GameObject player;
    public enum EnemyState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Dead
    }

    float health = 100;
    private EnemyState currentState;
    private float stateTimer = 0f;
    void Start()
    {
        enemy.isStopped = true;
    }

    // Update is called once per frame
    void Update()
    {
        enemy.SetDestination(player.transform.position);
    }

    public void takeDamage(float damage)
    {
        health -= damage;
        if (health < 0)
        {
            Destroy(gameObject);
        }
    }

    private void TransitionToState(EnemyState newState)
    {
        // Exit current state
        OnStateExit(currentState);

        // Enter new state
        currentState = newState;
        stateTimer = 0f;
        OnStateEnter(newState);
    }

    private void OnStateEnter(EnemyState state)
    {
        switch (state)
        {
            //case EnemyState.Patrol:
            //    SetNewPatrolPoint();
            //    break;
            //case EnemyState.Attack:
            //    Debug.Log("Entering attack state");
            //    break;
        }
    }

    private void OnStateExit(EnemyState state)
    {
        // Cleanup when leaving a state
    }
}
