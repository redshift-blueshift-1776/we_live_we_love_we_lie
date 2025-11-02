using System.Collections;
using UnityEngine;

public class BulletTracer : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    private float lifeTime = 0.12f;
    private float travelTime = 0.04f;
    private Color startColor = Color.yellow;
    private Color endColor = Color.clear;
    
    public void Initialize(Vector3 start, Vector3 end, float duration = 0.12f)
    {
        lifeTime = duration;
        travelTime = Mathf.Min(travelTime, lifeTime * 0.5f);
        transform.position = Vector3.zero;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.04f;
        lineRenderer.useWorldSpace = true;

        Gradient gradient = new Gradient();
        gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(startColor, 0.0f), new GradientColorKey(startColor, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
            );
        lineRenderer.colorGradient = gradient;

        StartCoroutine(AnimateTracer(start, end));
    }

    private IEnumerator AnimateTracer(Vector3 start, Vector3 end)
    {
        float t = 0f;

        while (t < travelTime)
        {
            float ratio = t / travelTime;

            Vector3 currentEnd = Vector3.Lerp(start, end, ratio);
            lineRenderer.SetPosition(1, currentEnd);
            t += Time.deltaTime;
            yield return null;
        }

        lineRenderer.SetPosition(1, end);

        float remaining = Mathf.Max(0f, lifeTime - travelTime);
        float u = 0f;
        while (u < remaining)
        {
            float ratio = u / remaining;
            float width = Mathf.Lerp(lineRenderer.startWidth, lineRenderer.endWidth, ratio);
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width * 0.25f;
            u += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
