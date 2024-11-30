using UnityEngine;

public class StickmanSeasonOne : MonoBehaviour
{
    [SerializeField]
    float moveSpeed;
    [SerializeField]
    bool useDumbMove = false;
    Rigidbody2D rb;
    Vector2 moveDirection;

    DefaultInputActions inputActions;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        inputActions ??= new();
        inputActions.Player.DumbMove.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.DumbMove.Disable();
    }

    public void FixedUpdate()
    {
        // Check input
        moveDirection = inputActions.Player.DumbMove.ReadValue<Vector2>();
        if (!useDumbMove) { moveDirection = moveDirection.normalized; }

        // Move the rigidbody
        rb.MovePosition((Vector2)transform.position + (Time.fixedDeltaTime * moveSpeed * moveDirection));
    }
}