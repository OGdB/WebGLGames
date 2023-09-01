using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

    private void Awake()
    {
        if (!connect) return;

        // Get all of the surrounding destructible blocks that connect.
        List<RaycastHit2D> hits = new();

        gameObject.layer = LayerMask.NameToLayer("Default");
        int mask = 1 << LayerMask.NameToLayer("Objects");
        hits.Add(Physics2D.Raycast(transform.position, Vector3.right, 0.6f, mask));
        hits.Add(Physics2D.Raycast(transform.position, Vector3.left, 0.6f, mask));
        hits.Add(Physics2D.Raycast(transform.position, Vector3.up, 0.6f, mask));
        //hits.Add(Physics2D.Raycast(transform.position, Vector3.down, 0.6f, mask));
        gameObject.layer = LayerMask.NameToLayer("Objects");

        if (testBool)
        {
            Debug.DrawRay(transform.position, -transform.right * 0.6f, Color.white, 3f);
            Debug.DrawRay(transform.position, transform.right * 0.6f, Color.cyan, 3f);
            Debug.DrawRay(transform.position, transform.up * 0.6f, Color.red, 3f);
            Debug.DrawRay(transform.position, -transform.up * 0.6f, Color.yellow, 3f);
        }

        bool hasHit = false;
        foreach (RaycastHit2D hit in hits)
        {
            if (hit)
            {
                hasHit = true;
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
        if (!hasHit) rb.bodyType = RigidbodyType2D.Dynamic;
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
        temp = collision.relativeVelocity.magnitude;
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

    private float lastPunchForce = 0;
    public float temp = 0;
    private void OnDrawGizmos()
    {
        if (!testBool) return;

        float force = rb.velocity.magnitude;
        if (force > 0.5f)
            lastPunchForce = force;
        DrawLabel(transform.position, lastPunchForce.ToString(), Color.yellow);

        DrawLabel(transform.position + Vector3.up * 0.25f, temp.ToString(), Color.white);

        void DrawLabel(Vector3 position, string text, Color color)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 24;
            style.normal.textColor = color;
            Handles.Label(position, text, style);
        }
    }
}
