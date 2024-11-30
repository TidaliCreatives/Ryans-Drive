using UnityEngine;
using UnityEngine.InputSystem;

public class BasicFirstPersonMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 4f;
    [SerializeField] Transform lookAt;

    DefaultInputActions inputActions;

    Vector2 moveInput;
    CharacterController controller;

    private void OnEnable()
    {
        inputActions ??= new();
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += UpdateMoveInput;
        inputActions.Player.Move.canceled += UpdateMoveInput;

    }
    private void OnDisable()
    {
        inputActions.Player.Move.performed -= UpdateMoveInput;
        inputActions.Player.Move.canceled -= UpdateMoveInput;
        inputActions.Player.Disable();
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        // Rotate towards look at
        Vector3 lookAtPos = lookAt.position;
        lookAtPos.y = transform.position.y;
        transform.LookAt(lookAtPos);

        // Move the player
        Vector3 move = transform.forward * moveInput.y + transform.right * moveInput.x;
        controller.Move(Time.fixedDeltaTime * moveSpeed * move);
    }

    private void UpdateMoveInput(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void Update()
    {
        if (inputActions.Player.Move.ReadValue<Vector2>() != null)
        {
            // I'm walking
        }
    }
}