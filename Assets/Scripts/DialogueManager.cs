using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

	public Image characterIcon;
	public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueArea;

    private Queue<DialogueLine> lines;
    
	public bool isDialogueActive = false;

	public float typingSpeed = 0.03f;

	public Animator animator;
    //public GameObject dialogueBox; // Dialogue box GameObject

	private Coroutine typingCoroutine; // keep track of current typing
	private DialogueLine currentLine; // finish current line
	private bool isTyping = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

		lines = new Queue<DialogueLine>();
    }

	public void StartDialogue(Dialogue dialogue)
	{
        isDialogueActive = true;

        //animator.Play("show");
        animator.SetTrigger("showTrigger");
		print("Dialogue started");

        lines.Clear();

		foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
		{
			lines.Enqueue(dialogueLine);
		}

		DisplayNextDialogueLine();
	}

	public void DisplayNextDialogueLine()
	{
		if (isTyping)
		{
			// if currently typing, skip to the end of the current line
			StopCoroutine(typingCoroutine);
			dialogueArea.text = currentLine.line; // show full line immediately
            isTyping = false;
            return;
        }
		else if (lines.Count == 0)
		{
			EndDialogue();
			return;
		}

		currentLine = lines.Dequeue();

		// update headshot and name
		characterIcon.sprite = currentLine.character.icon;
		characterName.text = currentLine.character.name;

		//StopAllCoroutines(); // stop any ongoing typing effect
		typingCoroutine = StartCoroutine(TypeSentence(currentLine));

        //StartCoroutine(TypeSentence(currentLine)); // start typing effect
    }

	private void Update()
	{
		if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
		{
			DisplayNextDialogueLine();
		}
		else if (isDialogueActive && Input.GetKeyDown(KeyCode.Z))
		{
			DisplayNextDialogueLine();
        }


    }

    IEnumerator TypeSentence(DialogueLine dialogueLine)
	{
		isTyping = true;
        dialogueArea.text = "";

		foreach (char letter in dialogueLine.line.ToCharArray())
		{
			dialogueArea.text += letter;
			yield return new WaitForSeconds(typingSpeed);
		}

		isTyping = false;
    }

	void EndDialogue()
	{
		isDialogueActive = false;
        animator.SetTrigger("hideTrigger");
		print("Dialogue ended");
    }
}
