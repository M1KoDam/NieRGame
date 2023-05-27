using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEventSystem : MonoBehaviour
{
    public DialogueTrigger[] _DialogueTriggers;
    private Queue<DialogueTrigger> _dialogues;

    // Start is called before the first frame update
    void Start()
    {
        _dialogues = new Queue<DialogueTrigger>();
        foreach (var dialogueTrigger in _DialogueTriggers)
        {
            _dialogues.Enqueue(dialogueTrigger);
        }

        var dialogue = _dialogues.Dequeue();
        dialogue.TriggerDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(_dialogues.Count);
    }
}
