using UnityEngine;
using System.Collections;


public class GapTeleport : MonoBehaviour
{
    public GameObject leftDestination;
    public GameObject rightDestination;
    public GameObject leftPopup;
    public GameObject rightPopup;

    public GameObject player;

    public Vector3 teleportOffset = Vector3.zero;

    public void Teleport()
    {
        StartCoroutine(TeleportRoutine(leftDestination, leftPopup));
    }

    public void TeleportLeft()
    {
        StartCoroutine(TeleportRoutine(leftDestination, leftPopup));
    }

    public void TeleportRight()
    {
        StartCoroutine(TeleportRoutine(rightDestination, rightPopup));
    }

    IEnumerator TeleportRoutine(GameObject destination, GameObject popupToDisable)
    {
        if (player == null || destination == null)
        {
            yield break;
        }

        PlayerController pc = player.GetComponent<PlayerController>();

        // LOCK movement
        pc.movementLocked = true;

        // TELEPORT
        Vector3 target = destination.transform.position + teleportOffset;
        target.y = player.transform.position.y;
        target.z = player.transform.position.z;
        player.transform.position = target;

        // wait ONE frame so world doesn't override it
        yield return null;

        // UNLOCK movement
        pc.movementLocked = false;

        if (popupToDisable != null)
            popupToDisable.SetActive(false);
    }


}
