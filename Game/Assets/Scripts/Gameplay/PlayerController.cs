using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float MoveSpeed;
    private Vector2 MoveDir;
    private Rigidbody2D _playerRB;

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

        MoveDir = new Vector2(moveH, moveV).normalized;
    }

    private void Move()
    {
        _playerRB.velocity = MoveDir*MoveSpeed;
    }
}
