using UnityEngine;
using System.Collections.Generic;

public class BlockManager : MonoBehaviour
{
    [Header("Block Prefabs (single or pair prefabs)")]
    [SerializeField] private List<GameObject> blockPrefabs;

    [Header("Possible Block Pairs (randomized phase)")]
    [SerializeField] private List<BlockPairData> blockPairs;

    [Header("Spawn Points (XZ positions only)")]
    [SerializeField] private Transform leftSpawn;
    [SerializeField] private Transform rightSpawn;

    [Header("Stacking Settings")]
    [SerializeField] private float layerHeight = 3f;   // height of each layer
    [SerializeField] private float baseOffset = 1f;    // how high the first layer is off the ground
    [SerializeField] private int numLayers = 12;

    [Header("Hardcoded Sequence (index of prefab, side)")]
    [SerializeField] private List<BlockPairData> hardcodedBlocks;

    [SerializeField] private GameObject gameManager;
    private IsThisAPlaceThatICallHome gm;

    private int blockIndex = 0;
    public int currentLayer = 0;  

    void Start()
    {
        gm = gameManager.GetComponent<IsThisAPlaceThatICallHome>();
        SpawnNextBlock();
    }

    public void SpawnNextBlock()
    {
        if (currentLayer < numLayers) {
            if (blockIndex < hardcodedBlocks.Count)
            {
                BlockPairData pair = hardcodedBlocks[blockIndex];
                SpawnBlock(pair.leftBlock, BlockSide.Left);
                SpawnBlock(pair.rightBlock, BlockSide.Right);
                blockIndex++;
                currentLayer++;
                gm.MoveCameraUp(currentLayer);
            }
            else
            {
                BlockPairData pair = blockPairs[Random.Range(0, blockPairs.Count)];

                // Each side independently
                SpawnBlock(pair.leftBlock, BlockSide.Left);
                SpawnBlock(pair.rightBlock, BlockSide.Right);

                currentLayer++;
                blockIndex++;
                gm.MoveCameraUp(currentLayer);
            }
        }
    }


    private void SpawnBlock(BlockSpawnData data, BlockSide side)
    {
        if (data == null || data.prefab == null) return; // nothing to spawn

        Transform baseSpawn = side == BlockSide.Left ? leftSpawn : rightSpawn;
        float yPos = baseOffset + currentLayer * layerHeight;
        Vector3 spawnPos = new Vector3(baseSpawn.position.x, yPos, baseSpawn.position.z);

        // default rotation
        Quaternion rot = Quaternion.identity;

        // random rotation from this block's list
        if (data.allowedRotations != null && data.allowedRotations.Count > 0)
        {
            Vector3 rotEuler = data.allowedRotations[Random.Range(0, data.allowedRotations.Count)];
            rot = Quaternion.Euler(rotEuler);
        }

        var blockObj = Instantiate(data.prefab, spawnPos, rot);
        var controller = blockObj.AddComponent<BlockController>();
        controller.Init(side, this);
    }

    private void SpawnSingle(GameObject prefab, BlockSide side, List<Vector3> allowedRotations = null)
    {
        Transform baseSpawn = side == BlockSide.Left ? leftSpawn : rightSpawn;
        float yPos = baseOffset + currentLayer * layerHeight;
        Vector3 spawnPos = new Vector3(baseSpawn.position.x, yPos, baseSpawn.position.z);

        Quaternion rot = Quaternion.identity;
        if (allowedRotations != null && allowedRotations.Count > 0)
        {
            Vector3 rotEuler = allowedRotations[Random.Range(0, allowedRotations.Count)];
            rot = Quaternion.Euler(rotEuler);
        }

        var blockObj = Instantiate(prefab, spawnPos, rot);
        var controller = blockObj.AddComponent<BlockController>();
        controller.Init(side, this);
    }


}

[System.Serializable]
public class BlockSpawnData
{
    public GameObject prefab;                     // can be null
    public List<Vector3> allowedRotations = new List<Vector3>();
}

[System.Serializable]
public class BlockPairData
{
    public BlockSpawnData leftBlock;
    public BlockSpawnData rightBlock;
}

public enum BlockSide { Left, Right }