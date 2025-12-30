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

    private double songStartDSP;
    private float secondsPerBeat;

    void Start()
    {
        beatManager = BeatManager.Instance;
        animationComponent = GetComponent<Animation>();

        secondsPerBeat = 60f / beatManager.tempo;
        songStartDSP = beatManager.audioSource.timeSamples /
                       (double)beatManager.audioSource.clip.frequency;

        SetDance(currentDance);
    }

    void Update()
    {
        if (!beatManager.audioSource.isPlaying)
            return;

        SampleDanceInterpolated();
    }

    void SampleDanceInterpolated()
    {
        AnimationClip clip = dances[currentDance];

        // Precise song time
        double songTime =
            AudioSettings.dspTime - songStartDSP;

        // Convert to beats
        double beatTime = songTime / secondsPerBeat;

        // Loop inside dance
        double beatInDance = beatTime % beatsPerLoop;
        float normalizedTime =
            (float)(beatInDance / beatsPerLoop);

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