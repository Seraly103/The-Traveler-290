using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class GapTeleport : MonoBehaviour
{
    public GameObject player;

    public int lvlNumber;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if(lvlNumber == 1)
            {
                SceneManager.LoadScene("RedQueen'sEntrance");
            }
            else if(lvlNumber == 2)
            {
                SceneManager.LoadScene("ChesireForest 1");
            }
        }
        
    }

   

}
