using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJoiner : MonoBehaviour
{
    [SerializeField]
    private GameObject playerOnePrefab;
    [SerializeField]
    private GameObject playerTwoPrefab;

    public bool testBool = false;

    // https://forum.unity.com/threads/solved-two-playerinputs.762614/
    /*The same control scheme can be used by arbitrary many players but not the same devices, by default.

    To have two players use the same device, you need to manually instantiate the players with those devices or pair the devices manually to the player's InputUsers.

    Code (CSharp):
    // Create two players both using the same keyboard and mouse.
    PlayerInput.Instantiate(playerPrefab, controlScheme: "Keyboard&Mouse", devices: new[] { Keyboard.current, Mouse.current });
    PlayerInput.Instantiate(playerPrefab, controlScheme: "Keyboard&Mouse", devices: new[] { Keyboard.current, Mouse.current });
    */

    private void Start()
    {
        PlayerInput.Instantiate(playerOnePrefab, controlScheme: "Player One", pairWithDevice: Keyboard.current);
        PlayerInput.Instantiate(playerTwoPrefab, controlScheme: "Player Two", pairWithDevice: Keyboard.current);
    }

/*    private void Update()
    {
        if (testBool)
        {
            testBool = false;

            GetComponent<PlayerInputManager>().playerPrefab = playerTwoPrefab;

            PlayerInput.Instantiate(playerTwoPrefab, controlScheme: "Player Two", pairWithDevice: Keyboard.current);
        }
    }*/
}
