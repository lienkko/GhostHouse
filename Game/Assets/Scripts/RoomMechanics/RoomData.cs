using System.Collections.Generic;
using UnityEngine;

public class RoomData : MonoBehaviour
{
    [HideInInspector] public DoorController PreviousRoomDoor;
    [HideInInspector] public DoorController NextRoomDoor;
    [System.Serializable]
    public struct DoorSpawnPoint
    {
        public DoorSide Side;

        public Transform SpawnPoint;
    }

    [Tooltip("������ ���� ��������� ���� ��� ���������� ������ � ���� �������.")]
    public DoorSpawnPoint[] AvailableDoorSpawns;

    [Tooltip("������ ���� ��������� ���� ��� ���������� ����� � ���� �������.")]
<<<<<<< HEAD
    public List<Transform> AvailableSafeSpawns;
=======
    public Transform[] AvailableSafeSpawns;
>>>>>>> SpiderKeys

    [Tooltip("������ ���� ������ � ���� �������.")]
    public GameObject[] Closets;

    public DoorSpawnPoint EnterBossDoor;
    public DoorSpawnPoint ExitBossDoor;
}
