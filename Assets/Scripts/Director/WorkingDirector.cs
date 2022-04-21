using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkingDirector : MonoBehaviour
{
    public bool isWorking = false;
    private WorkData currentWork;
    private ProjectData selectedProject;
    private float time;

    [SerializeField]
    PlayEndImageController playEndImageCtrler;
    [SerializeField]
    PieChartController pieChartCtrler;
    [SerializeField]
    Dropdown projectDropdown;
    [SerializeField]
    CurrentWorkMeterController currentWorkMeterCtrler;
    [SerializeField]
    TextMeshProUGUI currentCountText;

    DatabaseDirector databaseDirector;

    void Awake()
    {
        databaseDirector
            = GameObject.Find("DatabaseDirector").GetComponent<DatabaseDirector>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // 再生中を検出出来たら面白そうだけど、まあアカウント作らんとならんくなるな
        isWorking = false;
        playEndImageCtrler.ChangeButtonImage(isWorking);

        //DayData dayData = databaseDirector.FetchDayData(DateTime.Now.ToString("yyyyMMdd"));
        List<WorkData> dayData = databaseDirector.Fetch24hData();
        if (dayData == null) return;
        Debug.Log(dayData.Count);
        List<ProjectData> project = databaseDirector.FetchProjectList();
        pieChartCtrler.DisplayTodayPieChart(dayData, project);

        currentCountText.text = "00:00";
        InitializeCurrentWork();
    }

    // Update is called once per frame
    void Update()
    {
        if (isWorking)
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

    private void InitializeCurrentWork()
    {
        ProjectData p = databaseDirector.FindProject(ProjectConstants.DefaultProjectName);
        //Color c = new Color(p.pieColor.r, p.pieColor.g, p.pieColor.b);
        currentWorkMeterCtrler.UpdateColor(p.pieColor.GetWithColorFormat());
    }

    private void UpdateWorkTime()
    {
        //nowUnixSec = GetNowTotalSeconds();
        currentWork.endUnixSec = GetNowTotalSeconds();
        pieChartCtrler.UpdateCurrentWorkPiece(currentWork);
        int elapsed = currentWork.endUnixSec - currentWork.startUnixSec;
        print("now:" + elapsed);
        currentWorkMeterCtrler.UpdateMeter(elapsed);

        int h = elapsed / 3600;
        int m = (elapsed % 3600) / 60;
        int s = elapsed % 60;
        currentCountText.text = h > 0
            ? $"{String.Format("{0:00}", h)}:{String.Format("{0:00}", m)}:{String.Format("{0:00}", s)}"
            : $"{String.Format("{0:00}", m)}:{String.Format("{0:00}", s)}";
    }

    /// <summary>
    /// 設定更新用
    /// </summary>
    /// <param name="_maxMinute"></param>
    public void UpdateWorkMeterMax(float _maxMinute)
    {
        int elapsed = currentWork.endUnixSec - currentWork.startUnixSec;
        currentWorkMeterCtrler.UpdateWorkMax(_maxMinute, elapsed);
    }

    /// <summary>
    /// 外部から描画の更新が必要な場合に呼ばれる
    /// isClock12hの変更通知
    /// </summary>
    public void CallForNeedUpdateCurrentWorkPiece()
    {
        if (!isWorking) return;
        pieChartCtrler.UpdateCurrentWorkPiece(currentWork);
    }

    public void CallForNeedDisplayTodayPieChart()
    {
        //DayData dayData = databaseDirector.FetchDayData(DateTime.Now.ToString("yyyyMMdd"));
        List<WorkData> dayData = databaseDirector.Fetch24hData();
        if (dayData == null) return;

        List<ProjectData> project = databaseDirector.FetchProjectList();
        pieChartCtrler.DisplayTodayPieChart(dayData, project);
    }

    public void ChangeProjectOfCurrentWork()
    {
        databaseDirector.SetSelectedProject(projectDropdown.captionText.text);
        selectedProject = databaseDirector.FindProject(databaseDirector.FetchSelectedProject());
        //Color c = new Color(
        //    selectedProject.pieColor.r,
        //    selectedProject.pieColor.g,
        //    selectedProject.pieColor.b);
        Color c = selectedProject.pieColor.GetWithColorFormat();
        currentWorkMeterCtrler.UpdateColor(c);
        
        if (!isWorking) return;

        currentWork.projectName = projectDropdown.captionText.text;
        print(selectedProject.name);
        pieChartCtrler.ChangeCurrentColor(c, selectedProject.name);
        //currentWorkMeterCtrler.ChangeColor(c);
    }



    public void ToggleWork()
    {
        if (isWorking)
            EndWork();
        else
            StartWork();
    }

    private void StartWork()
    {
        isWorking = true;
        //startUnixSec = GetNowTotalSeconds();
        //nowUnixSec = startUnixSec;
        currentWork = new WorkData()
        {
            id = 0,
            startUnixSec = GetNowTotalSeconds(),
            endUnixSec = GetNowTotalSeconds(),
            projectName = projectDropdown.captionText.text
        };
        selectedProject = databaseDirector.FindProject(currentWork.projectName);

        playEndImageCtrler.ChangeButtonImage(isWorking);
        pieChartCtrler.CreateCurrentWorkPiece(currentWork, selectedProject);

        //Color c = new Color(
        //    selectedProject.pieColor.r / 255.0f,
        //    selectedProject.pieColor.g / 255.0f,
        //    selectedProject.pieColor.b / 255.0f);
        //pieChartCtrler.ChangeCurrentColor(c);

        time = 0;
    }

    private void EndWork()
    {
        isWorking = false;
        // todo: データを記録
        databaseDirector.AddEndedWork(currentWork);

        playEndImageCtrler.ChangeButtonImage(isWorking);
        time = 0;

        currentWorkMeterCtrler.InitializeMeter();
        //currentCountText.text = "00:00";
        pieChartCtrler.EndCurrentWork();
    }

    private int GetNowTotalSeconds()
        => (int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
}
