using UnityEngine;

public class KeyCollectible : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager.TryAddItem("Key");
            gameObject.SetActive(false);
        }
    }
}