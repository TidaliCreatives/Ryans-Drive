using UnityEngine;
using System;

[Serializable]
public class CustomDate
{
    // Creation Date
    [Range(0, 23)] public int Hour = 0;
    [Range(0, 59)] public int Minute = 0;
    [Range(1, 31)] public int Day = 1;
    [Range(1, 12)] public int Month = 1;
    [Range(2010, 2024)] public int Year = 2010;
}