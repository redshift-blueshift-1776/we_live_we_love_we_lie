using UnityEngine;

public class PlayerMovement7 : MonoBehaviour
{
    public GameObject camera;
    
    private CharacterController characterController;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Vector3 getInputDirectionVector()
    {
        Vector3 direction = Vector3.zero;
        Vector3 forward = new Vector3(camera.transform.forward.x, 0, camera.transform.forward.z);
        Vector3 right = new Vector3(camera.transform.right.x, 0, camera.transform.right.y);
        if (Input.GetKey(KeyCode.W))
        {
            direction += forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction -= right;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction -= forward;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += right;
        }
        return direction.normalized;
    }

    private void movePlayer()
    {
        Vector3 direction = getInputDirectionVector();

        CollisionFlags flags = characterController.collisionFlags;
        if ((flags & CollisionFlags.Sides) != 0)
        {

        }
    }
}
