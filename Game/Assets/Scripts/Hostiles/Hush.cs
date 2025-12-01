using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Hush : MonoBehaviour
{
    [SerializeField] private AudioClip _soundClip;
    [SerializeField] private Transform _hushModel;
    [SerializeField] private PlayerController _player;
    private AudioSource _audioSource;
    private Vector3 _pointNearPlayer;

    private bool _isWaitingForLight = false;    
    private bool _isPsst = false;
    [SerializeField] private bool _isDarkness = false;

    private float _distance;
    private float _remainingDistance;

    

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        
    }

    private void Update()
    {
        if (_isDarkness && !_isPsst)
            StartCoroutine(WaitForPsst());
    }

    private void FixedUpdate()
    {
        if (_isWaitingForLight)
        {
            _hushModel.position = Vector3.Lerp(_player.transform.position + _pointNearPlayer, _player.transform.position, 1 - (_remainingDistance / _distance));
            _remainingDistance -= 1f * Time.deltaTime;
        }
    }

    private IEnumerator WaitForPsst()
    {
        _isPsst = true;
        yield return new WaitForSeconds(5);
        MakePsst();
    }

    private void MakePsst()
    {
        _audioSource.PlayOneShot(_soundClip);
        Debug.Log("Psst");
        GetComponent<WhenCollide>().OnPlayerCollide += HidePsst;
        _hushModel.gameObject.SetActive(true);
        _pointNearPlayer = Random.insideUnitCircle.normalized * 3;
        _hushModel.position = _player.transform.position + _pointNearPlayer;
        _distance = _remainingDistance = Vector3.Distance(_player.transform.position, _hushModel.position);
        _isWaitingForLight = true;
    }

    private void HidePsst(Collider2D collider)
    {
        _isWaitingForLight = _isPsst = _isDarkness = false;
        GetComponent<WhenCollide>().OnPlayerCollide -= HidePsst;
        _hushModel.gameObject.SetActive(false);
    }
}
