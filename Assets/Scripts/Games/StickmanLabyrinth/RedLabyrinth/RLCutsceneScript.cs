using UnityEngine;

public class RLCutsceneScript : MonoBehaviour
{
    [SerializeField] float speed = 10.0f;
    [SerializeField] Vector3 target;

    [SerializeField] Animator animator;
    bool cutsceneControlPlayer;
    [SerializeField] StickmanSeasonTwoPlayer playerScript;
    [SerializeField] StickmanSeasonTwoPlayerAnimator playerAnimatorScript;

    DefaultInputActions inputActions;

    private void OnEnable()
    {
        DialogueSystem.OnDialogueEnded += DialogueSystem_OnDialogueEnded;

        // Enable input actions

        inputActions ??= new();
        inputActions.Player.Interact.Enable();
        inputActions.Player.Move.Enable();
    }



    private void OnDisable()
    {

        // Disable input actions
        inputActions.Player.Interact.Disable();
        inputActions.Player.Move.Disable();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("CutsceneTrigger"))
        {
            playerScript.inputActions.Player.Move.Disable();
            playerAnimatorScript.inputActions.Player.Move.Disable();
            playerAnimatorScript.moveRightAction.Disable();
            playerAnimatorScript.moveLeftAction.Disable();
            cutsceneControlPlayer = true;
            animator.SetBool("isWalking", true);

        }
    }

    void Update()
    {
        float step = speed * Time.deltaTime;
        if (cutsceneControlPlayer)
        {

            // move sprite towards the target location
            transform.position = Vector2.MoveTowards(transform.position, target, step);
        }

        if (transform.position == target)
        {
            // Debug.Log("AAAAAAAAAAAHHHHHHHHHHHH");
            animator.SetBool("isWalking", false);

        }
    }

    private void DialogueSystem_OnDialogueEnded()
    {
        cutsceneControlPlayer = false;
        playerScript.inputActions.Player.Move.Enable();
        playerAnimatorScript.inputActions.Player.Move.Enable();
        playerAnimatorScript.moveRightAction.Enable();
        playerAnimatorScript.moveLeftAction.Enable();
    }
}
