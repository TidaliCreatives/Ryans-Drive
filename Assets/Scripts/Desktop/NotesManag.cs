using UnityEngine;
using TMPro;

public class NotesManag : MonoBehaviour
{
    GameObject noteWindow;
    [SerializeField] TMP_InputField inputField;

    private void Start()
    {
        noteWindow = transform.GetChild(0).gameObject;
    }

    public void OpenNotes()
    {
        if (PlayerPrefs.HasKey("Notes"))
        {
            inputField.text = PlayerPrefs.GetString("Notes");
        }
        noteWindow.SetActive(true);
    }

    public void CloseNotes()
    {
        PlayerPrefs.SetString("Notes", inputField.text);
        noteWindow.SetActive(false);
    }
}