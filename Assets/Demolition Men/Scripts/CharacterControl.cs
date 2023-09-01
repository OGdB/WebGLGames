using System.Collections;
using UnityEditor;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    #region Properties
    [Header("Run Settings"), SerializeField]
    private float runspeed = 5f;
    [SerializeField]
    private float punchMoveSpeed = 3.5f;
    private float currentXSpeed;
    [SerializeField, Range(0f, 1f)]
    private float stepVolume = 1f;
    [SerializeField]
    private AudioClip[] stepSounds;


    [Header("Jump Settings"), SerializeField]
    private float jumpForce = 8f;
    [SerializeField, Tooltip("Time in seconds of cooldown between jumps.")]
    private float jumpCooldownTime = 0.5f;
    [SerializeField, Range(0f, 1f)]
    private float jumpVolume = 1f;
    [SerializeField]
    private AudioClip jumpSound;

    [Header("Gravity Fall Settings"), SerializeField]
    private float fallMultiplier = 2.5f;
    [SerializeField]
    private float lowJumpMultiplier = 2f;

    // PRIVATE
    private InputReader inputReader;
    private Rigidbody2D rb;
    private Animator anim;
    private AudioSource audioSource;
    private bool isGrounded = false;

    private int currentStepSound = 0;
    private bool jumpPressed = false;
    private bool jumpCooldown = false;
    private GroundedChecker groundChecker;

    [Space(15)]
    public bool debug = false;
    #endregion

    #region Initiation
    private void Awake()
    {
        inputReader = GetComponent<InputReader>();
        lowJumpMultiplier = (lowJumpMultiplier - 1) * Physics2D.gravity.y;
        fallMultiplier = (fallMultiplier - 1) * Physics2D.gravity.y;

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        LayerMask groundLayers = 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Objects");
        Collider2D collider = GetComponent<Collider2D>();
        Vector2 groundCheckBoxSize = new(0.2f, 0.15f);
        groundChecker = new(groundLayers, groundCheckBoxSize, collider);

        currentXSpeed = runspeed;
    }

    private void OnEnable()
    {
        // Jump
        inputReader.Standard.Jump.performed += _ => Jump();
        inputReader.Standard.Jump.started += _ => jumpPressed = true;
        inputReader.Standard.Jump.canceled += _ => jumpPressed = false;

        // Slower movement whilst punching 
        inputReader.Standard.Punch.started += _ => currentXSpeed = punchMoveSpeed;
        inputReader.Standard.Punch.canceled += _ => currentXSpeed = runspeed;
    }

    private void OnDisable()
    {
        // Jump
        inputReader.Standard.Jump.performed -= _ => Jump();
        inputReader.Standard.Jump.started -= _ => jumpPressed = true;
        inputReader.Standard.Jump.canceled -= _ => jumpPressed = false;

        // Slower movement whilst punching 
        inputReader.Standard.Punch.started -= _ => currentXSpeed = punchMoveSpeed;
        inputReader.Standard.Punch.canceled -= _ => currentXSpeed = runspeed;
    }
    #endregion

    private void FixedUpdate()
    {
        float xInput = inputReader.MoveInput;
        isGrounded = groundChecker.IsGrounded();

        // Movement & Rotation
        Movement(xInput);

        // Fall/Gravity mechanic.
        BetterGravity();

        // Run Animation
        anim.SetFloat("CurrentSpeed", Mathf.Abs(xInput));
        // Jump/Fall animation
        anim.SetBool("InAir", !isGrounded);

        void BetterGravity()
        {
            if (rb.velocity.y < 0)
            {
                rb.velocity += fallMultiplier * Time.deltaTime * Vector2.up;
            }
            else if (rb.velocity.y > 0 && !jumpPressed)
            {
                rb.velocity += lowJumpMultiplier * Time.deltaTime * Vector2.up;
            }
        }

        void Movement(float xInput)
        {
            if (xInput != 0)
            {
                float yRot = xInput > 0 ? 0 : -180f;  // Rotate the character 180 if moving to opposite direction than before.
                transform.rotation = Quaternion.Euler(0, yRot, 0);

                // Move in direction of input.
                rb.velocity = new(xInput * currentXSpeed, rb.velocity.y); ;
            }
        }
    }

    // TODO - Double Jump (?)
    /// <summary>
    /// Simple jump with cooldown.
    /// If on the ground and there is no cooldown, player can jump.
    /// </summary>
    private void Jump()
    {
        if (isGrounded && !jumpCooldown)
        {
            audioSource.PlayOneShot(jumpSound, jumpVolume);
            rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
            StartCoroutine(JumpCooldown());

            IEnumerator JumpCooldown()
            {
                jumpCooldown = true;
                yield return new WaitUntil(() => isGrounded);
                yield return new WaitForSeconds(jumpCooldownTime);
                jumpCooldown = false;
            }
        }
    }

    /// <summary>
    /// Movement Sound ~ Triggered by animation event.
    /// </summary>
    public void AudioStep()
    {
        audioSource.PlayOneShot(stepSounds[currentStepSound], stepVolume);

        currentStepSound++;
        currentStepSound %= stepSounds.Length;
    }


    private void OnValidate()
    {
        rb = GetComponent<Rigidbody2D>();
        // Grounded Check copy for debug reasons.
        LayerMask groundLayers = 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Objects");
        Collider2D collider = GetComponent<Collider2D>();
        Vector2 groundCheckBoxSize = new(0.2f, 0.15f);
        groundChecker = new(groundLayers, groundCheckBoxSize, collider);
    }
    private void OnDrawGizmos()
    {
        if (debug)
        {
            DrawLabel(transform.position, rb.velocity.y.ToString());
            groundChecker.GroundedGizmos();

            void DrawLabel(Vector3 position, string text)
            {
                GUIStyle style = new();
                style.fontSize = 24;
                style.normal.textColor = Color.white;
                Handles.Label(position, text, style);
            }
        }
    }
}