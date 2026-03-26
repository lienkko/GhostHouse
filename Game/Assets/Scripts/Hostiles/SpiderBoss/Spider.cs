using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Spider : MonoBehaviour
{
    NavMeshAgent agent;
    private float _rotationSpeed = 5f;
    [SerializeField] private Transform[] _patrolPoints;
    private bool _isOnPoint = true;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        Vector3 direction = agent.velocity;

        if (direction.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle - 90f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
        if (_isOnPoint)
        {
            agent.SetDestination(_patrolPoints[Random.Range(0, _patrolPoints.Length)].position);
            _isOnPoint = false;
        }
        if (agent.remainingDistance <= 0.02f)
            _isOnPoint = true;

    }
}
