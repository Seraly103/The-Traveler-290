using UnityEngine;

public class CollectableController : MonoBehaviour
{
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool useProximityFallback = true;
    [SerializeField] private float proximityRadius = 1.25f;

    private bool showConfirmation;
    private bool hasTriggered;
    private Transform playerTransform;

    private const string PopupMessage = "are you sure? some wonderfully impossible things might happen";

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
    }

    private void Update()
    {
        if (!useProximityFallback || hasTriggered || showConfirmation)
        {
            return;
        }

        if (playerTransform == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
            }
            return;
        }

        float distance = Vector3.Distance(playerTransform.position, transform.position);
        if (distance <= proximityRadius)
        {
            TryOpenPopup(playerTransform.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryOpenPopup(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryOpenPopup(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryOpenPopup(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryOpenPopup(collision.gameObject);
    }

    private void TryOpenPopup(GameObject otherObject)
    {
        if (hasTriggered || showConfirmation)
        {
            return;
        }

        if (!otherObject.CompareTag(playerTag))
        {
            return;
        }

        showConfirmation = true;
    }

    private void Collect()
    {
        hasTriggered = true;
        showConfirmation = false;
        Destroy(gameObject);
    }

    private void OnGUI()
    {
        if (!showConfirmation)
        {
            return;
        }

        const float width = 460f;
        const float height = 180f;
        Rect windowRect = new Rect((Screen.width - width) * 0.5f, (Screen.height - height) * 0.5f, width, height);

        GUI.Box(windowRect, "Collect Item");

        Rect labelRect = new Rect(windowRect.x + 16f, windowRect.y + 36f, windowRect.width - 32f, 60f);
        GUI.Label(labelRect, PopupMessage);

        Rect yesRect = new Rect(windowRect.x + 70f, windowRect.y + 110f, 140f, 36f);
        Rect noRect = new Rect(windowRect.x + windowRect.width - 210f, windowRect.y + 110f, 140f, 36f);

        if (GUI.Button(yesRect, "Yes"))
        {
            Collect();
        }

        if (GUI.Button(noRect, "No"))
        {
            showConfirmation = false;
        }
=======
=======
>>>>>>> parent of bac5c3a (Inventory system)
=======
>>>>>>> parent of bac5c3a (Inventory system)
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
<<<<<<< HEAD
<<<<<<< HEAD
>>>>>>> parent of bac5c3a (Inventory system)
=======
>>>>>>> parent of bac5c3a (Inventory system)
=======
>>>>>>> parent of bac5c3a (Inventory system)
    }
}
