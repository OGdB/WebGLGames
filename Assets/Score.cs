using UnityEngine;

public class Score : MonoBehaviour
{
    private static TMPro.TextMeshProUGUI scoreText;

    private void Start()
    {
        scoreText = GetComponent<TMPro.TextMeshProUGUI>();
    }

    public static void SetScore(int score) => scoreText.SetText($"Score: {score}");
}
