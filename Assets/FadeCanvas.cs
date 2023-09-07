using System.Collections;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

[RequireComponent(typeof(CanvasGroup))]
public class FadeCanvas : MonoBehaviour
{
    private static FadeCanvas _Singleton;

    [SerializeField]
    private bool fadeInThisScene = true;
    [SerializeField]
    private float fadeInSpeed = 2f;

    private static CanvasGroup _canvasGroup;
    private static WaitForFixedUpdate _update = new();
    private static Coroutine _currentCR;

    private static bool FadedIn = false;

    private void Awake()
    {
        if (_Singleton) Destroy(gameObject);

        _Singleton = this;

        _canvasGroup = GetComponent<CanvasGroup>();
        _update = new();
    }

    private IEnumerator Start()
    {
        if (fadeInThisScene)
        {
            _canvasGroup.alpha = 1f;
            yield return new WaitForSeconds(0.1f); // Small wait to ensure time is not 0
            FadeOutCanvas(fadeInSpeed);
        }
    }

    public static void FadeInCanvas(float speedInSeconds)
    {
        if (_currentCR != null) _Singleton.StopCoroutine(_currentCR);

        _currentCR = _Singleton.StartCoroutine(FadeInCR());

        IEnumerator FadeInCR()
        {
            _canvasGroup.blocksRaycasts = true;
            yield return FadeCanvasCR(speedInSeconds, 1f);
        }
    }
    public static void FadeOutCanvas(float speedInSeconds)
    {
        if (_currentCR != null) _Singleton.StopCoroutine(_currentCR);

        _currentCR = _Singleton.StartCoroutine(FadeOutCR());

        IEnumerator FadeOutCR()
        {
            _canvasGroup.blocksRaycasts = false;
            yield return FadeCanvasCR(speedInSeconds, 0f);
        }
    }
    public static void SwitchCanvasState(float speedInSeconds)
    {
        if (_currentCR != null) _Singleton.StopCoroutine(_currentCR);

        if (FadedIn)
            FadeOutCanvas(speedInSeconds);
        else
            FadeInCanvas(speedInSeconds);
    }

    private static IEnumerator FadeCanvasCR(float speedInSeconds, float targetAlpha)
    {
        FadedIn = targetAlpha > 0f ? true : false;

        float startTime = Time.unscaledTime;
        float startAlpha = _canvasGroup.alpha;

        speedInSeconds = Mathf.Abs(targetAlpha - _canvasGroup.alpha) * speedInSeconds;

        while (_canvasGroup.alpha != targetAlpha)
        {
            float timeSinceStarted = Time.unscaledTime - startTime;
            float progress = timeSinceStarted / speedInSeconds;

            _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);

            yield return _update;
        }

        _currentCR = null;
    }
}
