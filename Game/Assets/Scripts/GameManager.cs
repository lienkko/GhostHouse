using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //PlayerHandler
    [SerializeField] private GameObject _gameOverText;

    //-------------

    private void Awake()
    {
        PlayerController.OnDeath += OnPlayerDeath;
    }

    private void OnPlayerDeath()
    {
        _gameOverText.SetActive(true);
    }

    
}
