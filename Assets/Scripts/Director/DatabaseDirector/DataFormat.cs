using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class JsonSaveData
{
    public JsonDictionary<string, DayData> dailyDictionary;
    public List<JsonProjectData> projects;
    public string notificationSoundPath;
    public float progressMeterMax;
    public string resizingModeString;
    public TwoResizingData twoResizingStages;
    public ThreeResizingData threeResizingStages;

    public List<ProjectData> ConvertProjectsToLoadedFormat()
    {
        var loadedProjects = new List<ProjectData>();
        this.projects.ForEach(v =>
        {
            var newProject = new ProjectData()
            {
                id = v.id,
                name = v.name,
                pieColor = v.pieColor,
                totalSec = v.totalSec
            };
            Enum.TryParse(v.notificationModeString, out newProject.notificationMode);
            loadedProjects.Add(newProject);
        });

        return loadedProjects;
    }
}

public class LoadedSaveData
{
    public Dictionary<string, DayData> dailyDictionary;
    public List<ProjectData> projects;
    public string notificationSoundPath;
    public float progressMeterMax;
    public ResizingMode resizingMode;
    public TwoResizingData twoResizingStages;
    public ThreeResizingData threeResizingStages;

    public List<JsonProjectData> ConvertProjectsToJsonFormat()
    {
        var jsonProjects = new List<JsonProjectData>();
        this.projects.ForEach(v =>
        {
            var newJsonProject = new JsonProjectData()
            {
                id = v.id,
                name = v.name,
                pieColor = v.pieColor,
                notificationModeString = v.notificationMode.ToString(),
                totalSec = v.totalSec
            };
            jsonProjects.Add(newJsonProject);
        });
        return jsonProjects;
    }
}

public enum ResizingMode
{
    TwoStages,
    ThreeStages
}

public class TwoResizingData
{
    public int medium;
    public int small;
}

public class ThreeResizingData
{
    public int large;
    public int medium;
    public int small;
}

[System.Serializable]
public class DayData
{
    public List<WorkData> works;
    //public List<ProjectData> projects;
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
public class JsonProjectData
{
    public int id;
    public string name;
    public ColorData pieColor;
    public string notificationModeString;
    public int totalSec;

}

[System.Serializable]
public class ProjectData
{
    public int id;
    public string name;
    public ColorData pieColor;
    public NotificationMode notificationMode;
    public int totalSec;

   
}

[System.Serializable]
public enum NotificationMode
{
    None,
    Sound,
    Pomodoro
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