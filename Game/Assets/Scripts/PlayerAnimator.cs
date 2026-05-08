using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private static readonly int CrouchHash = Animator.StringToHash("Crouch");
    private static readonly int WalkHash = Animator.StringToHash("Walk");
    private static readonly int IdleHash = Animator.StringToHash("Idle");
    private static readonly int TrappedHash = Animator.StringToHash("Trapped");

    public enum AnimStates { Nothing, BigBob, FlashLight, Candle }
    private static Dictionary<AnimStates, string> _animParams = new() { { AnimStates.Nothing, "WithN" }, { AnimStates.BigBob, "WithBB" }, { AnimStates.Candle, "WithC" }, { AnimStates.FlashLight, "WithFL" } };
    private PlayerController _pc;
    private static Animator _am;
    private SpriteRenderer _sr;


    private void Awake()
    {
        _pc = GetComponent<PlayerController>();
        _am = GetComponent<Animator>();
        _sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (_am.GetBool(TrappedHash))
        {
            _am.SetBool(IdleHash, false);
            _am.SetBool(WalkHash, false);
            _am.SetBool(CrouchHash, false);
            return;
        }
        if (_pc.DeltaMove != Vector3.zero)
        {
            _am.SetBool(WalkHash, true);
            _am.SetBool(IdleHash, false);
        }
        else
        {
            _am.SetBool(WalkHash, false);
            _am.SetBool(IdleHash, true);
        }
        if (_pc.IsCrouching && _pc.DeltaMove != Vector3.zero)
        {
            _am.SetBool(WalkHash, false);
            _am.SetBool(CrouchHash, true);
            _am.SetBool(IdleHash, false);
        }
        else
        {
            _am.SetBool(CrouchHash, false);
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
