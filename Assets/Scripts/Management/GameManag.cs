using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManag : MonoBehaviour
{
    public static GameManag Instance { get; private set; }

    public static event Action<ShareableFileData> OnReloadingDesktop;


    /* References */
    [SerializeField] AudioMixer audioMixer;
    Transform linkGrid;

    private List<ShareableFileData> linkedFiles = new();
    private int sceneLoadCount = 0;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject); // Destroy this object, if there are more than one
        }
        else
        {
            Instance = this; // Set this to instance

            transform.parent = null; // Unparent

            DontDestroyOnLoad(gameObject); // Dont destroy this object, if this is the only one
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        VolumeSlider.OnMasterVolumeChanged += SetVolume;
        ContentManag.OnLinkCreated += AddToLinkedFileData;
        FileData.OnLinkRemoved += RemoveFromLinkedFileData;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        VolumeSlider.OnMasterVolumeChanged -= SetVolume;
        ContentManag.OnLinkCreated -= AddToLinkedFileData;
        FileData.OnLinkRemoved -= RemoveFromLinkedFileData;
    }

    private void Start()
    {
        // Bypass passwords
        if (!PlayerPrefs.HasKey("BypassPasswords"))
        {
            PlayerPrefs.SetInt("BypassPasswords", 0);
        }

        // Audio
        if (audioMixer == null)
        {
            audioMixer = Resources.Load<AudioMixer>("AudioMixer");
        }

        if (audioMixer != null)
        {
            if (!PlayerPrefs.HasKey("MasterVolume"))
            {
                PlayerPrefs.SetInt("MasterVolume", 80);
            }

            // Set the volume of the audioMixer
            audioMixer.SetFloat("MasterVolume", Mathf.Clamp(0.8f * PlayerPrefs.GetInt("MasterVolume", 80) - 80, -80, 0));
        }
        else
        {
            Debug.LogWarning("TDL: audioMixer is null!");
        }



        // If the scene is "Desktop", do link based operations
        if (SceneManager.GetActiveScene().name.Equals("Desktop"))
        {
            if (linkGrid == null)
            {
                linkGrid = GameObject.FindGameObjectWithTag("LinkGrid").transform;
            }

            if (linkGrid != null)
            {
                foreach (Transform child in linkGrid)
                {
                    if (child.TryGetComponent(out FileData fileData))
                    {
                        // If list doesnt contain shareable file data with the same name and type, add it
                        ShareableFileData shareableFileData = new(
                            fileData.myFileName,
                            fileData.myDataType,
                            fileData.myDateModified,
                            fileData.myFileSize,
                            fileData.isLocked,
                            fileData.lockCode,
                            fileData.textContent,
                            fileData.imageContent,
                            fileData.instantiator
                        );

                        if (!linkedFiles.Contains(shareableFileData))
                        {
                            linkedFiles.Add(shareableFileData);
                        }

                        // Destroy the object
                        Destroy(child.gameObject);
                    }
                    else
                    {
                        Debug.LogError("TDL: NULLREF. No object of type FileData found. OHNO! - GameManag");
                    }
                }

                foreach (ShareableFileData fileData in linkedFiles)
                {
                    OnReloadingDesktop?.Invoke(fileData);
                }
            }
            else
            {
                Debug.LogWarning("TDL: linkGrid is null!");
            }
        }
        else
        {
            Debug.LogWarning("Game should start in Desktop Scene!");
        }


        // Set cursor lockmode based on game type
        if (GameObject.FindGameObjectWithTag("FirstPersonController") != null)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
    }

    public void TogglePause()
    {
        Time.timeScale = (Time.timeScale == 0) ? 1 : 0;
    }

    public void TogglePause(bool doPause)
    {
        Time.timeScale = doPause ? 0 : 1;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Save player prefs
        PlayerPrefs.Save();

        sceneLoadCount++;

        if (audioMixer == null)
        {
            audioMixer = Resources.Load<AudioMixer>("AudioMixer");
        }

        if (audioMixer != null)
        {
            // Set the volume of the audioMixer
            audioMixer.SetFloat("MasterVolume", Mathf.Clamp(0.8f * PlayerPrefs.GetInt("MasterVolume", 80) - 80, -80, 0));
        }
        else
        {
            Debug.LogWarning("TDL: audioMixer is null! (OnSceneLoad)");
        }



        // If the scene is "Desktop", and not at the start of the game do link based operations
        if (scene.name.Equals("Desktop") && sceneLoadCount > 1)
        {
            if (linkGrid == null)
            {
                linkGrid = GameObject.FindGameObjectWithTag("LinkGrid").transform;
            }

            if (linkGrid != null)
            {
                foreach (Transform child in linkGrid)
                {
                    Destroy(child.gameObject);
                }

                foreach (ShareableFileData fileData in linkedFiles)
                {
                    OnReloadingDesktop?.Invoke(fileData);
                }
            }
            else
            {
                Debug.LogWarning("TDL: linkGrid is null!");
            }
        }

        // Set cursor lockmode based on game type
        if (GameObject.FindGameObjectWithTag("FirstPersonController") != null)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
    }

    void SetVolume(int volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Clamp(0.8f * volume - 80, -80, 0));
    }

    void AddToLinkedFileData(ShareableFileData shareableFileData)
    {
        // Check if the list contains the shareable file data
        foreach (ShareableFileData data in linkedFiles)
        {
            if (data.myFileName == shareableFileData.myFileName)
            {
                return;
            }
        }
        linkedFiles.Add(shareableFileData);
    }

    void RemoveFromLinkedFileData(ShareableFileData shareableFileData)
    {
        foreach (ShareableFileData data in linkedFiles)
        {
            if (data.myFileName == shareableFileData.myFileName)
            {
                linkedFiles.Remove(data);
                break;
            }
        }
    }
}