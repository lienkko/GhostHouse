using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class FlashlightItem : Item
{
    private bool _isactive = false;
    private Light2D _light2D;
    protected override void Awake()
    {
        base.Awake();
        _light2D = GetComponent<Light2D>();
    }
    private void Update()
    {

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
            _isactive = true;
            _light2D.enabled = true;
        }
    }
}
