using UnityEngine;

public enum SizeState { Normal, Small, Large }

/// <summary>
/// Attach to Alice's GameObject.
/// Mushrooms need no script — just tag them "ShrinkMushroom" or "GrowMushroom"
/// and give them a Collider2D set to Is Trigger.
/// </summary>
public class ShrinkingAndEnlarging : MonoBehaviour
{
    public static ShrinkingAndEnlarging Instance { get; private set; }

    [Header("Multipliers applied to Alice's scene scale")]
    public float smallMultiplier = 0.5f;
    public float largeMultiplier = 2f;

    [Header("Transition")]
    public float scaleSpeed = 3f;

    public static SizeState CurrentSize { get; private set; } = SizeState.Normal;

    private Vector3 normalScale;
    private Vector3 targetScale;
    private float feetY;
    private float normalHalfHeight;
    private float targetY;

    

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        normalScale = transform.localScale;
        CurrentSize = SizeState.Normal;
        targetScale = normalScale;

        var sr = GetComponent<SpriteRenderer>();
        normalHalfHeight = sr != null ? sr.bounds.extents.y : 0f;
        feetY = transform.position.y - normalHalfHeight;
        targetY = transform.position.y;
    }

    void Start()
    {
        transform.localScale = normalScale;
        targetY = transform.position.y;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
        float newY = Mathf.Lerp(transform.position.y, targetY, Time.deltaTime * scaleSpeed);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void SetTargetY(Vector3 newScale)
    {
        float scaleRatio = newScale.y / normalScale.y;
        targetY = feetY + normalHalfHeight * scaleRatio;
    }

    // Alice walks into a mushroom — no key press needed, eating is automatic
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("ShrinkMushroom"))
        {
            Shrink();
            InventoryManager.mushroomCollected = true;
            other.gameObject.SetActive(false);
        }
        
        else if (other.CompareTag("GrowMushroom"))
        {
            Enlarge();
            Destroy(other.gameObject);
        }
    }

    public void Shrink()
    {
        CurrentSize = SizeState.Small;
        targetScale = normalScale * smallMultiplier;
        SetTargetY(targetScale);
        Debug.Log("Alice shrank!");
    }

    public void Enlarge()
    {
        CurrentSize = SizeState.Large;
        targetScale = normalScale * largeMultiplier;
        SetTargetY(targetScale);
        Debug.Log("Alice enlarged!");
    }

    public void ResetSize()
    {
        CurrentSize = SizeState.Normal;
        targetScale = normalScale;
        SetTargetY(targetScale);
    }
}
