using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUIFieldsGetter : MonoBehaviour
{
    [Header("���� ���������")]
    [SerializeField] private GameObject _hintFieldText;
    [SerializeField] private GameObject _lockedImage;


    [Space(10)]
    [SerializeField] private GameObject _deathText;
    [SerializeField] private GameObject _winnerText;
    [SerializeField] private GameObject _buttonRestartGame;
    [SerializeField] private GameObject _buttonMenu;
    [SerializeField] private GameObject _trapSlider;

    [SerializeField] private TextMeshProUGUI _hpField;
    [SerializeField] private TextMeshProUGUI _keysCount;


    public GameObject HintFieldText => _hintFieldText;
    public GameObject LockedImage => _lockedImage;

    public GameObject DeathText => _deathText;
    public GameObject WinnerText => _winnerText;

    public GameObject ButtonRestartGame => _buttonRestartGame;
    public GameObject ButtonMenu => _buttonMenu;
    public TextMeshProUGUI HpField => _hpField;
    public GameObject TrapSlider => _trapSlider;
    public TextMeshProUGUI KeysCount => _keysCount;

    private void Awake()
    {
        _buttonRestartGame.GetComponent<Button>().onClick.AddListener(ReloadGame);
        _buttonMenu.GetComponent<Button>().onClick.AddListener(ToMenu);
    }

    private void ReloadGame()
    {
        GameManager.Instance.ReloadGame();
    }

    private void ToMenu()
    {
        SceneManager.LoadScene("main menu");
    }

}
