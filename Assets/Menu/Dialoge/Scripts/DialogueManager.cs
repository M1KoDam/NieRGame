using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public TMP_Text dialogueText;
    public TMP_Text speakerText;

    public Animator windowAnimator;
    public Animator speakerAnimator;

    public LevelEventSystem levelEventSystem;

    private Queue<string> _sentences;
    private Queue<string> _speakers;

    private void Awake()
    {
        _sentences = new Queue<string>();
        _speakers = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        windowAnimator.SetBool("StartOpen", true);
        _speakers.Clear();
        _sentences.Clear();

        foreach (var sentence in dialogue.sentences)
        {
            _sentences.Enqueue(sentence);
        }
        
        foreach (var speaker in dialogue.speakers)
        {
            _speakers.Enqueue(speaker);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        CancelInvoke(nameof(DisplayNextSentence));
        if (_sentences.Count == 0 || _speakers.Count == 0)
        {
            EndDialogue();
            return;
        }

        var sentence = _sentences.Dequeue();
        var speaker = _speakers.Dequeue();
        ChangeIcons(speaker);
        speakerText.text = speaker;
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    private void ChangeIcons(string speaker)
    {
        switch (speaker)
        {
            case "2B":
                speakerAnimator.Play("2BSpeak");
                break;
            case "6O":
                speakerAnimator.Play("6OSpeak");
                break;
            case "Pod":
                speakerAnimator.Play("PodSpeak");
                break;
            default:
                speakerAnimator.Play("PodSpeak");
                break;
        }
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (var letter in sentence)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds (0.05f);
        }
        
        if (dialogueText.text.Length == sentence.Length)
            Invoke(nameof(DisplayNextSentence), 3);
    }

    private void EndDialogue()
    {
        levelEventSystem.NextEvent();
        windowAnimator.SetBool("StartOpen", false);
    }
}
