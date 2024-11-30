using System;
using UnityEngine;
using System.Collections.Generic;


public class DialogueTrigger : MonoBehaviour
{
    public static event Action<List<DialogueData>> OnDialogueTrigger;

    [SerializeField] bool triggerOnceOnly = true;
    [SerializeField] bool doFreezeTime = true;
    [SerializeField] List<DialogueData> dialogueData = new();

    
    void OnTriggerEnter2D(Collider2D collider)
    {
        // If the player enters the trigger collider
        if (collider.CompareTag("Player"))
        {
            OnDialogueTrigger?.Invoke(dialogueData);

            if (doFreezeTime)
            {
                RequestTogglePause(true);
            }

            if (triggerOnceOnly)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        // If the player enters the trigger collider
        if (collider.CompareTag("Player"))
        {
            OnDialogueTrigger?.Invoke(dialogueData);

            if (doFreezeTime)
            {
                RequestTogglePause(true);
            }

            if (triggerOnceOnly)
            {
                Destroy(gameObject);
            }
        }
    }

    void RequestTogglePause(bool doPause)
    {
        if (GameManag.Instance != null)
        {
            GameManag.Instance.TogglePause(doPause);
        }
        else
        {
            // Create Game Manager instance if it doesn't exist
            GameObject go = new("GameManag (Instantiated)");
            go.AddComponent<GameManag>();

        }
    }
}