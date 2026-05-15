using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ControlsManager : MonoBehaviour
{
    public static ControlsManager Instance { get; private set; }

    [SerializeField] private JoystickController _joystick;
    [SerializeField] private InteractButton _interactButton;
    [SerializeField] private CrouchButton _crouchButton;
    [SerializeField] private Button _pauseButton;
    [SerializeField] private UseButton _useButton;
    [SerializeField] private DropButton _dropButton;
    [SerializeField] private HandButton _handButton;

    public Vector2 MoveDirection { get; private set; }
    public bool IsCrouching { get; private set; }
    public bool IsInteracting { get; private set; }
    public bool IsUsing { get; private set; }
    public bool IsDropping { get; private set; }
    public Button PauseButton { get { return _pauseButton; } }

    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        MoveDirection = _joystick.Direction;
        IsCrouching = _crouchButton.isHolding;
        IsInteracting = _interactButton.isHolding;
        IsDropping = _dropButton.isHolding;
        IsUsing = _useButton.isHolding;
    }
    public void ShowJoystick()
    {
        _joystick.gameObject.SetActive(true);
    }
    public void HideJoystick()
    {
        _joystick.Hide();
    }

    public void ShowInteractButton(string hintText = "Interact")
    {
        _interactButton.GetComponentInChildren<TextMeshProUGUI>().text = hintText;
        _interactButton.gameObject.SetActive(true);
    }
    public void HideInteractButton()
    {
        _interactButton.Hide();
    }

    public void ShowCrouchButton()
    {
        _crouchButton.gameObject.SetActive(true);
    }
    public void HideCrouchButton()
    {
        _crouchButton.Hide();
    }

    public void ShowHandButton(HandButton.HandButtonPressed listener)
    {
        _handButton.gameObject.SetActive(true);
        _handButton.SetListener(listener);
    }
    public void HideHandButton()
    {
        _handButton.Hide();
    }

    public void ShowUseButton()
    {
        _useButton.gameObject.SetActive(true);
    }
    public void HideUseButton()
    {
        _useButton.Hide();
    }
    public void ShowDropButton()
    {
        _dropButton.gameObject.SetActive(true);
    }
    public void HideDropButton()
    {
        _dropButton.Hide();
    }

    public void HideAllControls()
    {
        _joystick.Hide();
        _interactButton.Hide();
        _crouchButton.Hide();
        _handButton.Hide();
        _useButton.Hide();
        _dropButton.Hide();
    }
}
