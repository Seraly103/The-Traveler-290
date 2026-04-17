using UnityEngine;

public enum InteractableType { SmallDoor, MovableTree }

/// <summary>
/// One script for both the small door and the movable tree.
/// Set Type in the Inspector to choose behaviour.
/// Add a Collider2D (Is Trigger = true) to the same GameObject.
/// </summary>
public class SizeInteractable : MonoBehaviour
{
    [Header("Type")]
    public InteractableType type = InteractableType.SmallDoor;

    [Header("Interact Prompt")]
    public GameObject interactPrompt;

    [Header("Objects to reveal when interaction succeeds")]
    public GameObject[] objectsToReveal;

    [Header("Tree Only — how far it slides when pushed")]
    public Vector2 treeMoveOffset = new Vector2(3f, 0f);
    public float treeMoveSpeed = 2f;

    private bool playerInRange = false;
    private bool used = false;
    private bool isMoving = false;
    private Vector3 treeTargetPos;

    void Start()
    {
        treeTargetPos = transform.position;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!used && other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactPrompt != null) interactPrompt.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactPrompt != null) interactPrompt.SetActive(false);
        }
    }

    void Update()
    {
        if (playerInRange && !used && Input.GetKeyDown(KeyCode.E))
            TryInteract();

        // Slide tree toward target
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, treeTargetPos, treeMoveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, treeTargetPos) < 0.01f)
            {
                transform.position = treeTargetPos;
                isMoving = false;
                RevealObjects();
            }
        }
    }

    void TryInteract()
    {
        switch (type)
        {
            case InteractableType.SmallDoor:
                if (ShrinkingAndEnlarging.CurrentSize == SizeState.Small)
                    OpenDoor();
                else
                    Debug.Log("This door is too small. Maybe if I were smaller...");
                break;

            case InteractableType.MovableTree:
                if (ShrinkingAndEnlarging.CurrentSize == SizeState.Large)
                    PushTree();
                else
                    Debug.Log("This tree is huge. I'd need to be much bigger to move it...");
                break;
        }
    }

    void OpenDoor()
    {
        used = true;
        if (interactPrompt != null) interactPrompt.SetActive(false);
        RevealObjects();
        gameObject.SetActive(false);
    }

    void PushTree()
    {
        used = true;
        if (interactPrompt != null) interactPrompt.SetActive(false);
        treeTargetPos = transform.position + new Vector3(treeMoveOffset.x, treeMoveOffset.y, 0f);
        isMoving = true;
    }

    void RevealObjects()
    {
        foreach (var obj in objectsToReveal)
            if (obj != null) obj.SetActive(true);
    }
}
