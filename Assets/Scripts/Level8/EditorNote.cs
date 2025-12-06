using UnityEngine;

public class EditorNote : MonoBehaviour
{
    public float beat;
    public float x;
    public float y;

    private Renderer rend;
    private Color originalColor;

    void Start()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
    }

    public void SetSelected(bool sel)
    {
        if (rend == null) rend = GetComponent<Renderer>();
        rend.material.color = sel ? Color.yellow : originalColor;
    }
}