using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DatabaseDirector : MonoBehaviour
{
    string filePath;
    LoadedSaveData saveData;
    WindowDirector windowDirector;
    WorkingDirector workingDirector;
    [SerializeField]
    CurrentWorkMeterController currentWorkMeterCtrler;
    [SerializeField]
    ProjectDropdownController projectDropdownCtrler;

    void Awake()
    {
        filePath = Application.persistentDataPath + "/" + ".savedata.json";
        ImportSaveData();
        windowDirector = GameObject.Find("WindowDirector").GetComponent<WindowDirector>();
        workingDirector = GameObject.Find("WorkingDirector").GetComponent<WorkingDirector>();
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
            saveData = CreateDefaultData();
            ExportSaveData();
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
            threeResizingStages = jsonSaveData.threeResizingStages,
            selectedProject = jsonSaveData.selectedProject
        };
        Enum.TryParse(jsonSaveData.resizingModeString, out saveData.resizingMode);

        Debug.Log("imported save data");
    }

    public void ExportSaveData()
    {
        // Json出力用にJsonSaveDataを生成
        var jsonSaveData = new JsonSaveData()
        {
            dailyDictionary = new JsonDictionary<string, DayData>(saveData.dailyDictionary),
            projects = saveData.ConvertProjectsToJsonFormat(),
            notificationSoundPath = saveData.notificationSoundPath,
            progressMeterMax = saveData.progressMeterMax,
            resizingModeString = saveData.resizingMode.ToString(),
            twoResizingStages = saveData.twoResizingStages,
            threeResizingStages = saveData.threeResizingStages,
            selectedProject = saveData.selectedProject
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


    public void ApplySettings(
        float _progressBarMax,
        string _notificationSoundPath,
        ResizingMode _resizingMode,
        TwoResizingData _twoResizingData,
        ThreeResizingData _threeResizingData)
    //List<ProjectData> _projects)
    {
        saveData.progressMeterMax = _progressBarMax;
        saveData.notificationSoundPath = _notificationSoundPath;
        saveData.resizingMode = _resizingMode;
        saveData.twoResizingStages = _twoResizingData;
        saveData.threeResizingStages = _threeResizingData;
        //saveData.projects = _projects;
        ExportSaveData();

        //変更の通知
        // meterラベル meter controller
        // 通知音 meter controller
        // resizingモード window director
        windowDirector.UpdateScreenSize();
        workingDirector.UpdateWorkMeterMax(saveData.progressMeterMax);

    }

    public void ApplyProjectSettings(List<ProjectData> _project)
    {
        saveData.projects = _project;
        ExportSaveData();

        //変更の通知

        workingDirector.ChangeProjectOfCurrentWork();
        projectDropdownCtrler.UpdateItems();
    }

    public void ApplyProjectDelete(string _name)
    {
        var id = saveData.projects.FindIndex(v => v.name == _name);
        if (id == -1) return;

        saveData.projects.RemoveAt(id);
        ExportSaveData();

        //変更の通知
        projectDropdownCtrler.UpdateItems();
    }

    public void SetSelectedProject(string _name)
    {
        saveData.selectedProject = _name;
    }

    #region fetching_data_functions
    public DayData FetchDayData(string _day)
    {
        return saveData.dailyDictionary.ContainsKey(_day)
           ? saveData.dailyDictionary[_day]
           : null;
    }

    public List<ProjectData> FetchProjectList()
        => saveData.projects;

    public ProjectData FindProject(string _name)
        => saveData.projects.Find(v => v.name == _name);

    public string FetchNotificationSoundPath()
        => saveData.notificationSoundPath;

    public float FetchProgressMeterMax()
        => saveData.progressMeterMax;

    public ResizingMode FetchResizingMode()
        => saveData.resizingMode;

    public TwoResizingData FetchTwoResizingStages()
        => saveData.twoResizingStages;

    public ThreeResizingData FetchThreeResizingStages()
        => saveData.threeResizingStages;

    public string FetchSelectedProject()
        => saveData.selectedProject;

    //public List<WorkData> FetchTodayWorks()
    //{
    //    // データ構造
    //    // 年月日、worksで当たれて、workにprojectNameが紐づいてるほうがいいな
    //    //saveData.projects
    //}

    // OverWriteは変更可能関数をすべて定義しちゃうのが利便性高そう
    #endregion


    private LoadedSaveData CreateDefaultData()
    {
        return new LoadedSaveData()
        {
            dailyDictionary = new Dictionary<string, DayData>()
            {
                [DateTime.Now.ToString("yyyyMMdd")] = new DayData()
                {
                    works = new List<WorkData>()
                }
            },
            projects = new List<ProjectData>()
            {
                new ProjectData()
                {
                    id = 0,
                    name = "No Project",
                    pieColor = new ColorData() {
                        r = 143,
                        g = 193,
                        b = 16
                    },
                    notificationMode = NotificationMode.Sound,
                    totalSec = 0
                }
            },
            notificationSoundPath = "",
            progressMeterMax = 25,
            resizingMode = ResizingMode.ThreeStages,
            twoResizingStages = new TwoResizingData()
            {
                medium = 450,
                small = 150
            },
            threeResizingStages = new ThreeResizingData()
            {
                large = 800,
                medium = 450,
                small = 150
            }
        };
    }
}
