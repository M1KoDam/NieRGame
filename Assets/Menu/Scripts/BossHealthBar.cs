using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] private Goliath goliath;
    private Slider _slider;
    
    // Start is called before the first frame update
    void Start()
    {
        _slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _slider.value = goliath.GetHealth();
    }
}
