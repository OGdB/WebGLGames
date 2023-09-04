using UnityEngine;

public class DeleteLater : MonoBehaviour
{
    private static TMPro.TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TMPro.TextMeshProUGUI>();
    }

    public static void SetAmountOfBlocks(int amount) => text.SetText($"Total Amount of blocks: {amount}");
}
