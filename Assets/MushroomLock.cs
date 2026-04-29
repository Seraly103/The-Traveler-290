using UnityEngine;

public class MushroomLock : MonoBehaviour
{
    public GameObject mushroom;

    void Start()
    {
        UpdateMushroomVisibility();
    }

    public void CheckUnlock()
    {
        UpdateMushroomVisibility();
    }

    void UpdateMushroomVisibility()
    {
        if (mushroom == null) return;

        mushroom.SetActive(
            InventoryManager.talkedToCat &&
            !InventoryManager.mushroomCollected
        );
    }
    
}