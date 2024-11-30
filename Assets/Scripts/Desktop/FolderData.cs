using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FolderData : MonoBehaviour
{
    public static event Action<FolderData, List<GameObject>> OnFolderLoaded;

    [SerializeField] private Transform subfileTransform;
    [SerializeField] private Transform contentTransform;
    [SerializeField] private bool isLocked = false;
    [SerializeField] private string password = "";

    private List<GameObject> subfiles = new();

    private void Start()
    {
        Debug.Log(subfileTransform.childCount);

        foreach (Transform subfile in subfileTransform)
        {
            subfile.gameObject.SetActive(false);
            subfile.SetParent(contentTransform, false);
            subfiles.Add(subfile.gameObject);
        }
    }

    public void EnterFolder()
    {
        if (isLocked)
        {
            // Display password prompt
            Debug.Log($"Folder locked, password is {password}.");
        }
        else
        {
            OnFolderLoaded?.Invoke(this, subfiles);
        }
    }

    private void OnEnable()
    {
        OnFolderLoaded += LoadFolder;
    }

    private void OnDisable()
    {
        OnFolderLoaded -= LoadFolder;
    }

    private void LoadFolder(FolderData folder, List<GameObject> _)
    {
        // If the folder clicked on is this folder
        if (folder == this)
        {
            // Set every subfile active
            foreach (GameObject subfile in subfiles)
            {
                subfile.SetActive(true);
            }

            // Set this folder inactive
            gameObject.SetActive(false);

            return;
        }

        // Else, if the folder clicked on is not this folder

        // Set every subfile inactive
        foreach (GameObject subfile in subfiles)
        {
            subfile.SetActive(false);
        }

        // Set this folder inactive
        gameObject.SetActive(false);
    }
}