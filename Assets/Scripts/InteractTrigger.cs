using UnityEngine;
using Yarn.Unity;

public class InteractTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject interactPrompt; 
    private bool playerInRange = false;

    public  DialogueManager dialogueManager;
    
    public DialogueRunner dialogueRunner;

    public string conversationStartNode;

    

    void Start()
    {
        interactPrompt.SetActive(false);
    }

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
        Debug.Log("INTERACT CALLED");
        
        if (dialogueRunner == null)
        {
            dialogueRunner = FindObjectOfType<DialogueRunner>();
        }

        if (dialogueRunner == null)
        {
            Debug.LogError("No DialogueRunner found!");
            return;
        }

        string nodeToStart = conversationStartNode;

        if (InventoryManager.note > 0)
        {
            nodeToStart = "Note";
        }

        Debug.Log("Starting node: " + nodeToStart);

        dialogueRunner.onDialogueComplete.AddListener(() =>
        {
            MushroomLock mushroomLock = FindObjectOfType<MushroomLock>();
            if (mushroomLock != null)
            {
                mushroomLock.CheckUnlock();
            }
        });
        if (!dialogueRunner.IsDialogueRunning)
        {
            InventoryManager.talkedToCat = true;

            if (InventoryManager.instance != null)
            {
                InventoryManager.instance.UpdateYarnVariables();
            }
            interactPrompt.SetActive(false);
            dialogueRunner.StartDialogue(nodeToStart);
        }
    }
}
