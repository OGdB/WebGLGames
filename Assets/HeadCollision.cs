using UnityEngine;

public class HeadCollision : MonoBehaviour
{
    [SerializeField]
    private WeaponSO headDamage;
    [SerializeField]
    private float headHitForce;
    [SerializeField]
    private float minimumRelativeVelocity = 3f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Mathf.Abs(collision.relativeVelocity.y) >= minimumRelativeVelocity)
        {
            Rigidbody2D brickRb = collision.rigidbody;
            if (brickRb)
            {
                brickRb.AddForceAtPosition(transform.up * headHitForce, collision.contacts[0].point, ForceMode2D.Impulse);
                brickRb.TryGetComponent(out DestructibleBlock block);
                block?.Punched(collision.contacts[0].point, transform.up, headDamage);
            }
        }
    }
}
