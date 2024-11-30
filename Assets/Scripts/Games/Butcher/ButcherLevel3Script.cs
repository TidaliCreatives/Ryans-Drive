using UnityEngine;

public class ButcherLevel3Script : MonoBehaviour
{

    // This script is attached to 3 triggers that take effect on level 3 of freedom race

    [SerializeField] ButcherPlayer playerScript;
    [SerializeField] RaceLogic raceLogic;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SetPlayer();
        }
    }

    void SetPlayer()
    {

        if (raceLogic.levelIndex == 3 && playerScript.playerSpriteState == 8)
        {
            playerScript.playerSpriteState += 4; // This adds to the % used for the step animation, basically moves it to the next sprite stage

            raceLogic.environmentList[2].SetActive(false);
            playerScript.moveDelay = 3;
            gameObject.transform.Translate(new(0, -.7f, 0));
            playerScript.spRenderer.sprite = playerScript.animations[13];
        }

        if (raceLogic.levelIndex == 3 && playerScript.playerSpriteState == 4)
        {
            playerScript.playerSpriteState += 4; // This adds to the % used for the step animation, basically moves it to the next sprite stage

            raceLogic.environmentList[1].SetActive(false);
            playerScript.moveDelay = 1;
            gameObject.transform.Translate(new(0, -.7f, 0));
            playerScript.spRenderer.sprite = playerScript.animations[9];


        }

        if (raceLogic.levelIndex == 3 && playerScript.playerSpriteState == 0)
        {
            playerScript.playerSpriteState += 4; // This adds to the % used for the step animation, basically moves it to the next sprite stage

            raceLogic.environmentList[0].SetActive(false);
            playerScript.moveSpeed = 0.75f;
            playerScript.spRenderer.sprite = playerScript.animations[5];// This is so when you start the next level the sprite will update correctly
        }

    }
}