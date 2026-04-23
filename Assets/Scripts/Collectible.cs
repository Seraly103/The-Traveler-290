using UnityEngine;

public class Collectible : MonoBehaviour
{
    public string itemType;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("TOUCHED");

            if (!InventoryManager.TryAddItem(itemType))
            {
                return;
            }

            Destroy(gameObject);
        }
        
        
    }
}
