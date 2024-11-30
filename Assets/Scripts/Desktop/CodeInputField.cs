using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CodeInputField : MonoBehaviour
{
    string nameOfFile;
    string myCode = "Password123";
    [SerializeField] TextMeshProUGUI tmp_Message;
    FileData fileData;

    public void Init(string name, string code, FileData fileData)
    {
        nameOfFile = name;
        myCode = code;
        this.fileData = fileData;
    }

    // References
    [SerializeField] Image fileIcon;
    [SerializeField] TextMeshProUGUI fileName;

    void Start()
    {
        // Set file icon depening on file data type
        fileName.text = $"Unlock {fileData.myFileName}";
        fileIcon.sprite = fileData.GetIcon();
    }

    DefaultInputActions inputActions;

    private void OnEnable()
    {
        inputActions ??= new();

        inputActions.UI.Submit.Enable();

        inputActions.UI.Submit.performed += EnterCode;
    }

    private void OnDisable()
    {
        inputActions.UI.Submit.Disable();

        inputActions.UI.Submit.performed -= EnterCode;
    }

    void EnterCode(InputAction.CallbackContext ctx)
    {
        TMP_InputField inputField = GetComponentInChildren<TMP_InputField>();

        if (inputField == null) { Debug.LogError("InputField not found"); return; }

        if (ctx.performed)
        {
            if (inputField.text == myCode)
            {
                PlayerPrefs.SetInt($"{nameOfFile}IsLocked", 0);
                StartCoroutine(DisplayCodeWasRight());
                Debug.Log($"Code ({inputField.text}) is correct");
            }
            else
            {
                StartCoroutine(DisplayCodeWasWrong());
                Debug.Log($"Code ({inputField.text}) is incorrect");
            }
        }
    }

    public void EnterCode(TMP_InputField inputField)
    {
        if (inputField.text == myCode)
        {
            PlayerPrefs.SetInt($"{nameOfFile}IsLocked", 0);
            StartCoroutine(DisplayCodeWasRight());
            Debug.Log($"Code ({inputField.text}) is correct");
        }
        else
        {
            StartCoroutine(DisplayCodeWasWrong());
            Debug.Log($"Code ({inputField.text}) is incorrect");
        }
    }

    IEnumerator DisplayCodeWasWrong()
    {
        // Display message
        tmp_Message.gameObject.SetActive(true);

        // Wait for 2 seconds
        yield return new WaitForSeconds(2);

        // Hide message
        tmp_Message.gameObject.SetActive(false);

        yield break;
    }

    IEnumerator DisplayCodeWasRight()
    {
        // Display message
        tmp_Message.gameObject.SetActive(true);

        // Change message
        tmp_Message.text = "Unlocked!";
        tmp_Message.color = Color.green;

        // Wait for 2 seconds
        yield return new WaitForSeconds(1f);

        // Update lock state
        fileData.UpdateLockState();

        // Destroy this object
        Destroy(gameObject);

        yield break;
    }
}