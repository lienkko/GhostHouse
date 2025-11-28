using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _gameOverText;
    [SerializeField] private Text _healthPointsField;
    public TextMeshProUGUI OpenDoorText;
    public TextMeshProUGUI OpenSafeText;
    public TextMeshProUGUI HideText;
    public Image LockedText;

    private void Awake()
    {
        PlayerController.OnDeath += Death;
        PlayerController.OnDamage += ChangeHp;
    }

    private void Death()
    {
        _gameOverText.SetActive(true);
    }

    private void ChangeHp(int dmg, int hp)
    {
        _healthPointsField.text = hp.ToString();
    }
    
}
