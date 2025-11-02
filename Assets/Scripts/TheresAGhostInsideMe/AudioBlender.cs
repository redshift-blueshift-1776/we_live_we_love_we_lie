using UnityEngine;

public class AudioBlender : MonoBehaviour
{
    [SerializeField] public AudioSource song1;
    [SerializeField] public AudioSource song2;

    [SerializeField] public bool backAndForth;

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
            float r = Mathf.PingPong(Time.time * 0.2f, 1f);
            SetRatio(r);
        }
    }

    public void SetRatio(float r)
    {
        song1.volume = Mathf.Clamp01(2 * r - r * r);
        song2.volume = Mathf.Clamp01(1 - r * r);
    }
}