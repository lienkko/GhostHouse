using Unity.VisualScripting;
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
        if (_isactive)
        {
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
