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
    }

    // Start is called before the first frame update
    void Start()
    {
        ImportSaveData();

        //print(saveData.dailyDictionary);
        //print(saveData.dailyDictionary["20220401"].works[0].endUnixSec);

        //SaveSampleData();
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
            dailyDictionary = jsonSaveData.dailyDictionary.Dictionary
        };
    }

    public void ExportSaveData()
    {
        // Json出力用にJsonSaveDataを生成
        var jsonSaveData = new JsonSaveData()
        {
            dailyDictionary = new JsonDictionary<string, DayData>(saveData.dailyDictionary)
        };
        //jsonSaveData.dailyDictionary.Dictionary()
        string json = JsonUtility.ToJson(jsonSaveData);

        StreamWriter streamWriter = new StreamWriter(filePath);
        streamWriter.Write(json);
        streamWriter.Flush();
        streamWriter.Close();
    }

    public void AddEndedWork(WorkData _work)
    {
        string today = DateTime.Now.ToString("yyyyMMdd");

        ProjectData newProject = new ProjectData()
        {
            id = 0,
            name = _work.projectName,
            pieColor = new ColorData()
            {
                r = 255.0f,
                g = 0.0f,
                b = 0.0f
            },
            totalSec = 1000
        };

        if (saveData.dailyDictionary == null)
        {
            saveData.dailyDictionary = new Dictionary<string, DayData>();
        }

        if (!saveData.dailyDictionary.ContainsKey(today))
        {
            saveData.dailyDictionary[today] = new DayData()
            {
                works = new List<WorkData>(),
                projects = new List<ProjectData>()
            };
        }

        saveData.dailyDictionary[today].works.Add(_work);

        if (!IsProjectDataExist(today, newProject.name))
        {
            saveData.dailyDictionary[today].projects.Add(newProject);
        }


        ExportSaveData();
    }

    private bool IsProjectDataExist(string _day, string _name)
        => saveData.dailyDictionary[_day].projects.Exists(v => v.name == _name);


    public void SaveSampleData()
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


    public DayData FetchDayData(string _day)
    {
        print(_day);
        return saveData.dailyDictionary[_day];

    }


    //public List<WorkData> FetchTodayWorks()
    //{
    //    // データ構造
    //    // 年月日、worksで当たれて、workにprojectNameが紐づいてるほうがいいな
    //    //saveData.projects
    //}

    // OverWriteは変更可能関数をすべて定義しちゃうのが利便性高そう
}
