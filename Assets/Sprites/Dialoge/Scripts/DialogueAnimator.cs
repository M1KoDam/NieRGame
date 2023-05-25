using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueAnimator : MonoBehaviour
{
    public Animator startAnimator;
    public DialogueManager dm;

    public void OnTriggerEnter2D(Collider2D other)
    {
        startAnimator.SetBool("StartOpen", true);
    }
    
    public void OnTriggerExit2D(Collider2D other)
    {
        startAnimator.SetBool("StartOpen", false);
        dm.EndDialogue();
    }
}
