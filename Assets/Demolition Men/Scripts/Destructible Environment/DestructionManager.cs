using UnityEngine;

public class DestructionManager : MonoBehaviour
{
    public static DestructionManager Singleton;

    private static float minimumSegmentSize = 0.65f;

    [SerializeField]
    private BlockMaterialToPrefab[] blocks;
    public BlockMaterialToPrefab[] Blocks { get { return blocks; } }

    private void Awake()
    {
        if (Singleton != null) return;
        Singleton = this;

        // Although blocks starting in the scene should auto-connect, they shouldn't when they segment.
        foreach (var block in blocks)
        {
            block.associatedPrefab.GetComponent<Connectable>().connect = false;
        }
    }

    /// <summary>
    /// Splits a block inherting from 'DestructableBlock' into two pieces. Designed for sprites with tiled draw modes.
    /// </summary>
    /// <param name="origTransform">The transform of the original sprite</param>
    /// <param name="hitPoint">The point at which the sprite should split.</param>
    /// <param name="material">The 'blockmaterial' of the DestructableBlock.</param>
    /// <param name="lowerSegment">The lower segment to return.</param>
    /// <param name="upperSegment">The upper segment to return.</param>
    /// <returns>Returns whether a split on this block was possible.</returns>
    public static bool SplitSprite(Transform origTransform, Vector2 hitPoint, BlockMaterial material, out GameObject lowerSegment, out GameObject upperSegment)
    {
        // RETRIEVALS
        GameObject blockPrefab = GetPrefab(material);
        SpriteRenderer origSpr = origTransform.GetComponent<SpriteRenderer>();
        Vector2 origSize = origSpr.size;
        Quaternion origRotation = origTransform.rotation;
        float yExtent = (origSize.y / 2);

        Vector2 splitCenter = origTransform.InverseTransformPoint(hitPoint);
        splitCenter.x = 0f;
        splitCenter = origTransform.TransformPoint(splitCenter);

        // CALCULATIONS
        // 1. Get upper and lower bound of original block.  ~~ Is not 'local' in .bounds in spr/col.
        Vector2 upperBound = origTransform.position + origTransform.up * yExtent;
        Vector2 lowerBound = origTransform.position - origTransform.up * yExtent;

        // 2. Calculate the height of the upper- and lower sub-segment.
        float upperHeight = Vector2.Distance(upperBound, splitCenter);
        float lowerHeight = Vector2.Distance(lowerBound, splitCenter);

        // 2.1 Are the heights of the sub-segments above minimum size?
        bool bigEnough = upperHeight >= minimumSegmentSize && lowerHeight >= minimumSegmentSize;
        if (!bigEnough)
        {
            Debug.Log("One of the sub-segments wasn't big enough");
            lowerSegment = null;
            upperSegment = null;
            return false;
        }

        // 3. Calculate the position of the upper- and lower sub-segment.
        Vector2 upperPos = splitCenter + (Vector2)origTransform.up * upperHeight / 2;
        Vector2 lowerPos = splitCenter + -(Vector2)origTransform.up * lowerHeight / 2;

        // ASSIGNMENTS
        // 4. Instantiate and set the calculated variables.
        // Prefab, position, rotation, name
        origTransform.gameObject.SetActive(false);

        upperSegment = Instantiate(blockPrefab, upperPos, origRotation);
        lowerSegment = Instantiate(blockPrefab, lowerPos, origRotation);
        upperSegment.name = "Upper Segment";
        lowerSegment.name = "Lower Segment";

        // Size
        upperSegment.transform.localScale = origTransform.localScale;
        lowerSegment.transform.localScale = origTransform.localScale;
        upperSegment.GetComponent<SpriteRenderer>().size = new(origSize.x, upperHeight);
        lowerSegment.GetComponent<SpriteRenderer>().size = new(origSize.x, lowerHeight);

        Connectable originalConnectable = origTransform.GetComponent<Connectable>();
        // 5. Set connections of the sub-segments.
        if (originalConnectable.Connections.ContainsKey(Direction.Up))
            upperSegment.GetComponent<Connectable>().FindConnections(
                left: true,
                right: true,
                top: true,
                bottom: false);
        if (originalConnectable.Connections.ContainsKey(Direction.Down))
            lowerSegment.GetComponent<Connectable>().FindConnections(
                left: true,
                right: true,
                top: false,
                bottom: true);

        // 6. Disable original segment.

        return true;
    }

    private static GameObject GetPrefab(BlockMaterial material)
    {
        foreach (var block in Singleton.Blocks)
        {
            if (block.blockMaterial == material)
            {
                return block.associatedPrefab;
            }
        }

        // If this is reached, no prefab instance was found.
        Debug.LogError("No prefab associated with the material was found.");
        return null;
    }
}

[System.Serializable]
public class BlockMaterialToPrefab
{
    public BlockMaterial blockMaterial;
    public GameObject associatedPrefab;
}
