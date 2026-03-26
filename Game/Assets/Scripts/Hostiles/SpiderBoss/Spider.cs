using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Spider : MonoBehaviour
{
    private readonly float DistanceToKill = 2.5f;
    private readonly int NormalSpeed = 3;
    private readonly int TriggeredSpeed = 5;
    private NavMeshAgent _agent;
    private float _rotationSpeed = 5f;
    [SerializeField] private Transform[] _patrolPoints;
    private bool _isOnPoint = true;
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();

        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, PlayerController.Instance.transform.position);
        if (distanceToPlayer < DistanceToKill)
        {
            PlayerController.Instance.InflictDamage(100);
        }
        Vector3 direction = _agent.velocity;
        if (direction.sqrMagnitude > 0.01f)
        {
            RotateSpider(direction);
        }
        if (_isOnPoint)
        {
            NextPoint();
        }
        if (_agent.remainingDistance <= 0.02f)
        {
            StartCoroutine(Rest());
        }
    }
    private IEnumerator Rest()
    {
        _isOnPoint = true;
        _agent.speed = NormalSpeed;
        _agent.isStopped = true;
        yield return new WaitForSeconds(2f);
        _agent.isStopped = false;
    }
    private void NextPoint()
    {
        _agent.SetDestination(_patrolPoints[Random.Range(0, _patrolPoints.Length)].position);
        _isOnPoint = false;

    }
    private void RotateSpider(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle - 90f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);


    }
    public void Trigger(Vector3 target)
    {
        _agent.isStopped = false;
        _agent.SetDestination(target);
        _agent.speed = TriggeredSpeed;
    }
}
