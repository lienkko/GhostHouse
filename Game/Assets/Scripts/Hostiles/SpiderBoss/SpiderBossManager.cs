using UnityEngine;

public class SpiderBossManager : MonoBehaviour
{
    public static SpiderBossManager Instance;
    private int _keyCount = 0;
    private readonly int MaxKeyCount = 6;
    private DoorController _enterDoor;
    private DoorController _exitDoor;
    private void Awake()
    {
        Instance = this;
    }
    public void SetDoors(DoorController EnterDoor, DoorController ExitDoor)
    {
        _enterDoor = EnterDoor;
        _exitDoor = ExitDoor;
        _enterDoor.isDoorLocked = true;
        _exitDoor.isDoorLocked = true;
        _enterDoor.GetComponent<Interactive>().isInteractive = false;
        _exitDoor.GetComponent<Interactive>().isInteractive = false;
    }
    public void AddKey()
    {
        _keyCount++;
        if (_keyCount == MaxKeyCount)
        {
            OpenDoors();
        }
    }
    private void OpenDoors()
    {
        _enterDoor.isDoorLocked = false;
        _exitDoor.isDoorLocked = false;
        _enterDoor.GetComponent<Interactive>().isInteractive = true;
        _exitDoor.GetComponent<Interactive>().isInteractive = true;
    }
}
