using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProjectConstants
{
    public const int ScreenThreshold = 300;
    public static bool IsMoreThanThreshold(int _i)
        => _i > ScreenThreshold;
}
