using UnityEngine;

public class RESET : MonoBehaviour
{
    void Start()
    {
        PlayerPrefs.DeleteKey("QueensIntroPlayed");
        PlayerPrefs.Save();
        Debug.Log("RESET QueensIntroPlayed");
    }
}
