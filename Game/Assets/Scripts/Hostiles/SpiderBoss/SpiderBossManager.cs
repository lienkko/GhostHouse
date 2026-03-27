using Unity.VisualScripting;
using UnityEngine;

public class SpiderBossManager : MonoBehaviour
{
    public static SpiderBossManager Instance;
    private int _keyCount = 0;
    private readonly int MaxKeyCount = 10;
    private DoorController _enterDoor;
    private DoorController _exitDoor;
    [SerializeField] private Spider _spider;

    private void Awake()
    {
        Instance = this;
        GameManager.Instance.GameUIFields.KeysCount.gameObject.SetActive(true);
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
        UpdateKeysField();
        if (_keyCount == MaxKeyCount)
        {
            OpenDoor();
        }
        _spider.Trigger(PlayerController.Instance.transform.position);
    }
    private void OpenDoor()
    {
        _exitDoor.isDoorLocked = false;
        _exitDoor.GetComponent<Interactive>().isInteractive = true;
    }
    private bool IsPlayerRunning()
    {
        PlayerController pc = PlayerController.Instance;
        return pc.MoveDir != Vector2.zero && !pc.IsCrouching;
    }
    private void Update()
    {
        if (IsPlayerRunning())
        {
            _spider.Trigger(PlayerController.Instance.transform.position);
        }
    }
    private void UpdateKeysField()
    {
        GameManager.Instance.GameUIFields.KeysCount.text = $"{_keyCount}/{MaxKeyCount}";
    }
}
