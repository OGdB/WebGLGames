using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Color NormalColor { get => normalColor; set => normalColor = value; }
    [SerializeField] private Color normalColor;
    public Color HoverColor { get => hoverColor; set => hoverColor = value; }
    [SerializeField] private Color hoverColor;
    public Color SelectedColor { get => selectedColor; set => selectedColor = value; }
    [SerializeField] private Color selectedColor;

    private Button button;
    private Image image;
    public static ButtonInteraction currentlySelectedButton;
    private bool selected = false;

    private void Awake()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(SelectButton);
    }
    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }

    private void SelectButton()
    {
        currentlySelectedButton?.Deselect();

        selected = true;
        currentlySelectedButton = this;
        image.color = selectedColor;
    }
    public void Deselect()
    {
        selected = false;
        currentlySelectedButton = null;
        image.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!selected)
        {
            image.color = hoverColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!selected)
        {
            image.color = normalColor;
        }
    }
}
