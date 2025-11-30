using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WraithHandler : MonoBehaviour
{
    [SerializeField] private GameObject _textWarning;
    [SerializeField] private GameObject _wraithModel;
    [SerializeField] private AudioClip _whispers;
    [SerializeField] private AudioClip _iSeeYouWhisper;



    private AudioSource _audioSource;
    private float _distance = 0;
    private float _remainingDsitance = 0;
    private Vector3 _startPoint = Vector3.zero;
    private Vector3 _endPoint = Vector3.zero;
    private bool _isMoving = false;
    private bool _isWhispering;
    private DoorController[] _doors;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.volume = 0.3f;
    }

    private void FixedUpdate()
    {
        if (_isMoving)
        {
            _wraithModel.transform.position = Vector3.Lerp(_startPoint, _endPoint, 1-(_remainingDsitance/_distance));
            _remainingDsitance -= 7f * Time.deltaTime;
            if (_remainingDsitance < 0.2 * _distance && !_isWhispering)
            {
                FindAnyObjectByType<GameManager>().GetComponent<AudioSource>().PlayOneShot(_iSeeYouWhisper);
                
                _isWhispering = true;
            }
            if (_remainingDsitance < 0.8 * _distance && !_isWhispering)
            {
                FindAnyObjectByType<GameManager>().TurnOffLights(true);
            }
        }
        
        if (_isMoving && _wraithModel.transform.position == _endPoint)
        {
            _isMoving = false;
            _isWhispering = false;
            foreach (var door in _doors)
            {
                if (door.isActiveAndEnabled && !door.isDoorLocked)
                    door.GetComponent<Interactive>().isInteractive = true;
            }
            _wraithModel.SetActive(false);
        }
    }

    public void StartWraith(Vector3 point1, Vector3 point2, DoorController[] doors)
    {
        _doors = doors;
        foreach (var door in doors)
        {
            if (door.isActiveAndEnabled)
                door.GetComponent<Interactive>().isInteractive = false;
        }
        _startPoint = point1;
        _endPoint = point2;
        StartCoroutine(WraithWaiting());
    }

    public IEnumerator WraithWaiting()
    {
        yield return new WaitForSeconds(6);

        _wraithModel.SetActive(true);

        _audioSource.PlayOneShot(_whispers);
        StartMoving();
    }

    public void StartMoving()
    {
        _distance = _remainingDsitance = Vector3.Distance(_startPoint, _endPoint);
        _isMoving = true;
    }
}
