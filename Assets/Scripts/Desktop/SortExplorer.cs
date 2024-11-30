using System.Collections.Generic;
using UnityEngine;

public enum SortType
{
    Name,
    NameReversed,
    Type,
    TypeReversed,
    Date,
    DateReversed,
    Size,
    SizeReversed
}

public class SortExplorer : MonoBehaviour
{
    [SerializeField] Transform explorerContent;

    private SortType currentSortType = SortType.NameReversed;

    private void Start()
    {
        SortContent(currentSortType);
    }

    public void SortContent(string type)
    {
        SortType sortType = (SortType)System.Enum.Parse(typeof(SortType), type);

        if (sortType == currentSortType)
        {
            sortType = (SortType)((int)sortType + 1);
        }

        SortContent(sortType);

        currentSortType = sortType;
    }

    public void SortContent(string type, bool ignoreReversing)
    {
        SortType sortType = (SortType)System.Enum.Parse(typeof(SortType), type);

        if (sortType == currentSortType && ignoreReversing == false)
        {
            sortType = (SortType)((int)sortType + 1);
        }

        SortContent(sortType);

        currentSortType = sortType;
    }

    void SortContent(SortType type)
    {
        List<FileData> files = new();

        foreach (Transform child in explorerContent)
        {
            files.Add(child.GetComponent<FileData>());
        }

        switch (type)
        {
            case SortType.Name:
            case SortType.NameReversed:

                // Sort by name
                files.Sort((a, b) => a.myFileName.CompareTo(b.myFileName));

                if (type == SortType.NameReversed)
                {
                    files.Reverse();
                }

                break;


            case SortType.Type:
            case SortType.TypeReversed:
                // Sort by type
                files.Sort((a, b) => a.myDataTypeStr.CompareTo(b.myDataTypeStr));
                if (type == SortType.TypeReversed)
                {
                    files.Reverse();
                }

                break;


            case SortType.Date:
            case SortType.DateReversed:

                // Sort by date
                files.Sort((a, b) => a.myDateModified.CompareTo(b.myDateModified));
                if (type == SortType.DateReversed)
                {
                    files.Reverse();
                }

                break;


            case SortType.Size:
            case SortType.SizeReversed:

                // Sort by size
                files.Sort((a, b) => a.myFileSize.CompareTo(b.myFileSize));
                if (type == SortType.SizeReversed)
                {
                    files.Reverse();
                }

                break;


            default:

                Debug.LogError("Error in switch statement" + this.transform);

                break;
        }

        // Set the new order
        for (int i = 0; i < files.Count; i++)
        {
            files[i].transform.SetSiblingIndex(i);
        }
    }
}