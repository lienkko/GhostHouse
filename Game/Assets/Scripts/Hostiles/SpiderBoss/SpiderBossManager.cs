using System.Collections;
using TMPro;
using UnityEngine;

public class SpiderBossManager : MonoBehaviour
{
    private static WaitForSeconds _waitForSeconds0_05 = new(0.05f);
    private static WaitForSeconds _waitForSeconds3 = new(3);
    public static SpiderBossManager Instance;
    private int _keyCount = 0;
    private readonly int MaxKeyCount = 10;
    private DoorController _enterDoor;
    private DoorController _exitDoor;
    [SerializeField] private Spider _spider;
    [SerializeField] private TextMeshProUGUI _spiderWarning;


    private int _counter = 0;

    private void Awake()
    {
        Instance = this;
        GameManager.Instance.GameUIFields.KeysCount.gameObject.SetActive(true);
    }
    public void SetDoors(DoorController EnterDoor, DoorController ExitDoor)
    {
        _enterDoor = EnterDoor;
        _exitDoor = ExitDoor;
        _enterDoor.LockDoor();
        _exitDoor.LockDoor();
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
        _exitDoor.UnlockDoor();
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
    public IEnumerator ActivateWarning()
    {
        _spiderWarning.gameObject.SetActive(true);
        _spiderWarning.alpha = 1;
        _counter++;
        yield return _waitForSeconds3;
        if (_counter <= 1)
        {
            for (int i = 0; i < 10; i++)
            {
                if (_counter > 1)
                    break;
                _spiderWarning.alpha -= 0.1f;
                yield return _waitForSeconds0_05;
            }
            if (_counter <= 1)
                _spiderWarning.gameObject.SetActive(false);
        }
        _counter--;
    }
    private void UpdateKeysField()
    {
        GameManager.Instance.GameUIFields.KeysCount.text = $"{_keyCount}/{MaxKeyCount}";
    }
}
