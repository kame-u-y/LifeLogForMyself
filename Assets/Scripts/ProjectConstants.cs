using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProjectConstants
{
    public static int ScreenThreshold = 300;
    public static bool IsMoreThanThreshold(int _i)
        => _i > ScreenThreshold;

    public static Color DefaultColor =
        new Color(143 / 255.0f, 193 / 255.0f, 16 / 255.0f);


    public static string DefaultProjectName = "No Project";
}
