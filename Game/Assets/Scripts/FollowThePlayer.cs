using UnityEngine;

public class FollowThePlayer : MonoBehaviour
{
    void Update()
    {
        transform.position = PlayerController.Instance.transform.position + new Vector3(0,0.5f,0);
    }
}
