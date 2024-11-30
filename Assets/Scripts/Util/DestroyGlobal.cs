using UnityEngine;

public class DestroyGlobal : MonoBehaviour
{
    public GameObject toDestroy;

    private void Start()
    {
        // If the object to destroy is not set, set this object to be destroyed
        if (toDestroy == null)
        {
            toDestroy = gameObject;
        }
    }

    public void DestroyAfterDelay(float delay)
    {
        Destroy(toDestroy, delay);
    }
}