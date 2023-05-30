using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Player player;
    private Slider _slider;
    
    // Start is called before the first frame update
    void Start()
    {
        _slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _slider.value = player.GetHealth();
    }
}
