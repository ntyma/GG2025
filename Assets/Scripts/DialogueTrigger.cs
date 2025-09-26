using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueCharacter
{
    //public string name;
    public Sprite icon;
}

[System.Serializable]
public class DialogueLine
{
    public DialogueCharacter character;
    [TextArea(3, 10)]
    public string line;
}

[System.Serializable]
public class Dialogue
{
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public GameObject dialogueBoxUI;

    private bool trigger = false;

    public void TriggerDialogue()
    {
        dialogueBoxUI.SetActive(true);
        DialogueManager.Instance.StartDialogue(dialogue);
    }

    public void TriggerDialogueNoAudio()
    {
        dialogueBoxUI.SetActive(true);
        DialogueManager.Instance.StartDialogueNoAudio(dialogue);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player") // if the player enters the trigger area
        {
            print("Dialogue Triggered");
            trigger = true;
        }
    }

    private void Update()
    {
        if (trigger)
        {
            TriggerDialogue();
            gameObject.SetActive(false);
            trigger = false;
        }
    }
}
