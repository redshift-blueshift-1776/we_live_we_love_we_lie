using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3 startPosition;
    public Vector3 targetPosition;
    [SerializeField] public float speed = 5.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = startPosition;
        transform.LookAt(targetPosition);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
