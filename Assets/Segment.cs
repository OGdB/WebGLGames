using Unity.VisualScripting;
using UnityEngine;

public class Segment : DestructibleBlock
{
    Collider2D col;
    Vector3 punchPos;

    [SerializeField]
    float segmentationSize = 0.65f;

    protected override void Start()
    {
        base.Start();
        col = GetComponent<Collider2D>();
    }
    public override void Punched(Vector2 position, Vector3 direction, WeaponSO weapon)
    {
        punchPos = position;

        // If there is at least a distance of 0.5f on either side of the punch intersection.
        float upHeight = col.bounds.max.y - position.y;
        print($"upValue: {upHeight}");
        bool upSide = upHeight >= 0.8f;

        float downHeight = Mathf.Abs(col.bounds.min.y - position.y);
        print($"downValue: {downHeight}");
        bool downSide = downHeight >= 0.8f;

        if (upSide && downSide)
        {
            GameObject upperSegment = null;
            GameObject lowerSegment = null;

            DestructionManager.SplitSprite(gameObject, punchPos.y, thisMaterial, ref upperSegment, ref lowerSegment);

            upperSegment.GetComponent<Rigidbody2D>().AddForceAtPosition(direction * weapon.PushForce * 5f, position, ForceMode2D.Impulse);
            lowerSegment.GetComponent<Rigidbody2D>().AddForceAtPosition(direction * weapon.PushForce * 5f, position, ForceMode2D.Impulse);

        }
        else
        {
            print("Too small segment");
        }
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        col = GetComponent<Collider2D>();
    }

    protected override void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Vector2 punchCenter = new(col.bounds.center.x, punchPos.y);
        Gizmos.DrawSphere(punchCenter, 0.15f);

        Vector2 punchUp = punchCenter;
        punchUp += (Vector2)transform.up * segmentationSize;
        Gizmos.DrawSphere(punchUp, 0.15f);

        Vector2 punchDown = punchCenter;
        punchDown -= (Vector2)transform.up * segmentationSize;
        Gizmos.DrawSphere(punchDown, 0.15f);
    }
}
