using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class FileData : MonoBehaviour
{
    public static event Action<Transform> OnFolderLoaded;
    public static event Action<string> OnExecutableLoaded;
    public static event Action<ShareableFileData, DetectButtonClickKind> OnLinkCreated;
    public static event Action<ShareableFileData> OnLinkRemoved;

    // Data
    public string myFileName = "Default Filename";
    public string myDataTypeStr = "DefaultType";
    public DataType myDataType = DataType.Undefined;
    public CustomDate customDate = new();
    public string myDateModified = "Default Time";
    public int myFileSize = 0;
    public Transform instantiator;
    public bool isLocked = false;
    public string lockCode = "Password123";
    public ShareableFileData myShareableFileData;



    // Content
    [TextArea(5, 10)] public string textContent;
    public Sprite imageContent;

    // Scene index for exes
    public int sceneIndex;

    // Text Mesh Pro
    [SerializeField] TextMeshProUGUI tmpName;
    [SerializeField] TextMeshProUGUI tmpType;
    [SerializeField] TextMeshProUGUI tmpDate;
    [SerializeField] TextMeshProUGUI tmpSize;

    // Prefabs
    [SerializeField] GameObject prefab_Texteditor;
    [SerializeField] GameObject prefab_CodeInputField;
    [SerializeField] GameObject prefab_PNG;

    // References
    [SerializeField] GameObject typeIcon;
    [SerializeField] GameObject lockIcon;

    [SerializeField] Sprite iconDrive;
    [SerializeField] Sprite iconFolder;
    [SerializeField] Sprite iconExe;
    [SerializeField] Sprite iconText;
    [SerializeField] Sprite iconPNG;

    //[SerializeField] Button button;
    Transform canvas;


    // Constructor for all parameters
    public FileData(ShareableFileData shareableFileData)
    {
        Init(shareableFileData);
    }

    public void SetData(ShareableFileData shareableFileData)
    {
        Init(shareableFileData);
    }

    void Init(ShareableFileData shareableFileData)
    {
        this.myShareableFileData = shareableFileData;

        textContent = myShareableFileData.textContent;
        imageContent = myShareableFileData.imageContent;
        instantiator = myShareableFileData.instantiator;

        myFileName = myShareableFileData.myFileName;
        myDataType = myShareableFileData.myDataType;
        myDataTypeStr = myDataType.ToString();
        myDateModified = myShareableFileData.myDateModified;
        myFileSize = myDataType switch
        {
            DataType.Text => textContent.Length,
            DataType.PNG => imageContent.texture.width * imageContent.texture.height / 1000,
            _ => shareableFileData.myFileSize
        };
        isLocked = myShareableFileData.isLocked;
        lockCode = myShareableFileData.lockCode;

        SetTMPs();
    }

    // TMP.text is set here
    public void SetTMPs()
    {
        // Set name
        if (tmpName != null)
        {
            this.tmpName.text = myFileName;
        }

        // Set data type
        if (tmpType != null)
        {
            this.tmpType.text = myDataTypeStr;
        }


        // Set date modified
        if (tmpDate != null)
        {
            this.tmpDate.text = myDateModified;
        }

        // Set size
        if (tmpSize != null)
        {
            this.tmpSize.text = (myFileSize > 0) ? myFileSize.ToString() + " KB" : "";
        }

        // Set type icon
        if (typeIcon != null)
        {
            if (myDataType == DataType.PNG && imageContent != null)
            {
                typeIcon.GetComponent<Image>().sprite = imageContent;
            }
            else
            {
                switch (myDataType)
                {
                    case DataType.Drive:
                        typeIcon.GetComponent<Image>().sprite = iconDrive;
                        break;
                    case DataType.Folder:
                        typeIcon.GetComponent<Image>().sprite = iconFolder;
                        break;
                    case DataType.Executable:
                        typeIcon.GetComponent<Image>().sprite = iconExe;
                        break;
                    case DataType.Text:
                        typeIcon.GetComponent<Image>().sprite = iconText;
                        break;
                    case DataType.PNG:
                        typeIcon.GetComponent<Image>().sprite = iconPNG;
                        break;
                    default:
                        break;
                }
            }
        }

        // Set lock icon
        if (lockIcon != null)
        {
            isLocked = PlayerPrefs.GetInt($"{myFileName}IsLocked") == 1;
            lockIcon.SetActive(isLocked);
        }
    }

    private void Start()
    {
        // Find canvas by tag
        canvas = GameObject.FindGameObjectWithTag("Canvas").transform;

        if (myDataType == DataType.PNG && gameObject.CompareTag("Link"))
        {
            // Get Image Component In Children
            Image img = FindChildByTag(transform, "Image").GetComponent<Image>();
            img.sprite = imageContent;
        }

        if (isLocked && !PlayerPrefs.HasKey($"{myFileName}IsLocked"))
        {
            PlayerPrefs.SetInt($"{myFileName}IsLocked", 1);
        }

        UpdateLockState();
    }

    private void OnEnable()
    {
        PlayerPrefManag.OnPrefReset += UpdateLockState;
    }

    private void OnDisable()
    {
        PlayerPrefManag.OnPrefReset -= UpdateLockState;
    }

    public void LoadFolder()
    {
        OnFolderLoaded?.Invoke(instantiator);
    }

    public void Execute()
    {
        // If bypass passwords is true, set isLocked to false
        if (isLocked && PlayerPrefs.GetInt("BypassPasswords", 0) == 1)
        {
            isLocked = false;
        }
        else
        {
            // If locked, but player prefs does not have the key, set it to 1
            if (isLocked && !PlayerPrefs.HasKey($"{myFileName}IsLocked"))
            {
                PlayerPrefs.SetInt($"{myFileName}IsLocked", 1);
            }
            else
            {
                // Check if it is still locked
                isLocked = PlayerPrefs.GetInt($"{myFileName}IsLocked") == 1;
            }

            if (isLocked)
            {
                // Instantiate input field
                GameObject go = Instantiate(prefab_CodeInputField, canvas.transform);

                // Initialize input field
                if (go.TryGetComponent(out CodeInputField codeInputField))
                {
                    codeInputField.Init(myFileName, lockCode, this);
                }
                return;
            }
        }

        switch (myDataType)
        {
            case DataType.Executable:

                OnExecutableLoaded?.Invoke(myFileName);

                break;

            case DataType.Text:

                GameObject go1 = Instantiate(prefab_Texteditor, canvas.transform);

                TMP_InputField tmp1 = FindChildByTag(go1.transform, "Text").GetComponent<TMP_InputField>();

                tmp1.text = textContent;

                TextMeshProUGUI tmp2 = FindChildByTag(go1.transform, "Title").GetComponent<TextMeshProUGUI>();

                tmp2.text = $"{myFileName}.txt";

                break;

            case DataType.PNG:

                GameObject go2 = Instantiate(prefab_PNG, canvas.transform);

                Image img = FindChildByTag(go2.transform, "Image").GetComponent<Image>();

                img.sprite = imageContent;

                img.gameObject.GetComponent<ImageScaler>().UpdateSprite(imageContent, 1030 - 200, 1920 - 200);

                go2.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);

                TextMeshProUGUI tmp3 = FindChildByTag(go2.transform, "Title").GetComponent<TextMeshProUGUI>();

                tmp3.text = $"{myFileName}.png";

                break;

            case DataType.Folder:
            case DataType.Drive:

                LoadFolder();

                break;

            default:

                // Debug
                Debug.LogError("Default case in switch statement! " + transform);

                break;
        }
    }

    public void CreateLink()
    {
        DetectButtonClickKind buttonBehaviour = GetComponent<DetectButtonClickKind>();
        OnLinkCreated?.Invoke(myShareableFileData, buttonBehaviour);
    }
    public void RemoveLink()
    {
        OnLinkRemoved?.Invoke(myShareableFileData);
        Destroy(gameObject);
    }

    public void UpdateLockState()
    {
        // If bypass passwords is true
        if (isLocked && PlayerPrefs.GetInt("BypassPasswords", 0) == 1)
        {
            isLocked = false;
        }
        // Else check if this file is locked
        else if (PlayerPrefs.HasKey($"{myFileName}IsLocked"))
        {
            // Get lock state
            isLocked = PlayerPrefs.GetInt($"{myFileName}IsLocked") == 1;
        }
        // Else if shareable file data is not null, get lock state from it
        else if (myShareableFileData != null)
        {
            isLocked = myShareableFileData.isLocked;
        }

        // Set lock icon
        if (lockIcon != null)
        {
            lockIcon.SetActive(isLocked);
        }
    }

    public Sprite GetIcon()
    {
        return typeIcon.GetComponent<Image>().sprite;
    }

    Transform FindChildByTag(Transform parent, string tag)
    {
        foreach (Transform child in parent)
        {
            if (child.CompareTag(tag))
            {
                return child;
            }

            Transform found = FindChildByTag(child, tag);

            if (found != null)
            {
                return found;
            }
        }
        return null;
    }

}