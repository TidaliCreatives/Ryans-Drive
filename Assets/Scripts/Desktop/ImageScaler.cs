using UnityEngine;
using UnityEngine.UI;

public class ImageScaler : MonoBehaviour
{
    Image image;
    public float fixedHeight = 1030f;

    public void UpdateSprite(Sprite sprite, float fixedHeight, float maxWidth)
    {
        // Get the Image component
        Image image = GetComponent<Image>();

        // Set the new sprite
        image.sprite = sprite;

        // Ensure the image scales with its source
        image.type = Image.Type.Simple;

        // Adjust the RectTransform to maintain aspect ratio with a fixed height
        RectTransform rectTransform = image.GetComponent<RectTransform>();
        float aspectRatio = sprite.bounds.size.x / sprite.bounds.size.y;
        float newWidth = fixedHeight * aspectRatio;

        // Check if the new width exceeds the maximum width
        if (newWidth > maxWidth)
        {
            newWidth = maxWidth;
            fixedHeight = newWidth / aspectRatio;
        }

        rectTransform.sizeDelta = new Vector2(newWidth, fixedHeight);
    }
}