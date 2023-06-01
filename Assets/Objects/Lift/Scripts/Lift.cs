using System;
using UnityEngine;

public class Lift : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private GameObject freezer;
    [SerializeField] private GameObject buttons;
    [SerializeField, Range(0, 1)] private float speed;
    [SerializeField] private Transform[] movePoints;

    private LiftState _state = LiftState.Idle;
    private int _currentMovePointIndex;
    // private SpriteRenderer _buttonsRenderer;
    private Collider2D _collider;

    public Sounds sounds;

    private Transform TargetMovePoint => movePoints[_currentMovePointIndex + (int)_state];
    private float DistanceToTarget => Mathf.Abs(transform.position.y - TargetMovePoint.position.y);

    private void Start()
    {
        _currentMovePointIndex = FindInitialMovePointIndex();
        // _buttonsRenderer = buttons.GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        HandleStopping();
        HandlePlayerFreezing();
    }

    private void HandlePlayerFreezing()
    {
        if (_state == LiftState.Idle)
            freezer.SetActive(false);
        else
            freezer.SetActive(true);
    }

    private void HandleStopping()
    {
        if (_state != LiftState.Idle && DistanceToTarget < speed)
        {
            _currentMovePointIndex += (int)_state;
            _state = LiftState.Idle;
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleControls()
    {
        if (_state == LiftState.Idle && Input.GetAxis("Vertical") > 0 && _currentMovePointIndex + 1 < movePoints.Length)
            _state = LiftState.MovingUp;
        else if (_state == LiftState.Idle && Input.GetAxis("Vertical") < 0 && _currentMovePointIndex - 1 >= 0)
            _state = LiftState.MovingDown;
    }

    private void HandleMovement()
    {
        if (_state == LiftState.MovingDown)
            player.transform.position += Vector3.up * (speed * (int)_state);
        transform.position += Vector3.up * (speed * (int)_state);
    }

    private int FindInitialMovePointIndex()
    {
        var lowestDistance = float.PositiveInfinity;
        var closestMovePointIndex = -1;

        for (var i = 0; i < movePoints.Length; i++)
        {
            var distance = Math.Abs((transform.position - movePoints[i].position).magnitude);
            if (distance < lowestDistance)
            {
                lowestDistance = distance;
                closestMovePointIndex = i;
            }
        }

        return closestMovePointIndex;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if ((collision.collider == _collider || collision.otherCollider == _collider) && collision.gameObject.CompareTag("Player"))
            HandleControls();
    }
}