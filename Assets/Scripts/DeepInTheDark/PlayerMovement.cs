using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject body;
    public GameObject settings;

    public AudioManager audioManager;

    private float initialYaw;
    private float initialPitch;

    public Rigidbody playerRigidBody;
    private bool inSettings;

    private float sensitivityX;
    private float sensitivityY;

    public Slider sliderX;
    public Slider sliderY;

    private float g;
    private Vector3 acceleration;
    private Vector3 velocity;

    private const float jumpHeight = 1.5f;

    //values greater than 1 speed the player up!
    private Dictionary<string, float> frictionCoefficients = new Dictionary<string, float>
    {
        {"Default", 0.97f},
        {"Air", 0.995f },
        {"Ladder", 0.1f },
        {"Slime", 0.9f  },
        {"Ice", 1.05f }
    };
    private float currentFrictionCoefficient;


    private const float elasticEnergyExponentialDecayCoefficient = 0.125f;

    private const float minSpeed = 0.00001f;
    private const float minElasticCollisionVelocity = 1f;
    private const float maxElasticCollisionConservation = 0.8f;
    private const float ceilingReboundVelocity = -0.25f;

    private float walkingAcceleration = 50f;
    private float runningAcceleration = 100f;

    private const float maxSlowWalkHorizontalSpeed = 3f;
    private const float maxWalkingHorizontalSpeed = 5f;
    private const float maxRunningHorizontalSpeed = 8f;

    private Dictionary<string, float> maxSpeedMultipliers = new Dictionary<string, float>
    {
        {"Default", 1f},
        {"Air", 1f },   //same as default but subject to change
        {"Ladder", 0.25f },
        {"Slime", 0.5f  },
        {"Ice", 2f }
    };
    private float currMaxSpeedMultiplier;


    private Transform currentLadder = null;
    private const float defaultLadderFallingVelocity = -1.5f;   //speed at which player falls with no input on a ladder
    private const float climbingVelocity = 3f;
    private const float maxSidewaysVelocityOnLadder = 2f;     //max sideways speed of the player with respect to the ladder's forward vector

    private float yaw;
    private float pitch;

    private bool userInput;
    private bool isClimbing;
    private bool isJumping;
    //the maximum angled slope the player can walk up (in degrees)
    private const float maxSlopeAngle = 60f;

    private const float rotationSpeed = 10f;

    //stores current ground contact colliders and their normals; see rotatePlayer() for more info
    private Dictionary<Collider, List<Vector3>> groundContactPoints;


    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        //Cursor.visible = false;
        
        //initialYaw = player.transform.rotation.eulerAngles.y;
        initialYaw = playerCamera.transform.localRotation.eulerAngles.y;
        initialPitch = playerCamera.transform.localRotation.eulerAngles.x;
        while (initialYaw > 180f) {
            initialYaw -= 360f;
        }
        while (initialYaw < -180f)
        {
            initialYaw += 360f;
        }


        while (initialPitch > 180f)
        {
            initialPitch -= 360f;
        }
        while (initialPitch < -180f)
        {
            initialPitch += 360f;
        }
        initializeSounds();

        inSettings = false;
        settings.SetActive(inSettings);

        sensitivityX = sliderX.value;
        sensitivityY = sliderY.value;

        g = 9.8f;

        acceleration = new Vector3(0, -g, 0);
        velocity = Vector3.zero;

        currentFrictionCoefficient = frictionCoefficients["Default"];

        currMaxSpeedMultiplier = maxSpeedMultipliers["Default"];

        setPlayerYawPitch(initialYaw, initialPitch);

        userInput = false;
        isClimbing = false;
        isJumping = false;

        groundContactPoints = new Dictionary<Collider, List<Vector3>>();
    }

    void Update()
    {
        handleSettings();
        handleSensitivityChange();
        if (!inSettings)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            movePlayer();
            rotateCamera();
            handleJump();
        } else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void FixedUpdate()
    {
        rotatePlayer();
    }

    private void handleSettings()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            inSettings = !inSettings;
            settings.SetActive(inSettings);
        }
    }

    private void initializeSounds()
    {
        audioManager.setVolume("jump", 0.3f);
    }

    /// <summary>
    /// Rotates the camera based on mouse input.
    /// </summary>
    private void rotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivityX;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY;

        yaw += mouseX;
        pitch -= mouseY; //inverted
        pitch = Mathf.Clamp(pitch, -90f, 90f);

        //player.transform.rotation = Quaternion.Euler(0, yaw, 0);
        //playerRigidBody.MoveRotation(Quaternion.Euler(0, yaw, 0));
        playerCamera.transform.localRotation = Quaternion.Euler(pitch, yaw, 0);
    }

    /// <summary>
    /// Rotates the player model based on the angle of the slope that they are on.
    /// This takes in the average normal vector from all ground contact point normals
    /// that are being tracked in the Dictionary<Collider, List<Vector3>> groundContactPoints
    /// object. 
    /// </summary>
    private void rotatePlayer()
    {
        Vector3 averageNormal = averageVector(groundContactPoints.Values.ToList().SelectMany(l => l).ToList());

        if (groundContactPoints.Count > 0)
        {
            Vector3 forward = Vector3.ProjectOnPlane(body.transform.forward, averageNormal).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(forward, averageNormal);
            body.transform.rotation = Quaternion.Slerp(
                body.transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            //purposefully lock the rotation forward
            body.transform.up = Vector3.Slerp(body.transform.up, averageNormal, rotationSpeed * Time.deltaTime);
        }
    }

    public void setPlayerYawPitch(float yaw, float pitch)
    {
        this.yaw = yaw;
        this.pitch = pitch;
        rotateCamera();
    }

    /// <summary>
    /// Moves the player based on acceleration -> velocity -> position. However, 
    /// the acceleration and velocity are balanced such that max speed is achieved
    /// very quickly to make the movement more fluid.
    /// </summary>
    private void movePlayer()
    {
        if (playerRigidBody == null)
        {
            return;
        }

        bool isRunning = Input.GetKey(KeyCode.LeftShift) ? true : false;

        //change to LeftControl when deploying (Ctrl interferes with Unity editor shortcuts)
        bool isSlowWalking = Input.GetKey(KeyCode.CapsLock) ? true : false;

        if (getInputDirectionVector().magnitude > 0)
        {
            userInput = true;
            Vector3 inputVelocity = getInputDirectionVector();

            inputVelocity *= isRunning ? runningAcceleration : walkingAcceleration;

            //inputAcceleration should have no y component
            velocity += inputVelocity * Time.deltaTime;
        } else
        {
            userInput = false;
        }
        //scale velocity to max
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);

        //scale velocities down to max speeds if they were above
        float speedThreshold = 0f;
        if (isSlowWalking)
        {
            speedThreshold = maxSlowWalkHorizontalSpeed * currMaxSpeedMultiplier;
        } else if (isRunning)
        {
            speedThreshold = maxRunningHorizontalSpeed * currMaxSpeedMultiplier;
        } else
        {
            speedThreshold = maxWalkingHorizontalSpeed * currMaxSpeedMultiplier;
        }
        if (horizontalVelocity.magnitude > speedThreshold)
        {
            horizontalVelocity.Normalize();
            horizontalVelocity *= speedThreshold;
        }
        //update velocities in case they were scaled down
        velocity.x = horizontalVelocity.x;
        velocity.z = horizontalVelocity.z;

        //update vertical velocity
        if (isClimbing)
        {
            handleClimb();
        } else
        {
            velocity.y = (groundContactPoints.Count == 0) ? (velocity.y + acceleration.y * Time.deltaTime) : 0;
        }
        //handles movement on a ramp
        playerRigidBody.MovePosition(playerRigidBody.position + (
            groundContactPoints.Count > 0 && !isClimbing ? Vector3.ProjectOnPlane(velocity, body.transform.up) : velocity
            ) * Time.deltaTime);

        //only apply friction when the player stops giving input (WASD)
        if (!userInput)
        {
            //apply friction to the velocity vector
            Vector3 frictionVector = new Vector3(currentFrictionCoefficient, 1f, currentFrictionCoefficient);
            velocity.Scale(frictionVector);
        }
        velocity.x = Mathf.Abs(velocity.x) >= minSpeed ? velocity.x : 0;
        velocity.z = Mathf.Abs(velocity.z) >= minSpeed ? velocity.z : 0;
    }

    private const float jumpBufferTime = 0.2f; // Window to buffer jump input before landing
    private float jumpBufferTimer = 0f;
    private const float autoJumpTime = 1.5f;
    private float autoJumpTimer = 0f;
    /// <summary>
    /// Removed all ground contact points and sets upward velocity such that
    /// the jump height will be achieved. This is calculated using physics kinematics.
    /// </summary>
    private void handleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferTimer = jumpBufferTime;
        }

        if (jumpBufferTimer > 0)
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            autoJumpTimer += Time.deltaTime;
        }


        bool canCoyoteJump = Input.GetKey(KeyCode.Space) && jumpBufferTimer > 0;
        bool canAutoJump = autoJumpTimer >= autoJumpTime;

        if ((canCoyoteJump || canAutoJump) && groundContactPoints.Count > 0 && !isJumping)
        {
            autoJumpTimer = 0f;
            audioManager.playSound("jump");
            isJumping = true;
            clearGroundContacts();
            velocity.y = Mathf.Sqrt(2 * g * jumpHeight);
        }

        //reset jumping when landing on ground
        if (groundContactPoints.Count > 0)
        {
            isJumping = false;
        }

    }



    public void clearGroundContacts()
    {
        groundContactPoints.Clear();
    }

    /// <summary>
    /// This function handles how the user climbs a ladder, where any possible rotation of the 
    /// ladder (with respect to its axes) is accounted for. Players can also climb both sides of
    /// the ladder if they so wish. If the ladder's slope is not steep enough (past the maxSlopeAngle 
    /// for surface walking/running), then the player will instead walk on the ladder and not climb 
    /// it. Sideways velocity (with respect to the forward vector of the ladder) has been scaled down 
    /// to make it easier for the player to stay on the ladder once climbing.
    /// 
    /// There are a few unfixable bugs:
    /// 
    /// - First, the ladder prefab has a LadderObject collider and a Ladder collider (onTrigger). 
    /// The LadderObject collider's purpose is to stop the player from climbing the ladder from
    /// the side, so it's wider along the ladder's x axis. Entering the Ladder collider (onTrigger) 
    /// is what decides whether the player is climbing or not. For a ladder with a slope smaller 
    /// than maxSlopeAngle, if the player collides with the LadderObject collider before the
    /// Ladder collider (onTrigger), the player will still "fall down" the ladder. For this reason, 
    /// it is best to make sure such flat ladders always have their lower model hidden within the 
    /// floor such that the player could never interact with the LadderObject collider before the 
    /// Ladder collider (onTrigger) (see the geometry in the Prefab Editor for more details). 
    /// Otherwise, such low-sloped should be normal cubes (rotated) rather than ladders. 
    /// 
    /// - Next, when the ladder is sloped and the player decides to climb the side where the ladder
    /// meets the floor at an acute angle, the player will still be able to climb the ladder, but
    /// it will look stuttery because the player's collision is a box with an upward vector pointing
    /// in the same direction as the floor's normal vector. Due to this geometry, the upward ascent
    /// on the acute-angled side of the ladder is not smooth. Therefore, it is advisable to avoid
    /// using the ladder for such use cases.
    /// </summary>
    private void handleClimb()
    {
        //check for inputs again
        Vector3 inputDirection = getInputDirectionVector();
        float newVelocityY = 0f;

        //if player is moving away from the ladder, then stop climbing
        Vector3 ladderHorizontalPosition = new Vector3(currentLadder.position.x, 0, currentLadder.position.z);

        if (Vector3.Dot(ladderHorizontalPosition - player.transform.position, inputDirection) < 0)
        {
            isClimbing = false;
        }

        body.transform.up = Vector3.up;

        Vector3 ladderForwardHorizontal = new Vector3(currentLadder.forward.x, 0, currentLadder.forward.z).normalized;

        newVelocityY = Mathf.Abs(Vector3.Dot(ladderForwardHorizontal, inputDirection)) > 0.01f ? climbingVelocity : defaultLadderFallingVelocity;

        Vector3 ladderRight = currentLadder.right;
        Vector3 ladderUp = currentLadder.up;

        float rightComponent = Vector3.Dot(velocity, ladderRight);

        //slow down horizontal velocity so the player does not easily fall off the ladder
        rightComponent = Mathf.Clamp(rightComponent, -maxSidewaysVelocityOnLadder, maxSidewaysVelocityOnLadder);

        velocity = ladderUp * newVelocityY + ladderRight * rightComponent;
    }

    /// <summary>
    /// This function completely halts the movement of the player (including vertical).
    /// </summary>
    public void haltMovement()
    {
        velocity = Vector3.zero;
        playerRigidBody.linearVelocity = Vector3.zero;
        playerRigidBody.angularVelocity = Vector3.zero;
    }

    /// <summary>
    /// This function takes in the inputs of the player and returns a
    /// vector of the input direction with respect to the player's
    /// current forward vector at the time this function is called.
    /// </summary>
    /// <returns>A Vector3 of the input direction.</returns>
    private Vector3 getInputDirectionVector()
    {
        Vector3 inputDirection = Vector3.zero;
        Vector3 forward = playerCamera.transform.forward;
        Vector3 right = playerCamera.transform.right;

        forward.y = 0;
        right.y = 0;

        if (Input.GetKey(KeyCode.W))
        {
            inputDirection += forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputDirection -= right;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputDirection -= forward;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputDirection += right;
        }
        inputDirection.Normalize();
        return inputDirection;
    }

    private bool handleSlimeBounce(Collision collision)
    {
        if (collision.gameObject.CompareTag("Slime"))
        {
            //Debug.Log($"OnCollisionEnter with Slime - Frame: {Time.frameCount}, velocity.y: {velocity.y}");

            if (Mathf.Abs(velocity.y) >= minElasticCollisionVelocity)
            {
                velocity.y = Mathf.Abs(velocity.y *
                    Mathf.Clamp(1 - Mathf.Exp(
                        -elasticEnergyExponentialDecayCoefficient * Mathf.Abs(velocity.y)
                        )
                    , 0f, maxElasticCollisionConservation)
                    );
                //Debug.Log($"Bounced to: {velocity.y}");

                return true;
            }
            else
            {
                velocity.y = 0;
                playerRigidBody.linearVelocity = new Vector3(playerRigidBody.linearVelocity.x, 0, playerRigidBody.linearVelocity.z);
            }
        }
        return false;
    }

    private IEnumerator handleBlockCrumble(Collision collision)
    {
        bool validLanding = false;
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y >= Mathf.Cos(maxSlopeAngle * Mathf.Deg2Rad))
            {
                validLanding = true;
                break;
            }
        }
        if (!validLanding)
        {
            yield break;
        }

        CrumblingPlatform platformScript = collision.gameObject.GetComponent<CrumblingPlatform>();
        GameObject platformObject = collision.gameObject;

        if (!platformScript.isBlockCrumbling())
        {
            platformScript.setBlockCrumbling(true);
            StartCoroutine(platformScript.crumble());
        }
        yield return new WaitForSeconds(platformScript.getFadeTime());
        removeGroundCollider(platformObject);
    }


    private void addGroundCollider(Collision collision, ContactPoint contact)
    {
        if (!groundContactPoints.ContainsKey(collision.collider))
        {
            groundContactPoints.Add(collision.collider, new List<Vector3> { });
        }
        if (groundContactPoints[collision.collider].Count == 0)
        {
            groundContactPoints[collision.collider].Add(contact.normal);
        }
    }

    public void removeGroundCollider(Collider collider)
    {
        groundContactPoints.Remove(collider);
    }

    public void removeGroundCollider(GameObject obj)
    {
        Collider colliderToRemove = null;
        foreach (KeyValuePair<Collider, List<Vector3>> kvp in groundContactPoints)
        {
            if (kvp.Key.gameObject == obj)
            {
                colliderToRemove = kvp.Key;
                break;
            }
        }

        if (colliderToRemove != null)
        {
            groundContactPoints.Remove(colliderToRemove);

            if (groundContactPoints.Count == 0)
            {
                currentFrictionCoefficient = frictionCoefficients["Air"];
                currMaxSpeedMultiplier = maxSpeedMultipliers["Air"];
            }
        }
    }

    private void handleSensitivityChange()
    {
        setSensitivityX(sliderX.value);
        setSensitivityY(sliderY.value);
    }

    public void setSensitivityX(float x)
    {
        sensitivityX = x;
    }

    public void setSensitivityY(float y)
    {
        sensitivityY = y;
    }

    public float getInitialYaw()
    {
        return initialYaw;
    }

    public float getInitialPitch()
    {
        return initialPitch;
    }

    public void handleFrictionChange()
    {
        currentFrictionCoefficient = Mathf.Min(currentFrictionCoefficient, frictionCoefficients.GetValueOrDefault(tag, frictionCoefficients["Default"]));
        currMaxSpeedMultiplier = Mathf.Min(currMaxSpeedMultiplier, maxSpeedMultipliers.GetValueOrDefault(tag, maxSpeedMultipliers["Default"]));
    }

    /// <summary>
    /// This function is mainly used to add ground contact points to
    /// the Dictionary containing both information about the Collider
    /// and its Vector3 normal vector. It uses a foreach loop in case
    /// the collider is a mesh containing multiple faces with differing
    /// surface normal vectors.
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (collision.gameObject.CompareTag("Crumbling"))
            {
                StartCoroutine(handleBlockCrumble(collision));
            }

            if (contact.normal.y >= Mathf.Cos(maxSlopeAngle * Mathf.Deg2Rad))
            {
                if (handleSlimeBounce(collision))
                {
                    return;
                }

                addGroundCollider(collision, contact);
                playerRigidBody.linearVelocity = Vector3.zero;
            }


            if (contact.normal.y <= -Mathf.Cos(maxSlopeAngle * Mathf.Deg2Rad))
            {
                // flatten the normal so we only keep horizontal pushback
                Vector3 horizontalNormal = new Vector3(contact.normal.x, 0f, contact.normal.z).normalized;

                // 0.03 is the lowest possible without it breaking; do not change this
                Vector3 correction = horizontalNormal * 0.03f;
                playerRigidBody.MovePosition(playerRigidBody.position + correction);

                // cancel velocity into the ceiling only along horizontal direction
                velocity = Vector3.ProjectOnPlane(velocity, horizontalNormal);
                velocity.y = ceilingReboundVelocity;
                playerRigidBody.linearVelocity = new Vector3(playerRigidBody.linearVelocity.x, ceilingReboundVelocity, playerRigidBody.linearVelocity.z);
            }
        }
    }

    /// <summary>
    /// This function deals with halting the player's movement along a certain horizontal
    /// axis if the slope along that axis is too steep (greater than maxSlopeAngle).
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (collision.gameObject.CompareTag("Crumbling"))
            {
                CrumblingPlatform platformScript = collision.gameObject.GetComponent<CrumblingPlatform>();
                if (platformScript != null && platformScript.isBlockCrumbling())
                {
                    handleFrictionChange();
                    continue;
                }
            }
            if (contact.normal.y >= Mathf.Cos(maxSlopeAngle * Mathf.Deg2Rad))
            {
                addGroundCollider(collision, contact);
                playerRigidBody.linearVelocity = Vector3.zero;

                //set the type of friction 
                string tag = collision.gameObject.tag;

                handleFrictionChange();

                if (tag.Equals("Ice"))
                {
                    //this here sets it strictly to ice (no Mathf.Min)
                    currentFrictionCoefficient = frictionCoefficients.GetValueOrDefault(tag, frictionCoefficients["Default"]);
                    currMaxSpeedMultiplier = maxSpeedMultipliers.GetValueOrDefault(tag, maxSpeedMultipliers["Default"]);
                }
            }
            else if (contact.normal.y <= -Mathf.Cos(maxSlopeAngle * Mathf.Deg2Rad))
            {
                // flatten the normal so we only keep horizontal pushback
                Vector3 horizontalNormal = new Vector3(contact.normal.x, 0f, contact.normal.z).normalized;

                // 0.03 is the lowest possible without it breaking; do not change this
                Vector3 correction = horizontalNormal * 0.03f;
                playerRigidBody.MovePosition(playerRigidBody.position + correction);

                // cancel velocity into the ceiling only along horizontal direction
                velocity = Vector3.ProjectOnPlane(velocity, horizontalNormal);
                velocity.y = ceilingReboundVelocity;
                playerRigidBody.linearVelocity = new Vector3(playerRigidBody.linearVelocity.x, ceilingReboundVelocity, playerRigidBody.linearVelocity.z);
            }
            else
            {
                //stops players from clipping through corners
                if (!collision.gameObject.CompareTag("Slime"))
                {
                    velocity = Vector3.ProjectOnPlane(velocity, contact.normal);
                }
            }
        }


    }

    /// <summary>
    /// This function removes all <Collider, Vector3> key-value pairs
    /// of the current collider that the player leaves from groundContactPoints. 
    /// Note that this collider does not have to be a ground collider.
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionExit(Collision collision)
    {
        removeGroundCollider(collision.collider);
        if (groundContactPoints.Count == 0) {
            currentFrictionCoefficient = frictionCoefficients["Air"];
            currMaxSpeedMultiplier = maxSpeedMultipliers["Air"];
        }
        //sanity check
        //ceilingContactPointsPositions.Remove(collision.collider);
    }


    /// <summary>
    /// This function updates currentLadder and isClimbing so that the player can start
    /// climbing properly. However, climbing multiple ladders at once is not currently
    /// supported. If such a case presented itself, the player would climb the last
    /// ladder they came into contact with.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Ladder"))
        {
            //prevents climbing on a ladder that is too steep
            if (Mathf.Acos(other.transform.forward.y) < maxSlopeAngle * Mathf.Deg2Rad)
            {
                return;
            }
            currentLadder = other.transform;

            isClimbing = true;
        }
    }

    /// <summary>
    /// This function updates currentLader and isClimbing so that the player stops
    /// climbing a ladder and returns to normal player walking/running movement.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Ladder"))
        {
            currentLadder = null;
            isClimbing = false;
        }
    }



    /// <summary>
    /// This function takes in a list of Vector3 vectors, averages all their
    /// components, normalizes, and then returns this.
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    private Vector3 averageVector(List<Vector3> list)
    {
        Vector3 averageVector = new Vector3();
        foreach (Vector3 v in list)
        {
            averageVector.x += v.x;
            averageVector.y += v.y;
            averageVector.z += v.z;
        }

        averageVector /= list.Count;
        averageVector.Normalize();
        
        return averageVector;
    }
}
