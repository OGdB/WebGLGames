using UnityEngine;

/// <summary>
/// To attach to a button make it it a 'quit game' button.
/// </summary>
public class QuitButton : MonoBehaviour
{
    public static void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

}
