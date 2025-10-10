using System.Collections;
using UnityEngine;

public class CassetteBlock : MonoBehaviour
{
    public Renderer blockRenderer;
    public BoxCollider blockCollider;
    private IEnumerator currCoroutine;
    public float delay;
    public bool visible = true;
    public int clickType;
    private string soundName;

    public GameObject musicArea1;
    //public GameObject musicArea2;
    private AudioArea area1;
    //public AudioArea area2;

    public AudioManager audioManager;

    private const float fullInterval = 8f / 3f;

    void Start()
    {
        area1 = musicArea1.GetComponent<AudioArea>();
        soundName = clickType == 1 ? "click1" : "click2";
        startProcess();
    }

    // Update is called once per frame
    void Update()
    {
        blockRenderer.enabled = visible;
        blockCollider.enabled = visible;
    }

    public void startProcess()
    {
        if (currCoroutine != null)
        {
            StopCoroutine(currCoroutine);
        }
        currCoroutine = appearAndDisappear(delay);
        StartCoroutine(startClicking());
        StartCoroutine(currCoroutine);
    }

    public IEnumerator appearAndDisappear(float d)
    {
        yield return new WaitForSeconds(d);
        float t = 0;
        while (true)
        {
            t += Time.deltaTime;
            if (t >= fullInterval)
            {
                visible = !visible;
                t = 0;
            }
            yield return null;
        }
    }
    public IEnumerator startClicking()
    {
        float t = 0;
        float quarterInterval = fullInterval / 4f;
        int quarterCount = 0;
        while (true)
        {
            t += Time.deltaTime;
            if (t >= quarterInterval)
            {
                t = 0;
                float vol1 = area1.getVolume();
                if (vol1 > 0)
                {
                    if ((clickType == 1 && quarterCount % 2 == 0) || (clickType != 1 && quarterCount % 2 == 1))
                    {
                        audioManager.setVolume(soundName, area1.getVolume());
                        audioManager.playSound(soundName);
                    }
                }
                quarterCount = (quarterCount + 1) % 4;
            }

            yield return null;
        }
    }
}
