using System.Collections.Generic;
using UnityEngine;

public class Connectable : MonoBehaviour
{
    [SerializeField]
    private float rayLength = 0.1f;
    public Rigidbody2D rb;

    // Joints which have a connection to this block.
    public Dictionary<Direction, FixedJoint2D> Connections = new();

    [SerializeField]
    private float breakForce = 2000f;

    public bool testBool = false;
    public bool connect = true;

    private void Awake()
    {
        SpriteRenderer _spr = GetComponent<SpriteRenderer>();
        rb.mass = _spr.size.x * _spr.size.y;
    }

    private void OnEnable()
    {
        if (!connect) return;

        bool hasConnections = FindConnections(true, true, true, true);

        if (hasConnections)
        {
            rb.Sleep();
        }
    }

    public bool FindConnections(bool left, bool right, bool top, bool bottom)
    {
        List<RaycastHit2D> hits = new();
        Collider2D col = GetComponent<Collider2D>();
        int mask = 1 << LayerMask.NameToLayer("Objects");
        gameObject.layer = LayerMask.NameToLayer("Default");

        void TryRaycast(Vector2 origin, Vector2 direction, Direction connectionDirection)
        {
            RaycastHit2D hit = Physics2D.Raycast(origin, direction, rayLength, mask);
            if (hit && hit.transform.TryGetComponent(out Connectable connectable))
            {
                hits.Add(hit);
                FixedJoint2D newJoint = CreateJoint(transform.InverseTransformPoint(hit.point), hit.rigidbody);
                connectable.Connections[connectionDirection] = newJoint;
            }
        }

        if (left)
        {
            Vector2 origin = new(col.bounds.min.x, col.bounds.center.y);
            TryRaycast(origin, -transform.right, Direction.Left);
        }
        if (right)
        {
            Vector2 origin = new(col.bounds.max.x, col.bounds.center.y);
            TryRaycast(origin, transform.right, Direction.Right);
        }
        if (top)
        {
            Vector2 origin = new(col.bounds.center.x, col.bounds.max.y);
            TryRaycast(origin, transform.up, Direction.Up);
        }
        if (bottom)
        {
            Vector2 origin = new(col.bounds.center.x, col.bounds.min.y);
            TryRaycast(origin, -transform.up, Direction.Down);
        }

        gameObject.layer = LayerMask.NameToLayer("Objects");
        return hits.Count > 0;
    }


    /// <summary>
    /// Local anch
    /// </summary>
    /// <param name="anchorPosition">Local anchor position.</param>
    /// <param name="connectedRigidBody"></param>
    /// <returns></returns>
    private FixedJoint2D CreateJoint(Vector2 anchorPosition, Rigidbody2D connectedRigidBody)
    {
        FixedJoint2D newJoint = gameObject.AddComponent<FixedJoint2D>();

        newJoint.anchor = anchorPosition;
        newJoint.connectedBody = connectedRigidBody;
        newJoint.enableCollision = true;
        newJoint.dampingRatio = 1f;
        newJoint.breakForce = breakForce;
        newJoint.breakAction = JointBreakAction2D.Disable;

        return newJoint;
    }

    public void BreakConnection()
    {
        // Break connections within this gameobject.
        foreach (FixedJoint2D joint in GetComponents<FixedJoint2D>())
        {
            joint.enabled = false;
        }
        // Break external connections to this gameobject.
        foreach (FixedJoint2D joint in Connections.Values)
        {
            joint.enabled = false;
        }
        Connections.Clear();
    }

    private void OnValidate()
    {
        rb = GetComponent<Rigidbody2D>();
    }
}
