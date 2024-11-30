using UnityEngine;

public class CloseWindow : MonoBehaviour
{
    public void OpenWindowCmd(GameObject obj)
    {
        if (!obj.activeInHierarchy)
        {
            obj.SetActive(true);
        }
    }

    public void CloseWindowCmd(GameObject obj)
    {
        if (obj.activeInHierarchy)
        {
            obj.SetActive(false);
        }
    }
}