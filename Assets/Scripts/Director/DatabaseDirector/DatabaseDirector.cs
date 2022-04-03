using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DatabaseDirector : MonoBehaviour
{
    string filePath;
    LoadedSaveData saveData;

    void Awake()
    {
        filePath = Application.persistentDataPath + "/" + ".savedata.json";
        saveData = new LoadedSaveData();

        ImportSaveData();
    }

    // Start is called before the first frame update
    void Start()
    {

        //print(saveData.dailyDictionary);
        //print(saveData.dailyDictionary["20220401"].works[0].endUnixSec);

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
            return;

        StreamReader streamReader = new StreamReader(filePath);
        string json = streamReader.ReadToEnd();
        streamReader.Close();

        var jsonSaveData = JsonUtility.FromJson<JsonSaveData>(json);
        saveData = new LoadedSaveData()
        {
            dailyDictionary = jsonSaveData.dailyDictionary.Dictionary,
            projects = jsonSaveData.projects
        };
    }

    public void ExportSaveData()
    {
        // Json出力用にJsonSaveDataを生成
        var jsonSaveData = new JsonSaveData()
        {
            dailyDictionary = new JsonDictionary<string, DayData>(saveData.dailyDictionary),
            projects = saveData.projects
        };
        //jsonSaveData.dailyDictionary.Dictionary()
        string json = JsonUtility.ToJson(jsonSaveData, true);

        StreamWriter streamWriter = new StreamWriter(filePath);
        streamWriter.Write(json);
        streamWriter.Flush();
        streamWriter.Close();
    }

    public void AddEndedWork(WorkData _work)
    {
        string today = DateTime.Now.ToString("yyyyMMdd");



        if (saveData.dailyDictionary == null)
        {
            saveData.dailyDictionary = new Dictionary<string, DayData>();
        }

        if (!saveData.dailyDictionary.ContainsKey(today))
        {
            saveData.dailyDictionary[today] = new DayData()
            {
                works = new List<WorkData>()
                //projects = new List<ProjectData>()
            };
        }

        saveData.dailyDictionary[today].works.Add(_work);

        //ProjectData newProject = new ProjectData()
        //{
        //    id = 0,
        //    name = _work.projectName,
        //    pieColor = new ColorData()
        //    {
        //        r = 255.0f,
        //        g = 0.0f,
        //        b = 0.0f
        //    },
        //    totalSec = 1000
        //};

        //if (!IsProjectDataExist(today, newProject.name))
        //{
        //    saveData.dailyDictionary[today].projects.Add(newProject);
        //}

        ExportSaveData();
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

    //private bool IsProjectDataExist(string _day, string _name)
    //    => saveData.dailyDictionary[_day].projects.Exists(v => v.name == _name);


    private void SaveSampleWorkData()
    {
        WorkData newWork = new WorkData()
        {
            id = 0,
            startUnixSec = 100,
            endUnixSec = 335,
            projectName = "作業"
        };

        AddEndedWork(newWork);
    }

    private void SaveSampleProjectData()
    {
        ProjectData newProject = new ProjectData()
        {
            id = 0,
            name = "作業",
            pieColor = new ColorData()
            {
                r = 255.0f,
                g = 255.0f,
                b = 0.0f
            },
            totalSec = 10000
        };

        AddProject(newProject);
    }


    public DayData FetchDayData(string _day)
    {
        Debug.Log(saveData.dailyDictionary);
        return saveData.dailyDictionary.ContainsKey(_day)
           ? saveData.dailyDictionary[_day]
           : null;
    }

    public List<ProjectData> FetchProjectList()
        => saveData.projects;

    public ProjectData FindProject(string _name)
        => saveData.projects.Find(v => v.name == _name);


    //public List<WorkData> FetchTodayWorks()
    //{
    //    // データ構造
    //    // 年月日、worksで当たれて、workにprojectNameが紐づいてるほうがいいな
    //    //saveData.projects
    //}

    // OverWriteは変更可能関数をすべて定義しちゃうのが利便性高そう
}
