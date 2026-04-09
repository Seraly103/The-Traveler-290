using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class GapTeleport : MonoBehaviour
{
    public GameObject player;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            SceneManager.LoadScene("RedQueen'sEntrance");
        }
    }

   

}
