using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Spider : MonoBehaviour
{
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
        Vector3 direction = _agent.velocity;

        if (direction.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle - 90f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
        if (_isOnPoint)
        {
            _agent.SetDestination(_patrolPoints[Random.Range(0, _patrolPoints.Length)].position);
            _isOnPoint = false;
        }
        if (_agent.remainingDistance <= 0.02f)
        {
            _agent.speed = 3;
            StartCoroutine(Rest());
            _isOnPoint = true;
        }
    }
    private IEnumerator Rest()
    {
        _agent.isStopped = true;
        yield return new WaitForSeconds(2f);
        _agent.isStopped = false;
    }
    public void Trigger(Vector3 target)
    {
        _agent.SetDestination(target);
        _agent.speed = 5;
    }
}
