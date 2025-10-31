using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private PlayerController _pc;
    private Animator _am;
    private BoxCollider2D _bc;

    private void Awake()
    {
        _pc = GetComponent<PlayerController>();
        _am = GetComponent<Animator>();
        _bc = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (_pc.MoveDir != Vector2.zero)
            _am.SetBool("Walk", true);
        else
            _am.SetBool("Walk", false);
        if (_pc.IsCrouching)
        {
            _am.SetBool("Crouch", true);
            _bc.offset = new Vector2(0, 0.3f);
            _bc.size = new Vector2(0.6f, 0.6f);

        }
        else
        {
            _am.SetBool("Crouch", false);
            _bc.offset = new Vector2(0, 0.5f);
            _bc.size = new Vector2(0.7f, 1f);
        }
    }

}
