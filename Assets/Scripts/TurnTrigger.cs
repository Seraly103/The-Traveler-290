using UnityEngine;
using UnityEngine.SceneManagement;

public class TurnTrigger : MonoBehaviour
{
    public GameObject popUp;
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            popUp.SetActive(true);
        }
    }

    public void TeleportForest()
    {
        SceneManager.LoadScene("ChesireForest");
    }
        

    public void TeleportQueen()
    {
        SceneManager.LoadScene("RedQueen'sEntrance");
    }
}
