using UnityEngine;
using UnityEngine.SceneManagement;

public class Gate : MonoBehaviour
{
    public string nextString;

    void OnTriggerEnter2D()
    {
        if (other.CompareTag("Player"))
        {
            if(InventoryManager.instance.hasKey)
            {
                SceneManager.LoadScene(nextScene);
            }
            else
            {
                Debug.Log("You need the key!");
            }
        }
    }
}
