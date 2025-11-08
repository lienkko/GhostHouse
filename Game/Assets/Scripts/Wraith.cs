using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wraith : MonoBehaviour
{
    public Vector3 startPoint;
    public Vector3 endPoint;
    public bool isMoving = false;
    private float _distance;
    private float _remDsitance;
    public delegate void OnDestAction();
    public event OnDestAction OnFinish;

    public void StartMoving(Vector3 sp, Vector3 ep)
    {
        startPoint = sp;
        endPoint = ep;
        _distance = Vector3.Distance(sp, ep);
        _remDsitance = _distance;
        isMoving = true;
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            transform.position = Vector3.Lerp(startPoint, endPoint, 1-(_remDsitance/_distance));
            _remDsitance -= 3f * Time.deltaTime;
        }
        if (isMoving && transform.position == endPoint)
        {
            isMoving = false;
            OnFinish?.Invoke();
            gameObject.SetActive(false);
            
        }
    }


}
