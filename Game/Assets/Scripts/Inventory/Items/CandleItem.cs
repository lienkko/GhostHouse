using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class CandleItem : Item, IChargeableItem
{
    private bool _isactive = false;
    private const float MaxCharge = 100f;
    private float _candleCharge = 100f;
    [SerializeField] private float _dischargeSpeed = 5f;
    public float CandleCharge
    {
        get
        {
            return _candleCharge;
        }
        set
        {
            _candleCharge = value;

            if (_candleCharge > MaxCharge)
                _candleCharge = MaxCharge;

            if (_candleCharge < 0f)
                _candleCharge = 0f;
        }
    }
    public float CurrentChargeNormalized => CandleCharge / MaxCharge;
    public Item ItemObj => this;
    private Light2D _light2D;
    protected override void Awake()
    {
        base.Awake();
        _light2D = GetComponent<Light2D>();
        _light2D.enabled = false;
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
    public override void Hide()
    {
        base.Hide();
        _isactive = false;
        _light2D.enabled = false;
    }
    public override void Unhide()
    {
        base.Unhide();
        if (CandleCharge <= 0f)
            return;

        _isactive = true;
        _light2D.enabled = true;
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
