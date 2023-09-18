using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Standard pause screen script that should have most functionalities you need for pause screens.
/// </summary>
public class Menu : MonoBehaviour
{
    #region Properties
    [SerializeField, Space(10)]
    private InputAction pauseMenuButton;

    [Space(10), SerializeField, Range(0.01f, 2f), Tooltip("The speed at which the menu appears/disappears")]
    private float appearSpeed = 0.5f;

    public delegate void OnPauseButton(bool isPaused);
    public static event OnPauseButton OnPause;

    public static bool isPaused = false;
    #endregion

    private void OnEnable()
    {
        pauseMenuButton.Enable();

        pauseMenuButton.performed += _ => SwitchPauseState();
    }
    private void OnDisable()
    {
        pauseMenuButton.Disable();

        pauseMenuButton.performed -= _ => SwitchPauseState();
    }

    /// <summary>
    /// Either pauses or resumes the application.
    /// </summary>
    public void SwitchPauseState()
    {
        StopAllCoroutines();
        EventSystem.current.SetSelectedGameObject(null);
        // Not bothering to store in variable due to infrequent need of variable.
        CanvasGroup canvas = GetComponentInChildren<CanvasGroup>();
        StartCoroutine(SwitchPauseScreenCR(canvas, appearSpeed));
    }

    public void RestartLevel()
    {
        isPaused = false;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    private IEnumerator SwitchPauseScreenCR(CanvasGroup canvas, float speed)
    {
        // Change pause state on invoke.
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f; // Resume time if unpausing, vice versa.
        OnPause?.Invoke(isPaused);

        WaitForEndOfFrame frame = new();

        // Set target alpha
        float startAlpha = canvas.alpha;
        float targetAlpha = 1f;
        // Unscaled time as otherwise the coroutine will cease to function due to timescale change.
        float startTime = Time.unscaledTime;

        // If it is paused now, set resume targets, vice versa.
        if (isPaused)
        {
            canvas.interactable = true;
            canvas.blocksRaycasts = true;
        }
        else
        {
            targetAlpha = 0f;
            canvas.interactable = false;
            canvas.blocksRaycasts = false;
        }

        while (canvas.alpha != targetAlpha)
        {
            float timeSinceStarted = Time.unscaledTime - startTime;
            float progress = timeSinceStarted / speed;

            float alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);

            canvas.alpha = alpha;

            yield return frame;
        }
    }
}
