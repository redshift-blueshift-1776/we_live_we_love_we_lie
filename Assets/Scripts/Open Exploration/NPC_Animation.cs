using UnityEngine;
using UnityEngine.AI;

public class NPC_Animation : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform leftLeg;
    public Transform rightLeg;
    public Transform leftArm;
    public Transform rightArm;

    public float walkSpeed = 5f;
    public float runSpeed = 12f;

    public float walkAngle = 35f;
    public float runAngle = 60f;

    public float smoothness = 8f;

    void Update()
    {
        float speed = agent.velocity.magnitude;

        if (speed < 0.1f)
        {
            AnimateIdle();
        }
        else
        {
            bool running = speed > 4f;

            float animSpeed = running ? runSpeed : walkSpeed;
            float angle = running ? runAngle : walkAngle;

            float swing =
                Mathf.Sin(Time.time * animSpeed) * angle;

            ApplyRotation(leftLeg, swing);
            ApplyRotation(rightLeg, -swing);
            ApplyRotation(leftArm, -swing);
            ApplyRotation(rightArm, swing);
        }
    }

    void AnimateIdle()
    {
        ApplyRotation(leftLeg, 0);
        ApplyRotation(rightLeg, 0);
        ApplyRotation(leftArm, 0);
        ApplyRotation(rightArm, 0);
    }

    void ApplyRotation(Transform part, float targetX)
    {
        Quaternion target =
            Quaternion.Euler(targetX, 0, 0);

        part.localRotation =
            Quaternion.Lerp(
                part.localRotation,
                target,
                Time.deltaTime * smoothness
            );
    }
}