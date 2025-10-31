using UnityEngine;

public class Note : MonoBehaviour
{
    [SerializeField] public bool realNote;
    [SerializeField] public float duration;
    [SerializeField] public float delay;

    public SecondWeLiveWeLoveWeLie gm;

    [SerializeField] public GameObject[] indicators;
    [SerializeField] public GameObject[] circles;
    [SerializeField] public GameObject[] xs;

    [SerializeField] public GameObject hitSound;
    [SerializeField] public GameObject wrongSound;

    public bool noteHit;

    public float elapsed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        elapsed = 0;
        noteHit = false;
        hitSound.SetActive(false);
        wrongSound.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!gm.gameActive) {
            return;
        }
        if (noteHit) {
            Renderer r = gameObject.GetComponent<MeshRenderer>();
            r.enabled = false;
        }
        if (elapsed > duration + delay) {
            if (realNote && !noteHit) {
                gm.addScore(-5);
            }
            Destroy(gameObject);
        }

        if (realNote) {
            foreach (GameObject x in xs) {
                x.SetActive(false);
            }
            foreach (GameObject x in circles) {
                x.SetActive((elapsed > delay) && !noteHit);
            }
            foreach (GameObject x in indicators) {
                x.SetActive((elapsed > delay) && !noteHit);
                x.transform.localScale = new Vector3(1.0f - (elapsed - delay) / duration, 0.54f, 1.0f - (elapsed - delay) / duration);
            }
        } else {
            foreach (GameObject x in xs) {
                x.SetActive(!noteHit);
            }
            foreach (GameObject x in circles) {
                x.SetActive(false);
            }
            foreach (GameObject x in indicators) {
                x.SetActive(false);
            }
        }
        elapsed += Time.deltaTime;
    }

    public void OnTriggerEnter(Collider c) {
        // Debug.Log(c.gameObject.name);
        if (c.gameObject.name == "RealLaser") {
            hitNote();
        }
    }

    public void hitNote() {
        // Debug.Log("hitNote");
        if (elapsed < delay) {
            return;
        }
        noteHit = true;
        if (realNote) {
            hitSound.SetActive(true);
            int scoreToAdd = (int) (10 - 20 * Mathf.Abs(elapsed - delay - duration / 2f));
            scoreToAdd = (scoreToAdd > 1) ? scoreToAdd : 1;
            scoreToAdd = (scoreToAdd > 8) ? 10 : scoreToAdd;
            // Debug.Log(scoreToAdd);
            gm.addScore(scoreToAdd);
            // Destroy(gameObject);
        } else {
            wrongSound.SetActive(true);
            gm.addScore(-10);
            // Destroy(gameObject);
        }
    }
}
