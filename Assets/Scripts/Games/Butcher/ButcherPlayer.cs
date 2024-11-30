using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ButcherPlayer : MonoBehaviour
{
    [Header("Parameters")]
    public float moveSpeed;
    public float moveDelay; // How many times should the player press before actually moving

    [Header("References")]
    public SpriteRenderer spRenderer;
    public List<Sprite> animations; // List of sprites to animate
    [SerializeField] CanvasGroup level3Message;
    [SerializeField] Collider2D playerCollider;
    [SerializeField] RaceLogic raceLogic;

    public int stepCount = 1000; // To keep track of the current frame, can't go into negatives so start at 1000
    InputAction moveRightAction; // When pressing input button (D)
    public InputAction moveLeftAction; // When pressing input button (A)
    DefaultInputActions inputActions;
    public bool canMove = true;
    public int playerSpriteState; // Each increase of 4 sets the player sprite to it's next state

    public bool leftMirrorEnabled = false;
    public bool rightMirrorEnabled = false;

    private void OnEnable()
    {
        // Subscribe to events
        DialogueSystem.OnToggleDialogue += ToggleEnableMoving;

        // Enable input actions
        moveRightAction = new InputAction(binding: "<Keyboard>/D");
        moveRightAction.Enable();
        moveLeftAction = new InputAction(binding: "<Keyboard>/A");
        moveLeftAction.Enable();
        inputActions ??= new();
        inputActions.Player.Interact.Enable();

    }

    private void OnDisable()
    {
        // Unsubscribe from events
        DialogueSystem.OnToggleDialogue -= ToggleEnableMoving;

        // Disable input actions
        moveRightAction.Disable();
        moveLeftAction.Disable();
        inputActions.Player.Interact.Disable();
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("LeftMirrorActive", 0) == 1) { leftMirrorEnabled = true; }
        if (PlayerPrefs.GetInt("RightMirrorActive", 0) == 1) { rightMirrorEnabled = true; }
    }

    private void Update()
    {
        if (canMove)
        {
            canMove = raceLogic.fade.alpha == 0;
        }
        if (moveRightAction.triggered && canMove)
        {
            stepCount++;

            spRenderer.sprite = animations[(stepCount % 4) + playerSpriteState];

            if (stepCount % (1 + moveDelay) == 0)
            {
                transform.Translate(new(moveSpeed, 0, 0));
            }
        }

        if (moveLeftAction.triggered && canMove && leftMirrorEnabled && raceLogic.levelIndex != 3)
        {
            stepCount--;

            spRenderer.sprite = animations[(stepCount % 4) + playerSpriteState];

            if (stepCount % (1 + moveDelay) == 0)
            {
                transform.Translate(new(-moveSpeed, 0, 0));
            }
        }

        if (moveLeftAction.triggered && raceLogic.levelIndex == 3) { level3Message.alpha = 1; }

        level3Message.alpha -= 0.01f;
    }

    void ToggleEnableMoving(bool isInDialog)
    {
        canMove = !isInDialog;
    }
}