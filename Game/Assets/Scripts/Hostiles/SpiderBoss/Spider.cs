using System.Collections;
using NavMeshPlus.Extensions;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Spider : MonoBehaviour
{
    private readonly float DistanceToKill = 2.2f;
    private readonly int NormalSpeed = 3;
    private readonly int TriggeredSpeed = 5;
    public NavMeshAgent Agent { get; private set; }
    private float _rotationSpeed = 5f;
    [SerializeField] private Transform[] _patrolPoints;
    [SerializeField] private GameObject _webPrefab;
    private bool _isOnPoint = true;
    private Vector3 _lastPos;
    public bool IsWalking { get; private set; }
    public bool IsRunning { get; private set; }
    public Vector3 DeltaMove { get; private set; } = Vector3.zero;
    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();

        Agent.updateRotation = false;
        Agent.updateUpAxis = false;

        _lastPos = transform.position;
    }
    void Update()
    {
        DeltaMove = transform.position - _lastPos;
        _lastPos = transform.position;
        if (Agent.speed == NormalSpeed)
        {
            IsWalking = true;
            IsRunning = false;
        }
        else if (Agent.speed == TriggeredSpeed)
        {
            IsWalking = false;
            IsRunning = true;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, PlayerController.Instance.transform.position);
        if (distanceToPlayer < DistanceToKill)
        {
            PlayerController.Instance.InflictDamage(100);
        }
        Vector3 direction = Agent.velocity;
        if (Random.Range(0, 10000) < 5)
        {
            SpawnWeb();
        }
        // if (direction.sqrMagnitude > 0.01f)
        // {
        //     RotateSpider(direction);
        // }
        if (_isOnPoint)
        {
            NextPoint();
        }
        if (Agent.remainingDistance <= 0.02f)
        {
            StartCoroutine(Rest());
        }
    }
    private IEnumerator Rest()
    {
        _isOnPoint = true;
        Agent.speed = NormalSpeed;
        Agent.isStopped = true;
        yield return new WaitForSeconds(2f);
        Agent.isStopped = false;
    }
    private void NextPoint()
    {
        Agent.SetDestination(_patrolPoints[Random.Range(0, _patrolPoints.Length)].position);
        _isOnPoint = false;

    }
    private void RotateSpider(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle - 90f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);


    }
    private void SpawnWeb()
    {
        Instantiate(_webPrefab, transform.position, Quaternion.identity);
    }
    public void Trigger(Vector3 target)
    {
        Agent.isStopped = false;
        Agent.SetDestination(target);
        Agent.speed = TriggeredSpeed;
    }
}
