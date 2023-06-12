using System;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField, Range(0f, 1f)] private float alpha;
    private Vector3 _prevPos;
    private Camera _camera;
    private float _defaultCameraSize;
    private float _cameraSize;

    private void Start()
    {
        _prevPos = transform.position;
        _camera = GetComponent<Camera>();
        _defaultCameraSize = _camera.orthographicSize;
        _cameraSize = _defaultCameraSize;
    }

    private void FixedUpdate()
    {
        Debug.Log(_cameraSize);
        var newPosition = _prevPos + alpha * (player.transform.position - _prevPos);
        // var newPosition = (_prevPos + player.transform.position) / 2f;
        newPosition.z = _prevPos.z;
        
        transform.position = newPosition;
        
        _prevPos = transform.position;

        var deltaSize = _cameraSize - _camera.orthographicSize;
        if (Math.Abs(deltaSize) > 0.01f)
        {
            _camera.orthographicSize += (deltaSize >= 0 ? 1 : -1)*Time.deltaTime;
        }
    }

    public void SetSize(float size)
    {
        _cameraSize = size;
    }

    public void RollBack()
    {
        _camera.orthographicSize = _defaultCameraSize;
    }
}
