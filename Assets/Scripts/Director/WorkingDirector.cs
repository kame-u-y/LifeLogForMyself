using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkingDirector : SingletonMonoBehaviourFast<WorkingDirector>
{
    DatabaseDirector databaseDirector;
    MainUIDirector mainUIDirector;

    //[SerializeField]
    //PlayEndImageController playEndImageCtrler;
    //[SerializeField]
    //PieChartController pieChartCtrler;
    //[SerializeField]
    //Dropdown mainUIDirector.ProjectDropdown;
    //[SerializeField]
    //CurrentWorkMeterController mainUIDirector.CurrentWorkMeterCtrler;
    //[SerializeField]
    //TextMeshProUGUI mainUIDirector.CurrentCountTMP;

    public bool IsWorking { get; protected set; } = false;

    private WorkData currentWork;
    private ProjectData selectedProject;
    private float time;



    //private static WorkingDirector instance;
    //public static WorkingDirector Instance => instance;

    new void Awake()
    {
        base.Awake();
        databaseDirector = DatabaseDirector.Instance;
        mainUIDirector = MainUIDirector.Instance;

    }

    // Start is called before the first frame update
    void Start()
    {

        // 再生中を検出出来たら面白そうだけど、まあアカウント作らんとならんくなるな
        IsWorking = false;
        mainUIDirector.PlayEndImageCtrler.ChangeButtonImage(IsWorking);

        //DayData dayData = databaseDirector.FetchDayData(DateTime.Now.ToString("yyyyMMdd"));
        List<WorkData> dayData = databaseDirector.Fetch24hData();
        if (dayData == null) return;
        Debug.Log(dayData.Count);
        List<ProjectData> project = databaseDirector.FetchProjectList();
        mainUIDirector.PieChartCtrler.DisplayTodayPieChart(dayData, project);

        mainUIDirector.CurrentCountTMP.text = "00:00";
    }

    // Update is called once per frame
    void Update()
    {
        if (IsWorking)
        {
            // 1秒ごとに更新
            if (time >= 1.0f)
            {
                UpdateWorkTime();
                time = 0.0f;
            }
            time += Time.deltaTime;
        }
    }

    private void UpdateWorkTime()
    {
        //nowUnixSec = GetNowTotalSeconds();
        currentWork.endUnixSec = GetNowTotalSeconds();
        mainUIDirector.PieChartCtrler.UpdateCurrentWorkPiece(currentWork);
        int elapsed = currentWork.endUnixSec - currentWork.startUnixSec;
        print("now:" + elapsed);
        mainUIDirector.CurrentWorkMeterCtrler.UpdateMeter(elapsed);

        int h = elapsed / 3600;
        int m = (elapsed % 3600) / 60;
        int s = elapsed % 60;
        mainUIDirector.CurrentCountTMP.text = h > 0
            ? $"{String.Format("{0:00}", h)}:{String.Format("{0:00}", m)}:{String.Format("{0:00}", s)}"
            : $"{String.Format("{0:00}", m)}:{String.Format("{0:00}", s)}";
    }

    /// <summary>
    /// 設定更新用
    /// </summary>
    /// <param name="_maxMinute"></param>
    public void UpdateWorkMeterMax(float _maxMinute)
    {

        int elapsed = currentWork != null 
            ? currentWork.endUnixSec - currentWork.startUnixSec
            : 0;
        mainUIDirector.CurrentWorkMeterCtrler.UpdateWorkMax(_maxMinute, elapsed);
    }

    /// <summary>
    /// 外部から描画の更新が必要な場合に呼ばれる
    /// isClock12hの変更通知
    /// </summary>
    public void CallForNeedUpdateCurrentWorkPiece()
    {
        if (!IsWorking) return;
        mainUIDirector.PieChartCtrler.UpdateCurrentWorkPiece(currentWork);
    }

    public void CallForNeedDisplayTodayPieChart()
    {
        //DayData dayData = databaseDirector.FetchDayData(DateTime.Now.ToString("yyyyMMdd"));
        List<WorkData> dayData = databaseDirector.Fetch24hData();
        if (dayData == null) return;

        List<ProjectData> project = databaseDirector.FetchProjectList();
        mainUIDirector.PieChartCtrler.DisplayTodayPieChart(dayData, project);
    }

    public void ChangeProjectOfCurrentWork()
    {
        databaseDirector.SetSelectedProject(mainUIDirector.ProjectDropdown.captionText.text);
        selectedProject = databaseDirector.FindProject(databaseDirector.FetchSelectedProject());
        //Color c = new Color(
        //    selectedProject.pieColor.r,
        //    selectedProject.pieColor.g,
        //    selectedProject.pieColor.b);
        Color c = selectedProject.pieColor.GetWithColorFormat();
        mainUIDirector.CurrentWorkMeterCtrler.UpdateColor(c);
        
        if (!IsWorking) return;

        currentWork.projectName = mainUIDirector.ProjectDropdown.captionText.text;
        print(selectedProject.name);
        mainUIDirector.PieChartCtrler.ChangeCurrentColor(c, selectedProject.name);
        //mainUIDirector.CurrentWorkMeterCtrler.ChangeColor(c);
    }



    public void ToggleWork()
    {
        if (IsWorking)
            EndWork();
        else
            StartWork();
    }

    private void StartWork()
    {
        IsWorking = true;
        //startUnixSec = GetNowTotalSeconds();
        //nowUnixSec = startUnixSec;
        currentWork = new WorkData()
        {
            id = 0,
            startUnixSec = GetNowTotalSeconds(),
            endUnixSec = GetNowTotalSeconds(),
            projectName = mainUIDirector.ProjectDropdown.captionText.text
        };
        selectedProject = databaseDirector.FindProject(currentWork.projectName);

        mainUIDirector.PlayEndImageCtrler.ChangeButtonImage(IsWorking);
        mainUIDirector.PieChartCtrler.CreateCurrentWorkPiece(currentWork, selectedProject);

        //Color c = new Color(
        //    selectedProject.pieColor.r / 255.0f,
        //    selectedProject.pieColor.g / 255.0f,
        //    selectedProject.pieColor.b / 255.0f);
        //mainUIDirector.PieChartCtrler.ChangeCurrentColor(c);

        time = 0;
    }

    private void EndWork()
    {
        IsWorking = false;
        // todo: データを記録
        databaseDirector.AddEndedWork(currentWork);

        mainUIDirector.PlayEndImageCtrler.ChangeButtonImage(IsWorking);
        time = 0;

        mainUIDirector.CurrentWorkMeterCtrler.InitializeMeter();
        //mainUIDirector.CurrentCountTMP.text = "00:00";
        mainUIDirector.PieChartCtrler.EndCurrentWork();
    }

    private int GetNowTotalSeconds()
        => (int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
}
