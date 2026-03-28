using UnityEngine;
using System.Linq;

public class DoorController : MonoBehaviour
{
    [Tooltip("Если true, эта дверь ГЕНЕРИРУЕТ ПЕРВУЮ комнату.")]
    public bool IsStartingDoor = false;
    public bool IsTestBossDoor = false;

    [SerializeField] private SpriteRenderer _secondDigit;
    [SerializeField] private SpriteRenderer _firstDigit;
    [SerializeField] private Sprite[] _numbersSprites;
    public Transform HandSpawnPointLeft;
    public Transform HandSpawnPointRight;
    public Transform HandSpawnPointBot;
    public Transform HandSpawnPointTop;

    private DoorSide _doorSide;
    private bool _leadsToPreviousRoom;
    private int _targetRoomNumber = 0;
    private GameObject _previousRoomRoot;
    private GameObject _nextRoomRoot;

    [HideInInspector] public Transform TargetEntrySpawnPoint;
    [HideInInspector] public Transform ReturnTargetPoint;
    [HideInInspector] public bool isDoorLocked = true;

    // ---- fake doors 23.03.2026 --- //
    private bool _isFakeDoor = false;
    private bool _isBlockedByFake = false;
    private DoorController _linkedBackDoor;
    // ------------------------------ //

    public DoorSide GetSide() { return _doorSide; }

    public delegate void ChangeRoom(GameObject room);
    public static event ChangeRoom OnRoomChanged;

    private void Awake()
    {
        if (IsStartingDoor)
        {
            GetComponent<Interactive>().SetListener(ActivateDoor);
            GetComponent<Interactive>().isInteractive = false;
            SetTargetRoomNumber(1);
        }
        if (IsTestBossDoor)
        {
            GetComponent<Interactive>().SetListener(ActivateDoor);
            GetComponent<Interactive>().isInteractive = true;
            isDoorLocked = false;
            SetTargetRoomNumber(25);

        }
    }

    public void SetTargetRoomNumber(int number)
    {
        _targetRoomNumber = number;

        if ((_secondDigit != null) && (_firstDigit != null) && (_numbersSprites != null))
        {
            int tens = Mathf.Clamp(_targetRoomNumber / 10, 0, _numbersSprites.Length - 1);
            int units = Mathf.Clamp(_targetRoomNumber % 10, 0, _numbersSprites.Length - 1);
            _secondDigit.sprite = _numbersSprites[tens];
            _firstDigit.sprite = _numbersSprites[units];
        }
    }

    // -------------------------- fake doors 23.03.2026 -------------------------- //
    public void InitializeFake(DoorSide side, int fakeNumber, DoorController backDoor)
    {
        _doorSide = side;
        _targetRoomNumber = fakeNumber;
        _isFakeDoor = true;
        _linkedBackDoor = backDoor;

        SetTargetRoomNumber(fakeNumber);
        GetComponent<Interactive>().SetListener(ActivateDoor);
        GetComponent<Interactive>().isInteractive = true;
    }

    public void SetDoorVisualAndInteract(bool state)
    {
        _isBlockedByFake = !state;
        if (_firstDigit != null) _firstDigit.enabled = state;
        if (_secondDigit != null) _secondDigit.enabled = state;
        GetComponent<Interactive>().isInteractive = state;
    }
    // --------------------------------------------------------------------------- //

    // ------ Boss Spider ------

    public void Initialize(DoorSide side, bool leadsBack, bool isLocked, GameObject previousRoomRoot = null)
    {
        _doorSide = side;
        _leadsToPreviousRoom = leadsBack;
        _previousRoomRoot = previousRoomRoot;
        isDoorLocked = isLocked;

        GetComponent<Interactive>().SetListener(ActivateDoor);
        GetComponent<Interactive>().isInteractive = !isLocked;
    }

    public void ActivateDoor()
    {
        if (_isBlockedByFake) return;

        // ------------------------ fake doors 23.03.2026 ------------------------ //
        if (_isFakeDoor)
        {
            PlayerController.Instance.InflictDamage(40);
            if (_linkedBackDoor != null) _linkedBackDoor.SetDoorVisualAndInteract(true);
            GetComponent<Interactive>().isInteractive = false;
            return;
        }
        // ---------------------------------------------------------------------- //

        if (isDoorLocked && !IsStartingDoor && !_leadsToPreviousRoom) return;

        GameObject currentRoomRoot = gameObject.transform.parent.gameObject;
        if (_targetRoomNumber == 26)
        {
            GameManager.Instance.EndGame();
            return;
        }

        if (IsStartingDoor)
        {
            DoorSide exitSide = DoorSide.North;
            DoorSide entrySideForNextRoom = RoomsManager.Instance.GetOppositeSide(exitSide);

            if (_nextRoomRoot == null)
            {
                _nextRoomRoot = RoomsManager.Instance.GenerateNextRoom(Vector3.zero, exitSide, currentRoomRoot, transform);

                // Находим точку входа в первой созданной комнате для будущих возвратов
                if (_nextRoomRoot != null)
                {
                    DoorController backDoorInstance = _nextRoomRoot.GetComponentsInChildren<DoorController>()
                         .FirstOrDefault(d => d._leadsToPreviousRoom && d.GetSide() == entrySideForNextRoom);
                    if (backDoorInstance != null) this.TargetEntrySpawnPoint = backDoorInstance.transform;
                }
            }
            else
            {
                if (TargetEntrySpawnPoint != null)
                {
                    currentRoomRoot.SetActive(false);
                    _nextRoomRoot.SetActive(true);
                    RoomsManager.Instance.SetPlayerPositionWithOffset(TargetEntrySpawnPoint.position, entrySideForNextRoom);
                    OnRoomChanged?.Invoke(_nextRoomRoot);
                }
            }
            return;
        }

        if (_leadsToPreviousRoom)
        {
            if (_previousRoomRoot != null && ReturnTargetPoint != null)
            {
                currentRoomRoot.SetActive(false);
                _previousRoomRoot.SetActive(true);
                RoomsManager.Instance.SetPlayerPositionWithOffset(ReturnTargetPoint.position, RoomsManager.Instance.GetOppositeSide(_doorSide));
                OnRoomChanged?.Invoke(_previousRoomRoot);
            }
        }
        else
        {
            DoorSide entrySideForNextRoom = RoomsManager.Instance.GetOppositeSide(_doorSide);

            if (_nextRoomRoot == null)
            {
                _nextRoomRoot = RoomsManager.Instance.GenerateNextRoom(currentRoomRoot.transform.position, _doorSide, currentRoomRoot, transform);

                if (_nextRoomRoot != null)
                {
                    DoorController backDoorInstance = _nextRoomRoot.GetComponentsInChildren<DoorController>()
                         .FirstOrDefault(d => d._leadsToPreviousRoom && d.GetSide() == entrySideForNextRoom);
                    if (backDoorInstance != null) this.TargetEntrySpawnPoint = backDoorInstance.transform;
                }
            }
            else
            {
                if (TargetEntrySpawnPoint != null)
                {
                    currentRoomRoot.SetActive(false);
                    _nextRoomRoot.SetActive(true);
                    RoomsManager.Instance.SetPlayerPositionWithOffset(TargetEntrySpawnPoint.position, entrySideForNextRoom);
                }
                OnRoomChanged?.Invoke(_nextRoomRoot);
            }
        }
    }

    public void UnlockDoor()
    {
        GetComponent<Interactive>().isInteractive = true;
        isDoorLocked = false;
    }
}
