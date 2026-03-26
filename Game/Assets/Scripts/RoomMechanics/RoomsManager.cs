using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RoomsManager : MonoBehaviour
{
    public static RoomsManager Instance { get; private set; }

    [Header("Префабы комнат")]
    [SerializeField] private GameObject[] _roomPrefabs;
    [SerializeField] private GameObject _bossSpiderRoomPrefab;

    [Space(10)]
    [Header("Префабы дверей")]
    [SerializeField] private GameObject _eastWestDoorPrefab;
    [SerializeField] private GameObject _northDoorPrefab;
    [SerializeField] GameObject _southDoorPrefab;

    // -------------- fake doors 23.03.2026 ------------ //
    [Space(5)]
    [Header("Префабы фейковых дверей")]
    [SerializeField] private GameObject _fakeEastWestPrefab;
    [SerializeField] private GameObject _fakeNorthPrefab;
    [SerializeField] private GameObject _fakeSouthPrefab;
    // ------------------------------------------------- //

    [Space(5)]
    [Header("Префаб сейфа")]
    [SerializeField] private GameObject _safePrefab;

    [Space(5)]
    [Header("Пустой объект для комнат")]
    [SerializeField] private Transform _roomsParentObject;

    private int _roomNumber = 24;
    private readonly float _northEntryOffset = -1.0f;
    private readonly float _southEntryOffset = 1.0f;

    public GameObject CurrentRoom { get; private set; }

    private void Awake()
    {
        Instance = this;
        DoorController.OnRoomChanged += ChangeCurrentRoom;

        DoorController startDoor = FindObjectOfType<DoorController>();
        if (startDoor != null && startDoor.IsStartingDoor)
        {
            CurrentRoom = startDoor.transform.parent.gameObject;
            CurrentRoom.GetComponent<RoomData>().NextRoomDoor = startDoor;
        }
    }

    private void ChangeCurrentRoom(GameObject room)
    {
        CurrentRoom = room;
    }

    public GameObject GenerateNextRoom(Vector3 spawnPosition, DoorSide previousDoorSide, GameObject previousRoomRoot, Transform lastDoorTransform = null)
    {
        if (previousRoomRoot != null) previousRoomRoot.SetActive(false);
        _roomNumber++;
        // ------ Boss Spider ------
        bool isBossSpiderRoom = false;
        if (_roomNumber == 25)
        {
            isBossSpiderRoom = true;
        }
        // -------------------------
        GameObject selectedRoomPrefab;
        if (isBossSpiderRoom)
            selectedRoomPrefab = _bossSpiderRoomPrefab;
        else
            selectedRoomPrefab = _roomPrefabs[Random.Range(0, _roomPrefabs.Length)];
        CurrentRoom = Instantiate(selectedRoomPrefab, spawnPosition, Quaternion.identity, _roomsParentObject);
        RoomData roomData = CurrentRoom.GetComponent<RoomData>();
        RoomData.DoorSpawnPoint finalEntryPoint;
        DoorSide oppositeSide = GetOppositeSide(previousDoorSide);
        DoorController enterBossDoor;
        DoorController exitBossDoor;
        RoomData.DoorSpawnPoint? actualExitPoint = null;
        if (!isBossSpiderRoom)
        {
            List<RoomData.DoorSpawnPoint> entryCandidates = roomData.AvailableDoorSpawns
                .Where(d => d.Side == oppositeSide).ToList();

            if (entryCandidates.Count < 1)
            {
                Destroy(CurrentRoom);
                return GenerateNextRoom(spawnPosition, previousDoorSide, previousRoomRoot, lastDoorTransform);
            }

            finalEntryPoint = entryCandidates[Random.Range(0, entryCandidates.Count)];
            SpawnDoor(roomData, finalEntryPoint, true, previousRoomRoot, lastDoorTransform, _roomNumber - 1);
            RoomData.DoorSpawnPoint[] availableExits = roomData.AvailableDoorSpawns
                .Where(d => d.Side != oppositeSide).ToArray();

            if (availableExits.Length > 0)
            {
                actualExitPoint = availableExits[Random.Range(0, availableExits.Length)];
                SpawnDoor(roomData, actualExitPoint.Value, false, null, null, _roomNumber);
            }
            SetHideSpots(roomData);
            if (Random.Range(1, 21) > 17) GameManager.Instance.SummonWraith();
            // -------------------------------------- fake doors 23.03.2026 ------------------------------------ //
            if (Random.Range(1, 101) <= 20)
            {
                var remainingPoints = roomData.AvailableDoorSpawns
                    .Where(p => p.SpawnPoint != finalEntryPoint.SpawnPoint &&
                               (actualExitPoint == null || p.SpawnPoint != actualExitPoint.Value.SpawnPoint))
                    .ToList();

                if (remainingPoints.Count > 0)
                {
                    SpawnFakeDoor(roomData, remainingPoints[Random.Range(0, remainingPoints.Count)]);
                }
            }
            // ------------------------------------------------------------------------------------------------ //
        }
        else
        {
            finalEntryPoint = roomData.EnterBossDoor;
            enterBossDoor = SpawnDoor(roomData, finalEntryPoint, true, previousRoomRoot, lastDoorTransform, _roomNumber - 1, true);
            actualExitPoint = roomData.ExitBossDoor;
            exitBossDoor = SpawnDoor(roomData, actualExitPoint.Value, false, null, null, _roomNumber, true);
            SetKeyClosets(roomData);
            CurrentRoom.GetComponentInChildren<SpiderBossManager>().SetDoors(enterBossDoor, exitBossDoor);
        }
        Transform playerSpawnPoint = finalEntryPoint.SpawnPoint;
        if (playerSpawnPoint != null)
        {
            SetPlayerPositionWithOffset(playerSpawnPoint.position, oppositeSide);
        }

        return CurrentRoom;
    }

    // --------------------------------------- fake doors 23.03.2026 --------------------------------------- //
    private void SpawnFakeDoor(RoomData roomData, RoomData.DoorSpawnPoint point)
    {
        GameObject prefab = _fakeNorthPrefab;
        if (point.Side == DoorSide.South) prefab = _fakeSouthPrefab;
        else if (point.Side == DoorSide.East || point.Side == DoorSide.West) prefab = _fakeEastWestPrefab;

        GameObject fakeObj = Instantiate(prefab, point.SpawnPoint.position, Quaternion.identity, roomData.transform);
        DoorController fakeDc = fakeObj.GetComponent<DoorController>();

        int offset = Random.value > 0.5f ? 1 : -1;
        int fakeNum = _roomNumber + offset;
        if (fakeNum < 1) fakeNum = _roomNumber + 1;

        fakeDc.InitializeFake(point.Side, fakeNum, roomData.PreviousRoomDoor);

        if (roomData.PreviousRoomDoor != null)
        {
            roomData.PreviousRoomDoor.SetDoorVisualAndInteract(false);
        }
    }
    // ---------------------------------------------------------------------------------------------------- //

    private DoorController SpawnDoor(RoomData roomData, RoomData.DoorSpawnPoint spawnCandidate, bool isPreviousRoomDoor, GameObject previousRoomRoot = null, Transform returnTargetPoint = null, int doorTargetRoomNumber = 0, bool isBossDoor = false)
    {
        Transform spawnPoint = spawnCandidate.SpawnPoint;
        DoorSide targetSide = spawnCandidate.Side;
        GameObject prefab = _northDoorPrefab;

        if (targetSide == DoorSide.South) prefab = _southDoorPrefab;
        else if (targetSide == DoorSide.East || targetSide == DoorSide.West) prefab = _eastWestDoorPrefab;

        GameObject doorInstance = Instantiate(prefab, spawnPoint.position, Quaternion.identity, roomData.transform);
        DoorController dc = doorInstance.GetComponent<DoorController>();

        bool willSpawnSafe = !isBossDoor && !isPreviousRoomDoor && Random.Range(1, 11) > 8;

        dc.Initialize(targetSide, isPreviousRoomDoor, willSpawnSafe, previousRoomRoot);
        if (isPreviousRoomDoor) dc.ReturnTargetPoint = returnTargetPoint;
        dc.SetTargetRoomNumber(doorTargetRoomNumber);

        if (isPreviousRoomDoor) roomData.PreviousRoomDoor = dc;
        else roomData.NextRoomDoor = dc;

        if (willSpawnSafe)
        {
            SpawnSafe(roomData.AvailableSafeSpawns[Random.Range(0, roomData.AvailableSafeSpawns.Length)], dc);
        }
        return dc;
    }

    private void SpawnSafe(Transform spawnPoint, DoorController door)
    {
        if (_safePrefab == null) return;
        GameObject safeObject = Instantiate(_safePrefab, spawnPoint.position, Quaternion.identity, CurrentRoom.transform);
        safeObject.GetComponent<Safe>().Initialize(spawnPoint.tag, door);
    }

    public void SetPlayerPositionWithOffset(Vector3 doorPosition, DoorSide entrySide)
    {
        Vector3 finalPosition = doorPosition;
        if (entrySide == DoorSide.North) finalPosition.y += _northEntryOffset;
        else if (entrySide == DoorSide.South) finalPosition.y += _southEntryOffset;

        PlayerController.Instance.transform.position = finalPosition;
    }

    public DoorSide GetOppositeSide(DoorSide side)
    {
        switch (side)
        {
            case DoorSide.North: return DoorSide.South;
            case DoorSide.South: return DoorSide.North;
            case DoorSide.East: return DoorSide.West;
            case DoorSide.West: return DoorSide.East;
            default: return DoorSide.North;
        }
    }

    private void SetHideSpots(RoomData roomData)
    {
        if (roomData.Closets == null || roomData.Closets.Length == 0) return;

        int numOfSafeSpots = 0;
        bool[] boxOfClosetsIsHideSpot = new bool[roomData.Closets.Length];

        for (int i = 0; i < roomData.Closets.Length; i++)
        {
            if (Random.Range(1, 11) > 7)
            {
                boxOfClosetsIsHideSpot[i] = true;
                numOfSafeSpots++;
            }
        }

        if (numOfSafeSpots < 1 && roomData.Closets.Length > 0)
        {
            SetHideSpots(roomData);
            return;
        }

        for (int i = 0; i < roomData.Closets.Length; i++)
        {
            if (boxOfClosetsIsHideSpot[i])
            {
                GameObject hideSpotCloset = roomData.Closets[i];
                hideSpotCloset.GetComponent<HideSpot>().enabled = true;
                hideSpotCloset.GetComponent<HideSpot>().Initialize();
            }
        }
    }

    private void SetKeyClosets(RoomData roomData)
    {
        List<GameObject> keyClosets = roomData.Closets.ToList();
        for (int i = 0; i < 6; i++)
        {
            int closetIndex = Random.Range(0, keyClosets.Count);
            keyClosets[closetIndex].GetComponent<KeyCloset>().Initialize();
            keyClosets.RemoveAt(closetIndex);
        }
    }
}
