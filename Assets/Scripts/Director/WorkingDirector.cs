using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkingDirector : MonoBehaviour
{
    private bool isWorking = false;
    //private int startUnixSec;
    //private int nowUnixSec;
    private WorkData currentWork;
    private ProjectData currentProject;
    private float time;

    [SerializeField]
    PlayEndImageController playEndImage;

    [SerializeField]
    PieChartController pieChartController;

    [SerializeField]
    Dropdown projectDropdown;

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
        playEndImage.ChangeButtonImage(isWorking);

        DayData dayData = databaseDirector.FetchDayData(DateTime.Now.ToString("yyyyMMdd"));
        if (dayData == null) return;

        List<ProjectData> project = databaseDirector.FetchProjectList();
        pieChartController.DisplayTodayPieChart(dayData, project);
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

    private void UpdateWorkTime()
    {
        //nowUnixSec = GetNowTotalSeconds();
        currentWork.endUnixSec = GetNowTotalSeconds();
        // todo: UpdateDisplay
        pieChartController.UpdateCurrentWorkPiece(currentWork);
        print("now:" + (currentWork.endUnixSec - currentWork.startUnixSec));
    }



    /// <summary>
    /// 外部から描画の更新が必要な場合に呼ばれる
    /// isClock12hの変更通知
    /// </summary>
    public void CallForNeedUpdateCurrentWorkPiece()
    {
        if (!isWorking) return;
        pieChartController.UpdateCurrentWorkPiece(currentWork);
    }

    public void CallForNeedDisplayTodayPieChart()
    {
        DayData dayData = databaseDirector.FetchDayData(DateTime.Now.ToString("yyyyMMdd"));
        if (dayData == null) return;

        List<ProjectData> project = databaseDirector.FetchProjectList();
        pieChartController.DisplayTodayPieChart(dayData, project);
    }

    public void ChangeProjectOfCurrentWork()
    {
        if (!isWorking) return;
        currentWork.projectName = projectDropdown.captionText.text;
        currentProject = databaseDirector.FindProject(currentWork.projectName);
        Color c = new Color(currentProject.pieColor.r, currentProject.pieColor.g, currentProject.pieColor.b);
        print(currentProject.name);
        pieChartController.ChangeCurrentColor(c);

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
            projectName = "作業"
        };
        currentProject = databaseDirector.FindProject(currentWork.projectName);

        playEndImage.ChangeButtonImage(isWorking);
        pieChartController.CreateCurrentWorkPiece(currentWork, currentProject);

        time = 0;
    }

    private void EndWork()
    {
        isWorking = false;
        // todo: データを記録
        databaseDirector.AddEndedWork(currentWork);

        playEndImage.ChangeButtonImage(isWorking);
        time = 0;
    }

    private int GetNowTotalSeconds()
        => (int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
}
