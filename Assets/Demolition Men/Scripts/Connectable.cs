using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Connectable : MonoBehaviour
{
    public Rigidbody2D rb;
    public AudioSource audioSource;

    // Joints which have a connection to this block.
    public List<FixedJoint2D> connectedJoints = new();

    [SerializeField]
    private float breakForce = 2000f;

    public bool testBool = false;
    public bool connect = true;

    private void OnEnable()
    {
        if (!connect) return;

        FindConnections(true, true, true, false);
    }

    public List<RaycastHit2D> FindConnections(bool left, bool right, bool top, bool bottom)
    {
        List<RaycastHit2D> hits = new();

        Collider2D col = GetComponent<Collider2D>();

        int mask = 1 << LayerMask.NameToLayer("Objects");

        gameObject.layer = LayerMask.NameToLayer("Default");

        if (left)
            hits.Add(Physics2D.Raycast(new Vector2(col.bounds.min.x, col.bounds.center.y), -transform.right, 0.6f, mask));
        if (right)
            hits.Add(Physics2D.Raycast(new Vector2(col.bounds.max.x, col.bounds.center.y), transform.right, 0.6f, mask));
        if (top)
            hits.Add(Physics2D.Raycast(new Vector2(col.bounds.center.x, col.bounds.max.y), transform.up, 0.6f, mask));
        if (bottom)
            hits.Add(Physics2D.Raycast(new Vector2(col.bounds.center.x, col.bounds.min.y), -transform.up, 0.6f, mask));

        gameObject.layer = LayerMask.NameToLayer("Objects");

        foreach (RaycastHit2D hit in hits)
        {
            if (hit)
            {
                hit.transform.TryGetComponent(out Connectable conn);
                if (!conn) continue;

                // For each surrounding destructible block, add a fixed joint connecting to that block;
                FixedJoint2D newJoint = gameObject.AddComponent<FixedJoint2D>();

                newJoint.connectedBody = hit.rigidbody;
                newJoint.enableCollision = true;
                newJoint.dampingRatio = 1f;
                newJoint.breakForce = breakForce;
                newJoint.anchor = (transform.localPosition - hit.transform.localPosition).normalized / 2f;
                newJoint.breakAction = JointBreakAction2D.Disable;

                // Add this joint to this block's list of connectedjoints.
                conn.connectedJoints.Add(newJoint);
            }
        }

        return hits;

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
