using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    public enum AIState { Patrol, Search, Chase, Attack, Dead }

    [Header("Detection")]
    public float detectionRange = 20f;
    public float attackRange = 12f;
    public float fieldOfView = 110f;
    public float loseTargetTime = 5f;

    [Header("Attack")]
    public float fireRate = 1.2f;
    public int damagePerShot = 10;
    public float accuracy = 0.7f;

    [Header("Patrol")]
    public Transform[] patrolPoints;

    [Header("Cover")]
    public float coverSearchRadius = 8f;

    [Header("Audio")]
    public AudioClip fireClip;
    public AudioClip deathClip;
    public AudioClip alertClip;

    private NavMeshAgent _agent;
    private Transform _player;
    private AIState _state = AIState.Patrol;
    private float _nextFireTime;
    private float _loseTargetTimer;
    private int _patrolIndex;
    private AudioSource _audio;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _audio = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (_state == AIState.Dead || _player == null) return;
        switch (_state)
        {
            case AIState.Patrol: DoPatrol(); break;
            case AIState.Search:  DoSearch();  break;
            case AIState.Chase:   DoChase();   break;
            case AIState.Attack:  DoAttack();  break;
        }
    }

    void DoPatrol()
    {
        if (CanSeePlayer()) { Alert(); return; }
        if (patrolPoints.Length == 0) return;
        if (!_agent.hasPath || _agent.remainingDistance < 0.5f)
        {
            _patrolIndex = (_patrolIndex + 1) % patrolPoints.Length;
            _agent.SetDestination(patrolPoints[_patrolIndex].position);
        }
    }

    void DoSearch()
    {
        if (CanSeePlayer()) { SetState(AIState.Chase); return; }
        _loseTargetTimer -= Time.deltaTime;
        if (_loseTargetTimer <= 0f) SetState(AIState.Patrol);
    }

    void DoChase()
    {
        if (!CanSeePlayer()) { SetState(AIState.Search); return; }
        float dist = Vector3.Distance(transform.position, _player.position);
        if (dist <= attackRange) { SetState(AIState.Attack); return; }
        _agent.SetDestination(_player.position);
    }

    void DoAttack()
    {
        float dist = Vector3.Distance(transform.position, _player.position);
        if (!CanSeePlayer() || dist > attackRange * 1.3f) { SetState(AIState.Chase); return; }

        _agent.SetDestination(transform.position);
        transform.LookAt(_player.position);

        if (Time.time >= _nextFireTime)
        {
            _nextFireTime = Time.time + (1f / fireRate);
            ShootAtPlayer();
        }
    }

    void ShootAtPlayer()
    {
        if (_audio && fireClip) _audio.PlayOneShot(fireClip, 0.6f);
        if (Random.value <= accuracy)
        {
            string[] parts = { "body", "body", "leg", "head" };
            string part = parts[Random.Range(0, parts.Length)];
            int dmg = part == "head" ? 25 : part == "leg" ? 5 : 10;
            FindObjectOfType<HealthSystem>()?.TakeDamage(dmg, part);
        }
    }

    bool CanSeePlayer()
    {
        if (_player == null) return false;
        Vector3 dir = (_player.position - transform.position).normalized;
        float dist = Vector3.Distance(transform.position, _player.position);
        if (dist > detectionRange) return false;
        float angle = Vector3.Angle(transform.forward, dir);
        if (angle > fieldOfView * 0.5f) return false;
        return !Physics.Raycast(transform.position + Vector3.up, dir, dist, LayerMask.GetMask("Default"));
    }

    void Alert()
    {
        if (_audio && alertClip) _audio.PlayOneShot(alertClip, 0.8f);
        SetState(AIState.Chase);
    }

    void SetState(AIState newState)
    {
        _state = newState;
        _loseTargetTimer = loseTargetTime;
    }

    public void OnDeath()
    {
        _state = AIState.Dead;
        _agent.isStopped = true;
        if (_audio && deathClip) _audio.PlayOneShot(deathClip, 0.8f);
    }
}
