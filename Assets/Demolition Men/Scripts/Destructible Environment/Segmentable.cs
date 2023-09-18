using UnityEngine;

/// <summary>
/// This block can be broken up into segments when hit or falling hard enough.
/// </summary>
[RequireComponent (typeof(Rigidbody2D), typeof(Collider2D))]
public class Segmentable : DestructableBlock
{
    [SerializeField]
    private float breakHealth = 50;
    [SerializeField]
    private float segmentationVelocity = 11f;

    private bool hasSegmented = false;

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.magnitude > segmentationVelocity && !hasSegmented)
        {
            hasSegmented = true;
            print("Called on " + transform.name);
            Vector2 hitPoint = collision.contacts[0].point;
            bool split = DestructionManager.SplitSprite(transform, hitPoint, thisMaterial, out var lowerSegment, out var upperSegment);

            if (!split) return;

            lowerSegment.GetComponent<DestructableBlock>().SetHealth(blockHealth);
            upperSegment.GetComponent<DestructableBlock>().SetHealth(blockHealth);
            return;
        }


        base.OnCollisionEnter2D(collision);

    }
   
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
            bool split = DestructionManager.SplitSprite(transform, hitPoint, thisMaterial, out var lowerSegment, out var upperSegment);

            if (!split) return;

            BlackBoard.AmountOfBlocks--; // One block is now 2 blocks.
            upperSegment.GetComponent<Rigidbody2D>().AddForceAtPosition(direction * weapon.PushForce, hitPoint, ForceMode2D.Impulse);
            upperSegment.GetComponent<DestructableBlock>().SetHealth(blockHealth);
            lowerSegment.GetComponent<Rigidbody2D>().AddForceAtPosition(direction * weapon.PushForce, hitPoint, ForceMode2D.Impulse);
            lowerSegment.GetComponent<DestructableBlock>().SetHealth(blockHealth);
        }
    }
}
