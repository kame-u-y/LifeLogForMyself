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

    private WorkData currentWork;
    private ProjectData currentProject;

    public bool IsWorking { get; protected set; } = false;

    /// <summary>
    /// 作業中、一定間隔ごとに実行するためのカウント
    /// </summary>
    private float time;


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
        // 再生/停止ボタンの表示初期化
        mainUIDirector.PlayEndImageCtrler.ChangeButtonImage(IsWorking);
        // ログの表示
        List<WorkData> dayData = databaseDirector.Fetch24hData();
        if (dayData != null)
        {
            List<ProjectData> project = databaseDirector.FetchProjectList();
            mainUIDirector.PieChartCtrler.UpdateLogPieChart(dayData, project);
        }
        // 作業時間の表示
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
                ProceedCurrentWork();
                time = 0.0f;
            }
            time += Time.deltaTime;
        }
    }

    /// <summary>
    /// 開始/終了ボタンクリック時のイベント
    /// 作業の開始/終了
    /// </summary>
    public void ToggleWork()
    {
        if (IsWorking)
            EndWork();
        else
            StartWork();
    }

    /// <summary>
    /// 作業開始処理
    /// </summary>
    private void StartWork()
    {
        IsWorking = true;

        currentWork = new WorkData()
        {
            id = 0,
            startUnixSec = GetNowTotalSeconds(),
            endUnixSec = GetNowTotalSeconds(),
            projectName = mainUIDirector.ProjectDropdown.captionText.text
        };
        currentProject = databaseDirector.FindProject(currentWork.projectName);

        mainUIDirector.PlayEndImageCtrler.ChangeButtonImage(IsWorking);
        mainUIDirector.PieChartCtrler.CreateCurrentPie(currentWork, currentProject);

        time = 0;
    }

    /// <summary>
    /// 作業の進行処理
    /// </summary>
    private void ProceedCurrentWork()
    {
        currentWork.endUnixSec = GetNowTotalSeconds();

        // 経過時間
        int elapsed = currentWork.endUnixSec - currentWork.startUnixSec;
        print("now:" + elapsed);

        // 作業中のログの更新
        mainUIDirector.PieChartCtrler.UpdateCurrentPie(currentWork);
        // メーターの更新
        mainUIDirector.CurrentWorkMeterCtrler.UpdateMeter(elapsed);
        // 作業時間表示の更新（1時間を超えたら桁を増やす）
        int h = elapsed / 3600;
        int m = (elapsed % 3600) / 60;
        int s = elapsed % 60;
        mainUIDirector.CurrentCountTMP.text = h > 0
            ? $"{String.Format("{0:00}", h)}:{String.Format("{0:00}", m)}:{String.Format("{0:00}", s)}"
            : $"{String.Format("{0:00}", m)}:{String.Format("{0:00}", s)}";
    }

    /// <summary>
    /// 作業の終了処理
    /// </summary>
    private void EndWork()
    {
        IsWorking = false;

        databaseDirector.AddEndedWork(currentWork);

        mainUIDirector.PlayEndImageCtrler.ChangeButtonImage(IsWorking);
        mainUIDirector.CurrentWorkMeterCtrler.InitializeMeter();
        mainUIDirector.PieChartCtrler.EndCurrentWork();
        
        time = 0;
    }



    /// <summary>
    /// 設定更新用
    /// </summary>
    /// <param name="_maxMinute"></param>
    public void UpdateWorkMeterMax(float _maxMinute)
    {
        int elapsed = currentWork.endUnixSec - currentWork.startUnixSec;
        mainUIDirector.CurrentWorkMeterCtrler.UpdateWorkMax(_maxMinute, elapsed);
    }

    /// <summary>
    /// 外部から描画の更新が必要な場合に呼ばれる
    /// isClock12hの変更通知
    /// </summary>
    public void UpdateCurrentPie()
    {
        if (!IsWorking) return;
        mainUIDirector.PieChartCtrler.UpdateCurrentPie(currentWork);
    }

    /// <summary>
    /// 外部から
    /// </summary>
    public void UpdateLogPieChart()
    {
        List<WorkData> dayData = databaseDirector.Fetch24hData();
        if (dayData != null)
        {
            List<ProjectData> project = databaseDirector.FetchProjectList();
            mainUIDirector.PieChartCtrler.UpdateLogPieChart(dayData, project);
        }
    }

    public void SwitchCurrentProject()
    {
        string selectedProject = mainUIDirector.ProjectDropdown.captionText.text;
        databaseDirector.SetSelectedProject(selectedProject);
        currentProject = databaseDirector.FindProject(selectedProject);

        Color c = currentProject.pieColor.GetWithColorFormat();
        mainUIDirector.CurrentWorkMeterCtrler.UpdateColor(c);

        if (!IsWorking)
        {
            return;
        }
        currentWork.projectName = selectedProject;
        mainUIDirector.PieChartCtrler.UpdateCurrentPieColor(c, currentProject.name);
    }


    private int GetNowTotalSeconds()
        => (int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
}
