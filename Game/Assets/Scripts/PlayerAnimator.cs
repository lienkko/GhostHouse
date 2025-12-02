using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private PlayerController _pc;
    private Animator _am;
    private BoxCollider2D _bc;
    private SpriteRenderer _sr;
    

    private void Start()
    {
        _pc = GetComponent<PlayerController>();
        _am = GetComponent<Animator>();
        _bc = GetComponent<BoxCollider2D>();
        _sr = GetComponent<SpriteRenderer>();
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

}
