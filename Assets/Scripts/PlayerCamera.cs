using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField, Range(0f, 1f)] private float alpha;
    private Vector3 _prevPos;

    private void Start()
    {
        _prevPos = transform.position;
    }

    private void FixedUpdate()
    {
        var newPosition = _prevPos + alpha * (player.transform.position - _prevPos);
        // var newPosition = (_prevPos + player.transform.position) / 2f;
        newPosition.z = _prevPos.z;
        
        transform.position = newPosition;
        
        _prevPos = transform.position;
    }
}
