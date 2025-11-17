using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Hush : MonoBehaviour
{
    [SerializeField] private AudioClip _soundClip;
    [SerializeField] private Transform _hushModel;
    [SerializeField] private PlayerController player;
    private AudioSource _audioSource;
    
    private bool _isWaitingForPsst = false;
    private bool _isWaitingForLight = false;
    [SerializeField] private bool _isDarkness = false;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (_isDarkness && !_isWaitingForPsst)
            StartCoroutine(WaitForPsst());
    }

    private void FixedUpdate()
    {
        if (_isWaitingForLight)
        {

        }
    }

    private IEnumerator WaitForPsst()
    {
        _isWaitingForPsst = true;
        yield return new WaitForSeconds(5);
        MakePsst();
    }

    private void MakePsst()
    {
        _audioSource.PlayOneShot(_soundClip);
        Debug.Log("Psst");
        _hushModel.gameObject.SetActive(true);
        Vector2 randomPoint = Random.insideUnitCircle.normalized;
        _hushModel.position = player.transform.position + (Vector3)randomPoint * 3;
        _isWaitingForPsst = false;
        _isDarkness = false;
    }
}
