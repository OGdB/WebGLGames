using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WeaponData", order = 1)]
public class WeaponSO : ScriptableObject
{
    [SerializeField]
    private Weapon weapon;

    [SerializeField]
    private float pushForce = 3f;
    public float PushForce { get { return pushForce; } }

    [Header("Wood")]
    [SerializeField]
    private int woodDamage = 10;
    public int WoodDamage { get { return woodDamage; } }
    [SerializeField]
    private AudioClip onWoodSound;
    public AudioClip OnWoodSound { get { return onWoodSound; } }

    [Header("Bricks")]
    [SerializeField]
    private int brickDamage = 10;
    public int BrickDamage { get { return brickDamage; } }
    [SerializeField]
    private AudioClip onBrickSound;
    public AudioClip OnBrickSound { get { return onBrickSound; } }

    [Header("Metal")]
    [SerializeField]
    private int metalDamage = 10;
    public int MetalDamage { get { return metalDamage; } }
    [SerializeField]
    private AudioClip onMetalSound;
    public AudioClip OnMetalSound { get { return onMetalSound; } }

}