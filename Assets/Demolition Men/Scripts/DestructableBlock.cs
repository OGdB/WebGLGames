using UnityEditor;
using UnityEngine;

// TODO: Possible optimization with Dictionary
public class DestructableBlock : MonoBehaviour
{
    #region Properties
    [SerializeField]
    protected BlockMaterial thisMaterial = BlockMaterial.Brick;
    [SerializeField]
    private float breakVelocity = 15f;
    [SerializeField]
    protected int blockHealth = 100;
    [SerializeField]
    protected AudioClip onBreakSound;

    protected Rigidbody2D rb;

    public bool debug = false;
    #endregion

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.magnitude >= breakVelocity)
        {
            BreakBlock();
        }
    }

    public virtual void Punched(Vector2 position, Vector3 direction, WeaponSO weapon)
    {
        DamageBlock(position, direction, weapon);

        if (blockHealth <= 0)
        {
            BreakBlock();
        }
        if (blockHealth <= -20)
        {
            gameObject.SetActive(false);
        }
    }

    protected void DamageBlock(Vector2 position, Vector3 direction, WeaponSO weapon)
    {
        switch (thisMaterial)
        {
            case BlockMaterial.Brick:
                blockHealth -= weapon.BrickDamage; // Damage
                AudioEffectPlayer.PlaySoundEffect(weapon.OnBrickSound, transform.position);
                ParticleEffectPlayer.PlayBrickParticles(position);
                break;

            case BlockMaterial.Wood:
                blockHealth -= weapon.WoodDamage; // Damage
                AudioEffectPlayer.PlaySoundEffect(weapon.OnWoodSound, transform.position);
                ParticleEffectPlayer.PlayWoodParticles(position);
                break;

            case BlockMaterial.Metal:
                blockHealth -= weapon.MetalDamage; // Damage
                AudioEffectPlayer.PlaySoundEffect(weapon.OnMetalSound, transform.position);
                ParticleEffectPlayer.PlayMetalParticles(position);
                break;
        }
    }

    public void SetHealth(int amount) => blockHealth = amount;

    protected void BreakBlock()
    {
        TryGetComponent(out Connectable conn);
        conn?.BreakConnection();
        AudioEffectPlayer.PlaySoundEffect(onBreakSound, transform.position);
    }

    #region Gizmos
    protected virtual void OnValidate() => rb = GetComponent<Rigidbody2D>();

    protected virtual void OnDrawGizmos()
    {
        if (!debug) return;

        Gizmos.color = Color.white;
        DrawLabel(transform.position + Vector3.up * 0.5f, blockHealth.ToString());
        DrawLabel(transform.position, rb.velocity.magnitude.ToString());
        DrawLabel(transform.position + Vector3.down, "Sleeping: " + rb.IsSleeping().ToString());

        void DrawLabel(Vector3 position, string text)
        {
            GUIStyle style = new()
            {
                fontSize = 24
            };
            style.normal.textColor = Color.white;
            Handles.Label(position, text, style);
        }
    }
    #endregion
}

public enum BlockMaterial
{
    Brick = 1,
    Wood = 2,
    Metal = 3
}
