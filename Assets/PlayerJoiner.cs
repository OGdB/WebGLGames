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
        if (playerOnePrefab)
        {
            PlayerInput playerInput = PlayerInput.Instantiate(playerOnePrefab, controlScheme: "Player One", pairWithDevice: Keyboard.current);
            Rect rect = new(0, 0, 0.49f, 1f);
            playerInput.transform.parent.GetComponentInChildren<Camera>().rect = rect;
        }
        if (playerTwoPrefab)
        {
            PlayerInput playerInput = PlayerInput.Instantiate(playerTwoPrefab, controlScheme: "Player Two", pairWithDevice: Keyboard.current);
            Rect rect = new(0.51f, 0, 0.49f, 1f);
            playerInput.transform.parent.GetComponentInChildren<Camera>().rect = rect;
        }
    }

    private void SpawnPlayerOne()
    {
        if (playerOnePrefab)
        {
            PlayerInput.Instantiate(playerOnePrefab, controlScheme: "Player One", pairWithDevice: Keyboard.current);
        }
        else
        {
            Debug.LogWarning("Player one prefab missing.");
        }
    }
    private void SpawnPlayerTwo()
    {
        if (playerTwoPrefab)
        {
            PlayerInput.Instantiate(playerTwoPrefab, controlScheme: "Player Two", pairWithDevice: Keyboard.current);
        }
        else
        {
            Debug.LogWarning("Player two prefab missing.");
        }
    }


}
