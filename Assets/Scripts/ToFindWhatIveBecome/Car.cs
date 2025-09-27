using UnityEngine;

public class Car : MonoBehaviour
{
    [SerializeField] public float speed;
    [SerializeField] public Vector3 startPosition;
    [SerializeField] public Vector3 targetPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = startPosition;
        transform.LookAt(targetPosition);
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(targetPosition);
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
