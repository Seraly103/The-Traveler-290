using UnityEngine;
using Yarn.Unity;

public class DialogueManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private DialogueRunner dialogueManager;
    
    private bool interactable = true;

    private bool isCurrentConversation;

    public void Start() {
        //dialogueManager = FindAnyObjectByType<DialogueRunner>();
        // this would go in the Start() function, right after finding the DialogueManager
        // object, so that the listener begins before dialogue does the first time
        dialogueManager.onDialogueComplete.AddListener(EndConversation);
    }

    // then we need a function to tell Yarn Spinner to start from {specifiedNodeName}
    public string conversationStartNode;

    private void StartConversation() {
        Debug.Log("Starting node: " + conversationStartNode);
        dialogueManager.StartDialogue(conversationStartNode);
        isCurrentConversation = true;
    }

    public void StartDialogue()
    {
        if (!dialogueManager.IsDialogueRunning)
        {
            StartConversation();
        }
    }


    private void EndConversation() {
        if (isCurrentConversation) { 
            // TODO *stop animation or turn off indicator or whatever* HERE
            isCurrentConversation = false;
        }
    }
    
}
