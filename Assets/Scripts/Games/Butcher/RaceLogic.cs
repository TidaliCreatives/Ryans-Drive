using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class RaceLogic : MonoBehaviour
{
    public UnityEvent Event_AfterFadeOut;

    [Header("Parameters")]
    [SerializeField] float timeToFade; // How many seconds will the fade out take

    [Header("References")]
    public List<GameObject> environmentList;
    [SerializeField] List<GameObject> leftDialogueTriggerList;
    [SerializeField] List<GameObject> rightDialogueTriggerList;

    [SerializeField] Collider2D startCollider;  // Trigger for start line
    [SerializeField] Collider2D endCollider; // Trigger for finish line
    [SerializeField] GameObject winScreen;
    [SerializeField] GameObject rightIsEnabled;
    [SerializeField] GameObject leftIsEnabled;
    [SerializeField] GameObject noneIsEnabled;
    public CanvasGroup fade; // just a screen-size black image, used for fading out
    [SerializeField] TextMeshProUGUI timerText; // The timer at the top left of the screen
    [SerializeField] TextMeshProUGUI winScreenTimerText; // The timer that tells you how long you took on the win screen
    [SerializeField] ButcherPlayer playerScript;
    [SerializeField] AudioClip chopSound;
    [SerializeField] AudioClip errorSound;

    int wentToRight = 0;
    int wentToLeft = 0;
    int wentToNextLevel = 0;
    bool fadeOut;
    bool leftMirrorEnabled;
    bool rightMirrorEnabled;
    public int levelIndex; // 0 = level 1, 1 = level 2 etc
    Vector2 startPosition; // jank for reset to work

    float timer;
    Coroutine timerCoroutine;

    private void OnEnable()
    {
        // Subscribe to events
        DialogueSystem.OnDialogueEnded += DialogueFinished;
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        DialogueSystem.OnDialogueEnded -= DialogueFinished;
    }


    void Start()
    {
        startPosition = transform.position;
        Event_AfterFadeOut?.Invoke();
        leftMirrorEnabled = PlayerPrefs.GetInt("LeftMirrorActive", 0) == 1;
        rightMirrorEnabled = PlayerPrefs.GetInt("RightMirrorActive", 0) == 1;

        leftIsEnabled.SetActive(leftMirrorEnabled);
        rightIsEnabled.SetActive(rightMirrorEnabled);
        noneIsEnabled.SetActive(!leftMirrorEnabled && !rightMirrorEnabled);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (startCollider.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            if (timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);
            }

            timerCoroutine = StartCoroutine(RaceTimer());
        }

        if (endCollider.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            winScreenTimerText.text = "You took " + timer.ToString("F2") + " seconds!";

            if (timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);
            }
        }
    }

    void Update()
    {
        // Fading out
        if (fadeOut)
        {
            fade.alpha += 1 / timeToFade * Time.deltaTime;

            if (fade.alpha >= 1)
            {
                fadeOut = false;
            }
        }
    }

    IEnumerator RaceTimer()
    {
        while (true)
        {
            timer += Time.deltaTime;
            timerText.text = timer.ToString("F2");

            yield return new WaitForEndOfFrame();
        }
    }

    public void Restart()
    {
        if (levelIndex == 3)
        {
            PlayErrorSound();

            SceneManager.LoadScene("Desktop");

            return;
        }


        if (rightMirrorEnabled || leftMirrorEnabled)
        {
            if (wentToRight == 1)
            {
                playerScript.playerSpriteState += 4; // This adds to the % used for the step animation, basically moves it to the next sprite stage
                environmentList[levelIndex].SetActive(false);
            }

            leftDialogueTriggerList[levelIndex].SetActive(false);
            rightDialogueTriggerList[levelIndex].SetActive(false);

            levelIndex++;

            leftDialogueTriggerList[levelIndex].SetActive(true);
            rightDialogueTriggerList[levelIndex].SetActive(true);
        }

        fade.alpha = 0;

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }

        playerScript.stepCount = 1000;
        winScreen.SetActive(false);
        timer = 0;
        timerText.text = timer.ToString("F2");
        playerScript.canMove = true;
        SetPlayerStats();
    }

    public void DialogueFinished()
    {
        fadeOut = true;
        Invoke(nameof(SecretWinScreen), timeToFade + 0.5f);
        StartCoroutine(ResetCameraAfterDelay());
    }

    IEnumerator ResetCameraAfterDelay()
    {
        yield return new WaitForSeconds(timeToFade + 0.5f);

        transform.position = startPosition;
        Event_AfterFadeOut?.Invoke();

        yield break;
    }


    void SetPlayerStats()
    {
        if (wentToRight == 1)
        {
            wentToRight--;

            // This called when you hit reset, and will change players stats to the appropriate numbers for that level
            if (levelIndex == 1)
            {
                playerScript.moveSpeed = 0.75f;
                playerScript.spRenderer.sprite = playerScript.animations[5]; // This is so when you start the next level the sprite will update correctly

                return;
            }

            if (levelIndex == 2)
            {
                playerScript.moveDelay = 1;
                gameObject.transform.Translate(new(0, -0.7f, 0));
                playerScript.spRenderer.sprite = playerScript.animations[9];

                return;
            }

            if (levelIndex == 3)
            {
                playerScript.moveDelay = 3;
                gameObject.transform.Translate(new(0, -0.7f, 0));
                playerScript.spRenderer.sprite = playerScript.animations[13];

                return;
            }
        }
    }

    void SecretWinScreen()
    {
        if (gameObject.transform.position.x > 0)
        {
            if (rightMirrorEnabled)
            {
                PlayChopSound();
                wentToRight++;
            }
        }
        else
        {
            wentToLeft++;
        }
        wentToNextLevel++;

        if (wentToLeft == 1)
        {
            winScreenTimerText.text = "You chose wisdom!";
            wentToLeft--;
        }

        winScreen.SetActive(true);
    }

    void PlayChopSound()
    {
        GameObject gameObject = new("Chop One Shot");
        gameObject.transform.position = transform.position;
        AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
        audioSource.clip = chopSound;
        audioSource.spatialBlend = 0f;
        audioSource.volume = 1f;
        audioSource.Play();
        Destroy(gameObject, chopSound.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale));
    }

    void PlayErrorSound()
    {
        GameObject gameObject = new("Error One Shot");
        gameObject.transform.position = transform.position;
        AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
        audioSource.clip = errorSound;
        audioSource.spatialBlend = 0f;
        audioSource.volume = 1f;
        audioSource.Play();
        gameObject.transform.parent = null;
        DontDestroyOnLoad(gameObject);
        Destroy(gameObject, chopSound.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale));
    }
}