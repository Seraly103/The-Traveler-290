using UnityEngine;

public class Collectible : MonoBehaviour
{
    public string itemType;
    //private InventoryManager inventoryManager;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("TOUCHED");
            
            InventoryManager.instance.AddItem(itemType);

            Destroy(gameObject);
        }
        
        
    }
}
