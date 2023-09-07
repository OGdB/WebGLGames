using UnityEngine;

/// <summary>
/// Simple class for checking whether the user is grounded.
/// </summary>
public class GroundedChecker
{
    [SerializeField]
    private LayerMask solidLayers;  // Layers the player can stand on.
    [SerializeField]
    private Vector2 boxSize = Vector2.one;  // The size of the box ( full size )
    private Collider2D collider;

    // CONSTRUCTOR
    public GroundedChecker(LayerMask solidLayers, Vector2 boxSize, Collider2D collider)
    {
        this.solidLayers = solidLayers;
        this.boxSize = boxSize;
        this.collider = collider;
    }

    public bool IsGrounded()
    {
        Vector2 pos = new(collider.bounds.center.x, collider.bounds.min.y - boxSize.y / 2f);
        return Physics2D.OverlapBox(pos, boxSize, 0, solidLayers);
    }

    public void GroundedGizmos()
    {
        Gizmos.DrawSphere(collider.bounds.center, 0.05f);

        Gizmos.color = Color.green;
        Vector2 pos = new(collider.bounds.center.x, collider.bounds.min.y - boxSize.y / 2f);
        Gizmos.DrawSphere(pos, 0.05f);

        Gizmos.color = IsGrounded() ? Color.green : Color.red;
        Gizmos.DrawCube(pos, boxSize);
    }
}