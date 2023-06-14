using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLES : LevelEventSystem
{
    [SerializeField] private Animator _animatorLR;
    [SerializeField] private Animator _animatorRL;

    void Start()
    {
        _animatorLR.SetBool("Open", true);
        _animatorRL.SetBool("Open", true);
        Invoke(nameof(OpenMenu), 0.5f);
    }

    private void OpenMenu()
    {
        _animatorLR.SetBool("Open", false);
        _animatorRL.SetBool("Open", false);
    }

    public void CloseMenu()
    {
        _animatorLR.SetBool("Open", true);
        _animatorRL.SetBool("Open", true);
        Invoke(nameof(NextLevel), 0.5f);
    }
}
