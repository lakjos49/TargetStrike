using UnityEngine;

public class MovingTarget : Target
{
    [Header("Movement")]
    public float moveRange = 5f;
    public float moveSpeed = 2f;
    public bool randomizePhase = true;

    private Vector3 _startPos;
    private float _phase;

    void Start()
    {
        _startPos = transform.position;
        scoreValue = 20;
        targetType = TargetType.Moving;
        if (randomizePhase) _phase = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        if (!GameManager.Instance.IsGameRunning) return;
        _phase += moveSpeed * Time.deltaTime;
        float offset = Mathf.Sin(_phase) * (moveRange / 2f);
        transform.position = _startPos + transform.right * offset;
    }

    protected override void Respawn()
    {
        base.Respawn();
        transform.position = _startPos;
    }
}
