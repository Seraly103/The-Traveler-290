using UnityEngine;

public class GapTeleport : MonoBehaviour
{
    public Transform destination;
    public GameObject world;
    public GameObject popup;

    public void Teleport()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        player.transform.position = destination.position;

        popup.SetActive(false);
    }
}
