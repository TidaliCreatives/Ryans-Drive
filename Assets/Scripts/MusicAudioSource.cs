using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicAudioSource : MonoBehaviour
{
    // Clone check
    private void Awake()
    {
        // Get all objects with tag "MusicAudioSource"
        if (GameObject.FindGameObjectsWithTag("MusicAudioSource").Length > 1)
        {
            // Destroy this object, if there are more than one
            Destroy(gameObject);
        }
        else
        {
            // Unparent
            transform.parent = null;

            // Dont destroy this object, if this is the only one
            DontDestroyOnLoad(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        // Return if the game just started
        if (Time.time < 3f && !scene.name.Equals("Happy Birthday")) { return; }

        // When Happy Birthday scene was loaded, stop the background music
        if (scene.name.Equals("Happy Birthday") || scene.name.Contains("Game2"))
        {
            GetComponent<AudioSource>().Stop();
        }
        // If another scene was loaded and it's not playing, play the background music
        else if (!GetComponent<AudioSource>().isPlaying)
        {
            GetComponent<AudioSource>().Play();
        }
    }
}