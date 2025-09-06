using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Love_Truck_Passenger : MonoBehaviour
{
    public enum Pose {
        Default,
        KirKan,
        WeightsDance1,
        WeightsDance2,
        Aluminum1,
        Aluminum2,
        Floss1,
        Floss2,
        Floss3,
        Floss4,
        Floss5,
        Floss6,
        Floss7,
        Floss8,
        Floss9,
        Floss10,
        Champion1,
        Champion2,
        Champion3,
        Champion4,
    }

    public enum Dance {
        Default,
        KirKan,
        Weights,
        Champion,
        Aluminum,
        Floss,
        Floss24
    }

    // [SerializeField] public GameObject game;

    [SerializeField] public Dance dance;

    [SerializeField] private Pose currentPose;

    // public Game gameScript;

    [SerializeField] private GameObject gameAudio;

    public AudioSource audioSource;

    public char type; // char for which building they go to

    [Header("Materials")]

    [SerializeField] public Material wColor;
    [SerializeField] public Material aColor;
    [SerializeField] public Material sColor;
    [SerializeField] public Material dColor;

    [Header("Body Parts")]
    [SerializeField] public GameObject torso;
    [SerializeField] public GameObject leftLeg;
    [SerializeField] public GameObject rightLeg;

    [SerializeField] public GameObject leftLegLower;
    [SerializeField] public GameObject rightLegLower;

    [SerializeField] public GameObject leftArm;
    [SerializeField] public GameObject rightArm;

    [SerializeField] public GameObject leftArmLower;
    [SerializeField] public GameObject rightArmLower;

    [SerializeField] public GameObject head;

    [Header("Joints")]
    [SerializeField] public GameObject leftLegJoint;
    [SerializeField] public GameObject rightLegJoint;

    [SerializeField] public GameObject leftLegLowerJoint;
    [SerializeField] public GameObject rightLegLowerJoint;

    [SerializeField] public GameObject leftArmJoint;
    [SerializeField] public GameObject rightArmJoint;

    [SerializeField] public GameObject leftArmLowerJoint;
    [SerializeField] public GameObject rightArmLowerJoint;

    [SerializeField] private float tempo;

    private float secondsPerBeat;

    private double nextChangeTime;

    private Coroutine currentPoseCoroutine;


    void Start()
    {
        // game = GameObject.Find("Game");
        // gameScript = game.GetComponent<Game>();
        // gameAudio = gameScript.gameAudio;
        audioSource = gameAudio.GetComponent<AudioSource>();
        // tempo = gameScript.tempo;
        secondsPerBeat = 60f / tempo;

        nextChangeTime = BeatManager.Instance.GetNextBeatTime();
        currentPose = Pose.Default;

        // Immediately start dancing if active
        if (dance != Dance.Default && BeatManager.Instance.audioSource.isPlaying)
        {
            NextPose();
        }
    }
    private int lastPoseBeat = -1;

    void Update()
    {
        if (!BeatManager.Instance.audioSource.isPlaying) return;

        int currentBeat = BeatManager.Instance.GetCurrentBeatNumber();

        if (currentBeat != lastPoseBeat)
        {
            lastPoseBeat = currentBeat;
            NextPose();
        }

        UpdateColor();
    }

    private void UpdateColor()
    {
        Material color = null;

        switch (type)
        {
            case 'W': color = wColor; break;
            case 'A': color = aColor; break;
            case 'S': color = sColor; break;
            case 'D': color = dColor; break;
        }

        if (color != null)
        {
            head.GetComponent<MeshRenderer>().material = color;
            torso.GetComponent<MeshRenderer>().material = color;
        }
    }


    private IEnumerator ChangePose(Pose pose) {
        if (currentPose == pose)
            yield break;

        float duration = secondsPerBeat / 2f;
        float elapsed = 0f;

        // Store initial rotations
        Quaternion leftArmStart = leftArmJoint.transform.localRotation;
        Quaternion leftArmLowerStart = leftArmLowerJoint.transform.localRotation;
        Quaternion rightArmStart = rightArmJoint.transform.localRotation;
        Quaternion rightArmLowerStart = rightArmLowerJoint.transform.localRotation;
        Quaternion leftLegStart = leftLegJoint.transform.localRotation;
        Quaternion leftLegLowerStart = leftLegLowerJoint.transform.localRotation;
        Quaternion rightLegStart = rightLegJoint.transform.localRotation;
        Quaternion rightLegLowerStart = rightLegLowerJoint.transform.localRotation;

        // Target rotations based on pose
        Quaternion defaultRotation = Quaternion.Euler(0, 0, 0);
        Quaternion z90 = Quaternion.Euler(0, 0, 90);
        Quaternion z180 = Quaternion.Euler(0, 0, 180);
        Quaternion zn90 = Quaternion.Euler(0, 0, -90);

        Quaternion x45z45 = Quaternion.Euler(45, 0, 45);
        Quaternion x45zn45 = Quaternion.Euler(45, 0, -45);
        Quaternion xn45z45 = Quaternion.Euler(-45, 0, 45);
        Quaternion xn45zn45 = Quaternion.Euler(-45, 0, -45);

        Quaternion x90 = Quaternion.Euler(90, 0, 0);
        Quaternion xn90 = Quaternion.Euler(-90, 0, 0);
        Quaternion x90z45 = Quaternion.Euler(90, 0, 45);
        Quaternion x90zn45 = Quaternion.Euler(90, 0, -45);

        Quaternion leftArmTarget = leftArmStart;
        Quaternion leftArmLowerTarget = leftArmLowerStart;
        Quaternion rightArmTarget = rightArmStart;
        Quaternion rightArmLowerTarget = rightArmLowerStart;
        Quaternion leftLegTarget = leftLegStart;
        Quaternion leftLegLowerTarget = leftLegLowerStart;
        Quaternion rightLegTarget = rightLegStart;
        Quaternion rightLegLowerTarget = rightLegLowerStart;

        Quaternion z45 = Quaternion.Euler(0, 0, 45);
        Quaternion zn15 = Quaternion.Euler(0, 0, -15);
        Quaternion zn45 = Quaternion.Euler(0, 0, -45);
        Quaternion x60z45 = Quaternion.Euler(60, 0, 45);
        Quaternion x60zn45 = Quaternion.Euler(60, 0, -45);

        switch (pose)
        {
            case Pose.Default:
                leftLegTarget = defaultRotation;
                leftLegLowerTarget = defaultRotation;
                rightLegTarget = defaultRotation;
                rightLegLowerTarget = defaultRotation;
                leftArmTarget = defaultRotation;
                leftArmLowerTarget = defaultRotation;
                rightArmTarget = defaultRotation;
                rightArmLowerTarget = defaultRotation;
                currentPose = Pose.Default;
                break;

            case Pose.WeightsDance1:
                leftArmTarget = z180;
                leftArmLowerTarget = defaultRotation;
                rightArmTarget = zn90;
                rightArmLowerTarget = zn90;
                leftLegTarget = defaultRotation;
                leftLegLowerTarget = defaultRotation;
                rightLegTarget = defaultRotation;
                rightLegLowerTarget = defaultRotation;
                currentPose = Pose.WeightsDance1;
                break;

            case Pose.WeightsDance2:
                leftArmTarget = z90;
                leftArmLowerTarget = z90;
                rightArmTarget = z180;
                rightArmLowerTarget = defaultRotation;
                leftLegTarget = defaultRotation;
                leftLegLowerTarget = defaultRotation;
                rightLegTarget = defaultRotation;
                rightLegLowerTarget = defaultRotation;
                currentPose = Pose.WeightsDance2;
                break;

            case Pose.KirKan:
                // Add KirKan targets here if needed
                leftLegTarget = defaultRotation;
                leftLegLowerTarget = defaultRotation;
                rightLegTarget = zn15;
                rightLegLowerTarget = defaultRotation;
                leftArmTarget = z45;
                leftArmLowerTarget = zn90;
                rightArmTarget = defaultRotation;
                rightArmLowerTarget = defaultRotation;
                currentPose = Pose.KirKan;
                break;

            case Pose.Aluminum1:
                leftArmTarget = x90z45;
                leftArmLowerTarget = zn90;
                rightArmTarget = x60zn45;
                rightArmLowerTarget = z45;
                leftLegTarget = defaultRotation;
                leftLegLowerTarget = defaultRotation;
                rightLegTarget = defaultRotation;
                rightLegLowerTarget = defaultRotation;
                currentPose = Pose.Aluminum1;
                break;

            case Pose.Aluminum2:
                leftArmTarget = x60z45;
                leftArmLowerTarget = zn45;
                rightArmTarget = x90zn45;
                rightArmLowerTarget = z90;
                leftLegTarget = defaultRotation;
                leftLegLowerTarget = defaultRotation;
                rightLegTarget = defaultRotation;
                rightLegLowerTarget = defaultRotation;
                currentPose = Pose.Aluminum2;
                break;

            case Pose.Floss1:
                leftArmTarget = x45z45;
                leftArmLowerTarget = defaultRotation;
                rightArmTarget = x45z45;
                rightArmLowerTarget = defaultRotation;
                currentPose = Pose.Floss1;
                break;

            case Pose.Floss2:
                leftArmTarget = xn45zn45;
                leftArmLowerTarget = defaultRotation;
                rightArmTarget = xn45zn45;
                rightArmLowerTarget = defaultRotation;
                currentPose = Pose.Floss2;
                break;

            case Pose.Floss3:
                leftArmTarget = x45z45;
                leftArmLowerTarget = defaultRotation;
                rightArmTarget = x45z45;
                rightArmLowerTarget = defaultRotation;
                currentPose = Pose.Floss3;
                break;

            case Pose.Floss4:
                leftArmTarget = x45zn45;
                leftArmLowerTarget = defaultRotation;
                rightArmTarget = x45zn45;
                rightArmLowerTarget = defaultRotation;
                currentPose = Pose.Floss4;
                break;

            case Pose.Floss5:
                leftArmTarget = xn45z45;
                leftArmLowerTarget = defaultRotation;
                rightArmTarget = xn45z45;
                rightArmLowerTarget = defaultRotation;
                currentPose = Pose.Floss5;
                break;

            case Pose.Floss6:
                leftArmTarget = x45zn45;
                leftArmLowerTarget = defaultRotation;
                rightArmTarget = x45zn45;
                rightArmLowerTarget = defaultRotation;
                currentPose = Pose.Floss6;
                break;

            case Pose.Floss7:
                leftArmTarget = x45zn45;
                leftArmLowerTarget = defaultRotation;
                rightArmTarget = xn45z45;
                rightArmLowerTarget = defaultRotation;
                leftLegTarget = defaultRotation;
                leftLegLowerTarget = defaultRotation;
                rightLegTarget = defaultRotation;
                rightLegLowerTarget = defaultRotation;
                currentPose = Pose.Floss7;
                break;

            case Pose.Floss8:
                leftArmTarget = z90;
                leftArmLowerTarget = defaultRotation;
                rightArmTarget = zn90;
                rightArmLowerTarget = defaultRotation;
                leftLegTarget = defaultRotation;
                leftLegLowerTarget = defaultRotation;
                rightLegTarget = defaultRotation;
                rightLegLowerTarget = defaultRotation;
                currentPose = Pose.Floss8;
                break;

            case Pose.Floss9:
                leftArmTarget = xn45zn45;
                leftArmLowerTarget = defaultRotation;
                rightArmTarget = x45z45;
                rightArmLowerTarget = defaultRotation;
                leftLegTarget = defaultRotation;
                leftLegLowerTarget = defaultRotation;
                rightLegTarget = defaultRotation;
                rightLegLowerTarget = defaultRotation;
                currentPose = Pose.Floss9;
                break;

            case Pose.Floss10:
                leftArmTarget = z90;
                leftArmLowerTarget = defaultRotation;
                rightArmTarget = zn90;
                rightArmLowerTarget = defaultRotation;
                leftLegTarget = defaultRotation;
                leftLegLowerTarget = defaultRotation;
                rightLegTarget = defaultRotation;
                rightLegLowerTarget = defaultRotation;
                currentPose = Pose.Floss10;
                break;

            case Pose.Champion1:
                leftLegTarget = x90;
                leftLegLowerTarget = xn90;
                rightLegTarget = defaultRotation;
                rightLegLowerTarget = defaultRotation;
                leftArmTarget = x90z45;
                leftArmLowerTarget = x90;
                rightArmTarget = x90z45;
                rightArmLowerTarget = x90;
                currentPose = Pose.Champion1;
                break;
            
            case Pose.Champion2:
                leftLegTarget = defaultRotation;
                leftLegLowerTarget = defaultRotation;
                rightLegTarget = defaultRotation;
                rightLegLowerTarget = defaultRotation;
                leftArmTarget = defaultRotation;
                leftArmLowerTarget = defaultRotation;
                rightArmTarget = defaultRotation;
                rightArmLowerTarget = defaultRotation;
                currentPose = Pose.Champion2;
                break;

            case Pose.Champion3:
                leftLegTarget = defaultRotation;
                leftLegLowerTarget = defaultRotation;
                rightLegTarget = x90;
                rightLegLowerTarget = xn90;
                leftArmTarget = x90zn45;
                leftArmLowerTarget = x90;
                rightArmTarget = x90zn45;
                rightArmLowerTarget = x90;
                currentPose = Pose.Champion3;
                break;

            case Pose.Champion4:
                leftLegTarget = defaultRotation;
                leftLegLowerTarget = defaultRotation;
                rightLegTarget = defaultRotation;
                rightLegLowerTarget = defaultRotation;
                leftArmTarget = defaultRotation;
                leftArmLowerTarget = defaultRotation;
                rightArmTarget = defaultRotation;
                rightArmLowerTarget = defaultRotation;
                currentPose = Pose.Champion4;
                break;
            
            default:
                leftLegTarget = defaultRotation;
                currentPose = Pose.Default;
                break;
        }


        // Animate over time
        while (elapsed < duration) {
            float t = elapsed / duration;

            //Debug.Log(t);

            leftArmJoint.transform.localRotation = Quaternion.Slerp(leftArmStart, leftArmTarget, t);
            leftArmLowerJoint.transform.localRotation = Quaternion.Slerp(leftArmLowerStart, leftArmLowerTarget, t);
            rightArmJoint.transform.localRotation = Quaternion.Slerp(rightArmStart, rightArmTarget, t);
            rightArmLowerJoint.transform.localRotation = Quaternion.Slerp(rightArmLowerStart, rightArmLowerTarget, t);
            leftLegJoint.transform.localRotation = Quaternion.Slerp(leftLegStart, leftLegTarget, t);
            leftLegLowerJoint.transform.localRotation = Quaternion.Slerp(leftLegLowerStart, leftLegLowerTarget, t);
            rightLegJoint.transform.localRotation = Quaternion.Slerp(rightLegStart, rightLegTarget, t);
            rightLegLowerJoint.transform.localRotation = Quaternion.Slerp(rightLegLowerStart, rightLegLowerTarget, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final rotations are exactly set
        leftArmJoint.transform.localRotation = leftArmTarget;
        leftArmLowerJoint.transform.localRotation = leftArmLowerTarget;
        rightArmJoint.transform.localRotation = rightArmTarget;
        rightArmLowerJoint.transform.localRotation = rightArmLowerTarget;
        leftLegJoint.transform.localRotation = leftLegTarget;
        leftLegLowerJoint.transform.localRotation = leftLegLowerTarget;
        rightLegJoint.transform.localRotation = rightLegTarget;
        rightLegLowerJoint.transform.localRotation = rightLegLowerTarget;

        yield return null;
    }


    void NextPose() {
        //Debug.Log($"Starting new pose coroutine at dspTime: {AudioSettings.dspTime}");
        if (currentPoseCoroutine != null) {
            StopCoroutine(currentPoseCoroutine);
        }

        if (dance == Dance.Default) {
            currentPoseCoroutine = StartCoroutine(ChangePose(Pose.Default));
        } else if (dance == Dance.KirKan) {
            currentPoseCoroutine = StartCoroutine(ChangePose(Pose.KirKan));
        } else if (dance == Dance.Weights) {
            if (currentPose == Pose.WeightsDance1) {
                currentPoseCoroutine = StartCoroutine(ChangePose(Pose.WeightsDance2));
            } else {
                currentPoseCoroutine = StartCoroutine(ChangePose(Pose.WeightsDance1));
            }
        } else if (dance == Dance.Aluminum) {
            if (currentPose == Pose.Aluminum1) {
                currentPoseCoroutine = StartCoroutine(ChangePose(Pose.Aluminum2));
            } else {
                currentPoseCoroutine = StartCoroutine(ChangePose(Pose.Aluminum1));
            }
        } else if (dance == Dance.Floss) {
            if (currentPose == Pose.Floss1) {
                currentPoseCoroutine = StartCoroutine(ChangePose(Pose.Floss2));
            } else if (currentPose == Pose.Floss2) {
                currentPoseCoroutine = StartCoroutine(ChangePose(Pose.Floss3));
            } else if (currentPose == Pose.Floss3) {
                currentPoseCoroutine = StartCoroutine(ChangePose(Pose.Floss4));
            } else if (currentPose == Pose.Floss4) {
                currentPoseCoroutine = StartCoroutine(ChangePose(Pose.Floss5));
            } else if (currentPose == Pose.Floss5) {
                currentPoseCoroutine = StartCoroutine(ChangePose(Pose.Floss6));
            } else {
                currentPoseCoroutine = StartCoroutine(ChangePose(Pose.Floss1));
            }
        } else if (dance == Dance.Floss24) {
            if (currentPose == Pose.Floss7) {
                currentPoseCoroutine = StartCoroutine(ChangePose(Pose.Floss8));
            } else if (currentPose == Pose.Floss8) {
                currentPoseCoroutine = StartCoroutine(ChangePose(Pose.Floss9));
            } else if (currentPose == Pose.Floss9) {
                currentPoseCoroutine = StartCoroutine(ChangePose(Pose.Floss10));
            } else {
                currentPoseCoroutine = StartCoroutine(ChangePose(Pose.Floss7));
            }
        } else if (dance == Dance.Champion) {
            if (currentPose == Pose.Champion1) {
                currentPoseCoroutine = StartCoroutine(ChangePose(Pose.Champion2));
            } else if (currentPose == Pose.Champion2) {
                currentPoseCoroutine = StartCoroutine(ChangePose(Pose.Champion3));
            } else if (currentPose == Pose.Champion3) {
                currentPoseCoroutine = StartCoroutine(ChangePose(Pose.Champion4));
            } else {
                currentPoseCoroutine = StartCoroutine(ChangePose(Pose.Champion1));
            }
        } else {
            currentPoseCoroutine = StartCoroutine(ChangePose(Pose.Default));
        }
    }

}
