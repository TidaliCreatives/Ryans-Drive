using System;
using System.Collections.Generic;
using UnityEngine;

public class ContentManag : MonoBehaviour
{
    public static event Action<ShareableFileData> OnLinkCreated;

    [SerializeField] Transform content;
    [SerializeField] GameObject prefab_File;
    [SerializeField] GameObject prefab_Folder;
    [SerializeField] GameObject prefab_Drive;
    [SerializeField] GameObject backArrow;
    [SerializeField] TMPro.TextMeshProUGUI pathText;

    private SortExplorer sortExplorer;

    private List<Transform> path = new();

    readonly int maxLinks = 78;
    [SerializeField] GameObject explorerWindow;
    [SerializeField] Transform linkGrid;
    [SerializeField] GameObject prefab_DefaultLink;

    DefaultInputActions inputActions;

    private void OnEnable()
    {
        // Subscribe to the events
        FileData.OnFolderLoaded += LoadNewPage;
        BackArrow.OnGoingBack += LoadLastPage;
        FileData.OnLinkCreated += LinkToDesktop;
        GameManag.OnReloadingDesktop += LinkToDesktopOnReload;

        // Subscribe to input actions
        inputActions ??= new();
        inputActions.UI.Cancel.performed += _ => LoadLastPage();
        inputActions.UI.Cancel.Enable();
    }

    private void OnDisable()
    {
        // Unsubscribe from the events
        FileData.OnFolderLoaded -= LoadNewPage;
        BackArrow.OnGoingBack -= LoadLastPage;
        FileData.OnLinkCreated -= LinkToDesktop;
        GameManag.OnReloadingDesktop -= LinkToDesktopOnReload;

        // Unsubscribe from input actions
        inputActions.UI.Cancel.performed -= _ => LoadLastPage();
        inputActions.UI.Cancel.Disable();
    }

    private void Start()
    {
        sortExplorer = GetComponent<SortExplorer>();
        InstantiateChildren(transform);
        sortExplorer.SortContent("Date", true);
        UpdatePathText();
    }

    private void FixedUpdate()
    {
        if (path.Count > 0 && !backArrow.activeInHierarchy)
        {
            backArrow.SetActive(true);
            return;
        }
        if (path.Count == 0 && backArrow.activeInHierarchy)
        {
            backArrow.SetActive(false);
        }
    }

    public void LoadFirstPage()
    {
        ClearContent();
        InstantiateChildren(transform);
        ClearPathText();
    }

    void LoadLastPage()
    {
        // Remove the last parent
        if (path.Count > 0)
        {
            path.RemoveAt(path.Count - 1);
        }
        else
        {
            Debug.Log("Already in root folder!");
            return;
        }

        // Recall root folder if path is empty
        if (path.Count == 0)
        {
            UpdatePathText();
            ClearContent();
            InstantiateChildren(transform);
            return;
        }
        // Else
        // Remove all objects
        ClearContent();

        // Create new objects
        InstantiateChildren(path[^1]);

        // Name the path
        UpdatePathText();
    }

    public void ReloadCurrentPage()
    {
        InstantiateChildren(transform);
        UpdatePathText();
    }

    public void LinkToDesktop(ShareableFileData shareableFileData, DetectButtonClickKind buttonBehaviour)
    {
        // No duplicates (If link grid has fileData as child, which is the same as the fileData passed in, return)
        foreach (Transform child in linkGrid)
        {
            if (child.TryGetComponent<FileData>(out FileData fileData) && fileData.myFileName == shareableFileData.myFileName)
            {
                return;
            }
        }

        if (linkGrid.childCount < maxLinks)
        {
            GameObject link = Instantiate(prefab_DefaultLink, linkGrid);

            link.GetComponent<FileData>().SetData(shareableFileData);

            // Overriding the button behaviour
            if (link.TryGetComponent(out DetectButtonClickKind detecter))
            {
                detecter.Init(buttonBehaviour);
            }
            else
            {
                // Add the button behaviour component
                DetectButtonClickKind newDetecter = link.AddComponent<DetectButtonClickKind>();
                newDetecter.Init(buttonBehaviour);
            }

            link.name = link.GetComponent<FileData>().myFileName;
        }

        OnLinkCreated?.Invoke(shareableFileData);
    }

    public void LinkToDesktopOnReload(ShareableFileData shareableFileData)
    {
        if (linkGrid.childCount < maxLinks)
        {
            GameObject go = Instantiate(prefab_DefaultLink, linkGrid);

            go.GetComponent<FileData>().SetData(shareableFileData);

            go.name = go.GetComponent<FileData>().myFileName;
        }
    }

    void LoadNewPage(Transform parent)
    {
        ClearContent();
        InstantiateChildren(parent);
        path.Add(parent);
        UpdatePathText();
        if (!explorerWindow.activeInHierarchy)
        {
            explorerWindow.SetActive(true);
        }

    }

    void InstantiateChildren(Transform parent)
    {
        // Instantiate file prefab for each child of this transform
        foreach (Transform child in parent)
        {
            // Get the RawFileData component of the child
            RawFileData rawFileData = child.GetComponent<RawFileData>();

            // Instantiate the prefab for the file type
            GameObject go = Instantiate(prefab_File, content);

            go.name = $"{rawFileData.myFileName}.{rawFileData.myDataType} (Explorer)";

            // Get the FileData component of the prefab
            FileData fileData = go.GetComponent<FileData>();

            // Create a ShareableFileData object with the data of the RawFileData component
            ShareableFileData data = new(
                rawFileData.myFileName,
                rawFileData.myDataType,
                rawFileData.myDateModified,
                rawFileData.mySize,
                rawFileData.isLocked,
                rawFileData.lockCode,
                rawFileData.textContent,
                rawFileData.imageContent,
                child
            );

            // Set the data of the FileData component
            fileData.SetData(data);
        }
    }

    void ClearContent()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }

    void UpdatePathText()
    {
        string pathString = (path.Count == 0) ? "Explorer": path[0].GetComponent<RawFileData>().myFileName;
        for (int i = 1; i < path.Count; i++)
        {
            pathString += " \\ " + path[i].GetComponent<RawFileData>().myFileName;
        }
        pathText.text = pathString;
    }

    void ClearPathText()
    {
        path.Clear();
        UpdatePathText();
    }   
}