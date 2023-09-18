using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// To attach to a button to make it start the transition to a scene.
/// </summary>
public class SceneButton : MonoBehaviour
{
    [SerializeField]
    private float transitionSpeed = 2f;
    public void GoToScene(string sceneName)
    {
        FadeCanvas.SwitchCanvasState(transitionSpeed);

        _ = StartCoroutine(WaitForTransition());

        IEnumerator WaitForTransition()
        {
            yield return new WaitForSeconds(transitionSpeed);
            SceneManager.LoadScene(sceneName);
        }
    }
}
