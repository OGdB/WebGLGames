using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WeaponChoicePanel : MonoBehaviour
{
    public InputAction fistButtons;
    public InputAction axeButtons;
    public InputAction maceButtons;

    [SerializeField]
    private BlockDestruction blockDestructionScript;

    [SerializeField]
    private Button fistButton;
    [SerializeField]
    private Button axeButton;
    [SerializeField]
    private Button maceButton;

    [Space(10)]
    [SerializeField]
    private Color normalColor;
    [SerializeField]
    private Color hoverColor;
    [SerializeField]
    private Color selectedColor;

    private void Start()
    {
        foreach (var button in GetComponentsInChildren<ButtonInteraction>())
        {
            button.NormalColor = normalColor;
            button.HoverColor = hoverColor;
            button.SelectedColor = selectedColor;
        }
    }

    private void OnEnable()
    {
        fistButtons.Enable();
        fistButtons.started += _ => fistButton.onClick.Invoke();
        fistButtons.started += _ => blockDestructionScript.SetWeapon(Weapon.Fists);

        axeButtons.Enable();
        axeButtons.started += _ => axeButton.onClick.Invoke();
        axeButtons.started += _ => blockDestructionScript.SetWeapon(Weapon.Axe);

        maceButtons.Enable();
        maceButtons.started += _ => maceButton.onClick.Invoke();
        maceButtons.started += _ => blockDestructionScript.SetWeapon(Weapon.Mace);
    }


    private void OnDisable()
    {
        fistButtons.Disable();
        fistButtons.started -= _ => fistButton.onClick.Invoke();
        fistButtons.started -= _ => blockDestructionScript.SetWeapon(Weapon.Fists);
        fistButton.onClick.RemoveAllListeners();


        axeButtons.Disable();
        axeButtons.started -= _ => axeButton.onClick.Invoke();
        axeButtons.started -= _ => blockDestructionScript.SetWeapon(Weapon.Axe);
        axeButton.onClick.RemoveAllListeners();


        maceButtons.Disable();
        maceButtons.started -= _ => maceButton.onClick.Invoke();
        maceButtons.started -= _ => blockDestructionScript.SetWeapon(Weapon.Mace);
        maceButton.onClick.RemoveAllListeners();
    }
}
