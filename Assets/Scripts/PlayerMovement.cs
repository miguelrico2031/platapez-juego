using System;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _humanSpeed, _bugSpeed;
    [SerializeField] private Animator _bugAnimator, _humanAnimator = null;
    private NavMeshAgent _agent;

    private static readonly int _speedHash = Animator.StringToHash("Speed");
    private float _defaultSpeed;
    private Animator _activeAnimator;
    private Transform _activeSpriteTransform;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;

        _defaultSpeed = _agent.speed;
        SetHuman();
    }

    private void Update()
    {
        if (_activeAnimator is not null)
        {
            _activeAnimator.SetFloat(_speedHash, _agent.velocity.magnitude / _agent.speed);

            if(!_agent.isStopped)
            {
                var angle = Vector3.SignedAngle(_activeSpriteTransform.up, _agent.velocity, Vector3.forward);
                _activeSpriteTransform.Rotate(0f, 0f, angle);
            }
        }

        
    }


    public void SetTarget(Transform target)
    {
        _agent.isStopped = false;
        _agent.SetDestination(target.position);
    }

    public bool HasReachedTarget()
    {
        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
            {
                return true;
            }
        }

        return false;
    }

    public void CancelMovement()
    {
        _agent.ResetPath();
        _agent.isStopped = true;
    }

    public void SetHuman()
    {
        _agent.speed = _defaultSpeed * _humanSpeed;
        _activeAnimator = _humanAnimator;
        Handletransforms();
    }

    public void SetBug()
    {
        _agent.speed = _defaultSpeed * _bugSpeed;
        _activeAnimator = _bugAnimator;
        Handletransforms();
    }

    public void LookAt(Transform target)
    {
        if (!_agent.isStopped) return;
        var direction = target.position - _activeSpriteTransform.position;
        var angle = Vector3.SignedAngle(_activeSpriteTransform.up, direction.normalized, Vector3.forward);
        _activeSpriteTransform.Rotate(0f, 0f, angle);
    }       

    private void Handletransforms()
    {
        float angle = _activeSpriteTransform is null ? 0f : _activeSpriteTransform.eulerAngles.z;
        _activeSpriteTransform = _activeAnimator.transform;
        _activeSpriteTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    
}