using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
using UnityEngine.UI;
using Unity.VisualScripting;
using TMPro;

public class DetectButtonClickKind : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] float doubleClickTime = 0.5f;
    [SerializeField] bool doubleLeft = false;
    [SerializeField] bool doubleRight = false;
    [SerializeField] bool doubleMiddle = false;

    private float lastClickTimeLeft;
    private float lastClickTimeMiddle;
    private float lastClickTimeRight;

    public UnityEvent LeftClick;
    public UnityEvent MiddleClick;
    public UnityEvent RightClick;

    private void OnEnable()
    {
        LeftClick ??= new UnityEvent();
        MiddleClick ??= new UnityEvent();
        RightClick ??= new UnityEvent();

        lastClickTimeLeft = doubleClickTime;
        lastClickTimeMiddle = doubleClickTime;
        lastClickTimeRight = doubleClickTime;
    }

    void Update()
    {
        lastClickTimeLeft += Time.deltaTime;
        lastClickTimeMiddle += Time.deltaTime;
        lastClickTimeRight += Time.deltaTime;
    }

    public void Init(DetectButtonClickKind init)
    {
        //doubleClickTime = init.doubleClickTime;

        //doubleLeft = init.doubleLeft;
        //doubleRight = init.doubleRight;
        //doubleMiddle = init.doubleMiddle;

        //LeftClick = init.LeftClick;
        //MiddleClick = init.MiddleClick;
        //RightClick = init.RightClick;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        foreach (Transform child in transform)
        {
            if (eventData.pointerEnter == child.gameObject && (child.GetComponent<Image>() != null || child.GetComponent<TextMeshProUGUI>() != null))
            {
                if (eventData.button == PointerEventData.InputButton.Left)
                {
                    if (!doubleLeft || lastClickTimeLeft < doubleClickTime)
                    {
                        LeftClick?.Invoke();
                        lastClickTimeLeft = doubleClickTime;
                        return;
                    }
                    lastClickTimeLeft = 0f;
                    return;
                }

                if (eventData.button == PointerEventData.InputButton.Right)
                {
                    if (!doubleRight || lastClickTimeRight < doubleClickTime)
                    {
                        RightClick?.Invoke();
                        lastClickTimeRight = doubleClickTime;
                        return;
                    }
                    lastClickTimeRight = 0f;
                    return;
                }

                if (eventData.button == PointerEventData.InputButton.Middle)
                {
                    if (!doubleMiddle || lastClickTimeMiddle < doubleClickTime)
                    {
                        MiddleClick?.Invoke();
                        lastClickTimeMiddle = doubleClickTime;
                        return;
                    }
                    lastClickTimeMiddle = 0f;
                    return;
                }
            }
        }
    }
}