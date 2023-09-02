using UnityEngine;

public class Segment : DestructableBlock
{
    [SerializeField]
    private float breakHealth = 50;

    protected override void Start()
    {
        base.Start();
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
            GameObject upperSegment = null;
            GameObject lowerSegment = null;
            bool split = DestructionManager.SplitSprite(transform, hitPoint, thisMaterial, ref upperSegment, ref lowerSegment);
            if (!split) return;

            upperSegment.GetComponent<Rigidbody2D>().AddForceAtPosition(direction * weapon.PushForce, hitPoint, ForceMode2D.Impulse);
            upperSegment.GetComponent<DestructableBlock>().SetHealth(blockHealth);
            lowerSegment.GetComponent<Rigidbody2D>().AddForceAtPosition(direction * weapon.PushForce, hitPoint, ForceMode2D.Impulse);
            lowerSegment.GetComponent<DestructableBlock>().SetHealth(blockHealth);
        }
    }

    protected override void OnValidate()
    {
        base.OnValidate();
    }

    protected override void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.yellow;

/*        Gizmos.color = Color.white;
        Vector2 punchCenter = new(spr.bounds.center.x, lastPunchPosition.y);
        Gizmos.DrawSphere(punchCenter, 0.1f);

        Gizmos.color = Color.yellow;
        Vector2 punchUp = punchCenter;
        punchUp += (Vector2)transform.up * minimumSegmentSize;
        Gizmos.DrawSphere(punchUp, 0.15f);

        Vector2 punchDown = punchCenter;
        punchDown -= (Vector2)transform.up * minimumSegmentSize;
        Gizmos.DrawSphere(punchDown, 0.15f);*/
    }
}
