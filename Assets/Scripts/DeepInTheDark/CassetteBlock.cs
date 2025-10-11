using System.Collections;
using UnityEngine;

public class CassetteBlock : MonoBehaviour
{
    public Renderer blockRenderer;
    public BoxCollider blockCollider;
    private IEnumerator currCoroutine;
    public bool visible = true;
    public int clickType;
    private string soundName;

    public GameObject musicArea1;
    public GameObject musicArea2;
    private AudioArea area1;
    private AudioArea area2;

    public AudioManager audioManager;

    private const float fullInterval = 8f / 3f;

    void Start()
    {
        area1 = musicArea1.GetComponent<AudioArea>();
        area2 = musicArea2.GetComponent<AudioArea>();
        soundName = clickType == 1 ? "click1" : "click2";
        startProcess();
    }

    // Update is called once per frame
    void Update()
    {
        Color color = blockRenderer.material.color;
        blockRenderer.material.color = new Color(color.r, color.g, color.b, visible ? 1f : 0.1f);
        blockCollider.enabled = visible;
    }

    public void startProcess()
    {
        if (currCoroutine != null)
        {
            StopCoroutine(currCoroutine);
        }
        currCoroutine = appearAndDisappear();
        StartCoroutine(startClicking());
        StartCoroutine(currCoroutine);
    }

    public IEnumerator appearAndDisappear()
    {
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
    //public IEnumerator startClicking()
    //{
    //    float t = 0;
    //    float quarterInterval = fullInterval / 4f;
    //    int quarterCount = 0;
    //    while (true)
    //    {
    //        t += Time.deltaTime;
    //        if (t >= quarterInterval)
    //        {
    //            t = 0;
    //            float vol1 = area1.getVolume();
    //            float vol2 = area2.getVolume();

    //            float maxVol = Mathf.Max(vol1, vol2);

    //            if (maxVol > 0)
    //            {
    //                if ((clickType == 1 && quarterCount % 2 == 0) || (clickType != 1 && quarterCount % 2 == 1))
    //                {
    //                    audioManager.setVolume(soundName, maxVol);
    //                    audioManager.playSound(soundName);
    //                }
    //            }
    //            quarterCount = (quarterCount + 1) % 4;
    //        }

    //        yield return null;
    //    }
    //}
    public IEnumerator startClicking()
    {
        float startTime = Time.time;
        float quarterInterval = fullInterval / 4f;
        int lastQuarter = Mathf.FloorToInt((Time.time - startTime) / quarterInterval) % 4;

        while (true)
        {
            float elapsed = Time.time - startTime;
            int currentQuarter = Mathf.FloorToInt(elapsed / quarterInterval) % 4;

            // Only trigger when we move to a new quarter
            if (currentQuarter != lastQuarter)
            {
                lastQuarter = currentQuarter;

                float vol1 = area1.getVolume();
                float vol2 = area2.getVolume();
                float maxVol = Mathf.Max(vol1, vol2);

                if (maxVol > 0)
                {
                    if ((clickType == 1 && currentQuarter % 2 == 0) || (clickType != 1 && currentQuarter % 2 == 1))
                    {
                        audioManager.setVolume(soundName, maxVol);
                        audioManager.playSound(soundName);
                    }
                }
            }

            yield return null;
        }
    }
}
