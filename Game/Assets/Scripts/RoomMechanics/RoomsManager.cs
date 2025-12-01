using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RoomsManager : MonoBehaviour
{
    public static RoomsManager Instance { get; private set; }

    [Header("Префабы комнат")]
    [SerializeField]private GameObject[] _roomPrefabs;

    [Space(10)]
    [Header("Префабы дверей")]
    [SerializeField] private GameObject _eastWestDoorPrefab;
    [SerializeField] private GameObject _northDoorPrefab;
    [SerializeField] GameObject _southDoorPrefab;

    [Space(5)]
    [Header("Префаб сейфа")]
    [SerializeField] private GameObject _safePrefab;

    [Space(5)]
    [Header("Пустой объект для комнат")]
    [SerializeField] private Transform _roomsParentObject;

    private int _roomNumber = 1;
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
        if (previousRoomRoot != null)
        {
            previousRoomRoot.SetActive(false);
        }

        _roomNumber++;

        GameObject selectedRoomPrefab = _roomPrefabs[Random.Range(0, _roomPrefabs.Length)];
        CurrentRoom = Instantiate(selectedRoomPrefab, spawnPosition, Quaternion.identity, _roomsParentObject);
        RoomData roomData = CurrentRoom.GetComponent<RoomData>();

        DoorSide oppositeSide = GetOppositeSide(previousDoorSide);

        List<RoomData.DoorSpawnPoint> entryCandidates = roomData.AvailableDoorSpawns
            .Where(d => d.Side == oppositeSide)
            .ToList();

        if (entryCandidates.Count < 1)
        {
            Destroy(CurrentRoom);
            return GenerateNextRoom(spawnPosition, previousDoorSide, previousRoomRoot, lastDoorTransform);
        }

        RoomData.DoorSpawnPoint finalEntryPoint = entryCandidates[Random.Range(0, entryCandidates.Count)];
        SpawnDoor(roomData, finalEntryPoint, true, previousRoomRoot, lastDoorTransform, _roomNumber - 1);

        RoomData.DoorSpawnPoint[] availableExits = roomData.AvailableDoorSpawns
            .Where(d => d.Side != oppositeSide)
            .ToArray();

        if (availableExits.Length > 0)
        {
            int randomIndex = Random.Range(0, availableExits.Length);
            DoorSide newExitSide = availableExits[randomIndex].Side;

            if (newExitSide != oppositeSide)
            {
                SpawnDoor(roomData, newExitSide, false, _roomNumber);
            }
        }

        Transform playerSpawnPoint = finalEntryPoint.SpawnPoint;

        if (playerSpawnPoint != null)
        {
            SetPlayerPositionWithOffset(playerSpawnPoint.position, oppositeSide);
        }

        SetHideSpots(roomData);

        if (Random.Range(1,21) > 17)
            GameManager.Instance.SummonWraith();
        
        return CurrentRoom;
    }

    private void SpawnDoor(RoomData roomData, RoomData.DoorSpawnPoint spawnCandidate, bool isPreviousRoomDoor, GameObject previousRoomRoot = null, Transform returnTargetPoint = null, int doorTargetRoomNumber = 0)
    {
        Transform spawnPoint = spawnCandidate.SpawnPoint;
        DoorSide targetSide = spawnCandidate.Side;

        GameObject doorPrefabToUse = null;

        switch (targetSide)
        {
            case DoorSide.East:
            case DoorSide.West:
                doorPrefabToUse = _eastWestDoorPrefab;
                break;
            case DoorSide.North:
                doorPrefabToUse = _northDoorPrefab;
                break;
            case DoorSide.South:
                doorPrefabToUse = _southDoorPrefab;
                break;
        }

        if (doorPrefabToUse == null)
        {
            Debug.LogError($"Не назначен префаб для стороны {targetSide}. Проверьте RoomGenerator!");
            return;
        }

        GameObject doorInstance = Instantiate(doorPrefabToUse, spawnPoint.position, Quaternion.identity, roomData.transform);

        DoorController doorController = doorInstance.GetComponent<DoorController>();

        if (isPreviousRoomDoor)
        {
            roomData.PreviousRoomDoor = doorController;
        }
        else
        {
            roomData.NextRoomDoor = doorController;
        }

        if (doorController != null)
        {
            bool isDoorLocked = false;
            if (!isPreviousRoomDoor)
            {
                if (isDoorLocked = Random.Range(1, 11) > 8)
                {
                    SpawnSafe(roomData.AvailableSafeSpawns[Random.Range(0, roomData.AvailableSafeSpawns.Length)], doorController);
                }
            }

            doorController.Initialize(targetSide, isPreviousRoomDoor, isDoorLocked, previousRoomRoot);

            if (isPreviousRoomDoor)
            {
                doorController.ReturnTargetPoint = returnTargetPoint;
            }

            doorController.SetTargetRoomNumber(doorTargetRoomNumber);

        }
    }

    private void SpawnDoor(RoomData roomData, DoorSide targetSide, bool isPreviousRoomDoor, int doorTargetRoomNumber = 0)
    {
        List<RoomData.DoorSpawnPoint> candidates = roomData.AvailableDoorSpawns
            .Where(d => d.Side == targetSide)
            .ToList();

        if (candidates.Count == 0) return;

        RoomData.DoorSpawnPoint finalCandidate = candidates[Random.Range(0, candidates.Count)];

        SpawnDoor(roomData, finalCandidate, isPreviousRoomDoor, null, null, doorTargetRoomNumber);
    }

    private void SpawnSafe(Transform spawnPoint, DoorController door)
    {
        GameObject safeObject = Instantiate(_safePrefab, spawnPoint.position,Quaternion.identity, CurrentRoom.transform);
        
        safeObject.GetComponent<Safe>().Initialize(spawnPoint.tag, door);
    }

    private void SetHideSpots(RoomData roomData)
    {
        int numOfSafeSpots = 0;
        bool[] boxOfClosetsIsHideSpot = new bool[roomData.Closets.Length];

        for (int i = 0; i < roomData.Closets.Length;i++)
        {
            if (Random.Range(1, 11) > 7)
            {
                boxOfClosetsIsHideSpot[i] = true;
                numOfSafeSpots++;
            }
        }
        if (numOfSafeSpots < 1)
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
                hideSpotCloset.GetComponent <HideSpot>().Initialize();
            }
        }
    }

    public void SetPlayerPositionWithOffset(Vector3 doorPosition, DoorSide entrySide)
    {
        Vector3 finalPosition = doorPosition;
        switch (entrySide)
        {
            case DoorSide.North:
                finalPosition.y += _northEntryOffset;
                break;

            case DoorSide.South:
                finalPosition.y += _southEntryOffset;
                break;
        }

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
}