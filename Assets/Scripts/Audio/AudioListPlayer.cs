using System.Collections.Generic;
using UnityEngine;

public class AudioListPlayer : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] float volume = 1f;
    [SerializeField] float pitchDif = 0.2f;
    [Space]
    [Header("Audio Clips")]
    [SerializeField] List<AudioClip> defaultClips = new();
    [SerializeField] List<AudioClip> cultistClips = new();
    [SerializeField] List<AudioClip> mirrorClips = new();
    [SerializeField] List<AudioClip> playerClips = new();
    [SerializeField] List<AudioClip> writingClips = new();



    public void PlayRandomAudio(DialogueData data, bool isTypingHint)
    {
        // Higher chance to not say something, if the typing speed is higher
        if (Random.Range(0f, 1f) < (data.CharactersPerSecond - 32f) / 64f)
        {
            return;
        }

        AudioClip clip = data.dialogueSound switch
        {
            DialogueSound.Cultist => cultistClips.Count > 0 ?   // If dialogueSound is Cultist
            cultistClips[Random.Range(0, cultistClips.Count)] : // Play a random cultist clip
            defaultClips[Random.Range(0, defaultClips.Count)],  // If cultistClips is empty, play a random default clip

            DialogueSound.Mirror => mirrorClips.Count > 0 ?     // If dialogueSound is Mirror
            mirrorClips[Random.Range(0, mirrorClips.Count)] :   // Play a random mirror clip
            defaultClips[Random.Range(0, defaultClips.Count)],  // If mirrorClips is empty, play a random default clip

            DialogueSound.Player => playerClips.Count > 0 ?     // If dialogueSound is Player
            playerClips[Random.Range(0, playerClips.Count)] :   // Play a random player clip
            defaultClips[Random.Range(0, defaultClips.Count)],  // If playerClips is empty, play a random default clip

            DialogueSound.Writing => writingClips.Count > 0 ?   // If dialogueSound is Writing
            writingClips[Random.Range(0, writingClips.Count)] : // Play a random writing clip
            defaultClips[Random.Range(0, defaultClips.Count)],  // If writingClips is empty, play a random default clip

            _ => defaultClips[Random.Range(0, defaultClips.Count)] // In any other case
        };

        // Create a new audio source to play the clip
        GameObject gameObject = new("One shot audio");
        gameObject.transform.position = transform.position;
        AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
        audioSource.clip = clip;
        audioSource.spatialBlend = 0f;
        if (isTypingHint)
        {
            audioSource.pitch = 0.234f;
        }
        else
        {
            audioSource.pitch = Random.Range(1f - pitchDif, 1f + pitchDif) + (data.CharactersPerSecond - 32f) / 48f;
        }
        audioSource.volume = data.dialogueSound switch
        {
            DialogueSound.Cultist => 0.2f,
            DialogueSound.Mirror => 1f,
            DialogueSound.Player => 1f,
            DialogueSound.Writing => 0.1f,
            _ => volume
        };
        audioSource.Play();
        Destroy(gameObject, clip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale) + 0.2f);

        return;
    }
}