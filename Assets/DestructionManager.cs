using UnityEngine;

public class DestructionManager : MonoBehaviour
{
    public static DestructionManager Singleton;


    [SerializeField]
    private BlockMaterialToPrefab[] blocks;
    public BlockMaterialToPrefab[] Blocks { get { return blocks; } }

    private void Awake()
    {
        if (Singleton != null) return;
        Singleton = this;
    }

    public static void SplitSprite(GameObject originalGo, float splitY, BlockMaterial material, ref GameObject upperSegment, ref GameObject lowerSegment)
    {
        GameObject blockPrefab = null;
        foreach (var block in Singleton.Blocks)
        {
            if (block.blockMaterial == material)
            {
                blockPrefab = block.associatedPrefab;
            }
        }
        if (!blockPrefab)
        {
            Debug.LogError("No prefab associated to the material was found.");
            return;
        }

        if (!originalGo.TryGetComponent<SpriteRenderer>(out var originalRenderer))
        {
            Debug.LogError("The original GameObject does not have a SpriteRenderer component.");
            return;
        }

        // Calculate the size of the upper and lower parts
        Vector3 upperSize = new(originalRenderer.size.x, splitY - originalRenderer.bounds.min.y, 1f);
        Vector3 lowerSize = new(originalRenderer.size.x, originalRenderer.bounds.max.y - splitY, 1f);

                // Set the size and position of the upper and lower parts
        Vector3 upperPos = new(originalRenderer.bounds.center.x, splitY - upperSize.y / 2, 0f);
        Vector3 lowerPos = new(originalRenderer.bounds.center.x, splitY + lowerSize.y / 2, 0f);

        Quaternion rotation = originalGo.transform.rotation;

        // Create two new GameObjects for the split parts
        upperSegment = Instantiate(blockPrefab, upperPos, rotation);
        Connectable upperConnectable = upperSegment.GetComponent<Connectable>();
        upperConnectable.connect = false;
        upperConnectable.FindConnections(true, true, true, false);

        lowerSegment = Instantiate(blockPrefab, lowerPos, rotation);
        Connectable lowerConnectable = lowerSegment.GetComponent<Connectable>();
        lowerConnectable.connect = false;
        upperConnectable.FindConnections(true, true, false, true);

        upperSegment.GetComponent<SpriteRenderer>().size = upperSize;
        lowerSegment.GetComponent<SpriteRenderer>().size = upperSize;

        originalGo.SetActive(false);
    }
}

[System.Serializable]
public class BlockMaterialToPrefab
{
    public BlockMaterial blockMaterial;
    public GameObject associatedPrefab;
}
