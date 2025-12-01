using UnityEngine;
using System.Linq;

public class DoorController : MonoBehaviour
{
    [Tooltip("Если true, эта дверь ГЕНЕРИРУЕТ ПЕРВУЮ комнату.")]
    public bool IsStartingDoor = false;


    [SerializeField] private SpriteRenderer _secondDigit;
    [SerializeField] private SpriteRenderer _firstDigit;
    [SerializeField] private Sprite[] _numbersSprites;

    private DoorSide _doorSide;
    private bool _leadsToPreviousRoom;
    private int _targetRoomNumber = 0;
    private GameObject _previousRoomRoot;
    private GameObject _nextRoomRoot;


    [HideInInspector] public Transform TargetEntrySpawnPoint;
    [HideInInspector] public Transform ReturnTargetPoint;
    [HideInInspector] public bool isDoorLocked = true; 

    public DoorSide GetSide() { return _doorSide; }

    public delegate void ChangeRoom(GameObject room);
    public static event ChangeRoom OnRoomChanged;

    private void Awake()
    {
        if (IsStartingDoor)
        {
            GetComponent<Interactive>().SetListener(ActivateDoor);
            SetTargetRoomNumber(1);
        }
    }

    public void SetTargetRoomNumber(int number)
    {
        _targetRoomNumber = number;

        if ((_secondDigit != null) && (_firstDigit != null) && (_numbersSprites != null))
        {
            _secondDigit.sprite = _numbersSprites[_targetRoomNumber/10];
            _firstDigit.sprite = _numbersSprites[_targetRoomNumber%10];
        }
    }

    public void Initialize(DoorSide side, bool leadsBack,bool isLocked, GameObject previousRoomRoot = null)
    {
        if (!isLocked)
        {
            GetComponent<Interactive>().isInteractive = true;
            isDoorLocked = false;
        }
        _doorSide = side;
        _leadsToPreviousRoom = leadsBack;
        _previousRoomRoot = previousRoomRoot;
        GetComponent<Interactive>().SetListener(ActivateDoor);
    }

    public void ActivateDoor()
    {
        
            GameObject currentRoomRoot = gameObject.transform.parent.gameObject;

        if (IsStartingDoor)
        {
            DoorSide exitSide = DoorSide.North;
            DoorSide entrySideForNextRoom = RoomsManager.Instance.GetOppositeSide(exitSide);

            if (_nextRoomRoot == null)
            {
                _nextRoomRoot = RoomsManager.Instance.GenerateNextRoom(
                    Vector3.zero, exitSide, currentRoomRoot, gameObject.transform
                );

                if (_nextRoomRoot != null)
                {
                    DoorSide oppositeSide = RoomsManager.Instance.GetOppositeSide(exitSide);

                    DoorController backDoorInstance = _nextRoomRoot.GetComponentsInChildren<DoorController>()
                         .FirstOrDefault(d => d._leadsToPreviousRoom && d.GetSide() == oppositeSide);

                    if (backDoorInstance != null)
                    {
                        this.TargetEntrySpawnPoint = backDoorInstance.transform;
                        this.enabled = false;
                    }
                }
            }
            else
            {
                if (TargetEntrySpawnPoint != null)
                {
                    currentRoomRoot.SetActive(false);
                    _nextRoomRoot.SetActive(true);

                    RoomsManager.Instance.SetPlayerPositionWithOffset(
                        TargetEntrySpawnPoint.position,
                        entrySideForNextRoom
                    );
                    this.enabled = false;
                }
                OnRoomChanged?.Invoke(_nextRoomRoot);
            }
            return;
        }

        if (_leadsToPreviousRoom)
        {
            if (_previousRoomRoot != null && ReturnTargetPoint != null)
            {
                currentRoomRoot.SetActive(false);
                _previousRoomRoot.SetActive(true);

                DoorSide entrySideForPreviousRoom = RoomsManager.Instance.GetOppositeSide(_doorSide);

                DoorController targetDoorController = ReturnTargetPoint.GetComponent<DoorController>();
                if (targetDoorController != null)
                {
                    targetDoorController.enabled = true;
                }

                RoomsManager.Instance.SetPlayerPositionWithOffset(
                    ReturnTargetPoint.position,
                    entrySideForPreviousRoom
                );
                OnRoomChanged?.Invoke(_previousRoomRoot);
            }
        }

        else
        {
            DoorSide entrySideForNextRoom = RoomsManager.Instance.GetOppositeSide(_doorSide);

            if (_nextRoomRoot == null)
            {
                _nextRoomRoot = RoomsManager.Instance.GenerateNextRoom(
                    currentRoomRoot.transform.position, _doorSide, currentRoomRoot, gameObject.transform
                );

                if (_nextRoomRoot != null)
                {
                    DoorController backDoorInstance = _nextRoomRoot.GetComponentsInChildren<DoorController>()
                         .FirstOrDefault(d => d._leadsToPreviousRoom && d.GetSide() == entrySideForNextRoom);
                    if (backDoorInstance != null)
                    {
                        this.TargetEntrySpawnPoint = backDoorInstance.transform;
                    }
                }
            }
            else
            {
                if (TargetEntrySpawnPoint != null)
                {
                    currentRoomRoot.SetActive(false);
                    _nextRoomRoot.SetActive(true);

                    RoomsManager.Instance.SetPlayerPositionWithOffset(
                        TargetEntrySpawnPoint.position,
                        entrySideForNextRoom
                    );
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