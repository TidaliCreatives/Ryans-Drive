using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelControlScript : MonoBehaviour
{
    [SerializeField] CanvasGroup fade;
    [SerializeField] float timeToFade;
    [SerializeField] bool fadeOut; // Fade out = from clear to black screen
    [SerializeField] bool fadeIn; // Fade in = from black screen to clear
    [SerializeField] string nextLevel;

    private void Start()
    {
        // Set screen to black if trying to fade in
        if (fadeIn)
        {
            fade.alpha = 1;
        }
        else
        {
            fade.alpha = 0;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            fadeOut = true;

            Invoke(nameof(ChangeLevel), timeToFade);
        }
    }

    private void Update()
    {
        // This is code for a fade in / out system

        if (fadeOut)
        {
            fade.alpha += Time.deltaTime / timeToFade;

            if (fade.alpha >= 1f)
            {
                fadeOut = false;
            }
        }

        if (fadeIn)
        {
            fade.alpha -= Time.deltaTime / timeToFade;

            if (fade.alpha <= 0f)
            {
                fadeIn = false;
            }
        }
    }
    void ChangeLevel()
    {
        SceneManager.LoadScene(nextLevel);
    }
}