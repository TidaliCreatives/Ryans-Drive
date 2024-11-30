using System;
using System.Collections.Generic;
using UnityEngine;

public class StickmanSeasonTwoPlayer : MonoBehaviour
{
    public static event Action<Vector2> PlayerMovement;
    [SerializeField] float moveSpeed = 3;

    Rigidbody2D rb;
    Vector2 moveDirection;
    bool canMove = true;

    public DefaultInputActions inputActions;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        DialogueTrigger.OnDialogueTrigger += FreezePlayer;
        DialogueSystem.OnDialogueEnded += UnfreezePlayer;

        inputActions ??= new();
        inputActions.Player.Move.Enable();
    }

    private void OnDisable()
    {
        DialogueTrigger.OnDialogueTrigger -= FreezePlayer;
        DialogueSystem.OnDialogueEnded -= UnfreezePlayer;

        inputActions.Player.Move.Disable();
    }

    public void FixedUpdate()
    {
        if (canMove)
        {
            // Check input
            moveDirection = inputActions.Player.Move.ReadValue<Vector2>();

            // Move the rigidbody
            rb.MovePosition((Vector2)transform.position + (Time.fixedDeltaTime * moveSpeed * moveDirection));
        }
        else
        {
            moveDirection = Vector2.zero;
        }

        PlayerMovement?.Invoke(moveDirection);
    }

    void FreezePlayer(List<DialogueData> list)
    {
        canMove = false;
    }

    void UnfreezePlayer()
    {
        canMove = true;
    }
}