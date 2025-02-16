using UnityEngine;
using UnityEngine.AI;

public class AdvancedPathfinding : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float stoppingDistance = 1f;

    private NavMeshAgent _agent;
    private Transform _target;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        if (_agent == null)
        {
            _agent = gameObject.AddComponent<NavMeshAgent>();
        }

        _agent.speed = moveSpeed;
        _agent.stoppingDistance = stoppingDistance;
    }

    private void Update()
    {
        if (_target != null)
        {
            _agent.SetDestination(_target.position);

            // Check if the agent has reached the destination
            if (!_agent.pathPending && _agent.remainingDistance <= stoppingDistance)
            {
                Debug.Log("Reached destination!");
                _target = null; // Stop moving
            }
        }
    }

    // Set a new target to move towards
    public void SetTarget(Transform target)
    {
        _target = target;
    }

    // Stop the agent
    public void Stop()
    {
        _target = null;
        _agent.ResetPath();
    }

    // Check if a position is reachable
    public bool IsPositionReachable(Vector3 position)
    {
        NavMeshPath path = new NavMeshPath();
        _agent.CalculatePath(position, path);
        return path.status == NavMeshPathStatus.PathComplete;
    }

    // Draw the path in the editor for debugging
    private void OnDrawGizmos()
    {
        if (_agent != null && _agent.hasPath)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < _agent.path.corners.Length - 1; i++)
            {
                Gizmos.DrawLine(_agent.path.corners[i], _agent.path.corners[i + 1]);
            }
        }
    }
}
