using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MoveBackground : MonoBehaviour
{
	public float speed;
	private float _x;
	[SerializeField] private float endPoint;
	[SerializeField] private float startPoint;
	
	// Use this for initialization
	void Start () {
		//PontoOriginal = transform.position.x;
	}
	
	// Update is called once per frame
	void Update () {
		_x = transform.position.x;
		_x += speed * Time.deltaTime;
		transform.position = new Vector3 (_x, transform.position.y, transform.position.z);
		
		if (_x <= endPoint)
		{
			_x = startPoint;
			transform.position = new Vector3 (_x, transform.position.y, transform.position.z);
		}
	}
}
