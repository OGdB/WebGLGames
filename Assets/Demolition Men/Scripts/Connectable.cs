using System.Collections.Generic;
using UnityEngine;

public class Connectable : MonoBehaviour
{
    [SerializeField]
    private float rayLength = 0.1f;
    public Rigidbody2D rb;
    public AudioSource audioSource;

    // Joints which have a connection to this block.
    public List<FixedJoint2D> connectedJoints = new();

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
            if (testBool)
                print("Put to sleep");
        }
    }

    public bool FindConnections(bool left, bool right, bool top, bool bottom)
    {
        List<RaycastHit2D> hits = new();

        Collider2D col = GetComponent<Collider2D>();

        int mask = 1 << LayerMask.NameToLayer("Objects");

        gameObject.layer = LayerMask.NameToLayer("Default");

        if (left)
        {
            Vector2 origin = new Vector2(col.bounds.min.x, col.bounds.center.y);
            hits.Add(Physics2D.Raycast(origin, -transform.right, rayLength, mask));
            Debug.DrawRay(origin, -transform.right * rayLength, Color.yellow, 7f);
        }
        if (right)
        {
            Vector2 origin = new Vector2(col.bounds.max.x, col.bounds.center.y);
            hits.Add(Physics2D.Raycast(origin, transform.right, rayLength, mask));
            Debug.DrawRay(origin, transform.right * rayLength, Color.yellow, 7f);

        }
        if (top)
        {
            Vector2 origin = new Vector2(col.bounds.center.x, col.bounds.max.y);
            hits.Add(Physics2D.Raycast(origin, transform.up, rayLength, mask));
            Debug.DrawRay(origin, transform.up * rayLength, Color.yellow, 7f);

        }
        if (bottom)
        {
            Vector2 origin = new Vector2(col.bounds.center.x, col.bounds.min.y);
            hits.Add(Physics2D.Raycast(origin, -transform.up, rayLength, mask));
            Debug.DrawRay(origin, -transform.up * rayLength, Color.yellow, 7f);

        }

        gameObject.layer = LayerMask.NameToLayer("Objects");

        bool hasConnections = false;

        foreach (RaycastHit2D hit in hits)
        {
            if (hit)
            {
                hit.transform.TryGetComponent(out Connectable conn);
                if (!conn) continue;
                hasConnections = true;

                // For each surrounding destructible block, add a fixed joint connecting to that block;
                FixedJoint2D newJoint = gameObject.AddComponent<FixedJoint2D>();

                newJoint.anchor = transform.InverseTransformPoint(hit.point);
                newJoint.connectedBody = hit.rigidbody;
                newJoint.enableCollision = true;
                newJoint.dampingRatio = 1f;
                newJoint.breakForce = breakForce;
                newJoint.breakAction = JointBreakAction2D.Disable;

                // Add this joint to this block's list of connectedjoints.
                conn.connectedJoints.Add(newJoint);
            }
        }

        return hasConnections;

    }

    public void BreakConnection()
    {
        rb.mass = 3f;
        // Break connections within this gameobject.
        foreach (FixedJoint2D joint in GetComponents<FixedJoint2D>())
        {
            joint.enabled = false;
        }
        // Break external connections to this gameobject.
        foreach (var joint in connectedJoints)
        {
            joint.enabled = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.magnitude > 8f)
        {
            audioSource.Play();
        }
    }

    private void OnValidate()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }
}
