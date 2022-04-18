using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ConvertProjectColor : MonoBehaviour
{
    string filePath;
    LoadedSaveData saveData;

    void Awake()
    {
        filePath = Application.persistentDataPath + "/" + ".savedata.json";
        ImportSaveData();
        saveData.projects.ForEach(
            v =>
            {
                if (v.pieColor.r > 1.0f || v.pieColor.g > 1.0f || v.pieColor.b > 1.0f)
                {
                    v.pieColor.r /= 255.0f;
                    v.pieColor.g /= 255.0f;
                    v.pieColor.b /= 255.0f;
                }
            });
        ExportSaveData();
    }

    // Start is called before the first frame update
    void Start()
    {
        //SaveSampleWorkData();
        //SaveSampleProjectData();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ImportSaveData()
    {
        if (!File.Exists(filePath))
        {
            Debug.Log("file not found error");
            //ExportSaveData();
            return;
        }
        StreamReader streamReader = new StreamReader(filePath);
        string json = streamReader.ReadToEnd();
        streamReader.Close();

        var jsonSaveData = JsonUtility.FromJson<JsonSaveData>(json);
        saveData = new LoadedSaveData()
        {
            dailyDictionary = jsonSaveData.dailyDictionary.Dictionary,
            projects = jsonSaveData.ConvertProjectsToLoadedFormat(),
            notificationSoundPath = jsonSaveData.notificationSoundPath,
            progressMeterMax = jsonSaveData.progressMeterMax,
            twoResizingStages = jsonSaveData.twoResizingStages,
            threeResizingStages = jsonSaveData.threeResizingStages
        };
        Enum.TryParse(jsonSaveData.resizingModeString, out saveData.resizingMode);

        Debug.Log("imported save data");
    }

    public void ExportSaveData()
    {
        // JsonèoóÕópÇ…JsonSaveDataÇê∂ê¨
        var jsonSaveData = new JsonSaveData()
        {
            dailyDictionary = new JsonDictionary<string, DayData>(saveData.dailyDictionary),
            projects = saveData.ConvertProjectsToJsonFormat(),
            notificationSoundPath = saveData.notificationSoundPath,
            progressMeterMax = saveData.progressMeterMax,
            resizingModeString = saveData.resizingMode.ToString(),
            twoResizingStages = saveData.twoResizingStages,
            threeResizingStages = saveData.threeResizingStages
        };
        //Debug.Log(jsonSaveData.twoResizingStages.small);
        //Debug.Log(jsonSaveData.twoResizingStages.medium);
        //jsonSaveData.dailyDictionary.Dictionary()
        string json = JsonUtility.ToJson(jsonSaveData, true);
        Debug.Log(json);
        StreamWriter streamWriter = new StreamWriter(filePath);
        streamWriter.Write(json);
        streamWriter.Flush();
        streamWriter.Close();
    }



    public void AddProject(ProjectData _project)
    {
        print(saveData.projects);
        print(_project);
        if (_project == null)
            return;
        if (saveData.projects.Exists(v => v.name == _project.name))
            return;

        saveData.projects.Add(_project);
        ExportSaveData();
    }

}
