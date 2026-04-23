using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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

    [Header("Small Door — scene to load (must be in Build Settings)")]
    public string doorSceneName = "";

    [Header("Objects to reveal when interaction succeeds (tree only)")]
    public GameObject[] objectsToReveal;

    [Header("Tree Only — how far it slides when pushed")]
    public Vector2 treeMoveOffset = new Vector2(3f, 0f);
    public float treeMoveSpeed = 2f;

    private bool playerInRange = false;
    private bool used = false;
    private bool isMoving = false;
    private Vector3 treeTargetPos;
    private GameObject sizeHint;
    private TextMeshPro sizeHintText;
    private float hintTimer = 0f;

    void Start()
    {
        treeTargetPos = transform.position;

        // Auto-create a world-space "Press E" prompt if none assigned
        if (interactPrompt == null)
        {
            interactPrompt = new GameObject("InteractPrompt");
            interactPrompt.transform.SetParent(transform);
            interactPrompt.transform.localPosition = new Vector3(0f, 1.5f, -1f); // negative Z = in front of sprites

            var tmp = interactPrompt.AddComponent<TextMeshPro>();
            tmp.text = "Press E";
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontSize = 4f;
            tmp.color = Color.yellow;
            tmp.sortingOrder = 100; // render on top of everything

            interactPrompt.SetActive(false);
        }

        // Create a separate hint label for "you need to be smaller/bigger"
        var hintObj = new GameObject("SizeHint");
        hintObj.transform.SetParent(transform);
        hintObj.transform.localPosition = new Vector3(0f, 2.5f, -1f);
        sizeHintText = hintObj.AddComponent<TextMeshPro>();
        sizeHintText.alignment = TextAlignmentOptions.Center;
        sizeHintText.fontSize = 3f;
        sizeHintText.color = Color.red;
        sizeHintText.sortingOrder = 100;
        hintObj.SetActive(false);
        sizeHint = hintObj;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"SizeInteractable trigger entered by: {other.name} tag: {other.tag}");
        if (!used && other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactPrompt != null)
                interactPrompt.SetActive(true);
            else
                Debug.LogWarning($"SizeInteractable on {gameObject.name}: Interact Prompt is not assigned in the Inspector!");
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
        {
            Debug.Log($"E pressed, CurrentSize={ShrinkingAndEnlarging.CurrentSize}");
            TryInteract();
        }

        // Hide size hint after a short delay
        if (hintTimer > 0f)
        {
            hintTimer -= Time.deltaTime;
            if (hintTimer <= 0f && sizeHint != null)
                sizeHint.SetActive(false);
        }

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
                    ShowHint("I need to be smaller...");
                break;

            case InteractableType.MovableTree:
                if (ShrinkingAndEnlarging.CurrentSize == SizeState.Large)
                    PushTree();
                else
                    ShowHint("I need to be bigger...");
                break;
        }
    }

    void ShowHint(string message)
    {
        if (sizeHint == null) return;
        sizeHintText.text = message;
        sizeHint.SetActive(true);
        hintTimer = 2f;
    }

    void OpenDoor()
    {
        used = true;
        if (interactPrompt != null) interactPrompt.SetActive(false);

        if (!string.IsNullOrEmpty(doorSceneName))
            SceneManager.LoadScene(doorSceneName);
        else
            Debug.LogWarning("SmallDoor: no scene name set in the Inspector!");
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
