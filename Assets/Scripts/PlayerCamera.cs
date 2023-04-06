using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCamera : MonoBehaviour
{
    public Player player;
    private Vector3 _deltaPos;
    
    // Start is called before the first frame update
    void Start()
    {
        _deltaPos = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position + _deltaPos;
    }
}
