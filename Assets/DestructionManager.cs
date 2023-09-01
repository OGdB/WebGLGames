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

    public static void SplitSprite(GameObject originalGo, float splitHeightPosition, BlockMaterial material, ref GameObject lowerSegment, ref GameObject upperSegment)
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
            Debug.LogError("No prefab associated with the material was found.");
            return;
        }

        if (!originalGo.TryGetComponent<SpriteRenderer>(out var originalRenderer))
        {
            Debug.LogError("The original GameObject does not have a SpriteRenderer component.");
            return;
        }

        // Calculate the size and position of the upper and lower parts
        Vector3 originalSize = originalRenderer.bounds.size;
        float upperHeight = originalSize.y - (splitHeightPosition - originalRenderer.bounds.min.y);
        float lowerHeight = splitHeightPosition - originalRenderer.bounds.min.y;

        Vector3 upperSize = new Vector3(originalSize.x, upperHeight, 1f);
        Vector3 lowerSize = new Vector3(originalSize.x, lowerHeight, 1f);

        Vector3 upperPos = originalRenderer.bounds.center + Vector3.up * (upperHeight * 0.5f);
        Vector3 lowerPos = originalRenderer.bounds.center - Vector3.up * (lowerHeight * 0.5f);

        Quaternion rotation = originalGo.transform.rotation;

        // Create two new GameObjects for the split parts
        // Lower
        lowerSegment = Instantiate(blockPrefab, lowerPos, rotation);
        lowerSegment.GetComponent<SpriteRenderer>().size = lowerSize;
        Connectable lowerConnectable = lowerSegment.GetComponent<Connectable>();
        lowerConnectable.connect = false;
        lowerConnectable.FindConnections(true, true, false, true);
        lowerSegment.name = "Lower Segment";

        // Upper
        upperSegment = Instantiate(blockPrefab, upperPos, rotation);
        upperSegment.GetComponent<SpriteRenderer>().size = upperSize;
        Connectable higherConnectable = upperSegment.GetComponent<Connectable>();
        higherConnectable.connect = false;
        higherConnectable.FindConnections(true, true, true, false);
        upperSegment.name = "Upper Segment";


        originalGo.SetActive(false);
    }

}

[System.Serializable]
public class BlockMaterialToPrefab
{
    public BlockMaterial blockMaterial;
    public GameObject associatedPrefab;
}
