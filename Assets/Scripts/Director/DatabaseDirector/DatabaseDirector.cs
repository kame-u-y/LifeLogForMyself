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
    /// �V���O���g��
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
        // Json�o�͗p��JsonSaveData�𐶐�
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
    /// ��Ƃ̏I�����ۑ�
    /// �I�����̓��t�ɕۑ������
    /// </summary>
    /// <param name="_work"></param>
    public void AddEndedWork(WorkData _work)
    {
        string today = DateTime.Today.ToString("yyyyMMdd");
        int todayStart = (int)DateTime.Today.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

        // ���t���Ƃ̎��������݂��Ȃ��Ƃ��쐬
        if (saveData.dailyDictionary == null)
        {
            saveData.dailyDictionary = new Dictionary<string, DayData>();
        }
        // �����̎������ڂ����݂��Ȃ��Ƃ��쐬
        if (!saveData.dailyDictionary.ContainsKey(today))
        {
            saveData.dailyDictionary[today] = new DayData()
            {
                works = new List<WorkData>()
            };
        }


        if (_work.startUnixSec >= todayStart)
        {
            // ��Ƃ������܂����Ȃ��ꍇ�A�ʏ폈��
            saveData.dailyDictionary[today].works.Add(_work);
        }
        else
        {
            // ��Ƃ������܂����ꍇ�A�����ƍ���ɍ�ƋL�^�𕪊�
            string yesterday = DateTime.Today.AddDays(-1).ToString("yyyyMMdd");

            // ����̎������ڂ����݂��Ȃ��Ƃ��쐬
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
            projectName = "���"
        };

        AddEndedWork(newWork);
    }

    private void SaveSampleProjectData()
    {
        ProjectData newProject = new ProjectData()
        {
            id = 0,
            name = "���",
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

        //�ύX�̒ʒm
        // meter���x�� meter controller
        // �ʒm�� meter controller
        // resizing���[�h window director
        windowDirector.UpdateScreenSize();
        workingDirector.UpdateWorkMeterMax(saveData.progressMeterMax);

    }

    public void ApplyProjectSettings(List<ProjectData> _project)
    {
        saveData.projects = _project;
        ExportSaveData();

        //�ύX�̒ʒm

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

        //�ύX�̒ʒm
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
    /// ���O�\�����[�h�p
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
        // ������works��ǉ�
        if (saveData.dailyDictionary.ContainsKey(today))
        {
            works.AddRange(saveData.dailyDictionary[today].works);
        }
        // �����܂���work�͂�����̂ň����͑���ɂ��Ȃ��Ă悢
        string yesterday = DateTime.Today.AddDays(-1).ToString("yyyyMMdd");
        Debug.Log(yesterday);
        if (saveData.dailyDictionary.ContainsKey(yesterday))
        {
            int nowYesterday = (int)DateTime.Now.AddDays(-1).Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            var now = (int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            Debug.Log(now - nowYesterday);
            Debug.Log(nowYesterday);

            // �����work�ŏI�����Ԃ�������24���ԑO����̂��݂̂̂��擾
            List<WorkData> yesterdayWorks = saveData.dailyDictionary[yesterday].works
                .FindAll(v => v.endUnixSec > nowYesterday);
            Debug.Log(yesterdayWorks.Count);
            // 24���ԑO�����O�ɊJ�n���ꂽ���̂͊J�n��24���ԑO�ɐݒ�
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
    //    // �f�[�^�\��
    //    // �N�����Aworks�œ�����āAwork��projectName���R�Â��Ă�ق���������
    //    //saveData.projects
    //}

    // OverWrite�͕ύX�\�֐������ׂĒ�`�����Ⴄ�̂����֐�������
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
