using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIFieldsGetter : MonoBehaviour
{
    [Header("���� ���������")]
    [SerializeField] private GameObject _openSafeText;
    [SerializeField] private GameObject _openDoorText;
    [SerializeField] private GameObject _hideText;
    [SerializeField] private GameObject _startGameText;
    [SerializeField] private GameObject _lockedImage;

    [Space(10)]
    [SerializeField] private GameObject _deathText;
    [SerializeField] private GameObject _buttonRestartGame;
    [SerializeField] private TextMeshProUGUI _hpField;

    [Space(10)]
    [SerializeField] private GameObject _consoleWindow;


    public GameObject OpenSafeText => _openSafeText;
    public GameObject OpenDoorText => _openDoorText;
    public GameObject HideText => _hideText;
    public GameObject StartGameText => _startGameText;
    public GameObject LockedImage => _lockedImage;

    public GameObject DeathText => _deathText;

    public GameObject ButtonRestartGame => _buttonRestartGame;
    public TextMeshProUGUI HpField => _hpField;

    public GameObject ConsoleWindow => _consoleWindow;

    private void Awake()
    {
        _buttonRestartGame.GetComponent<Button>().onClick.AddListener(ReloadGame);
    }

    private void ReloadGame()
    {
        GameManager.Instance.ReloadGame();
    }

    
}
