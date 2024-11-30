using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TDLEventTrigger : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] bool oneTimeTrigger = true;
    [SerializeField] string targetTag = "Player";
    [SerializeField] float delay = 0.0f;

    public UnityEvent Event_Trigger;

    Coroutine coroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            // Trigger event
            Event_Trigger?.Invoke();

            // Destroy this object, if one time trigger
            if (oneTimeTrigger)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            if (coroutine != null)
            {
                // Do nothing
                return;
            }

            coroutine = StartCoroutine(Execute());

        }
    }

    IEnumerator Execute()
    {
        // Wait for delay
        yield return new WaitForSeconds(delay);

        // Trigger event
        Event_Trigger?.Invoke();

        coroutine = null;

        // Destroy this object, if one time trigger
        if (oneTimeTrigger)
        {
            Destroy(gameObject);
        }

        yield break;
    }
}