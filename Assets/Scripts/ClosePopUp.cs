using UnityEngine;

public class ClosePopUp : MonoBehaviour
{
    public GameObject popUp;

    public void CloseGate()
    {
        popUp.SetActive(false);
    }
}
