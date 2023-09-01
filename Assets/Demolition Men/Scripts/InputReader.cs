using UnityEngine;

/// <summary>
/// Centralized class for reading input on a player.
/// </summary>
public class InputReader : MonoBehaviour
{
    [SerializeField]
    private PlayerInput input;
    public PlayerInput Input { get => input; private set => input = value; }
    public PlayerInput.StandardActions Standard;

    private void Awake()
    {
        Input = new();
        Standard = Input.Standard;
    }
    private void OnEnable()
    {
        Input.Enable();
    }
    private void OnDisable()
    {
        Input.Disable();
    }

    public float MoveInput => Input.Standard.Move.ReadValue<float>();
}
