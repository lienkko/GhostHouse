using UnityEngine;
using System.Linq;

public class DoorController : MonoBehaviour
{
    private GameManager _gm;

    [Tooltip("Если true, эта дверь ГЕНЕРИРУЕТ ПЕРВУЮ комнату.")]
    public bool IsStartingDoor = false;


    public SpriteRenderer SecondDigit;
    public SpriteRenderer FirstDigit;
    public Sprite[] NumbersSprites;
    private RoomGenerator _generator;
    private DoorSide _doorSide;
    private bool _leadsToPreviousRoom;
    private int _targetRoomNumber = 0;

    public Transform ReturnTargetPoint;
    private GameObject _previousRoomRoot;

    public Transform TargetEntrySpawnPoint;
    private GameObject _nextRoomRoot;


    public DoorSide GetSide() { return _doorSide; }

    private void Awake()
    {
        if (IsStartingDoor)
        {
            GetComponent<Interactive>().SetListener(ActivateDoor);
        }
        _gm = FindAnyObjectByType<GameManager>();
        
    }

    void Start()
    {
        if (IsStartingDoor)
        {
            SetTargetRoomNumber(1);
        }
    }

    public void SetTargetRoomNumber(int number)
    {
        _targetRoomNumber = number;

        if ((SecondDigit != null) && (FirstDigit != null) && (NumbersSprites != null))
        {
            SecondDigit.sprite = NumbersSprites[_targetRoomNumber/10];
            FirstDigit.sprite = NumbersSprites[_targetRoomNumber%10];
        }
    }

    public void Initialize(RoomGenerator generator, DoorSide side, bool leadsBack,bool isLocked, GameObject previousRoomRoot = null)
    {
        if (!isLocked)
        {
            GetComponent<Interactive>().isInteractive = true;
        }
        _generator = generator;
        _doorSide = side;
        _leadsToPreviousRoom = leadsBack;
        _previousRoomRoot = previousRoomRoot;
        GetComponent<Interactive>().SetListener(ActivateDoor);
    }

    private void ActivateDoor(GameObject player)
    {

        Transform parentTransform = gameObject.transform.parent;
        if (parentTransform == null)
        {
            Debug.LogError("Объект двери не является дочерним элементом комнаты.");
            return;
        }
        GameObject currentRoomRoot = parentTransform.gameObject;

        if (_generator == null)
        {
            _generator = FindObjectOfType<RoomGenerator>();
            if (_generator == null)
            {
                Debug.LogError("RoomGenerator не найден в сцене!");
                return;
            }
        }

        if (IsStartingDoor)
        {
            DoorSide exitSide = DoorSide.North;
            DoorSide entrySideForNextRoom = _generator.GetOppositeSide(exitSide);

            if (_nextRoomRoot == null)
            {
                _nextRoomRoot = _generator.GenerateNextRoom(
                    Vector3.zero, exitSide, currentRoomRoot, gameObject.transform
                );

                if (_nextRoomRoot != null)
                {
                    DoorSide oppositeSide = _generator.GetOppositeSide(exitSide);

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
                if (_generator.Player != null && TargetEntrySpawnPoint != null)
                {
                    currentRoomRoot.SetActive(false);
                    _nextRoomRoot.SetActive(true);

                    _generator.SetPlayerPositionWithOffset(
                        TargetEntrySpawnPoint.position,
                        entrySideForNextRoom
                    );

                    this.enabled = false;
                }
            }
            return;
        }

        if (_leadsToPreviousRoom)
        {
            if (_previousRoomRoot != null && _generator.Player != null && ReturnTargetPoint != null)
            {
                currentRoomRoot.SetActive(false);
                _previousRoomRoot.SetActive(true);

                DoorSide entrySideForPreviousRoom = _generator.GetOppositeSide(_doorSide);

                DoorController targetDoorController = ReturnTargetPoint.GetComponent<DoorController>();
                if (targetDoorController != null)
                {
                    targetDoorController.enabled = true;
                }

                _generator.SetPlayerPositionWithOffset(
                    ReturnTargetPoint.position,
                    entrySideForPreviousRoom
                );
            }
        }

        else
        {
            DoorSide entrySideForNextRoom = _generator.GetOppositeSide(_doorSide);

            if (_nextRoomRoot == null)
            {
                _nextRoomRoot = _generator.GenerateNextRoom(
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
                if (_generator.Player != null && TargetEntrySpawnPoint != null)
                {
                    currentRoomRoot.SetActive(false);
                    _nextRoomRoot.SetActive(true);

                    _generator.SetPlayerPositionWithOffset(
                        TargetEntrySpawnPoint.position,
                        entrySideForNextRoom
                    );
                }
            }
        }
    }


    public void UnlockDoor()
    {
        GetComponent<Interactive>().isInteractive = true;
    }
}