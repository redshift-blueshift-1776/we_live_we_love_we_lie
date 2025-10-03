using System.Collections;
using UnityEngine;

public class CrumblingPlatform : MonoBehaviour
{
    private bool isCrumbling;
    private Color originalColor;
    private BoxCollider blockCollider;
    private Renderer blockRenderer;
    private float fadeTime;
    private float recoveryTime;
    public PlayerMovement playerMovement;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isCrumbling = false;
        blockCollider = gameObject.GetComponent<BoxCollider>();
        blockRenderer = gameObject.GetComponent<Renderer>();
        originalColor = blockRenderer.material.color;
        fadeTime = 2.0f;
        recoveryTime = 5.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool isBlockCrumbling()
    {
        return isCrumbling;
    }

    public void setBlockCrumbling(bool b)
    {
        isCrumbling = b;
    }

    public float getFadeTime()
    {
        return fadeTime;
    }

    public float getRecoveryTime()
    {
        return recoveryTime;
    }

    public IEnumerator crumble()
    {
        Color color = blockRenderer.material.color;
        float elapsed = 0;
        float duration = fadeTime;
        while (elapsed < duration)
        {
            float t = elapsed / duration;

            blockRenderer.material.color = Color.Lerp(color, new Color(color.r, color.g, color.b, 1 - t), t);

            elapsed += Time.deltaTime;
            yield return null;
        }
        blockRenderer.enabled = false;
        blockCollider.enabled = false;

        yield return new WaitForSeconds(recoveryTime);
        isCrumbling = false;
        blockCollider = gameObject.AddComponent<BoxCollider>();
        initialize();
    }

    public void initialize()
    {
        if (isCrumbling)
        {
            return;
        }
        isCrumbling = false;
        blockRenderer.enabled = true;
        blockCollider.enabled = true;
        blockRenderer.material.color = originalColor;
    }
}
