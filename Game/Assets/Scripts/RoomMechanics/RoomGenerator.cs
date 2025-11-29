using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro.EditorUtilities;

public class RoomGenerator : MonoBehaviour
{
    public GameObject[] RoomPrefabs;
    public GameObject EastWestDoorPrefab;
    public GameObject NorthDoorPrefab;
    public GameObject SouthDoorPrefab;

    public GameObject SafePrefab;

    public Transform Player;
    public Transform RoomsParent;

    public float NorthEntryOffset = -1.0f;
    public float SouthEntryOffset = 1.0f;

    private GameObject _currentRoomInstance;

    private int _roomDepth = 1;

    private GameManager _gm;

    void Start()
    {
        _gm = FindAnyObjectByType<GameManager>();
        if (Player == null)
        {
            Debug.LogError("Игрок не назначен в генераторе!");
        }

        DoorController startDoor = FindObjectOfType<DoorController>();
        if (startDoor != null && startDoor.IsStartingDoor)
        {
            _currentRoomInstance = startDoor.transform.parent.gameObject;
        }
    }

    public GameObject GenerateNextRoom(Vector3 spawnPosition, DoorSide previousDoorSide, GameObject previousRoomRoot, Transform lastDoorTransform = null)
    {
        if (previousRoomRoot != null)
        {
            previousRoomRoot.SetActive(false);
        }

        if (RoomPrefabs.Length == 0)
        {
            Debug.LogError("Массив RoomPrefabs пуст!");
            return null;
        }

        _roomDepth++;
        int newRoomNumber = _roomDepth;

        GameObject selectedRoomPrefab = RoomPrefabs[Random.Range(0, RoomPrefabs.Length)];
        GameObject newRoomInstance = Instantiate(selectedRoomPrefab, spawnPosition, Quaternion.identity, RoomsParent);

        RoomData roomData = newRoomInstance.GetComponent<RoomData>();
        if (roomData == null)
        {
            Debug.LogError("В префабе комнаты отсутствует компонент RoomData!");
            Destroy(newRoomInstance);
            return null;
        }

        _currentRoomInstance = newRoomInstance;

        DoorSide oppositeSide = GetOppositeSide(previousDoorSide);

        Transform returnSpawnPoint = lastDoorTransform;

        List<RoomData.DoorSpawnPoint> entryCandidates = roomData.AvailableDoorSpawns
            .Where(d => d.Side == oppositeSide)
            .ToList();
        if (entryCandidates.Count == 0)
        {
            Destroy(newRoomInstance);
            return GenerateNextRoom(spawnPosition,previousDoorSide,previousRoomRoot, lastDoorTransform);
        }
        RoomData.DoorSpawnPoint finalEntryPoint = entryCandidates[Random.Range(0, entryCandidates.Count)];

        SpawnDoor(roomData, finalEntryPoint, true, previousRoomRoot, returnSpawnPoint, newRoomNumber - 1);

        RoomData.DoorSpawnPoint[] availableExits = roomData.AvailableDoorSpawns
            .Where(d => d.Side != oppositeSide)
            .ToArray();

        if (availableExits.Length > 0)
        {
            int randomIndex = Random.Range(0, availableExits.Length);
            DoorSide newExitSide = availableExits[randomIndex].Side;

            if (newExitSide != oppositeSide)
            {
                SpawnDoor(roomData, newExitSide, false, newRoomNumber);
            }
        }

        Transform playerSpawnPoint = finalEntryPoint.SpawnPoint;

        if (Player != null && playerSpawnPoint != null)
        {
            SetPlayerPositionWithOffset(playerSpawnPoint.position, oppositeSide);
        }

        SetHideSpots(roomData);
        if (Random.Range(1,21) > 17)
            _gm.SummonWraith(newRoomInstance);
        
        return newRoomInstance;
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
                doorPrefabToUse = EastWestDoorPrefab;
                break;
            case DoorSide.North:
                doorPrefabToUse = NorthDoorPrefab;
                break;
            case DoorSide.South:
                doorPrefabToUse = SouthDoorPrefab;
                break;
        }

        if (doorPrefabToUse == null)
        {
            Debug.LogError($"Не назначен префаб для стороны {targetSide}. Проверьте RoomGenerator!");
            return;
        }

        GameObject doorInstance = Instantiate(doorPrefabToUse, spawnPoint.position, Quaternion.identity, roomData.transform);

        DoorController doorController = doorInstance.GetComponent<DoorController>();
        
        if (doorController != null)
        {
            bool isDoorLocked = false;
            if (!isPreviousRoomDoor)
            {
                isDoorLocked = Random.Range(1, 11) > 8;
                if (isDoorLocked)
                {
                    SpawnSafe(roomData.AvailableSafeSpawns[Random.Range(0, roomData.AvailableSafeSpawns.Length)], doorController);
                }
            }
            doorController.Initialize(this, targetSide, isPreviousRoomDoor, isDoorLocked, previousRoomRoot);

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
        GameObject safeObject = Instantiate(SafePrefab, spawnPoint.position,Quaternion.identity, _currentRoomInstance.transform);
        
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
        if (Player == null) return;

        Vector3 finalPosition = doorPosition;

        switch (entrySide)
        {
            case DoorSide.North:
                finalPosition.y += NorthEntryOffset;
                break;

            case DoorSide.South:
                finalPosition.y += SouthEntryOffset;
                break;
        }

        Player.position = finalPosition;
        
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