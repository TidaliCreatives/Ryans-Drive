using UnityEngine;

public enum DialogueSound
{
    Default = 0,
    Cultist,
    Mirror,
    Player,
    Writing
}

[System.Serializable]
public class DialogueData
{
    // Text content of the dialogue
    [TextArea(3, 10)]
    public string TextData = "...";

    // The speed at which the dialogue is displayed
    public float CharactersPerSecond = 7f;

    // The name of the character speaking
    public string SpeakerName = "";

    // Color of the name
    public Color SpeakerColor = Color.white;

    // The sound to play when the dialogue is displayed
    public DialogueSound dialogueSound = DialogueSound.Default;
}