
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Yarn.Unity;
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    
    // Persistent flag to track if inventory UI has been revealed
    private static bool inventoryRevealed = false;

    // Static inventory counts so they persist across scenes
    public static int mushroom;
    public static int note;
    public static bool hasKey;

    public TextMeshProUGUI MushroomText;
    public TextMeshProUGUI NoteText;
    public TextMeshProUGUI KeyText;

    public Image MushroomIcon;
    public Image KeyIcon;

    public Image NoteIcon;

    void Start()
    {
        // If inventory was already revealed, show the UI elements for collected items
        if (inventoryRevealed)
        {
            if (mushroom > 0)
            {
                MushroomText.gameObject.SetActive(true);
                MushroomIcon.gameObject.SetActive(true);
            }
            
            if (note > 0)
            {
                NoteText.gameObject.SetActive(true);
                NoteIcon.gameObject.SetActive(true);
            }
            
            if (hasKey)
            {
                KeyText.gameObject.SetActive(true);
                KeyIcon.gameObject.SetActive(true);
            }
            
            // Update the text to show the correct numbers
            UpdateUI();
        }
        else
        {
            // First time - hide all inventory UI
            MushroomText.gameObject.SetActive(false);
            KeyText.gameObject.SetActive(false);
            NoteText.gameObject.SetActive(false);

            MushroomIcon.gameObject.SetActive(false);
            KeyIcon.gameObject.SetActive(false);
            NoteIcon.gameObject.SetActive(false);
        }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public static bool TryAddItem(string itemType)
    {
        if (instance == null)
        {
            Debug.LogError($"InventoryManager is missing. Could not add item '{itemType}'.");
            return false;
        }

        instance.AddItem(itemType);
        
        return true;
    }

    public void AddItem(string itemType)
    {
        // Mark inventory as revealed on first item collection
        inventoryRevealed = true;

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
        UpdateYarnVariables();
    }

    void UpdateUI()
    {
        MushroomText.text = mushroom.ToString();

        
    
        NoteText.text = note.ToString();
        KeyText.text = hasKey ? "1" : "0";
    }

    public void UpdateYarnVariables()
    {
        Debug.Log("YarnVarriables updated");
        var yarn = FindObjectOfType<DialogueRunner>();

        if (yarn != null)
        {
            yarn.VariableStorage.SetValue("$hasNote", note > 0);
            yarn.VariableStorage.SetValue("$hasKey", hasKey);
            yarn.VariableStorage.SetValue("$hasMushroom", mushroom > 0);
        }
    }

    
    
}