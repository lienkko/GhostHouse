using UnityEngine;
using System.Collections;

public class BloodCleanerBossManager : MonoBehaviour
{
    [Header("Настройка врага")]
    [SerializeField] private GameObject _bossPrefab;

    [Header("Таймер спавна")]
    [SerializeField] private bool _spawnOnRoomEnter = true;
    [SerializeField] private float _spawnDelay = 5f;

    public static BloodCleanerBossManager Instance;
    private BloodCleaner _bloodCleanerBoss;
    private Coroutine _spawnCoroutine;
    private bool _isSpawning = false;
    private bool _isEnemyActive = false;
    private DoorController _enterDoor;
    private DoorController _exitDoor;

    void Awake()
    {
        Instance = this;
        if (_bossPrefab == null)
        {
            _bossPrefab = GetComponentInChildren<BloodCleaner>()?.gameObject;
        }

        if (_spawnOnRoomEnter)
        {
            // Босс активируется через _spawnDelay после входа в комнату
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
        _isSpawning = true;

        float timer = _spawnDelay;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        ActivateBoss();
        _isSpawning = false;
    }

    private void ActivateBoss()
    {
        if (_bossPrefab == null) return;

        _bossPrefab.SetActive(true);

        if (_bloodCleanerBoss != null)
        {
            _bloodCleanerBoss.Activate();
        }

        _isEnemyActive = true;
    }

    public void SetDoors(DoorController EnterDoor, DoorController ExitDoor)
    {
        _enterDoor = EnterDoor;
        _exitDoor = ExitDoor;
        _enterDoor.isDoorLocked = true;
        _enterDoor.GetComponent<Interactive>().isInteractive = false;
    }

    public void DespawnEnemy()
    {
        if (_bloodCleanerBoss != null)
        {
            Destroy(_bloodCleanerBoss);
            _bloodCleanerBoss = null;
        }
        _isEnemyActive = false;
    }

    private void OnDisable()
    {
        DespawnEnemy();
    }
}