using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DialogueSystem : MonoBehaviour
{
    public static event Action<bool> OnToggleDialogue;
    public static event Action OnDialogueEnded;

    [Header("Parameters")]
    [SerializeField] bool waitsForInput = true; // To determine when to wait for player input
    [SerializeField] bool doShowContinuePrompt = true; // To determine whether to show the continue prompt
    [Range(0f, 1f), SerializeField] float maxRandomPauseDelay = 0.1f;
    [Space]

    [Header("References")]
    [SerializeField] GameObject dialogueBox; // Reference to the dialogue box
    [SerializeField] TextMeshProUGUI dialogueText; // Reference to the text component of the dialogue box
    [SerializeField] AudioListPlayer audioListPlayer; // Reference to the audio list player

    // Private parameters
    DefaultInputActions inputActions;
    Animator animator;
    Coroutine ExecuteDialogueCoroutine;
    bool inputReceived = false;
    float currentWaitTimer = 0f;
    int talkInterval = 1;

    string dontTalkSymbols = ".:,;#-_!?()'";

    void OnEnable()
    {
        // Subscribe to events
        SceneManager.sceneLoaded += OnSceneLoaded;
        DialogueTrigger.OnDialogueTrigger += ReceiveDialogue;

        // Enable input actions
        inputActions ??= new();
        inputActions.Player.Interact.Enable();
        inputActions.Player.Interact.started += UpdateInputReceived;
    }

    void OnDisable()
    {
        // Unsubscribe from events
        SceneManager.sceneLoaded -= OnSceneLoaded;
        DialogueTrigger.OnDialogueTrigger -= ReceiveDialogue;

        // Disable input actions
        inputActions.Player.Interact.started -= UpdateInputReceived;
        inputActions.Player.Interact.Disable();
    }


    private void Start()
    {
        if (GameObject.FindGameObjectsWithTag("DialogueSystem").Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.parent = null;

            DontDestroyOnLoad(gameObject);
        }

        dialogueBox.SetActive(false);
        animator = GetComponent<Animator>();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (animator != null)
        {
            animator.enabled = false;
        }
        dialogueBox.SetActive(false);
        dialogueText.text = "";
        if (ExecuteDialogueCoroutine != null)
        {
            StopCoroutine(ExecuteDialogueCoroutine);
        }
    }

    void UpdateInputReceived(InputAction.CallbackContext ctx)
    {
        inputReceived = true;
    }

    // Receive dialogue data from DialogueTrigger event
    void ReceiveDialogue(List<DialogueData> dialogueData)
    {
        if (ExecuteDialogueCoroutine != null)
        {
            StopCoroutine(ExecuteDialogueCoroutine);
        }
        ExecuteDialogueCoroutine = StartCoroutine(ExecuteDialogue(dialogueData));
    }

    // Coroutine to execute dialogue based on dialogue data received from DialogueTrigger event
    IEnumerator ExecuteDialogue(List<DialogueData> dialogueData)
    {
        // Enable dialogue box
        animator.enabled = true;
        animator.SetTrigger("Start");

        // Set dialog text
        dialogueText.text = "... (Press E)";
        
        // Disable movement etc
        OnToggleDialogue?.Invoke(true);

        // Wait for player input to continue to first dialogue line
        while (waitsForInput)
        {
            // If player presses interact, continue to next dialogue line
            if (inputReceived)
            {
                inputReceived = false;
                waitsForInput = false;
                break;
            }

            // Wait for next frame
            yield return new WaitForEndOfFrame();
        }

        foreach (DialogueData data in dialogueData)
        {
            // Check type
            if (data.SpeakerName.Contains("Cultist"))
            {
                data.dialogueSound = DialogueSound.Cultist;
            }
            else if (data.SpeakerName.Contains("Mirror"))
            {
                data.dialogueSound = DialogueSound.Mirror;
            }
            else if (data.SpeakerName.Contains("Player"))
            {
                data.dialogueSound = DialogueSound.Player;
            }
            else if (data.SpeakerName.Contains("Writing"))
            {
                data.dialogueSound = DialogueSound.Writing;
            }

            // Clear dialogue text
            dialogueText.text = "";

            if (data.SpeakerName != string.Empty)
            {
                string hexColor = ColorUtility.ToHtmlStringRGB(data.SpeakerColor);
                dialogueText.text = $"<b><color=#{hexColor}>{data.SpeakerName}</color></b>\n";
            }

            // Write dialogue text letter by letter at a rate of data.charactersPerSecond times per second
            for (int i = 0; i < data.TextData.Length; i++)
            {
                char letter = data.TextData[i];

                // Skip in-string formatting such as <i>, <b>, <color>, etc.
                while (letter == '<')
                {
                    // Write '<'
                    dialogueText.text += letter;

                    while (!letter.Equals('>')) // While current letter is not '>'
                    {
                        i++; // Go to next letter
                        letter = data.TextData[i]; // Get next letter
                        dialogueText.text += letter; // Add letter to dialogue text
                    }

                    // Go to next letter
                    i++;
                    if (i < data.TextData.Length)
                    {
                        letter = data.TextData[i];
                    }
                    else
                    {
                        break;
                    }
                }

                if (inputReceived || i >= data.TextData.Length)
                {
                    inputReceived = false;
                    
                    if (data.SpeakerName != string.Empty)
                    {
                        string hexColor = ColorUtility.ToHtmlStringRGB(data.SpeakerColor);
                        dialogueText.text = $"<b><color=#{hexColor}>{data.SpeakerName}</color></b>\n";
                        dialogueText.text += data.TextData;
                    }
                    else
                    {
                        dialogueText.text = data.TextData;
                    }
                    break;
                }

                // Add letter to dialogue text
                dialogueText.text += letter;

                // Update talk interval based on dialogue sound
                talkInterval = data.dialogueSound switch
                {
                    DialogueSound.Cultist => 5,
                    DialogueSound.Mirror => 3,
                    DialogueSound.Player => 3,
                    DialogueSound.Writing => 1,
                    _ => 3
                };

                // Play audio at intervals and when letter is not a symbol
                if (dialogueText.text.Length % talkInterval == 0 && !dontTalkSymbols.Contains(letter))
                {
                    if (dialogueText.text.Contains("<i>") && !dialogueText.text.Contains("</i>")) // If currently typing out italics, play different audio
                    {
                        audioListPlayer.PlayRandomAudio(data, true);
                    }
                    else
                    {
                        audioListPlayer.PlayRandomAudio(data, false);
                    }
                }

                // Wait for next letter to be typed
                currentWaitTimer = (1f / data.CharactersPerSecond) + UnityEngine.Random.Range(0f, maxRandomPauseDelay);
                if (letter == '.' || letter == '!' || letter == '?' || letter == ';')
                {
                    currentWaitTimer += 0.3f;
                }
                else if (letter == ',' || letter == '-')
                {
                    currentWaitTimer += 0.1f;
                }

                while (currentWaitTimer > 0f)
                {
                    if (inputReceived)
                    {
                        if (data.SpeakerName != string.Empty)
                        {
                            string hexColor = ColorUtility.ToHtmlStringRGB(data.SpeakerColor);
                            dialogueText.text = $"<b><color=#{hexColor}>{data.SpeakerName}</color></b>\n";
                            dialogueText.text += data.TextData;
                        }
                        else
                        {
                            dialogueText.text = data.TextData;
                        }
                        break;
                    }
                    currentWaitTimer -= Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
            }

            // Wait for player input, after one dialogue line is fully typed
            waitsForInput = true;

            while (waitsForInput)
            {
                // If player presses interact, continue to next dialogue line
                if (inputReceived)
                {
                    inputReceived = false;
                    waitsForInput = false;
                    break;
                }

                // Wait for next frame
                yield return new WaitForEndOfFrame();
            }
        }

        if (doShowContinuePrompt)
        {
            dialogueText.text = "";

            string hexColorGray = ColorUtility.ToHtmlStringRGB(Color.gray);
            string endMessage = $"<b><color=#{hexColorGray}>Continue?</color></b>";

            // Type out end message
            for (int i = 0; i < endMessage.Length; i++)
            {
                // Load letter
                char letter = endMessage[i];

                // Skip in-string formatting such as <i>, <b>, <color>, etc.
                while (letter == '<')
                {
                    // Write '<'
                    dialogueText.text += letter;

                    while (!letter.Equals('>')) // While current letter is not '>'
                    {
                        i++; // Go to next letter
                        letter = endMessage[i]; // Get next letter
                        dialogueText.text += letter; // Add letter to dialogue text
                    }

                    // Go to next letter
                    i++;
                    if (i < endMessage.Length)
                    {
                        letter = endMessage[i];
                    }
                    else
                    {
                        break;
                    }
                }

                if (inputReceived || i >= endMessage.Length)
                {
                    inputReceived = false;

                    break;
                }

                // Add letter to dialogue text
                dialogueText.text += letter;

                // Wait for next letter to be typed
                currentWaitTimer = 0.02f;

                while (currentWaitTimer > 0f)
                {
                    if (inputReceived)
                    {
                        dialogueText.text = endMessage;
                        break;
                    }
                    currentWaitTimer -= Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }

                if (inputReceived)
                {
                    inputReceived = false;
                    dialogueText.text = endMessage;
                    break;
                }
            }

            // Wait for player input, after one dialogue line is fully typed
            waitsForInput = true;

            while (waitsForInput)
            {
                // If player presses interact, continue to next dialogue line
                if (inputReceived)
                {
                    inputReceived = false;
                    waitsForInput = false;
                    break;
                }

                // Wait for next frame
                yield return new WaitForEndOfFrame();
            }
        }

        // Close dialogue box
        animator.SetTrigger("Close");

        // Resume time
        OnToggleDialogue?.Invoke(false);

        // Invoke dialogue ended event
        OnDialogueEnded?.Invoke();

        yield break;
    }
}