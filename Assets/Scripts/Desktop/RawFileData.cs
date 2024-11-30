    using System;
using UnityEngine;

public enum DataType
{
    Undefined,
    Drive,
    Executable,
    Folder,
    PNG,
    Text
}

public class RawFileData : MonoBehaviour
{
    public string myFileName = "Default Filename";
    public DataType myDataType = DataType.Undefined;
    public CustomDate customDate = new();
    public string myDateModified = "Default Time";
    public int mySize = 0;
    public bool isLocked = false;
    public string lockCode = "Password123";

    // Content
    [TextArea(15, 20)]
    public string textContent;
    public Sprite imageContent;

    private void Awake()
    {
        if (myFileName.Equals("Default Filename"))
        {
            myFileName = name;
        }
        else if (name.Equals("Default Filename"))
        {
            name = myFileName;
        }

        myDateModified = $"" +
            $"{customDate.Year}-" +
            $"{(customDate.Month >= 10 ? customDate.Month : "0" + customDate.Month)}-" +
            $"{(customDate.Day >= 10 ? customDate.Day : "0" + customDate.Day)} " +
            $"{(customDate.Hour >= 10 ? customDate.Hour : "0" + customDate.Hour)}:" +
            $"{(customDate.Minute >= 10 ? customDate.Minute : "0" + customDate.Minute)}";

        if (myDataType == DataType.Folder && gameObject.CompareTag("Diary"))
        {
            this.myFileName = $"{this.myDateModified}_{this.myFileName}";
            this.name = $"{this.myDateModified}_{this.name}";
        }

        // If file type is undefined, check for any values and assign a type accordingly
        if (myDataType == DataType.Undefined)
        {
            if (imageContent != null)
            {
                myDataType = DataType.PNG;
                return;
            }

            if (textContent != null)
            {
                myDataType = DataType.Text;
                return;
            }

            if (!myFileName.Equals(("Default Filename")))
            {
                myDataType = DataType.Executable;
                return;
            }

            myDataType = DataType.Folder;
        }
    }

    private void Start()
    {
        if (isLocked && !PlayerPrefs.HasKey($"{myFileName}IsLocked"))
        {
            PlayerPrefs.SetInt($"{myFileName}IsLocked", 1);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        myDateModified = $"" +
            $"{customDate.Year}-" +
            $"{(customDate.Month >= 10 ? customDate.Month : "0" + customDate.Month)}-" +
            $"{(customDate.Day >= 10 ? customDate.Day : "0" + customDate.Day)} " +
            $"{(customDate.Hour >= 10 ? customDate.Hour : "0" + customDate.Hour)}:" +
            $"{(customDate.Minute >= 10 ? customDate.Minute : "0" + customDate.Minute)}";
    }
#endif
}