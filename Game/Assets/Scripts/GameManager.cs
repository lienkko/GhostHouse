using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //WraithHandler
    [SerializeField] private GameObject _objWraith;
    [SerializeField] private GameObject _textWarning;

    [SerializeField] private Transform _wraithStartPoint;
    [SerializeField] private Transform _wraithEndPoint;

    private bool _isWraithWaiting = false;
    //-------------

    //PlayerHandler
    [SerializeField] private GameObject _gameOverText;

    //-------------

    private void Awake()
    {
        PlayerController.OnDeath += OnPlayerDeath;
    }


    private void Update()
    {
        if (!_isWraithWaiting)
            StartCoroutine(WaitForWraith());
    }

    IEnumerator WaitForWraith()
    {
        _isWraithWaiting = true;
        yield return new WaitForSeconds(5);
        StartCoroutine(WraithFlight());
    }

    IEnumerator WraithFlight()
    {
        _objWraith.GetComponent<Wraith>().OnFinish += EndWraithFlight;

        _textWarning.SetActive(true);
        yield return new WaitForSeconds(5);
        _textWarning.SetActive(false);
        
        _objWraith.SetActive(true);
        _objWraith.GetComponent<Wraith>().StartMoving(_wraithStartPoint.position, _wraithEndPoint.position);
    }

    private void EndWraithFlight()
    {
        _isWraithWaiting = false;
    }

    private void OnPlayerDeath()
    {
        _gameOverText.SetActive(true);
    }
}
