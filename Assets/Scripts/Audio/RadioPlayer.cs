using System.Collections.Generic;
using UnityEngine;

public class RadioPlayer : MonoBehaviour
{
    [SerializeField] List<AudioClip> clips;
    List<AudioClip> remainingClips;

    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (clips.Count > 0)
        {
            remainingClips = clips;

            if (remainingClips.Count > 0)
            {
                audioSource.clip = remainingClips[Random.Range(0, remainingClips.Count)];
                audioSource.Play();
                remainingClips.Remove(audioSource.clip);
            }
        }
    }

    private void Update()
    {
        if (!audioSource.isPlaying && clips.Count > 0)
        {
            if (remainingClips.Count == 0)
            {
                remainingClips = clips;
            }
            audioSource.clip = remainingClips[Random.Range(0, remainingClips.Count)];
            audioSource.Play();
            remainingClips.Remove(audioSource.clip);
        }
    }
}