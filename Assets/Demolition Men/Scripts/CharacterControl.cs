using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterControl : MonoBehaviour
{
    #region Properties
    [Header("Run Settings"), SerializeField]
    private float runspeed = 50f;
    [SerializeField]
    private float attackMoveSpeed = 30f;
    private float currentXSpeed;
    [SerializeField, Range(0f, 1f)]
    private float stepVolume = 1f;
    [SerializeField]
    private AudioClip[] stepSounds;


    [Header("Jump Settings"), SerializeField]
    private float jumpForce = 16;
    [SerializeField, Tooltip("Time in seconds of cooldown between jumps.")]
    private float jumpCooldownTime = 0.3f;
    [SerializeField, Range(0f, 1f)]
    private float jumpVolume = 1f;
    [SerializeField]
    private AudioClip jumpSound;

    [Header("Gravity Fall Settings"), SerializeField]
    private float fallMultiplier = 2.5f;
    [SerializeField]
    private float lowJumpMultiplier = 2f;

    // PRIVATE
    private float xInput = 0;
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

    private void Awake()
    {
        lowJumpMultiplier = (lowJumpMultiplier - 1) * Physics2D.gravity.y;
        fallMultiplier = (fallMultiplier - 1) * Physics2D.gravity.y;

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        string thisPlayerLayer = LayerMask.LayerToName(gameObject.layer);
        var PlayerOneLayer = 1 << LayerMask.NameToLayer("Player1");
        var playerTwoLayer = 1 << LayerMask.NameToLayer("Player2");
        LayerMask otherPlayer = thisPlayerLayer == "Player1" ?  playerTwoLayer : PlayerOneLayer;
        LayerMask walkableLayers = 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Objects") | otherPlayer;
        
        Collider2D collider = GetComponent<Collider2D>();
        Vector2 groundCheckBoxSize = new(0.2f, 0.15f);
        groundChecker = new(walkableLayers, groundCheckBoxSize, collider);

        currentXSpeed = runspeed;
    }

    /// <summary>
    /// Called by PlayerInput component on move.
    /// </summary>
    /// <param name="input"></param>
    private void OnMove(InputValue input)
    {
        xInput = input.Get<float>();

        if (xInput == 0) return;

        float yRot = xInput > 0 ? 0 : -180f;  // Rotate the character 180 if moving to opposite direction than before.
        transform.rotation = Quaternion.Euler(0, yRot, 0);
    }
    // TODO - Double Jump (?)
    /// <summary>
    /// Called by PlayerInput once on jump.
    /// If on the ground and there is no cooldown, player can jump.
    /// </summary>
    private void OnJump(InputValue value)
    {
        jumpPressed = value.isPressed;

        if (jumpPressed && isGrounded && !jumpCooldown)
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
    private void OnPunch(InputValue input)
    {
        currentXSpeed = input.isPressed ? attackMoveSpeed : runspeed;
    }

    private void FixedUpdate()
    {
        isGrounded = groundChecker.IsGrounded();
        GravityAmplification();

        SetAnimations();

        Movement();
    }

    /// <summary>
    /// More platform-esque gravity. Increases the speed at which the character falls.
    /// </summary>
    private void GravityAmplification()
    {
        if (isGrounded) return;  // don't need to add add gravity if player is already on the ground.

        if (rb.velocity.y < 0)
        {
            rb.velocity += fallMultiplier * Time.deltaTime * Vector2.up;
        }
        else if (rb.velocity.y > 0 && !jumpPressed)
        {
            rb.velocity += lowJumpMultiplier * Time.deltaTime * Vector2.up;
        }
    }

    /// <summary>
    /// Set animations based on key values.
    /// </summary>
    private void SetAnimations()
    {
        // Run Animation
        anim.SetFloat("CurrentSpeed", Mathf.Abs(xInput));
        // Jump/Fall animation
        anim.SetBool("InAir", !isGrounded);
    }

    /// <summary>
    /// Move in forward dirrection if there is input.
    /// </summary>
    private void Movement()
    {
        if (xInput == 0) return;
        rb.AddForce(transform.right * currentXSpeed, ForceMode2D.Force);
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