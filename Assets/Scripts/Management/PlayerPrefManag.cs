using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPrefManag : MonoBehaviour
{
    public static event Action OnPrefReset;

    [SerializeField] bool clearOnStart = false;

    readonly int defaultMasterVolume = 80;

    private void Start()
    {
        if (clearOnStart)
        {
            PlayerPrefs.DeleteAll();
        }
    }

    public void ResetPrefs()
    {
        PlayerPrefs.DeleteAll();

        PlayerPrefs.SetInt("MasterVolume", defaultMasterVolume);

        OnPrefReset?.Invoke();

        SceneManager.LoadSceneAsync("Desktop");
    }
}