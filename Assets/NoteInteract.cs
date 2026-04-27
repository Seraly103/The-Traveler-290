using UnityEngine;

public class NoteInteract : MonoBehaviour
{
    public string itemType = "Note";

    [Header("UI")]
    public GameObject interactText; 
    public GameObject notePanel;

    private bool playerNear = false;
    private bool alreadyCollected = false;

    void Start()
    {
        interactText.SetActive(false);
        notePanel.SetActive(false);
    }

    void Update()
    {
        if (playerNear && Input.GetKeyDown(KeyCode.E) && !alreadyCollected)
        {
            OpenNote();
        }
    }

    void OpenNote()
    {
        notePanel.SetActive(true);
        interactText.SetActive(false);
    }

    public void CloseNote()
    {
        notePanel.SetActive(false);

        if (!InventoryManager.TryAddItem(itemType))
        {
            return;
        }

        alreadyCollected = true;
        
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !alreadyCollected)
        {
            playerNear = true;
            interactText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;
            interactText.SetActive(false);
        }
    }
}
