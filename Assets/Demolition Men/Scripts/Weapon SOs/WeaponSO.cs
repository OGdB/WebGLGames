using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WeaponData", order = 1)]
public class WeaponSO : ScriptableObject
{
    [SerializeField]
    private Weapon weapon;

    [SerializeField]
    private float pushForce = 3f;
    public float PushForce { get { return pushForce; } }

    [SerializeField]
    private WeaponStats[] weaponStats;

    public WeaponStats GetWeaponStats(BlockMaterial mat)
    {
        foreach (var weaponStats in weaponStats)
        {
            if (weaponStats.Material == mat) return weaponStats;
        }

        return null;
    }

    [Serializable]
    public class WeaponStats
    {
        public WeaponStats(BlockMaterial material, int damageToMaterial, AudioClip onMaterialHitSound)
        {
            this.material = material;
            this.damageToMaterial = damageToMaterial;
            this.onMaterialHitSound = onMaterialHitSound;
        }

        [SerializeField]
        private BlockMaterial material;
        [SerializeField]
        private int damageToMaterial;
        [SerializeField]
        private AudioClip onMaterialHitSound;

        public BlockMaterial Material { get => material; }
        public int DamageToMaterial { get => damageToMaterial; }
        public AudioClip OnMaterialHitSound { get => onMaterialHitSound; }
    }
}