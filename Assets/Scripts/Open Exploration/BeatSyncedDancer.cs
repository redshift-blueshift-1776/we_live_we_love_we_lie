using UnityEngine;

public class BeatSyncedLegacyDancer : MonoBehaviour
{
    public BeatManager beatManager;
    public Animation animationComponent;

    [Header("Dance Clips (Legacy)")]
    public AnimationClip[] dances;

    [Header("Dance Settings")]
    public int beatsPerLoop = 8;

    [SerializeField] private int currentDance = 0;

    // [Header("Fine Tune")]
    // [Tooltip("Shift animation earlier/later in beats")]
    // [SerializeField] private float beatPhaseOffset = 0f;


    // private double songStartDSP;
    // private float secondsPerBeat;

    void Start()
    {
        beatManager = BeatManager.Instance;
        animationComponent = GetComponent<Animation>();

        SetDance(currentDance);
    }

    private int lastBeat = -1;

    void Update()
    {
        double elapsed = AudioSettings.dspTime - beatManager.StartDspTime;
        if (elapsed < 0)
        {
            return;
        }
        
        int currentBeat = Mathf.FloorToInt(
            (float)(elapsed / beatManager.secondsPerBeat));

        if (currentBeat != lastBeat)
        {
            OnBeat(currentBeat);
            lastBeat = currentBeat;
        }
    }

    void OnBeat(int beatNumber)
    {
        int beatInDance = beatNumber % beatsPerLoop;
        float normalizedTime = (float)beatInDance / beatsPerLoop;

        AnimationClip clip = dances[currentDance];
        float clipTime = normalizedTime * clip.length;

        animationComponent[clip.name].time = clipTime;
        animationComponent.Sample();
    }

    
    public void SetDance(int index)
    {
        currentDance = Mathf.Clamp(index, 0, dances.Length - 1);

        animationComponent.Stop();
        animationComponent.clip = dances[currentDance];
        animationComponent.Play();

        Debug.Log($"Changed Dance To: {dances[currentDance].name}");
    }
}