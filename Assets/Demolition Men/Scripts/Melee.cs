using UnityEngine;

public class Melee : MonoBehaviour
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

    [Header("Head Box Settings"), SerializeField]
    private Vector2 headPosOffset = Vector3.up;
    [SerializeField]
    private Vector2 headBoxSize = Vector3.one;
    [SerializeField, Tooltip("The amount of force to apply upwards when hitting a block with the head.")]
    private float headHitForce = 3f;

    // PRIVATES
    private InputReader inputReader;
    private Animator anim;
    private Rigidbody2D rb;
    private RaycastHit2D[] punchBoxCastHits;
    private int currentPunchClip = 0;
    private AudioSource audioSource;
    #endregion

    #region Initiation
    private void Awake()
    {
        inputReader = GetComponent<InputReader>();
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

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

    private void FixedUpdate()
    {
        PunchOverlapBoxCast();
        HeadPunchBoxCast();
    }

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

        // TODO: Use the 
        foreach (var hit in punchBoxCastHits)
        {
            if (hit)
            {
                Rigidbody2D hitBody = hit.rigidbody;
                if (hit.rigidbody)
                {
                    WeaponSO currentWeapon = weapons[currentWeaponData].weaponData;
                    hitBody.AddForceAtPosition(transform.right * currentWeapon.PushForce, hit.point, ForceMode2D.Impulse);
                    hitBody.TryGetComponent(out DestructibleBlock block);
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
    private void HeadPunchBoxCast()
    {
        Vector2 headBoxPos = transform.position;
        headBoxPos += headPosOffset;
        RaycastHit2D[] headBoxCastHits = Physics2D.BoxCastAll(headBoxPos, headBoxSize, 0, transform.up, 0, punchableMasks);
        if (rb.velocity.y > 0.7f)
        {
            foreach (var hit in headBoxCastHits)
            {
                if (hit)
                {
                    Rigidbody2D hitBody = hit.rigidbody;
                    if (hit.rigidbody)
                    {
                        hitBody.AddForceAtPosition(transform.up * headHitForce, hit.point, ForceMode2D.Impulse);
                        hitBody.TryGetComponent(out DestructibleBlock block);
                        block?.Punched(hit.point, transform.up, weapons[0].weaponData);
                    }

                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Head box
        Vector2 headBoxPos = transform.position;
        headBoxPos += headPosOffset;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(headBoxPos, headBoxSize);

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
