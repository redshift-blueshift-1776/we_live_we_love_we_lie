using UnityEngine;

public class Note : MonoBehaviour
{
    [SerializeField] public bool realNote;
    [SerializeField] public float duration;  // in seconds (already scaled by secondsPerBeat)
    [SerializeField] public float delay;     // in seconds (already scaled by secondsPerBeat)

    public SecondWeLiveWeLoveWeLie gm;

    [SerializeField] public GameObject[] indicators;
    [SerializeField] public GameObject[] circles;
    [SerializeField] public GameObject[] xs;

    [SerializeField] public GameObject hitSound;
    [SerializeField] public GameObject wrongSound;

    public bool noteHit;

    // Cached BeatManager reference for performance
    private BeatManager beatManager;
    private double noteStartTime; // absolute DSP time when note should begin

    void Start()
    {
        noteHit = false;
        hitSound.SetActive(false);
        wrongSound.SetActive(false);

        beatManager = BeatManager.Instance;
        noteStartTime = beatManager.StartDspTime + delay;
    }

    void Update()
    {
        if (!gm.gameActive)
            return;

        // Get current song time in seconds, synced to audio playback
        double songTime = AudioSettings.dspTime - beatManager.StartDspTime;
        double timeSinceNoteStart = songTime - delay;

        // Hide the mesh if already hit
        if (noteHit)
        {
            Renderer r = gameObject.GetComponent<MeshRenderer>();
            r.enabled = false;
        }

        // Destroy note after it has fully passed
        if (songTime > delay + duration)
        {
            if (realNote && !noteHit)
                gm.addScore(-5);

            Destroy(gameObject);
            return;
        }

        // Visibility + scaling logic
        if (realNote)
        {
            foreach (GameObject x in xs)
                x.SetActive(false);

            bool isActive = (songTime > delay) && !noteHit;

            foreach (GameObject x in circles)
                x.SetActive(isActive);

            foreach (GameObject x in indicators)
            {
                x.SetActive(isActive);
                if (isActive)
                {
                    float progress = (float)((songTime - delay) / duration);
                    progress = Mathf.Clamp01(progress);
                    x.transform.localScale = new Vector3(1f - progress, 0.54f, 1f - progress);
                }
            }
        }
        else
        {
            foreach (GameObject x in xs)
                x.SetActive(!noteHit);
            foreach (GameObject x in circles)
                x.SetActive(false);
            foreach (GameObject x in indicators)
                x.SetActive(false);
        }
    }

    public void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.name == "RealLaser")
            hitNote();
    }

    public void hitNote()
    {
        if (noteHit) {
            return;
        }
        double songTime = AudioSettings.dspTime - beatManager.StartDspTime;

        // Ignore hits that come before note appears
        if (songTime < delay)
            return;

        noteHit = true;

        if (realNote)
        {
            hitSound.SetActive(true);

            // Compute accuracy based on distance from note center
            double noteCenter = delay + duration / 2f;
            float diff = (float)Mathf.Abs((float)(songTime - noteCenter));

            int scoreToAdd = (int)(10 - 20 * diff);
            scoreToAdd = Mathf.Clamp(scoreToAdd, 1, 10);

            scoreToAdd = (scoreToAdd > 8) ? 10 : scoreToAdd;

            gm.addScore(scoreToAdd);
        }
        else
        {
            wrongSound.SetActive(true);
            gm.addScore(-10);
        }
    }
}