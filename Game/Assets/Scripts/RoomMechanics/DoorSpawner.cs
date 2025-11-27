using UnityEngine;
using System.Collections.Generic;

public class DoorSpawner : MonoBehaviour
{
    public GameObject DoorPrefab;

    public CompositeCollider2D RoomCollider;

    public float MinSegmentLength = 1f;

    void Start()
    {
        if (DoorPrefab == null || RoomCollider == null)
        {
            Debug.LogError("DoorPrefab или RoomCollider не назначен в DoorSpawner!");
            return;
        }

        SpawnDoorOnBoundary();
    }

    private void SpawnDoorOnBoundary()
    {
        List<Vector2> boundaryPoints = new List<Vector2>();

        for (int i = 0; i < RoomCollider.pathCount; i++)
        {
            Vector2[] pathPoints = new Vector2[RoomCollider.GetPathPointCount(i)];
            RoomCollider.GetPath(i, pathPoints);

            boundaryPoints.AddRange(pathPoints);
        }

        if (boundaryPoints.Count < 2)
        {
            Debug.LogError("Недостаточно точек для определения границы комнаты.");
            return;
        }

        int startIndex = Random.Range(0, boundaryPoints.Count);
        int endIndex = (startIndex + 1) % boundaryPoints.Count;

        Vector2 startPoint = boundaryPoints[startIndex];
        Vector2 endPoint = boundaryPoints[endIndex];

        float segmentLength = Vector2.Distance(startPoint, endPoint);
        if (segmentLength < MinSegmentLength)
        {
            Debug.LogWarning("Случайный сегмент слишком короткий. Повтор выбора.");
            SpawnDoorOnBoundary();
            return;
        }

        float t = Random.Range(0.2f, 0.8f); 
        Vector2 spawnPositionLocal = Vector2.Lerp(startPoint, endPoint, t);

        Vector3 worldSpawnPosition = RoomCollider.transform.TransformPoint(spawnPositionLocal);

        GameObject newDoor = Instantiate(DoorPrefab, worldSpawnPosition, Quaternion.identity);

        newDoor.transform.SetParent(transform);

        Debug.Log($"Дверь создана в мировой позиции: {worldSpawnPosition}");
    }
}