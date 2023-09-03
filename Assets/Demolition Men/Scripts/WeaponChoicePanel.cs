using UnityEngine;
using UnityEngine.EventSystems;
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

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(fistButton.gameObject);
    }

    private void OnEnable()
    {
        fistButtons.Enable();
        fistButtons.started += _ => blockDestructionScript.SetWeapon(Weapon.Fists);
        fistButtons.started += _ => EventSystem.current.SetSelectedGameObject(fistButton.gameObject);
        fistButton.onClick.AddListener(() => EventSystem.current.SetSelectedGameObject(fistButton.gameObject));

        axeButtons.Enable();
        axeButtons.started += _ => blockDestructionScript.SetWeapon(Weapon.Axe);
        axeButtons.started += _ => EventSystem.current.SetSelectedGameObject(axeButton.gameObject);
        axeButton.onClick.AddListener(() => EventSystem.current.SetSelectedGameObject(axeButton.gameObject));

        maceButtons.Enable();
        maceButtons.started += _ => blockDestructionScript.SetWeapon(Weapon.Mace);
        maceButtons.started += _ => EventSystem.current.SetSelectedGameObject(maceButton.gameObject);
        maceButton.onClick.AddListener(() => EventSystem.current.SetSelectedGameObject(maceButton.gameObject));
    }


    private void OnDisable()
    {
        fistButtons.Disable();
        fistButtons.started -= _ => blockDestructionScript.SetWeapon(Weapon.Fists);
        fistButtons.started -= _ => blockDestructionScript.SetWeapon(Weapon.Fists);
        fistButton.onClick.RemoveAllListeners();


        axeButtons.Disable();
        axeButtons.started -= _ => blockDestructionScript.SetWeapon(Weapon.Axe);
        axeButtons.started -= _ => blockDestructionScript.SetWeapon(Weapon.Axe);
        axeButton.onClick.RemoveAllListeners();


        maceButtons.Disable();
        maceButtons.started -= _ => blockDestructionScript.SetWeapon(Weapon.Mace);
        maceButtons.started -= _ => blockDestructionScript.SetWeapon(Weapon.Mace);
        maceButton.onClick.RemoveAllListeners();
    }
}
