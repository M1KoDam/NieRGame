using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundFlightSound : MonoBehaviour
{
    public Sounds sounds;

    private bool _isSoundPlaying;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (sounds.AllSounds is not null && !_isSoundPlaying)
        {
            _isSoundPlaying = true;   
            sounds.AllSounds["BackGroundFlightSound"].PlaySoundLoop();
        }
    }
}
