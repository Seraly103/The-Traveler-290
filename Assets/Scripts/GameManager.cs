using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;


public class GameManager : MonoBehaviour
{
     public DialogueRunner dialogueRunner;
    public string nextSceneName;

    void Start()
    {
        dialogueRunner.onDialogueComplete.AddListener(LoadNextScene);
    }

    // Update is called once per frame
    void LoadNextScene()
    {
        SceneManager.LoadScene("Alice's House");
    }
}
