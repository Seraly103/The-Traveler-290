using UnityEngine;
using UnityEngine.SceneManagement;

public class Gate : MonoBehaviour
{
    public GameObject popUp;
    public GameObject locked;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && InventoryManager.hasKey)
        {
            popUp.SetActive(true);
        }
        else
        {
           locked.SetActive(true); 
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            popUp.SetActive(false);

            locked.SetActive(false); 
        }
    }
}
