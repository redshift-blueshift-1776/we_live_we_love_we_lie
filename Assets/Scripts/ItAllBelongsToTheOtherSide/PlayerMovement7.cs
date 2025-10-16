using UnityEngine;

public class PlayerMovement7 : MonoBehaviour
{
    [SerializeField] private GameObject playerCamera;
    private CharacterController characterController;

    private const float m = 1.0f;   //mass
    private const float g = 9.8f;
    private const float gForce = m * g;

    private const float staticCoefficientOfFriction = 0.75f;
    private const float kineticCoefficientOfFriction = 0.7f;

    private float airResistanceCoefficient = 1f;

    private float minHorizontalComponentVelocityThreshold = 0.0001f;

    private const float moveForceMagnitude = 30f;

    private const float maxSpeed = 3f;

    private Vector3 velocity;
    private bool isGrounded;

    
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }
    void Start()
    {
        velocity = Vector3.zero;
        isGrounded = false;


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Vector3 netForce = getNetForce();

        Vector3 frictionForce = getFrictionForce();

        Vector3 acceleration = getAcceleration(netForce);

        velocity += acceleration * Time.fixedDeltaTime;

        handleVerticalVelocityConstraints();
        handleHorizontalVelocityConstraints(frictionForce);

        movePlayer();
    }

    private Vector3 getNetForce()
    {
        return getVerticalForce() + getHorizontalForce();
    }

    private Vector3 getAcceleration(Vector3 netForce)
    {
        return new Vector3(netForce.x / m, netForce.y / m, netForce.z / m);
    }


    //TO-DO: modify for jumping
    private Vector3 getVerticalForce()
    {
        Vector3 verticalForce = Vector3.zero;
        verticalForce.y = -m * g + airResistanceCoefficient * velocity.y;

        return verticalForce;
    }

    private Vector3 getHorizontalForce()
    {
        Vector3 horizontalForce = Vector3.zero;

        Vector3 externalForce = getExternalForce();
        Vector3 frictionForce = getFrictionForce();

        horizontalForce = externalForce;
        if (isGrounded)
        {
            horizontalForce += frictionForce;
        }

        return horizontalForce;
    }

    private Vector3 getExternalForce()
    {
        Vector3 externalForce = Vector3.zero;
        Vector3 inputDirection = getInputDirectionVector() * moveForceMagnitude;
        externalForce.x = inputDirection.x;
        externalForce.z = inputDirection.z;

        return externalForce;
    }

    private Vector3 getFrictionForce()
    {
        Vector3 frictionForce = new Vector3(-sign(velocity.x), 0, -sign(velocity.z));
        bool isEffectivelyMoving = Mathf.Abs(velocity.x) >= minHorizontalComponentVelocityThreshold
            && Mathf.Abs(velocity.z) >= minHorizontalComponentVelocityThreshold;
        float frictionForceComponent = (isEffectivelyMoving ? kineticCoefficientOfFriction : staticCoefficientOfFriction) * m * g;
        frictionForce.x *= frictionForceComponent;
        frictionForce.z *= frictionForceComponent;

        return frictionForce;
    }

    private void handleVerticalVelocityConstraints()
    {
        CollisionFlags flags = characterController.collisionFlags;
        if ((flags & CollisionFlags.Below) != 0)
        {
            velocity.y = 0;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void handleHorizontalVelocityConstraints(Vector3 frictionForce)
    {
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);

        if (horizontalVelocity.magnitude > maxSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
            velocity.x = horizontalVelocity.x;
            velocity.z = horizontalVelocity.z;
        }

        if (Vector3.Dot(horizontalVelocity, frictionForce) > 0)
        {
            velocity.x = 0;
            velocity.z = 0;
        }

        if (Mathf.Abs(velocity.x) < minHorizontalComponentVelocityThreshold)
        {
            velocity.x = 0;
        }

        if (Mathf.Abs(velocity.z) < minHorizontalComponentVelocityThreshold)
        {
            velocity.z = 0;
        }
    }

    private Vector3 getInputDirectionVector()
    {
        Vector3 direction = Vector3.zero;
        Vector3 forward = new Vector3(playerCamera.transform.forward.x, 0, playerCamera.transform.forward.z);
        Vector3 right = new Vector3(playerCamera.transform.right.x, 0, playerCamera.transform.right.z);

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
        characterController.Move(velocity * Time.deltaTime);
    }

    private int sign(float x)
    {
        if (x < 0)
        {
            return -1;
        }
        else if (x > 0)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}
