using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class FlashlightItem : Item
{
    private bool _isactive = false;
    private float _flashLightCharge = 0.1f;
    private readonly float _dischargeSpeed = 0.02f;
    public float FlaslightCharge
    {
        get
        {
            return _flashLightCharge;
        }
        set
        {
            _flashLightCharge = value;
            if (_flashLightCharge > 1)
            {
                _flashLightCharge = 1;
            }
            if (_flashLightCharge < 0)
            {
                _flashLightCharge = 0;
            }
        }
    }
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
            FlaslightCharge -= _dischargeSpeed * Time.deltaTime;

            if (FlaslightCharge <= 0)
            {
                FlaslightCharge = 0;
                _isactive = false;
                _light2D.enabled = false;
                return;
            }

            Vector2 dir = PlayerController.Instance.MoveDir;

            if (dir != Vector2.zero)
            {
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);
            }
        }
    }
    public override bool UseAndDestroy()
    {
        ChangeMode();
        return false;
    }
    public override void HideItem()
    {
        base.HideItem();
        _isactive = false;
        _light2D.enabled = false;
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
            if (FlaslightCharge <= 0)
                return;
            _isactive = true;
            _light2D.enabled = true;
        }
    }
}
