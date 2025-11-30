using UnityEngine;

public class FollowThePlayer : MonoBehaviour
{
    private Transform _player;

    private void Awake()
    {
        _player = FindAnyObjectByType<PlayerController>().transform;
    }

    void Update()
    {
        transform.position = _player.position + new Vector3(0,0.5f,0);
    }
}
