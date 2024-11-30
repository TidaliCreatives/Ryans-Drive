using System;
using System.Collections.Generic;
using UnityEngine;

public class BackArrow : MonoBehaviour
{
    public static event Action OnGoingBack;

    public void GoBack()
    {
        OnGoingBack?.Invoke();
    }
}