using UnityEngine;
using UnityEngine.UI;
public class SpeakerImageChange : MonoBehaviour
{
    public Sprite speakerOn;
    public Sprite speakerOn50;
    public Sprite speakerOff;
    public void ChangeImage(Slider slider)
    {
        if (slider.value > 50)
        {
            GetComponent<Image>().sprite = speakerOn;
        }
        else if (slider.value > 0)
        {
            GetComponent<Image>().sprite = speakerOn50;
        }
        else
        {
            GetComponent<Image>().sprite = speakerOff;
        }
    }
}