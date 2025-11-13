using UnityEngine;

public class SquareRing : MonoBehaviour
{
    [SerializeField] public GameObject[] walls;
    public float H;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Color targetColor = Color.HSVToRGB(H % 1.0f, 1, 1);
        foreach (GameObject wall in walls) {
            Renderer r = wall.GetComponent<MeshRenderer>();
            r.material.color = targetColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
