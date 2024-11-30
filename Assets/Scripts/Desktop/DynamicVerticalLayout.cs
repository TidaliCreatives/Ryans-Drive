using UnityEngine;
using UnityEngine.UI;

public class DynamicVerticalLayout : MonoBehaviour
{
    void FixedUpdate()
    {
        VerticalLayoutGroup layoutGroup = GetComponent<VerticalLayoutGroup>();
        RectTransform contentRect = GetComponent<RectTransform>();

        float totalHeight = 0f;
        foreach (RectTransform child in contentRect)
        {
            totalHeight += child.sizeDelta.y;
        }

        // Add spacing between elements
        totalHeight += layoutGroup.spacing * (contentRect.childCount - 1);

        // Add padding
        totalHeight += layoutGroup.padding.top + layoutGroup.padding.bottom;

        // Set the height of the Content object
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, totalHeight);
    }
}
