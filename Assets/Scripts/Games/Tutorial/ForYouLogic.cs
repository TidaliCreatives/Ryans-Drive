using UnityEngine;
using UnityEngine.SceneManagement;

public class ForYouLogic : MonoBehaviour
{

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
   
    void DialogueFinished()
    {
        SceneManager.LoadScene("Desktop");

        return;
    }
}
