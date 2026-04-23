using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterGate : MonoBehaviour
{
    public string nextScene;
    public void Enter()
    {
        
        Debug.Log("Button pressed");
        SceneManager.LoadScene("Combat");
    }
        
    
}
