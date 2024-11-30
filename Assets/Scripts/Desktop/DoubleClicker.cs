using UnityEngine;
using UnityEngine.Events;
public class DoubleClicker : MonoBehaviour
{
    private float timeBetweenClicks = 0.5f; // The maximum time between clicks to be considered a double click

    private float timeSinceLastClick = 0f; // The time since the last click

    private bool hasBeenClicked = false; // Whether the object has been clicked in the last timeBetweenClicks seconds

    public UnityEvent OnDoubleClick; // The event to be invoked when a double click is detected

    private void Update()
    {
        timeSinceLastClick += Time.deltaTime;

        if (timeSinceLastClick >= timeBetweenClicks)
        {
            hasBeenClicked = false;
        }
    }

    // This method is called when the object is clicked
    public void OnClick()
    {
        if (hasBeenClicked)
        {
            // Invoke the event
            OnDoubleClick?.Invoke();
            hasBeenClicked = false;
        }
        else
        {
            hasBeenClicked = true;
            timeSinceLastClick = 0f;
        }
    }
}