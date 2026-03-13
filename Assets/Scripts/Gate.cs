using UnityEngine;
using UnityEngine.SceneManagement;

public class Gate : MonoBehaviour
{
    public GameObject popUp;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && InventoryManager.instance.hasKey)
        {
            popUp.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            popUp.SetActive(false);
        }
    }
}
