using UnityEngine;
using System.Linq;

public class DoorController : MonoBehaviour
{
    [Tooltip("Если true, эта дверь ГЕНЕРИРУЕТ ПЕРВУЮ комнату.")]
    public bool IsStartingDoor = false;
    public GameObject OutlineVisual;

    private RoomGenerator _generator;
    private DoorSide _doorSide;
    private bool _leadsToPreviousRoom;

    public Transform ReturnTargetPoint;
    private GameObject _previousRoomRoot;

    public Transform TargetEntrySpawnPoint;

    private GameObject _nextRoomRoot;
    private bool _playerIsNear = false;

    public DoorSide GetSide() { return _doorSide; }

    public void Initialize(RoomGenerator generator, DoorSide side, bool leadsBack, GameObject previousRoomRoot = null)
    {
        _generator = generator;
        _doorSide = side;
        _leadsToPreviousRoom = leadsBack;
        _previousRoomRoot = previousRoomRoot;
    }

    private void Update()
    {
        if (_playerIsNear && Input.GetKeyDown(KeyCode.E))
        {
            ActivateDoor();
        }
    }

    private void ActivateDoor()
    {
        if (OutlineVisual != null)
        {
            OutlineVisual.SetActive(false);
        }

        GameObject currentRoomRoot = gameObject.transform.parent.gameObject;

        if (_generator == null)
        {
            _generator = FindObjectOfType<RoomGenerator>();
            if (_generator == null) return;
        }

        if (IsStartingDoor)
        {
            if (_nextRoomRoot == null)
            {
                _nextRoomRoot = _generator.GenerateNextRoom(
                    Vector3.zero,
                    DoorSide.East,
                    currentRoomRoot,
                    gameObject.transform
                );

                if (_nextRoomRoot != null)
                {
                    DoorSide oppositeSide = _generator.GetOppositeSide(DoorSide.East);
                    DoorController backDoorInstance = _nextRoomRoot.GetComponentsInChildren<DoorController>()
                         .FirstOrDefault(d => d._leadsToPreviousRoom && d.GetSide() == oppositeSide);
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
                    _generator.Player.position = TargetEntrySpawnPoint.position;
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
                _generator.Player.position = ReturnTargetPoint.position;
            }
        }

        else
        {
            if (_nextRoomRoot == null)
            {
                _nextRoomRoot = _generator.GenerateNextRoom(
                    currentRoomRoot.transform.position,
                    _doorSide,
                    currentRoomRoot,
                    gameObject.transform
                );

                if (_nextRoomRoot != null)
                {
                    DoorSide oppositeSide = _generator.GetOppositeSide(_doorSide);
                    DoorController backDoorInstance = _nextRoomRoot.GetComponentsInChildren<DoorController>()
                         .FirstOrDefault(d => d._leadsToPreviousRoom && d.GetSide() == oppositeSide);
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
                    _generator.Player.position = TargetEntrySpawnPoint.position;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerIsNear = true;
            if (OutlineVisual != null) OutlineVisual.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerIsNear = false;
            if (OutlineVisual != null) OutlineVisual.SetActive(false);
        }
    }
}