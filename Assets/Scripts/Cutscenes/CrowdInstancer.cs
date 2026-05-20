using UnityEngine;

public class CrowdInstancer : MonoBehaviour
{
    public Mesh mesh;
    public Material material;

    public int count = 1000;

    Matrix4x4[] matrices;

    void Start()
    {
        matrices = new Matrix4x4[count];

        for (int i = 0; i < count; i++)
        {
            Vector3 pos =
                new Vector3(
                    Random.Range(-50f, 50f),
                    0,
                    Random.Range(-50f, 50f)
                );

            Quaternion rot =
                Quaternion.Euler(
                    0,
                    Random.Range(0, 360),
                    0
                );

            Vector3 scale =
                Vector3.one * Random.Range(0.8f, 1.2f);

            matrices[i] =
                Matrix4x4.TRS(pos, rot, scale);
        }
    }

    void Update()
    {
        Graphics.DrawMeshInstanced(
            mesh,
            0,
            material,
            matrices
        );
    }
}