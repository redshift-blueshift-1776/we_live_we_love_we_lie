using UnityEngine;

public class Open_Exploration_Scooter : MonoBehaviour
{
    [SerializeField] public GameObject scooterMountSound;
    [SerializeField] public bool usingThis;
    [SerializeField] public BoxCollider thisCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        usingThis = false;
        scooterMountSound.SetActive(false);
        thisCollider.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Mount(GameObject g) {
        Debug.Log("Mounting Scooter");
        scooterMountSound.SetActive(false);
        scooterMountSound.SetActive(true);
        if (!usingThis)
        {
            usingThis = true;
            transform.SetParent(g.transform);
            transform.SetLocalPositionAndRotation(new(0, -0.5f, 0), Quaternion.identity);
            transform.localScale = new(1, 0.25f, 1);
            thisCollider.enabled = false;
            if (g.TryGetComponent<Player_Movement_Open_Exploration>(out var pmoe))
            {
                pmoe.vehicle = Player_Movement_Open_Exploration.OpenExplorationVehicle.Scooter;
            }
        }
    }

    public void Dismount(GameObject g) {
        Debug.Log("Dismounting Scooter");
        scooterMountSound.SetActive(false);
        scooterMountSound.SetActive(true);
        if (usingThis)
        {
            usingThis = false;
            transform.SetParent(null);
            // transform.SetLocalPositionAndRotation(new(0, -0.5f, 0), Quaternion.identity);
            thisCollider.enabled = true;
            g.transform.rotation = Quaternion.Euler(0, 0, 0);
            if (g.TryGetComponent<Player_Movement_Open_Exploration>(out var pmoe))
            {
                pmoe.vehicle = Player_Movement_Open_Exploration.OpenExplorationVehicle.Walking;
            }
        }
    }
}
