using UnityEngine;

public class InitializeTest : MonoBehaviour
{
    public Safe SafeObj;
    public DoorController Door;

    private void Awake()
    {
        SafeObj = FindAnyObjectByType<Safe>();
        Door = FindAnyObjectByType<DoorController>();
        SafeObj.Initialize("TopPoint", Door);
        Destroy(gameObject);
    }
}
