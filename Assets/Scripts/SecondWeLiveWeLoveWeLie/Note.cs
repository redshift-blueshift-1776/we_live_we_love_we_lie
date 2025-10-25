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

    public float elapsed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        elapsed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gm.gameActive) {
            return;
        }
        if (elapsed > duration + delay) {
            gm.addScore(-10);
            Destroy(gameObject);
        }

        if (realNote) {
            foreach (GameObject x in xs) {
                x.SetActive(false);
            }
            foreach (GameObject x in circles) {
                x.SetActive(elapsed > delay);
            }
            foreach (GameObject x in indicators) {
                x.SetActive(elapsed > delay);
                x.transform.localScale = new Vector3(1.0f - (elapsed - delay) / duration, 0.54f, 1.0f - (elapsed - delay) / duration);
            }
        } else {
            foreach (GameObject x in xs) {
                x.SetActive(true);
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
        if (realNote) {
            int scoreToAdd = (int) (10 - 20 * Mathf.Abs(elapsed - delay - duration / 2f));
            scoreToAdd = (scoreToAdd > 0) ? scoreToAdd : 0;
            // Debug.Log(scoreToAdd);
            gm.addScore(scoreToAdd);
            Destroy(gameObject);
        } else {
            gm.addScore(-10);
            Destroy(gameObject);
        }
    }
}
