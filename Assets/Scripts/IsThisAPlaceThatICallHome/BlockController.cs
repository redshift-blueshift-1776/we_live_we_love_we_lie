using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class BlockController : MonoBehaviour
{
    private BlockSide side;
    private BlockManager manager;
    private bool placed = false;

    private Renderer rend;
    private Rigidbody rb;

    // Pair control
    private static int activeUnplacedInLayer = 0;

    public void Init(BlockSide side, BlockManager manager)
    {
        this.side = side;
        this.manager = manager;
        activeUnplacedInLayer++;
    }

    void Start()
    {
        rend = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();

        // Ghost mode
        SetTransparency(0.5f);
        rb.isKinematic = true;
    }

    void Update()
    {
        if (placed) return;

        Vector3 move = Vector3.zero;

        if (side == BlockSide.Left)
        {
            if (Input.GetKey(KeyCode.A)) move.x -= 1;
            if (Input.GetKey(KeyCode.D)) move.x += 1;
            if (Input.GetKey(KeyCode.W)) move.z += 1;
            if (Input.GetKey(KeyCode.S)) move.z -= 1;
        }
        else 
        {
            if (Input.GetKey(KeyCode.LeftArrow)) move.x -= 1;
            if (Input.GetKey(KeyCode.RightArrow)) move.x += 1;
            if (Input.GetKey(KeyCode.UpArrow)) move.z += 1;
            if (Input.GetKey(KeyCode.DownArrow)) move.z -= 1;
        }

        transform.position += move * Time.deltaTime * 5f;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlaceBlock();
        }
    }

    void PlaceBlock()
    {
        placed = true;
        SetTransparency(1f);
        rb.isKinematic = false;

        activeUnplacedInLayer--;
        if (activeUnplacedInLayer <= 0)
        {
            // All blocks in this layer placed → spawn the next layer
            manager.SpawnNextBlock();
        }
    }

    private void SetTransparency(float alpha)
    {
        if (rend != null)
        {
            Color c = rend.material.color;
            c.a = alpha;
            rend.material.color = c;
        }
    }
}