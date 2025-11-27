using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _gameOverText;
    [SerializeField] private Text _healthPointsField;

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
