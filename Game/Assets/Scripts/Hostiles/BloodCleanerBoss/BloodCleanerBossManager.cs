using UnityEngine;
using System.Collections;

public class BloodCleanerBossManager : MonoBehaviour
{
    [Header("��������� �����")]
    [SerializeField] private GameObject _bossPrefab;

    [Header("������ ������")]
    [SerializeField] private bool _spawnOnRoomEnter = true;
    [SerializeField] private float _spawnDelay = 5f;

    public static BloodCleanerBossManager Instance;
    private BloodCleaner _bloodCleanerBoss;
    private Coroutine _spawnCoroutine;
    private DoorController _enterDoor;

    void Awake()
    {
        Instance = this;
        if (_bossPrefab == null)
        {
            _bossPrefab = GetComponentInChildren<BloodCleaner>()?.gameObject;
        }

        if (_spawnOnRoomEnter)
        {
            // ���� ������������ ����� _spawnDelay ����� ����� � �������
            StartSpawnTimer();
        }
    }

    public void StartSpawnTimer()
    {
        if (_spawnCoroutine != null)
            StopCoroutine(_spawnCoroutine);

        _spawnCoroutine = StartCoroutine(SpawnWithDelay());
    }

    private IEnumerator SpawnWithDelay()
    {

        float timer = _spawnDelay;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        ActivateBoss();
    }

    private void ActivateBoss()
    {
        if (_bossPrefab == null) return;

        _bossPrefab.SetActive(true);

        if (_bloodCleanerBoss != null)
        {
            _bloodCleanerBoss.Activate();
        }

    }

    public void SetDoors(DoorController EnterDoor, DoorController ExitDoor)
    {
        _enterDoor = EnterDoor;
        _enterDoor.LockDoor(true);
    }

    public void DespawnEnemy()
    {
        if (_bloodCleanerBoss != null)
        {
            Destroy(_bloodCleanerBoss);
            _bloodCleanerBoss = null;
        }
    }

    private void OnDisable()
    {
        DespawnEnemy();
    }
}