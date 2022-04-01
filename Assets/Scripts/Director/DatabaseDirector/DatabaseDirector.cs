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

        print(saveData.dailyDictionary["20220401"].works[0].endUnixSec);

        //SaveSampleData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ImportSaveData()
    {
        if (File.Exists(filePath))
        {
            StreamReader streamReader = new StreamReader(filePath);
            string json = streamReader.ReadToEnd();
            streamReader.Close();

            var jsonSaveData = JsonUtility.FromJson<JsonSaveData>(json);
            saveData = new LoadedSaveData()
            {
                dailyDictionary = jsonSaveData.dailyDictionary.Dictionary
            };
        }
    }
    
    public void ExportSaveData()
    {
        // Json出力用にJsonSaveDataを生成
        var jsonSaveData = new JsonSaveData()
        {
            dailyDictionary = new JsonDictionary<string, DayData>(saveData.dailyDictionary)
        };
        string json = JsonUtility.ToJson(jsonSaveData);

        StreamWriter streamWriter = new StreamWriter(filePath);
        streamWriter.Write(json);
        streamWriter.Flush();
        streamWriter.Close();
    }


    public void SaveSampleData()
    {
        //saveData = new SaveData()
        //{
        //    projects = new List<ProjectData>{
        //        new ProjectData() {
        //            id = 0,
        //            name = "作業",
        //            pieColor = new ColorData() {
        //                r = 255.0f,
        //                g = 0.0f,
        //                b = 0.0f },
        //            totalSec = 300,
        //            works = new List<WorkData>()
        //            {
        //                new WorkData()
        //                {
        //                    id = 0,
        //                    startUnixSec = 115,
        //                    endUnixSec = 237
        //                }
        //            }
        //        }
        //    }
        //};

        saveData = new LoadedSaveData()
        {
            dailyDictionary = new Dictionary<string, DayData>
            {
                ["20220401"] = new DayData()
                {
                    works = new List<WorkData>()
                    {
                        new WorkData()
                        {
                            id = 0,
                            startUnixSec = 100,
                            endUnixSec = 335,
                            projectName = "作業"
                        }
                    },
                    projects = new List<ProjectData>()
                    {
                        new ProjectData() {
                            id = 0,
                            name = "作業",
                            pieColor = new ColorData()
                            {
                                r = 255.0f,
                                g = 0.0f,
                                b = 0.0f
                            },
                            totalSec = 1000
                        }
                    }
                }
            }
        };

        ExportSaveData();
    }

    public void AddEndedWork(WorkData _work)
    {
        //int projectId = -1;
        //for (int i=0; i<saveData.projects.Count; i++)
        //{
        //    print(saveData.projects[i].name);
        //    if (saveData.projects[i].name == _projectName)
        //    {
        //        print("data project detected");
        //        projectId = i;
        //        break;
        //    }
        //}
        //if (projectId == -1)
        //{

        //} 
        //else
        //{
        //    print("new data added");
        //    saveData.projects[projectId].works.Add(_work);
        //}

        DateTime dt = DateTime.Now;
        string today = dt.ToString("yyyyMMdd");
        saveData.dailyDictionary[today].works.Add(_work);
        ExportSaveData();
    }

    //public List<WorkData> FetchTodayWorks()
    //{
    //    // データ構造
    //    // 年月日、worksで当たれて、workにprojectNameが紐づいてるほうがいいな
    //    //saveData.projects
    //}

    // OverWriteは変更可能関数をすべて定義しちゃうのが利便性高そう
}
