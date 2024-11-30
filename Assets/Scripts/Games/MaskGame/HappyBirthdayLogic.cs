using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/// <summary>
/// All the logic to make the happy birthday game work, including the background guessing and the sign guessing,
/// as well as the secret, which is found by guessing the background enough times in a row.
/// </summary>

public class HappyBirthdayLogic : MonoBehaviour
{
    [Header("Parameters")]
    readonly float pitchLoss = 0.1f; // How much the pitch should be lowered for each consecutive guess (after 2)
    [SerializeField] List<Color> colors = new(); // The colors of the backgrounds
    readonly int neededConsecutiveGuesses = 11; // How many consecutive guesses are needed to find the secret
    readonly float maxWaitTimeForNewSign = 7f; // The maximum time to wait for a new sign
    [Space]

    [Header("References")]
    [SerializeField] AudioClip cheer; // The sound effect to play when the player answers correctly
    [SerializeField] AudioClip boo; // The sound effect to play when the player answers not (correctly)
    [SerializeField] AudioClip iDontObey; // The sound effect to play when the player sees the secret code
    [SerializeField] AudioClip lightsOff; // The sound effect to play when the player sees the secret code
    [SerializeField] CanvasGroup canvasGroup; // For blending to black by changing alpha
    [SerializeField] Image backgroundImage; // The background image to change the color
    [SerializeField] Animator signAnimator; // The animator of the sign
    [SerializeField] Animator secretAnimator; // The animator of the secret
    [SerializeField] List<GameObject> DeactivateOnFoundSecret; // The objects to deactivate when the secret is found

    int consecutiveBackgroundGuesses = 0; // How many times the player has guessed the background in a row
    int currentBackground; // The current background (0 = green/happy, 1 = grey/neutral, 2 = blue/sad)
    int currentSignMood = 0; // The current sign mood (0 = happy, 1 = neutral, 2 = sad)
    float timeInScene = 0f; // How long the player has been in the scene
    float timeCorrectBackgroundChoosing = 0f;
    float timeCorrectSignChoosing = 0f;
    float lastSignTime = 0f; // The time after the application was started, when the last sign was shown
    float waitTime = 2f; // How long to wait until the next sign is shown
    bool canBackgroundGuess = false; // If the player has guessed this background
    bool foundSecret = false; // If the player has found the secret
    bool setLastSignTimeFinal = false; // If the last sign time has been set to the final time
    bool hasAnswered = false; // If the player has answered the current sign
    bool musicPlaying = true; // If the music is playing

    private AudioSource audioSource; // The audio source of the music

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        timeInScene = 0f;
        lastSignTime = Time.time;
    }

    private void Update()
    {
        if (foundSecret)
        {
            if (!setLastSignTimeFinal)
            {
                lastSignTime = Time.time;
                setLastSignTimeFinal = true;
                StartCoroutine(SecretReveal());
            }

            if (musicPlaying)
            {
                // Fade out music
                if (audioSource.volume > 0f)
                {
                    audioSource.volume -= Time.deltaTime / 2f;
                }
                else
                {
                    audioSource.volume = 0f;
                    musicPlaying = false;
                }
            }

            return;
        }

        timeInScene += Time.deltaTime;

        timeCorrectSignChoosing += Time.deltaTime;

        timeCorrectBackgroundChoosing += Time.deltaTime / (consecutiveBackgroundGuesses / 3f + 1);

        if (timeCorrectBackgroundChoosing > 60f / 35f)
        {
            timeCorrectBackgroundChoosing -= 60f / 35f;

            ChangeBackground();

            canBackgroundGuess = true;
        }

        if (Time.time > lastSignTime + waitTime)
        {
            // Update the last sign time and wait time
            lastSignTime = Time.time;
            waitTime = 3f;
            waitTime = Mathf.Clamp(waitTime - timeCorrectSignChoosing / maxWaitTimeForNewSign, 0.1f, waitTime);

            // Show a new sign
            ShowNewSign();

            if (timeInScene > 2f && !hasAnswered)
            {
                PlaySound(boo);
                timeCorrectSignChoosing = 0f;
            }

            // Make it able to guess again
            hasAnswered = false;
        }
    }

    public void CheckAnswer(int answer)
    {
        if (canBackgroundGuess)
        {
            if (answer == currentBackground)
            {
                consecutiveBackgroundGuesses++;

                if (consecutiveBackgroundGuesses > 2)
                {
                    audioSource.pitch = 1f - pitchLoss * (consecutiveBackgroundGuesses - 2);
                }

                if (consecutiveBackgroundGuesses >= neededConsecutiveGuesses)
                {
                    foundSecret = true;
                    foreach (GameObject go in DeactivateOnFoundSecret)
                    {
                        go.SetActive(false);
                    }
                }
            }
            else
            {
                audioSource.pitch = 1f;
                consecutiveBackgroundGuesses = 0;
            }

            canBackgroundGuess = false;
        }
            
        if (!hasAnswered && answer == currentSignMood || answer == -1)
        {
            // Play cheer sound
            PlaySound(cheer);

            hasAnswered = true;

            return;
        }
        else if (!hasAnswered || answer == -2)
        {
            // Play boo sound
            PlaySound(boo);

            // Reset the time for correct sign choosing
            timeCorrectSignChoosing = 0f;

            // 
            hasAnswered = true;
        }
    }

    void ChangeBackground()
    {
        // Find a new answer, which is not the current answer
        int newBackground;
        do { newBackground = Random.Range(0, 3); } while (newBackground == currentBackground);
        currentBackground = newBackground;

        backgroundImage.color = colors[currentBackground];
    }

    void ShowNewSign()
    {
        bool isLeft = Random.Range(0, 2) == 0;
        signAnimator.SetBool("isLeft", isLeft);

        currentSignMood = Random.Range(0, 3);
        switch (currentSignMood)
        {
            case 0:
                signAnimator.SetTrigger("Happy");
                break;
            case 1:
                signAnimator.SetTrigger("Neutral");
                break;
            case 2:
                signAnimator.SetTrigger("Sad");
                break;
        }
    }

    IEnumerator SecretReveal()
    {
        PlaySound(lightsOff);

        secretAnimator.SetTrigger("Trigger");

        yield return new WaitForSeconds(2f);

        PlaySound(iDontObey);

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("Desktop");

        yield break;
    }

    void PlaySound(AudioClip clip)
    {
        // Create a new audio source to play the clip
        GameObject gameObject = new("One shot audio");
        gameObject.transform.position = transform.position;
        AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
        audioSource.clip = clip;
        audioSource.spatialBlend = 0f;
        audioSource.Play();
        Destroy(gameObject, clip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale) + 0.2f);
    }
}