using UnityEngine;

public class GoliathHead : MonoBehaviour
{
    [SerializeField] public float speed;
    [SerializeField] private float rotationSpeed;

    private Vector2 _currentRelativePosition = Vector2.zero;
    private float _currentRotation;

    public bool rotationOnTarget => Mathf.Abs(_currentRotation - targetRotation) < rotationSpeed;
    public bool positionOnTarget => (_currentRelativePosition - targetRelativePosition).magnitude < speed;
    public Vector2 targetRelativePosition { get; set; }
    public float targetRotation { get; set; }

    private void FixedUpdate()
    {
        HandleRotation();
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (!positionOnTarget)
        {
            var delta = targetRelativePosition - _currentRelativePosition;
            transform.position += (Vector3)delta.normalized * speed;
            _currentRelativePosition += delta * speed;
        }
    }

    private void HandleRotation()
    {
        if (!rotationOnTarget)
        {
            var currentSpeed = _currentRotation > targetRotation ? -rotationSpeed : rotationSpeed;
            transform.Rotate(Vector3.forward, currentSpeed);
            _currentRotation += currentSpeed;
        }
    }
}