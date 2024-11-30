using System.Collections;
using UnityEngine;

public class EnableChildrenAfterDelay : MonoBehaviour
{
    [SerializeField] float delay = 1f;

    void Start()
    {
        StartCoroutine(EnableAfterDelay(delay));
    }

    IEnumerator EnableAfterDelay(float delay)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(delay);

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }

        yield break;
    }
}