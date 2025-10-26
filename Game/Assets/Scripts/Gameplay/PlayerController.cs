using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool _isSneaking;
    private bool _isCrouching;
    private Rigidbody2D _playerRB;
    private Vector2 _moveDir;


    public float MoveSpeed;
    public Vector2 MoveDir { get => _moveDir; }
    public bool IsCrouching { get => _isCrouching; }

    private void Awake()
    {
        _playerRB = GetComponent<Rigidbody2D>();
        MoveSpeed = 5f;
    }
    private void Update()
    {
        InputMovement();
    }

    private void FixedUpdate()
    {
        Move(); 
    }

    private void InputMovement()
    {

        float moveH = Input.GetAxisRaw("Horizontal");
        float moveV = Input.GetAxisRaw("Vertical");
        _isCrouching = Input.GetKey(KeyCode.C);
        _isSneaking = Input.GetKey(KeyCode.LeftShift);


        _moveDir = new Vector2(moveH, moveV).normalized;
    }

    private void Move()
    {
        if (_isCrouching)
            MoveSpeed = 1.5f;
        else if (_isSneaking)
            MoveSpeed = 2f;
        else
            MoveSpeed = 5f;
        _playerRB.velocity = _moveDir * MoveSpeed;
    }
}
