using System.Collections;
using UnityEngine;

public class WinGameFunctionality : MonoBehaviour
{
    public static event System.Action<string> OnWinGame;

    [SerializeField]
    private float timeUntilDesktop = 0.7f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            transform.GetChild(0).gameObject.SetActive(true);
            StartCoroutine(BackToDesktopIn(timeUntilDesktop));
        }
    }

    IEnumerator BackToDesktopIn(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        OnWinGame?.Invoke("Desktop");

        yield break;
    }
}