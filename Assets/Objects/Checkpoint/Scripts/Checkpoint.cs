using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private GameLES gameLes;
    [SerializeField] private Sounds sounds;
    private float _saveDelay = 10;

    private void FixedUpdate()
    {
        _saveDelay += Time.deltaTime;
    }

    public void SetCheckpoint()
    {
        if (_saveDelay >= 10)
        {
            sounds.AllSounds["CheckPoints"].PlaySound();
            gameLes.SaveCheckpoint(this);
            _saveDelay = 0;
        }
    }
}
