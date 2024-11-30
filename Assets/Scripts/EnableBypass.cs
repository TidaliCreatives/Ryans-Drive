using UnityEngine;

public class EnableBypass : MonoBehaviour
{
    public void EnablePasswordBypass(bool enabled)
    {
        if (enabled)
        {
            PlayerPrefs.SetInt("BypassPasswords", 1);
        }
        else
        {
            PlayerPrefs.SetInt("BypassPasswords", 0);
        }
    }
}