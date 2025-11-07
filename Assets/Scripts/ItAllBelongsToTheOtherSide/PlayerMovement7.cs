using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

public class PlayerMovement7 : MonoBehaviour
{
    [SerializeField] private Level7 levelScript;
    [SerializeField] private AudioManager audioManager;

    [SerializeField] private GameObject playerCamera;
    private CharacterController characterController;
    [SerializeField] private GameObject body;

    [SerializeField] private GameSettings gameSettings;
    [SerializeField] private Player7 player7;
    [SerializeField] private Weapon weaponScript;

    private const float m = 1.0f;   //mass
    private const float g = 9.8f;

    private const float staticCoefficientOfFriction = 2.5f;
    private const float kineticCoefficientOfFriction = 2.4f;

    private float airResistanceVerticalCoefficient = 0.25f;
    private float airResistanceHorizontalCoefficient = 0.1f;

    private float minHorizontalComponentVelocityThreshold = 0.0001f;

    private const float walkForceMagnitude = 50f;
    private const float runForceMagnitude = 100f;

    private const float airAccelerationMultiplier = 0.6f;
    private const float airForceMagnitude = 60f;

    private const float bhopSpeedRetention = 0.95f;
    private const float landingSpeedRetention = 0.7f;
    private const float bhopTimingWindow = 0.1f;
    private float timeSinceGrounded = 0f;
    private bool wasGroundedLastFrame = false;

    private const float maxWalkSpeed = 5f;
    private const float maxRunSpeed = 7f;
    private const float maxAirSpeed = 15f;

    private const float maxJumpHeight = 1.6f;

    private float sensitivityX = 2.5f;
    private float sensitivityY = 2f;

    public Slider sliderX;
    public Slider sliderY;

    private float yaw;
    private float pitch;

    private Vector3 velocity = Vector3.zero;
    private bool isGrounded = false;
    private bool isSprinting = false;

    private bool inUIMenu = false;
    
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yaw = playerCamera.transform.localRotation.eulerAngles.y;
        pitch = playerCamera.transform.localRotation.eulerAngles.x;
        while (Mathf.Abs(yaw) > 180f)
        {
            yaw -= Mathf.Sign(yaw) * 360f;
        }

        StartCoroutine(handleFootstepSounds());
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = checkGrounded();
        inUIMenu = gameSettings.isInSettings() || player7.getIsInWeaponShop();
        if (!inUIMenu && !levelScript.getPlayerDead())
        {
            rotateCamera();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isSprinting = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isSprinting = false;
        }

        handleSensitivityChange();
    }

    private void handleSensitivityChange()
    {
        sensitivityX = sliderX.value;
        sensitivityY = sliderY.value;
    }

    private void FixedUpdate()
    {
        if (levelScript.getPlayerDead())
        {
            return;
        }
        if (isGrounded)
        {
            timeSinceGrounded = timeSinceGrounded + (!wasGroundedLastFrame ? 0f : Time.fixedDeltaTime);
        } 

        Vector3 netForce = getNetForce();

        Vector3 frictionForce = getGroundFrictionForce();

        Vector3 acceleration = getAcceleration(netForce);

        velocity += acceleration * Time.fixedDeltaTime;
        
        handleHorizontalVelocityChanges(frictionForce);

        movePlayer();
        handleVerticalVelocityChanges();
    }

    private Vector3 getNetForce()
    {
        return getVerticalForce() + getHorizontalForce();
    }

    private Vector3 getAcceleration(Vector3 netForce)
    {
        return new Vector3(netForce.x / m, netForce.y / m, netForce.z / m);
    }


    private Vector3 getVerticalForce()
    {
        Vector3 verticalForce = Vector3.zero;

        Vector3 gravityForce = Vector3.zero;

        if (!isGrounded) {
            gravityForce.y = -m * g;
        }

        Vector3 airResistanceForce = Vector3.zero;
        airResistanceForce.y = -airResistanceVerticalCoefficient * velocity.y;

        verticalForce = gravityForce + airResistanceForce;


        return verticalForce;
    }

    private float getInitialJumpVelocity()
    {
        float k = airResistanceVerticalCoefficient;
        float h = maxJumpHeight;

        /** from solving first order linear differential equation v' + kv/m = -g through integrating factor method:
         *  f(v_0) = v_0 - mg/k * ln(1 + kv_0/(mg)) - kh/m
         *  f'(v_0) = 1 - mg / (mg + kv_0)
         *  initial upward velocity is sqrt(2mgh) without air resistance; use as initial guess
         *  apply newton's method of approximation
         */
        float v0 = Mathf.Sqrt(2 * m * g * h);
        const float iterations = 3;
        for (int i = 0; i < iterations; i++)
        {
            float f = v0 - (m * g / k) * Mathf.Log(1 + k * v0 / (m * g)) - k * h / m;
            float fPrime = 1 - m * g / (m * g + k * v0);
            v0 = v0 - f / fPrime;
        }

        return v0;
    }

    private Vector3 getHorizontalForce()
    {
        Vector3 horizontalForce = Vector3.zero;

        Vector3 externalForce = getExternalForce();

        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);

        float maxSpeedMult = weaponScript.getMovementMultiplier();
        if (isGrounded)
        {
            
            bool passingMaxSpeed = (isSprinting && horizontalVelocity.magnitude > maxRunSpeed * maxSpeedMult)
                                    || (!isSprinting && horizontalVelocity.magnitude > maxWalkSpeed * maxSpeedMult);

            if (!passingMaxSpeed)
            {
                horizontalForce += externalForce;
            }

            horizontalForce += getGroundFrictionForce();
        } else
        {
            if (horizontalVelocity.magnitude < maxAirSpeed * maxSpeedMult)
            {
                horizontalForce += getAirStrafeForce(externalForce, horizontalVelocity);
            }

            horizontalForce += getAirResistanceForce();
        }

        return horizontalForce;
    }

    private Vector3 getExternalForce()
    {
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        Vector3 externalForce = Vector3.zero;

        float multiplier = weaponScript.getMovementMultiplier();
        Vector3 inputDirection = getInputDirectionVector() * (isSprinting ? runForceMagnitude : walkForceMagnitude) * multiplier;
        externalForce.x = inputDirection.x;
        externalForce.z = inputDirection.z;

        return externalForce;
    }

    private Vector3 getGroundFrictionForce()
    {
        Vector3 frictionForce = new Vector3(-sign(velocity.x), 0, -sign(velocity.z));
        bool isEffectivelyMoving = Mathf.Abs(velocity.x) >= minHorizontalComponentVelocityThreshold
            && Mathf.Abs(velocity.z) >= minHorizontalComponentVelocityThreshold;
        float frictionForceComponent = (isEffectivelyMoving ? kineticCoefficientOfFriction : staticCoefficientOfFriction) * m * g;
        frictionForce.x *= frictionForceComponent;
        frictionForce.z *= frictionForceComponent;

        return frictionForce;
    }

    private Vector3 getAirResistanceForce()
    {
        Vector3 frictionForce = new Vector3(-sign(velocity.x), 0, -sign(velocity.z));
        float frictionForceComponent = airResistanceHorizontalCoefficient * new Vector3(velocity.x, 0, velocity.z).magnitude;
        frictionForce.x *= frictionForceComponent;
        frictionForce.z *= frictionForceComponent;

        return frictionForce;
    }

    private Vector3 getAirStrafeForce(Vector3 inputForce, Vector3 currentVelocity)
    {
        if (inputForce.magnitude < 0.01f)
        {
            return Vector3.zero;
        }

        Vector3 airForce = inputForce.normalized * airForceMagnitude;
        Vector3 direction = inputForce.normalized;

        float alignment = Vector3.Dot(currentVelocity, direction);

        if (alignment > 0.85f && currentVelocity.magnitude > 1f)
        {
            return Vector3.zero;
        }

        return direction * airForceMagnitude * airAccelerationMultiplier;
    }

    private void handleVerticalVelocityChanges()
    {
        if (characterController.isGrounded)
        {
            if (!wasGroundedLastFrame)
            {
                if (timeSinceGrounded < bhopTimingWindow)
                {
                    velocity.x *= bhopSpeedRetention;
                    velocity.z *= bhopSpeedRetention;
                } else
                {
                    velocity.x *= landingSpeedRetention;
                    velocity.z *= landingSpeedRetention;
                }
            }
            velocity.y = 0;
        }

        wasGroundedLastFrame = isGrounded;
        handleJump();
    }

    private void handleHorizontalVelocityChanges(Vector3 frictionForce)
    {
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);

        if (Vector3.Dot(horizontalVelocity, frictionForce) > 0)
        {
            velocity.x = 0;
            velocity.z = 0;
        }

        float maxSpeed = (isGrounded
        ? (isSprinting ? maxRunSpeed : maxWalkSpeed)
        : maxAirSpeed) * weaponScript.getMovementMultiplier();
        if (horizontalVelocity.magnitude > maxSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
            velocity.x = horizontalVelocity.x;
            velocity.z = horizontalVelocity.z;
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

    private void handleJump()
    {
        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            isGrounded = false;
            velocity.y = getInitialJumpVelocity();
            timeSinceGrounded = 0f;
        }
    }

    private void movePlayer()
    {
        characterController.Move(velocity * Time.deltaTime);
    }

    
    private void rotateCamera()
    {
        float zoomSlowdownFactor = weaponScript.getScoped() ? 0.33f : 1;
        float mouseX = Input.GetAxis("Mouse X") * sensitivityX * zoomSlowdownFactor;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY * zoomSlowdownFactor;

        yaw += mouseX;
        pitch -= mouseY; //inverted
        pitch = Mathf.Clamp(pitch, -90f, 90f);

        transform.localRotation = Quaternion.Euler(0, yaw, 0);
        playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0, 0);
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

    private bool isMakingNoise = false;
    private IEnumerator handleFootstepSounds()
    {
        float t = 0;
        audioManager.setVolume("footsteps", 0.25f);
        while (true)
        {
            if (velocity.magnitude > 0.1f && isGrounded && isSprinting)
            {
                if (t > 0.75f)
                {
                    audioManager.playSound("footsteps");
                    t = 0;
                }
                isMakingNoise = true;
            } else
            {
                isMakingNoise = false;
            }
            t += Time.deltaTime;
            yield return null;
        }
    }
    public bool getIsMakingSprintingNoise()
    {
        return isMakingNoise;
    }

    public Vector3 getVelocity()
    {
        return velocity;
    }

    public float getCurrentMaxSpeed()
    {
        if (isGrounded)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                return maxRunSpeed;
            } else
            {
                return maxWalkSpeed;
            }
        } 

        return Mathf.Infinity;
    }

    public bool checkGrounded()
    {
        float xWidth = body.transform.localScale.x;
        float yWidth = 2 * body.transform.localScale.y;
        float zWidth = body.transform.localScale.z;

        float initX = body.transform.position.x - xWidth / 2;
        float initY = body.transform.position.y - yWidth / 2;
        float initZ = body.transform.position.z - zWidth / 2;
        for (float x = 0; x <= xWidth; x += xWidth / 4)
        {
            for (float z = 0; z <= zWidth; z += zWidth / 4)
            {
                Vector3 origin = new Vector3(initX + x, initY, initZ + z);
                if (Physics.Raycast(origin, Vector3.down, 0.2f))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void setInUIMenu(bool b)
    {
        inUIMenu = b;
    }
}
