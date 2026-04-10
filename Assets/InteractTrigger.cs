using UnityEngine;
using Yarn.Unity;

public class InteractTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject interactPrompt; 
    private bool playerInRange = false;

    public  DialogueManager dialogueManager;

    public string conversationStartNode;

    

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Player entered trigger");
        if (other.CompareTag("Player"))
        {
            interactPrompt.SetActive(true);
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Player exited trigger");
        if (other.CompareTag("Player"))
        {
            interactPrompt.SetActive(false);
            playerInRange = false;
        }
    }

    void Update()
    {
        
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E");
            Interact();
        }
    }

     void Interact()
    {
        

        dialogueManager = FindObjectOfType<DialogueManager>();

        Debug.Log("INTERACT CALLED");

        interactPrompt.SetActive(false);

       
        
        dialogueManager.StartDialogue();
        
    }
}
