using UnityEngine;
using Yarn.Unity;


public class RabbitTrigger : MonoBehaviour
{
    
    public DialogueRunner dialogueRunner;
    public string startNode = "Lorina_and_Bunny_Pt1";

    public RabbitMover rabbitMover;


    private bool triggered = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("TRIGGERED by: " + other.name);

        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            dialogueRunner.StartDialogue(startNode);

            rabbitMover.MoveOnce();
        }
    }


}
