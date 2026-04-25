using UnityEngine;
using Yarn.Unity;
using System.Collections;

public class QueenRabbitMover : MonoBehaviour
{
    public RabbitMover rabbitMover;

    public DialogueRunner dialogueRunner;
    public string startNode;

    public Transform bunny;
    public Transform bunnyPoint;

    private string introKey = "QueensIntroPlayed";

    private bool triggered = false;

    private bool hasPlayed = false;

    private void Start()
    {
        
        if (PlayerPrefs.GetInt(introKey, 0) == 1)
        {
            bunny.position = bunnyPoint.position;
            gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;
        if (PlayerPrefs.GetInt(introKey, 0) == 1) return;

        triggered = true;
        StartCoroutine(PlayIntro());
        
    }

    IEnumerator PlayIntro()
    {
        
        dialogueRunner.StartDialogue(startNode);

        yield return new WaitUntil(() => !dialogueRunner.IsDialogueRunning);

        rabbitMover.MoveOnce();


        yield return new WaitForSeconds(1f);

        PlayerPrefs.SetInt(introKey, 1);
        PlayerPrefs.Save();

        gameObject.SetActive(false);
    }
}
