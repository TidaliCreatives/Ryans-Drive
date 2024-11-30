using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class StickmanSeasonTwoPlayerAnimator : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] List<AudioClip> stepClips = new();

    public InputAction moveRightAction; // When pressing input button (D)
    public InputAction moveLeftAction; // When pressing input button (A)
    public DefaultInputActions inputActions;

    bool isWalking = false;
    float stepTimer = 0f;
    private void OnEnable()
    {
        // Subscribe to events
        StickmanSeasonTwoPlayer.PlayerMovement += UpdateIsMoving;

        // Enable input actions
        moveRightAction = new InputAction(binding: "<Keyboard>/D");
        moveRightAction.Enable();
        moveLeftAction = new InputAction(binding: "<Keyboard>/A");
        moveLeftAction.Enable();
        inputActions ??= new();
        inputActions.Player.Interact.Enable();
        inputActions.Player.Move.Enable();
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        StickmanSeasonTwoPlayer.PlayerMovement -= UpdateIsMoving;

        // Disable input actions
        moveRightAction.Disable();
        moveLeftAction.Disable();
        inputActions.Player.Interact.Disable();
        inputActions.Player.Move.Disable();
    }

    private void Update()
    {
        if (moveRightAction.triggered) 
        {
            gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        if (moveLeftAction.triggered) 
        {
            gameObject.transform.eulerAngles = new Vector3(0, 180, 0);
        }

        if (inputActions.Player.Move.IsPressed()) 
        {
            animator.SetBool("isWalking", true);
        }
        else if (!inputActions.Player.Move.IsPressed() && inputActions.Player.Move.enabled)
        {
            animator.SetBool("isWalking", false);
        }

        if (isWalking)
        {
            stepTimer += Time.deltaTime;

            if (stepTimer >= 0.341f)
            {
                // Reset timer
                stepTimer -= 0.341f;

                // Create oneshot audio
                AudioClip clip = stepClips[Random.Range(0, stepClips.Count)];
                GameObject gameObject = new("One shot audio");
                gameObject.transform.position = transform.position;
                AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
                audioSource.clip = clip;
                audioSource.spatialBlend = 0f;
                audioSource.volume = 0.4f;
                audioSource.Play();
                Destroy(gameObject, clip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale) + 0.2f);
            }
        }
    }

    void UpdateIsMoving(Vector2 moveDirection)
    {
        if (moveDirection != Vector2.zero)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }
}