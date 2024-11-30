using UnityEngine;


[System.Serializable]
public class ShareableFileData
{
    public string myFileName; // Name of the file
    public DataType myDataType; // Type of the file
    public string myDateModified = "Default Time"; // Date modified
    public int myFileSize = 0; // File size
    public bool isLocked = false; // Is the file locked
    public string lockCode = "Password123"; // Password for the file
    public Transform instantiator; // Transform of the object that instantiates this file

    // Content
    [TextArea(5, 10)] public string textContent; // Text content
    public Sprite imageContent; // Image content
    public int sceneIndex; // Scene index for executable files

    // Constructor for all parameters
    public ShareableFileData(string fileName, DataType dataType, string dateModified, int size, bool isLocked, string lockCode, string textContent, Sprite imageContent, Transform instantiator)
    {
        this.myFileName = fileName;
        this.myDataType = dataType;
        this.myDateModified = dateModified;
        this.myFileSize = size;
        this.isLocked = isLocked;
        this.lockCode = lockCode;
        this.textContent = textContent;
        this.imageContent = imageContent;
        this.instantiator = instantiator;
    }
}