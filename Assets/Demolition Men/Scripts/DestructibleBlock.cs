using UnityEditor;
using UnityEngine;

// TODO: Possible optimization with Dictionary
public class DestructibleBlock : MonoBehaviour
{
    #region Properties
    [SerializeField]
    private BlockMaterial thisMaterial = BlockMaterial.Brick;
    [SerializeField]
    private int blockHealth = 20;
    [SerializeField]
    private AudioClip onBreakSound;

    private Rigidbody2D rb;

    public bool debug = false;
    #endregion

    private void Start() => rb = GetComponent<Rigidbody2D>();

    public virtual void Punched(Vector3 position, Vector3 direction, WeaponSO weapon)
    {
        switch (thisMaterial)
        {
            case BlockMaterial.Brick:
                blockHealth -= weapon.BrickDamage; // Damage
                AudioEffectPlayer.PlaySoundEffect(weapon.OnBrickSound, transform.position);
                ParticleEffectPlayer.PlayBrickParticles(transform.position);
                break;

            case BlockMaterial.Wood:
                blockHealth -= weapon.WoodDamage; // Damage
                AudioEffectPlayer.PlaySoundEffect(weapon.OnWoodSound, transform.position);
                ParticleEffectPlayer.PlayWoodParticles(transform.position);
                break;

            case BlockMaterial.Metal:
                blockHealth -= weapon.MetalDamage; // Damage
                AudioEffectPlayer.PlaySoundEffect(weapon.OnMetalSound, transform.position);
                ParticleEffectPlayer.PlayMetalParticles(transform.position);
                break;

        }

        rb.AddForceAtPosition(direction * weapon.PushForce, position);

        if (blockHealth <= 0)
            BreakBlock();
        if (blockHealth <= -20)
            gameObject.SetActive(false);
    }

    private void BreakBlock()
    {
        TryGetComponent(out Connectable conn);
        conn?.BreakConnection();
        AudioEffectPlayer.PlaySoundEffect(onBreakSound, transform.position);
    }

    #region Gizmos
    private void OnValidate() => rb = GetComponent<Rigidbody2D>();

    private void OnDrawGizmos()
    {
        if (!debug) return;

        Gizmos.color = Color.white;
        DrawLabel(transform.position + Vector3.up * 0.5f, blockHealth.ToString());

        void DrawLabel(Vector3 position, string text)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 24;
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
