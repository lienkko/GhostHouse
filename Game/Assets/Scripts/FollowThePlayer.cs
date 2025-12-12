using UnityEngine;

public class FollowThePlayer : MonoBehaviour
{
    private void Awake()
    {
        PlayerController.Instance.OnDeath += TurnOff;
    }
    private void Update()
    {
        transform.position = PlayerController.Instance.transform.position + new Vector3(0,0.5f,0);
    }

    private void TurnOff()
    {
        gameObject.SetActive(false);
    } 
}
