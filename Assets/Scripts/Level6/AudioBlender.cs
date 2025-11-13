using UnityEngine;

public class AudioBlender : MonoBehaviour
{
    [SerializeField] public AudioSource song1;
    [SerializeField] public AudioSource song2;

    [SerializeField] public bool backAndForth;

    [SerializeField] private float minVolume = 0.2f;
    [SerializeField] private float maxVolume = 1.0f;

    void Start()
    {
        song1.Stop();
        song2.Stop();

        double startTime = AudioSettings.dspTime + 0.2;

        song1.PlayScheduled(startTime);
        song2.PlayScheduled(startTime);
    }

    void Update()
    {
        if (backAndForth) {
            float r = Mathf.PingPong(Time.time * minVolume, maxVolume);
            SetRatio(r);
        }
    }

    public void SetRatio(float r)
    {
        song1.volume = Mathf.Clamp01(maxVolume * (2 * r - r * r));
        song2.volume = Mathf.Clamp01(maxVolume * (1f - r * r));
    }
}