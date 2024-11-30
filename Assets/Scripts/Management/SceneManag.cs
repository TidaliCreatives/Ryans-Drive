using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManag : MonoBehaviour
{
    public static SceneManag Instance { get; private set; }

    DefaultInputActions inputActions;

    public static event Action<string> OnNewSceneLoaded;

    [SerializeField] Animator loadingScreenAnimator;

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
        // Subscribe to scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Subscribe to events
        FileData.OnExecutableLoaded += LoadSceneAsync;

        // Subsribe to win game event
        WinGameFunctionality.OnWinGame += LoadSceneAsync;

        // Initialize input actions
        inputActions ??= new();

        // Enable input actions
        inputActions.Player.BackToDesktop.Enable();

        // Add input actions callbacks
        inputActions.Player.BackToDesktop.performed += _ => LoadSceneAsync("Desktop");
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        FileData.OnExecutableLoaded -= LoadSceneAsync;

        // Unsubscribe from win game event
        WinGameFunctionality.OnWinGame -= LoadSceneAsync;

        // Disable input actions
        if (inputActions != null)
        {
            inputActions.Player.BackToDesktop.Disable();

            // Remove input actions callbacks
            inputActions.Player.BackToDesktop.performed -= _ => LoadSceneAsync("Desktop");
        }
    }

    // Load scene async by index
    public void LoadSceneAsync(int sceneIndex)
    {
        // Load scene async
        SceneManager.LoadSceneAsync(sceneIndex);
    }

    // Load scene async by name
    public void LoadSceneAsync(string sceneName)
    {
        // Load scene async (if current and new scene are not desktop)
        if (SceneManager.GetActiveScene().name.Equals("Desktop") && sceneName.Equals("Desktop"))
        {
            Debug.Log("Already on Desktop!");
            return;
        }

        // Check if the new scene is a story game
        if (storyGames.Contains(sceneName))
        {
            // Loading screen into scene load
            StartCoroutine(LoadingScreenIntoSceneLoad(sceneName));
            return;
        }
        SceneManager.LoadSceneAsync(sceneName);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (this == null)
        {
            return;
        }

        // Invoke event
        OnNewSceneLoaded?.Invoke(scene.name);

        // When entering a story scene, enable loading end screen
        if (storyGames.Contains(scene.name))
        {
            // Fade out loading screen
            loadingScreenAnimator.SetTrigger("EndLoad");
        }
        else // When entering a non-story scene, disable loading end screen
        {
            loadingScreenAnimator.Play("StoryGameSplashScreenFadeOut", 0, 1);
            return;
        }
    }

    private IEnumerator LoadingScreenIntoSceneLoad(string sceneName)
    {
        loadingScreenAnimator.SetTrigger("StartLoad");

        yield return new WaitForEndOfFrame();

        float clipLength = loadingScreenAnimator.GetCurrentAnimatorStateInfo(0).length;

        // Wait for duration
        yield return new WaitForSeconds(clipLength);

        // Build name correctly
        string leaveOut = ",;.:-_\"\\";
        string temp = "";

        foreach (char c in sceneName)
        {
            if (!leaveOut.Contains(c))
            {
                temp += c;
            }
        }

        // Load scene async
        SceneManager.LoadSceneAsync(temp);
    }

    // List of story games, which are important and alter the loading screen
    readonly string[] storyGames =
    {
        "Game2-1",
        "Game2-2",
        "Game2-3",
        "Race For Freedom",
        "Isolate",
        "Race For Freedom",
        "Christmas",
        "\"Happy Birthday\""
    };
}