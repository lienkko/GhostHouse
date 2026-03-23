using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class CandleItem : Item
{
    private bool _isactive = false;
    private float _candleCharge = 100f;
    public float CandleCharge
    {
        get
        {
            return _candleCharge;
        }
        set
        {
            _candleCharge = value;

            if (_candleCharge > 100f)
                _candleCharge = 100f;

            if (_candleCharge < 0f)
                _candleCharge = 0f;
        }
    }
    private float _dischargeSpeed = 0.04f;
    private Light2D _light2D;
    protected override void Awake()
    {
        base.Awake();
        _light2D = GetComponent<Light2D>();
    }
    private void Update()
    {
        if (_isactive)
        {
            CandleCharge -= _dischargeSpeed * Time.deltaTime;

            if (CandleCharge <= 0)
            {
                CandleCharge = 0;
                _isactive = false;
                _light2D.enabled = false;
                return;
            }
        }
    }
    public override bool UseAndDestroy()
    {
        ChangeMode();
        return false;
    }

    private void ChangeMode()
    {
        if (_isactive)
        {
            _isactive = false;
            _light2D.enabled = false;
        }
        else
        {
            if (CandleCharge <= 0)
                return;

            _isactive = true;
            _light2D.enabled = true;
        }
    }
}
