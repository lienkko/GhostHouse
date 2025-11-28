using UnityEngine;

public class RoomData : MonoBehaviour
{
    [System.Serializable]
    public struct DoorSpawnPoint
    {
        public DoorSide Side;

        public Transform SpawnPoint;
    }

    [Tooltip("Список всех доступных мест для размещения дверей в этой комнате.")]
    public DoorSpawnPoint[] AvailableDoorSpawns;

    [Tooltip("Список всех доступных мест для размещения сейфа в этой комнате.")]
    public Transform[] AvailableSafeSpawns;

    [Tooltip("Список всех шкафов в этой комнате.")]
    public GameObject[] Closets;
}