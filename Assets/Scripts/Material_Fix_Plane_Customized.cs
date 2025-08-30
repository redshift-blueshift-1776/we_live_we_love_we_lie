using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Material_Fix_Plane_Customized : MonoBehaviour
{
    [SerializeField] public float tileSizeX;
    [SerializeField] public float tileSizeZ;
    // Start is called before the first frame update
    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        Vector3 scale = transform.lossyScale;

        renderer.material = new Material(renderer.material);

        Vector2 tiling = new Vector2(scale.x / tileSizeX, scale.z / tileSizeZ);
        renderer.material.mainTextureScale = tiling;

        // Center the texture
        renderer.material.mainTextureOffset = new Vector2(0.5f * (1 - tiling.x), 0.5f * (1 - tiling.y));
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
