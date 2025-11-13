using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RisingLava : MonoBehaviour
{
    [SerializeField] private Player5Easy playerScript;
    [SerializeField] private float initialDelay;
    [SerializeField] private float secondsUntilTop;

    [SerializeField] private Image lavaScreen;
    [SerializeField] private Image deathScreen;

    [SerializeField] private GameObject lava;
    [SerializeField] private GameObject lightObject;

    private float topY = 181;
    void Start()
    {
        lavaScreen.gameObject.SetActive(false);
        deathScreen.gameObject.SetActive(false);
        StartCoroutine(rise());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerScript.kill();
            StartCoroutine(deathScene());
        }
    }

    private IEnumerator rise()
    {
        yield return new WaitForSeconds(initialDelay);

        float bottomY = transform.position.y;
        float initialScaleY = lava.transform.localScale.y;
        float targetScaleY = topY - bottomY;

        float t = 0;
        while (t < secondsUntilTop)
        {
            t += Time.deltaTime;
            float deltaScale = (targetScaleY - initialScaleY) / secondsUntilTop * Time.deltaTime;

            Vector3 scale = lava.transform.localScale;
            scale.y += deltaScale;
            lava.transform.localScale = scale;

            Vector3 pos = lava.transform.position;
            pos.y = bottomY + scale.y / 2f;
            lava.transform.position = pos;

            lightObject.transform.position += Vector3.up * deltaScale;
            yield return null;
        }

        lava.transform.localScale = new Vector3(lava.transform.localScale.x, targetScaleY, lava.transform.localScale.z);
        lava.transform.position = new Vector3(lava.transform.position.x, bottomY + targetScaleY / 2f, lava.transform.position.z);
    }

    private const float fadeoutTime = 2f;
    private IEnumerator deathScene()
    {
        lavaScreen.gameObject.SetActive(true);
        deathScreen.gameObject.SetActive(true);

        Color color = deathScreen.color;
        float r = color.r;
        float g = color.g;
        float b = color.b;

        float t = 0;
        while (t < fadeoutTime)
        {
            t += Time.deltaTime;
            deathScreen.color = new Color(r, g, b, t / fadeoutTime);
            yield return null;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        yield return null;
    }
}
