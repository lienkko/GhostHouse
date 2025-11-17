using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WraithHandler : MonoBehaviour
{
    [SerializeField] private GameObject _textWarning;
    [SerializeField] private GameObject _wraithModel;

    private float _distance = 0;
    private float _remainingDsitance = 0;
    private Vector3 _startPoint = Vector3.zero;
    private Vector3 _endPoint = Vector3.zero;
    private bool _isMoving = false;
    private bool _isWraithWaiting = false;

    

    private void Update()
    {
        if (!_isWraithWaiting)
            StartCoroutine(WaitForWraith());
    }

    private void FixedUpdate()
    {
        if (_isMoving)
        {
            setPoints();

            _wraithModel.transform.position = Vector3.Lerp(_startPoint, _endPoint, 1-(_remainingDsitance/_distance));
            _remainingDsitance -= 3f * Time.deltaTime;
        }
        if (_isMoving && _wraithModel.transform.position == _endPoint)
        {
            _isMoving = false;
            EndWraithFlight();
            _wraithModel.SetActive(false);
        }
    }

    private void setPoints()
    {
        Camera cam = Camera.main;
        float offset = cam.orthographicSize * cam.aspect + 5;
        _startPoint.x = cam.transform.position.x - offset;
        _endPoint.x = cam.transform.position.x + offset;
        _startPoint.y = _endPoint.y = cam.transform.position.y;
    }

    private IEnumerator WaitForWraith()
    {
        _isWraithWaiting = true;
        yield return new WaitForSeconds(5);
        StartCoroutine(WraithWarning());
    }

    private IEnumerator WraithWarning()
    {

        _textWarning.SetActive(true);
        yield return new WaitForSeconds(5);
        _textWarning.SetActive(false);

        _wraithModel.SetActive(true);
        StartMoving();
    }

    public void StartMoving()
    {
        setPoints();
        _distance = _remainingDsitance = Vector3.Distance(_startPoint, _endPoint);
        _isMoving = true;
    }

    private void EndWraithFlight()
    {
        _isWraithWaiting = false;
    }
}
