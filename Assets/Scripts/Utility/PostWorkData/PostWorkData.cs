using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PostWorkData : MonoBehaviour
{
    string filePath;
    private LoadedSaveData saveData;

    [SerializeField]
    private int year;
    [SerializeField]
    private int month;
    [SerializeField]
    private int day;
    [SerializeField]
    private int hour;
    [SerializeField]
    private int minute;
    [SerializeField]
    private int second;

    private void Awake()
    {

        filePath = Application.persistentDataPath + "/" + ".savedata.json";
        ImportSaveData();

    }

    // Start is called before the first frame update
    void Start()
    {

        //PostWork(2022, 4, 26, 5, 10, 0, 5, 30, 0, "開発");
        //PostWork(2022, 4, 26, 9, 10, 30, 9, 30, 0, "開発");
        //PostWork(year, month, day, hour, minute, second, hour, minute+10, second, "読書");
        int y = 2022;
        int M = 4;
        int d = 28;
        int h = DateTime.Now.Hour;
        int m = DateTime.Now.Minute;
        int s = DateTime.Now.Second;
        PostWork(2022, 4, 27, 20, 0, s, 20, 30, s, "kintore");
        PostWork(2022, 4, 27, 10, 30, s, 11, 0, s, "開発");
        PostWork(2022, 4, 28, 3, 0, s, 4, 0, s, "英語");
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
    public void PostWork(
        int _y,
        int _m,
        int _d,
        int _startH,
        int _startM,
        int _startS,
        int _endH,
        int _endM,
        int _endS,
        string _projectName)
    {
        //string yyyyMMdd = $"{String.Format("{0:0000}", _y)}{String.Format("{0:00}", _m)}{String.Format("{0:00}", _d)}";
        string day = new DateTime(_y, _m, _d).ToString("yyyyMMdd");
        int dayStart = (int)new DateTime(_y, _m, _d).Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

        // 日付ごとの辞書が存在しないとき作成
        if (saveData.dailyDictionary == null)
        {
            saveData.dailyDictionary = new Dictionary<string, DayData>();
        }
        // 今日の辞書項目が存在しないとき作成
        if (!saveData.dailyDictionary.ContainsKey(day))
        {
            saveData.dailyDictionary[day] = new DayData()
            {
                works = new List<WorkData>()
            };
        }

        int startSec = GetTotalSec(_y, _m, _d, _startH, _startM, _startS);
        int endSec = GetTotalSec(_y, _m, _d, _endH, _endM, _endS);
        
        WorkData work = new WorkData()
        {
            id = 0,
            projectName = _projectName,
            startUnixSec = startSec,
            endUnixSec = endSec,
        };

        if (work.startUnixSec >= dayStart)
        {
            // 作業が日をまたがない場合、通常処理
            saveData.dailyDictionary[day].works.Add(work);
        }
        else
        {
            // 作業が日をまたぐ場合、今日と昨日に作業記録を分割
            string yesterday = new DateTime(_y, _m, _d).AddDays(-1).ToString("yyyyMMdd");

            // 昨日の辞書項目が存在しないとき作成
            if (!saveData.dailyDictionary.ContainsKey(yesterday))
            {
                saveData.dailyDictionary[yesterday] = new DayData()
                {
                    works = new List<WorkData>()
                };
            }

            WorkData yesterdayWork = work.ShallowCopy();
            yesterdayWork.endUnixSec = dayStart - 1;
            saveData.dailyDictionary[yesterday].works.Add(yesterdayWork);

            WorkData todayWork = work.ShallowCopy();
            todayWork.startUnixSec = dayStart;
            saveData.dailyDictionary[day].works.Add(todayWork);
        }

        ExportSaveData();
    }


    private int GetTotalSec(
        int _y, 
        int _m, 
        int _d, 
        int _hour, 
        int _min, 
        int _sec)
        => (int)new DateTime(_y, _m, _d, _hour, _min, _sec).Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

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
