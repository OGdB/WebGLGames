using UnityEngine;

public class Segment : DestructableBlock
{
    [SerializeField]
    private float breakHealth = 50;

    /// <summary>
    /// Handles with consequences of being hit.
    /// </summary>
    /// <param name="hitPoint">world position where the gameobject was hit.</param>
    /// <param name="direction">the direction hit in.</param>
    /// <param name="weapon">The weapon used upon the block</param>
    public override void Punched(Vector2 hitPoint, Vector3 direction, WeaponSO weapon)
    {
        // Lower health
        base.Punched(hitPoint, direction, weapon);

        if (blockHealth <= breakHealth)
        {
            GameObject upperSegment = null;
            GameObject lowerSegment = null;
            bool split = DestructionManager.SplitSprite(transform, hitPoint, thisMaterial, ref upperSegment, ref lowerSegment);
            if (!split) return;

            BlackBoard.AmountOfBlocks--; // One block is now 2 blocks.
            upperSegment.GetComponent<Rigidbody2D>().AddForceAtPosition(direction * weapon.PushForce, hitPoint, ForceMode2D.Impulse);
            upperSegment.GetComponent<DestructableBlock>().SetHealth(blockHealth);
            lowerSegment.GetComponent<Rigidbody2D>().AddForceAtPosition(direction * weapon.PushForce, hitPoint, ForceMode2D.Impulse);
            lowerSegment.GetComponent<DestructableBlock>().SetHealth(blockHealth);
        }
    }
}
