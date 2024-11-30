using UnityEngine;

public class UnlockSecrets : MonoBehaviour
{
    public void UnlockLeftMirror()
    {
        PlayerPrefs.SetInt("LeftMirrorActive", 1);
    }

    public void UnlockRightMirror()
    {
        PlayerPrefs.SetInt("RightMirrorActive", 1);
    }
}