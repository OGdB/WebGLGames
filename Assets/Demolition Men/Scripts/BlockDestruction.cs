using UnityEngine;

/// <summary>
/// Responsible for any of the player's interaction with the destructible environment
/// </summary>
public class BlockDestruction : MonoBehaviour
{
    #region Properties
    [SerializeField]
    private Weapon currentWeapon = Weapon.Fists;
    [SerializeField]
    private WeaponData[] weapons;
    private int currentWeaponData;

    [SerializeField, Space(5)]
    private AudioClip[] punchSoundClips;
    [SerializeField]
    private Vector2 boxCenterOffset = new(0.85f, 0.23f);
    [SerializeField]
    private Vector2 punchBoxSize = Vector2.one;
    [SerializeField]
    private LayerMask punchableMasks;

    // PRIVATES
    private InputReader inputReader;
    private Animator anim;
    private RaycastHit2D[] punchBoxCastHits;
    private int currentPunchClip = 0;
    private AudioSource audioSource;

    [Space(15)]
    public bool debug = false;
    #endregion

    #region Initiation
    private void Awake()
    {
        inputReader = GetComponent<InputReader>();
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();

        SetWeapon(Weapon.Fists);
    }
    private void OnEnable()
    {
        inputReader.Standard.Punch.started += _ => Attack();
        inputReader.Standard.Punch.canceled += _ => StopAttack();
    }
    private void OnDisable()
    {
        inputReader.Standard.Punch.started -= _ => Attack();
        inputReader.Standard.Punch.canceled -= _ => StopAttack();
    }
    #endregion

    private void FixedUpdate() => PunchOverlapBoxCast();

    public void SetWeapon(Weapon newWeapon)
    {
        currentWeapon = newWeapon;
        for (int i = 0; i < weapons.Length; i++)
        {
            WeaponData weapon = weapons[i];
            if (currentWeapon == weapon.weapon)
            {
                currentWeaponData = i;
            }
        }
    }
    public void SetWeapon(int newWeapon) => SetWeapon((Weapon)newWeapon);

    private void Attack()
    {
        anim.Play(currentWeapon.ToString());
    }
    private void StopAttack()
    {
        anim.Play("Idle");
    }

    public void Hit()
    {
        // Audio
        audioSource.PlayOneShot(punchSoundClips[currentPunchClip]);
        currentPunchClip++;
        currentPunchClip %= punchSoundClips.Length;

        foreach (var hit in punchBoxCastHits)
        {
            if (hit)
            {
                Rigidbody2D hitBody = hit.rigidbody;
                if (hit.rigidbody)
                {
                    WeaponSO currentWeapon = weapons[currentWeaponData].weaponData;
                    hitBody.AddForceAtPosition(transform.right * currentWeapon.PushForce, hit.point, ForceMode2D.Impulse);
                    hitBody.TryGetComponent(out DestructableBlock block);
                    block?.Punched(hit.point, transform.right, currentWeapon);
                }

            }
        }
    }

    private void PunchOverlapBoxCast()
    {
        Vector2 punchBoxPos = (Vector2)transform.position + ((Vector2)transform.right * boxCenterOffset);
        punchBoxPos.y += boxCenterOffset.y;
        punchBoxCastHits = Physics2D.BoxCastAll(punchBoxPos, punchBoxSize, 0, transform.right, 0, punchableMasks);
    }

    private void OnDrawGizmos()
    {
        if (!debug) return;

        // Punch box
        Vector2 punchBoxPos = (Vector2)transform.position + ((Vector2)transform.right * boxCenterOffset);
        punchBoxPos.y += boxCenterOffset.y;
        RaycastHit2D[] hits = Physics2D.BoxCastAll(punchBoxPos, punchBoxSize, 0, transform.right, 0, punchableMasks);

        Gizmos.DrawWireCube(punchBoxPos, punchBoxSize);

        foreach (var hit in hits)
        {
            if (hit)
            {
                Gizmos.color = Color.yellow;
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(hit.point, 0.1f);
            }
        }
    }
}

[System.Serializable]
public class WeaponData
{
    public Weapon weapon;
    public WeaponSO weaponData;
}
