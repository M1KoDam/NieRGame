using UnityEngine;

public class MovingBackground : MonoBehaviour
{
    [SerializeField, Range(0f, 0.05f)] private float speed;

    private Material _material;

    private void Start()
    {
        _material = GetComponent<Renderer>().material;
    }

    private void FixedUpdate()
    {
        var offset = _material.mainTextureOffset;
        offset.x += speed;
        _material.mainTextureOffset = offset;
    }
}
