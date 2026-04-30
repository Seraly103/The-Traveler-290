using UnityEngine;

public class RibbonCollectible : MonoBehaviour
{
    void Start()
    {
        // Hide it if already collected
        if (InventoryManager.Ribbon > 0)
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager.TryAddItem("Ribbon");
            gameObject.SetActive(false);
        }
    }
}