using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DatabaseDirector : SingletonMonoBehaviourFast<DatabaseDirector>
{
    string filePath;
    private LoadedSaveData saveData;
    private WindowDirector windowDirector;
    private WorkingDirector workingDirector;
    private MainUIDirector mainUIDirector;

    /// <summary>
    /// シングルトン
    /// </summary>
    //private static DatabaseDirector instance;
    //public static DatabaseDirector Instance => instance;

    new void Awake()
    {
        base.Awake();
        //if (instance == null)
        //{
        //    instance = this;
        //    DontDestroyOnLoad(gameObject);
        //}
        //else
        //{
        //    Destroy(gameObject);
        //    return;
        //}

        filePath = Application.persistentDataPath + "/" + ".savedata.json";
        ImportSaveData();
    }

    // Start is called before the first frame update
    void Start()
    {

        windowDirector = WindowDirector.Instance;
        workingDirector = WorkingDirector.Instance;
        mainUIDirector = MainUIDirector.Instance;
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
            selectedProject = jsonSaveData.GetSelectedOrDefaultProject()
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

    /// <summary>
    /// 作業の終了時保存
    /// 終了時の日付に保存される
    /// </summary>
    /// <param name="_work"></param>
    public void AddEndedWork(WorkData _work)
    {
        string today = DateTime.Today.ToString("yyyyMMdd");
        int todayStart = (int)DateTime.Today.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

        // 日付ごとの辞書が存在しないとき作成
        if (saveData.dailyDictionary == null)
        {
            saveData.dailyDictionary = new Dictionary<string, DayData>();
        }
        // 今日の辞書項目が存在しないとき作成
        if (!saveData.dailyDictionary.ContainsKey(today))
        {
            saveData.dailyDictionary[today] = new DayData()
            {
                works = new List<WorkData>()
            };
        }


        if (_work.startUnixSec >= todayStart)
        {
            // 作業が日をまたがない場合、通常処理
            saveData.dailyDictionary[today].works.Add(_work);
        }
        else
        {
            // 作業が日をまたぐ場合、今日と昨日に作業記録を分割
            string yesterday = DateTime.Today.AddDays(-1).ToString("yyyyMMdd");

            // 昨日の辞書項目が存在しないとき作成
            if (!saveData.dailyDictionary.ContainsKey(yesterday))
            {
                saveData.dailyDictionary[yesterday] = new DayData()
                {
                    works = new List<WorkData>()
                };
            }

            WorkData yesterdayWork = _work.ShallowCopy();
            yesterdayWork.endUnixSec = todayStart - 1;
            saveData.dailyDictionary[yesterday].works.Add(yesterdayWork);

            WorkData todayWork = _work.ShallowCopy();
            todayWork.startUnixSec = todayStart;
            saveData.dailyDictionary[today].works.Add(todayWork);
        }

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
                r = 1.0f,
                g = 1.0f,
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

        workingDirector.UpdateCurrentPie();
        workingDirector.SwitchCurrentProject();
        mainUIDirector.ProjectDropdownCtrler.UpdateItems();
        mainUIDirector.PieChartCtrler.UpdateTodayColors(FetchProjectList());
        
    }

    public void ApplyProjectDelete(string _name)
    {
        var id = saveData.projects.FindIndex(v => v.name == _name);
        if (id == -1) return;

        saveData.SetSelectedOrDefaultProject(_name);

        saveData.projects.RemoveAt(id);
        ExportSaveData();

        //変更の通知
        mainUIDirector.ProjectDropdownCtrler.UpdateItems();
        mainUIDirector.PieChartCtrler.UpdateTodayColors(FetchProjectList());

    }

    public void SetSelectedProject(string _name)
    {
        saveData.selectedProject = _name;
        ExportSaveData();
    }

    #region fetching_data_functions
    /// <summary>
    /// ログ表示モード用
    /// </summary>
    /// <param name="_day"></param>
    /// <returns></returns>
    public DayData FetchDayData(string _day)
    {
        return saveData.dailyDictionary.ContainsKey(_day)
           ? saveData.dailyDictionary[_day]
           : null;
    }

    public List<WorkData> Fetch24hData()
    {
        List<WorkData> works = new List<WorkData>();

        string today = DateTime.Today.ToString("yyyyMMdd");
        // 今日のworksを追加
        if (saveData.dailyDictionary.ContainsKey(today))
        {
            works.AddRange(saveData.dailyDictionary[today].works);
        }
        // 日をまたぐworkはちぎるので一昨日は相手にしなくてよい
        string yesterday = DateTime.Today.AddDays(-1).ToString("yyyyMMdd");
        Debug.Log(yesterday);
        if (saveData.dailyDictionary.ContainsKey(yesterday))
        {
            int nowYesterday = (int)DateTime.Now.AddDays(-1).Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            var now = (int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            Debug.Log(now - nowYesterday);
            Debug.Log(nowYesterday);

            // 昨日のworkで終了時間が今から24時間前より後のもののみを取得
            List<WorkData> yesterdayWorks = saveData.dailyDictionary[yesterday].works
                .FindAll(v => v.endUnixSec > nowYesterday);
            Debug.Log(yesterdayWorks.Count);
            // 24時間前よりも前に開始されたものは開始を24時間前に設定
            yesterdayWorks.ForEach(v =>
            {

                if (v.startUnixSec < nowYesterday)
                {
                    v.startUnixSec = nowYesterday;
                }
            });
            works.AddRange(yesterdayWorks);
        }
        return works;
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
                    name = ProjectConstants.DefaultProjectName,
                    pieColor = new ColorData() {
                        r = 143 / 255.0f,
                        g = 193 / 255.0f,
                        b = 16 / 255.0f
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
