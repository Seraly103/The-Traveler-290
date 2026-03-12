using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    public int mushroom;
    public int note;
    public bool hasKey;

    public TextMeshProUGUI MushroomText;
    public TextMeshProUGUI NoteText;
    public TextMeshProUGUI KeyText;

    public Image MushroomIcon;
    public Image KeyIcon;

    public Image NoteIcon;

    void Start()
    {
        MushroomText.gameObject.SetActive(false);
        KeyText.gameObject.SetActive(false);
        NoteText.gameObject.SetActive(false);

        MushroomIcon.gameObject.SetActive(false);
        KeyIcon.gameObject.SetActive(false);
        NoteIcon.gameObject.SetActive(false);
        
    }

    void Awake()
    {
        instance = this;
    }

    public void AddItem(string itemType)
    {
        if(itemType == "Mushroom")
        {
            mushroom+=1;
            MushroomText.gameObject.SetActive(true);
            MushroomIcon.gameObject.SetActive(true);
            Debug.Log(mushroom);
        }

        if(itemType == "Note")
        {
            note+=1;
            NoteText.gameObject.SetActive(true);
            NoteIcon.gameObject.SetActive(true);
            

        }

        if(itemType == "Key")
        {
            hasKey = true;
            KeyText.gameObject.SetActive(true);
            KeyIcon.gameObject.SetActive(true);
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        MushroomText.text = mushroom.ToString();

        
    
        NoteText.text = note.ToString();
        KeyText.text = hasKey ? "1" : "0";
    }

    
    
}
