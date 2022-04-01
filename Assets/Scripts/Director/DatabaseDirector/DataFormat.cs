using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class JsonSaveData
{
    public JsonDictionary<string, DayData> dailyDictionary;
}

public class LoadedSaveData
{
    public Dictionary<string, DayData> dailyDictionary;
}


[System.Serializable]
public class DayData
{
    public List<WorkData> works;
    public List<ProjectData> projects;
}

[System.Serializable]
public class WorkData
{
    public int id;
    public int startUnixSec;
    public int endUnixSec;
    public string projectName;
}

[System.Serializable]
public class ProjectData
{
    public int id;
    public string name;
    public ColorData pieColor;
    public int totalSec;
}

[System.Serializable]
public class ColorData
{
    public float r;
    public float g;
    public float b;
}


#region TestDataClass
[System.Serializable]
public class TestParent
{
    public TestChild child;
}

[System.Serializable]
public class TestChild
{
    public string ch;
}
#endregion