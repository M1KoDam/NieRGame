using System;
using UnityEngine;

public class Lift : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private GameObject buttons;
    [SerializeField, Range(0, 1)] private float speed;
    [SerializeField] private Transform[] movePoints;

    private LiftState _state = LiftState.Idle;
    private int _currentMovePointIndex;
    private SpriteRenderer _buttonsRenderer;

    private Transform TargetMovePoint => movePoints[_currentMovePointIndex + (int)_state];
    private float DistanceToTarget => Mathf.Abs(transform.position.y - TargetMovePoint.position.y);

    private void Start()
    {
        _currentMovePointIndex = FindInitialMovePointIndex();
        _buttonsRenderer = buttons.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        HandleControls();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleControls()
    {
        if (_state == LiftState.Idle && Input.GetAxis("Vertical") > 0 && _currentMovePointIndex + 1 < movePoints.Length)
        {
            _state = LiftState.MovingUp;
        }
        else if (_state == LiftState.Idle && Input.GetAxis("Vertical") < 0 && _currentMovePointIndex - 1 >= 0)
        {
            _state = LiftState.MovingDown;
        }
        else if (_state != LiftState.Idle && DistanceToTarget < 0.1)
        {
            _currentMovePointIndex += (int)_state;
            _state = LiftState.Idle;
        }
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
}