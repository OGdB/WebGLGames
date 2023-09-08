using UnityEditor;
using UnityEngine;

public class DestructableBlock : MonoBehaviour
{
    #region Properties
    [SerializeField]
    protected BlockMaterial thisMaterial = BlockMaterial.Brick;
    [SerializeField]
    private float hitSoundVelocity = 8f;
    [SerializeField]
    private float breakVelocity = 10f;
    [SerializeField]
    private float destroyVelocity = 15f;
    [SerializeField]
    protected int blockHealth = 100;
    [SerializeField]
    protected AudioClip onBreakSound;

    protected Rigidbody2D rb;

    private delegate void ParticleEffects();
    private ParticleEffects PlayParticleEffects;
    private Vector3 particlesPosition;

    public bool debug = false;
    private bool debugBreak = false;
    #endregion

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        BlackBoard.AmountOfBlocks++;

        switch (thisMaterial)
        {
            case BlockMaterial.Brick:
                PlayParticleEffects += () => ParticleEffectPlayer.PlayBrickParticles(particlesPosition);
                break;
            case BlockMaterial.Wood:
                PlayParticleEffects += () => ParticleEffectPlayer.PlayWoodParticles(particlesPosition);
                break;
            case BlockMaterial.Metal:
                PlayParticleEffects += () => ParticleEffectPlayer.PlayMetalParticles(particlesPosition);
                break;
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        float magnitude = collision.relativeVelocity.magnitude;

        if (magnitude >= hitSoundVelocity)
        {
            AudioEffectPlayer.PlaySoundEffect(onBreakSound, transform.position);

            if (magnitude >= breakVelocity)
            {
                BreakBlock();

                debugBreak = true;

                if (magnitude >= destroyVelocity)
                {
                    DestroyBlock();
                }
            }
        }
    }

    /// <summary>
    /// Called when the block is hit.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="direction"></param>
    /// <param name="weapon"></param>
    public virtual void Punched(Vector2 position, Vector3 direction, WeaponSO weapon)
    {
        // The information on the effects of this weapon to this block.
        WeaponSO.WeaponStats weaponStats = weapon.GetWeaponStats(thisMaterial);

        // Damage to material
        blockHealth -= weaponStats.DamageToMaterial;
        // Sound
        AudioEffectPlayer.PlaySoundEffect(weaponStats.OnMaterialHitSound, transform.position);
        // Particles
        particlesPosition = position;
        PlayParticleEffects();


        if (blockHealth <= 0)
        {
            BreakBlock();
        }
        if (blockHealth <= -20)
        {
            DestroyBlock();
        }
    }

    public void SetHealth(int amount) => blockHealth = amount;

    protected void BreakBlock()
    {
        TryGetComponent(out Connectable conn);
        conn?.BreakConnection();
        AudioEffectPlayer.PlaySoundEffect(onBreakSound, transform.position);
    }
    protected void DestroyBlock()
    {
        BlackBoard.BlocksDestroyed++;
        BlackBoard.AmountOfBlocks--;
        Score.SetScore(BlackBoard.BlocksDestroyed);
        gameObject.SetActive(false);
    }

    #region Gizmos
    protected virtual void OnValidate() => rb = GetComponent<Rigidbody2D>();

#if UNITY_EDITOR
    protected virtual void OnDrawGizmos()
    {
        if (debugBreak)
        {
            Gizmos.DrawCube(transform.position, Vector2.one * 0.5f);
        }

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
#endif
#endregion
}

public enum BlockMaterial
{
    Brick = 1,
    Wood = 2,
    Metal = 3
}
