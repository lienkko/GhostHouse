using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public enum AnimStates { Nothing, BigBob, FlashLight, Candle }
    [SerializeField]
    private static Dictionary<AnimStates, string> _animParams = new Dictionary<AnimStates, string> { { AnimStates.Nothing, "WithN" }, { AnimStates.BigBob, "WithBB" }, { AnimStates.Candle, "WithC" }, { AnimStates.FlashLight, "WithFL" } };
    private PlayerController _pc;
    private static Animator _am;
    private BoxCollider2D _bc;
    private SpriteRenderer _sr;


    private void Start()
    {
        _pc = GetComponent<PlayerController>();
        _am = GetComponent<Animator>();
        _bc = GetComponent<BoxCollider2D>();
        _sr = GetComponent<SpriteRenderer>();
        _am.SetBool("WithN", true);
    }

    private void Update()
    {
        if (_pc.DeltaMove != Vector3.zero)
        {
            _am.SetBool("Walk", true);
            _am.SetBool("Idle", false);
        }
        else
        {
            _am.SetBool("Walk", false);
            _am.SetBool("Idle", true);
        }
        if (_pc.IsCrouching)
        {
            _am.SetBool("Walk", false);
            _am.SetBool("Crouch", true);
            _am.SetBool("Idle", false);
        }
        else
        {
            _am.SetBool("Crouch", false);
        }

        if (_pc.LastHorizontalVector > 0)
            _sr.flipX = true;
        else if (_pc.LastHorizontalVector < 0)
            _sr.flipX = false;

    }
    public static void ChangeAnimState(AnimStates state)
    {
        foreach (var key in _animParams.Keys)
        {
            if (key == state)
            {
                _am.SetBool(_animParams[key], true);
            }
            else
            {
                _am.SetBool(_animParams[key], false);
            }
        }
    }

}
