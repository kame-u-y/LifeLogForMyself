using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkingDirector : SingletonMonoBehaviourFast<WorkingDirector>
{
    AppDirector appDirector;
    DatabaseDirector databaseDirector;
    MainUIDirector mainUIDirector;

    private WorkData currentWork;
    private ProjectData currentProject;

    public bool IsWorking { get; protected set; } = false;

    /// <summary>
    /// 作業中、一定間隔ごとに実行するためのカウント
    /// </summary>
    private float time;

    private int activeStart = 0;
    //private int activeSec = 0;

    private int restStart = 0;
    //private int restSec = 0;

    private enum PomodoroState
    {
        Active,
        Rest,
    }
    private PomodoroState pomodoroState;


    new void Awake()
    {
        base.Awake();

        appDirector = AppDirector.Instance;
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
            mainUIDirector.PieChartCtrler.CreateLogPieChart(dayData, project);

            if (appDirector.isClock12h)
            {
                mainUIDirector.PieChartCtrler.SwitchAMPM(!appDirector.IsAm());
            }
        }
        // 作業時間の表示
        mainUIDirector.CurrentCountTMP.text = "00:00";
    }

    // Update is called once per frame
    void Update()
    {
        // 1秒ごとに更新
        if (time >= 1.0f)
        {
            // 時系列データに応じて、24h前のログデータを縮める
            mainUIDirector.PieChartCtrler.UpdateLogPie(GetNowTotalSeconds());

            // 24時だったならば、データを一新する
            if (GetNowTotalSeconds() == GetTodayTotalSeconds())
            {
                // ログの表示
                List<WorkData> dayData = databaseDirector.Fetch24hData();
                if (dayData != null)
                {
                    List<ProjectData> project = databaseDirector.FetchProjectList();
                    mainUIDirector.PieChartCtrler.CreateLogPieChart(dayData, project);
                }
                if (appDirector.isClock12h)
                {
                    mainUIDirector.PieChartCtrler.SwitchAMPM(false);
                }
            }
            // 12時ならば昨日の午後のログを表示
            else if (GetNowTotalSeconds() == Get12hTotalSeconds())
            {
                if (appDirector.isClock12h)
                {
                    mainUIDirector.PieChartCtrler.SwitchAMPM(true);
                }
            }

            if (IsWorking)
            {
                ProceedCurrentWork();
            }
                time = 0.0f;
        }
            time += Time.deltaTime;
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

        if (currentProject.notificationMode == NotificationMode.Pomodoro)
        {
            activeStart = currentWork.startUnixSec;
            pomodoroState = PomodoroState.Active;
        }

        time = 0;
    }

    /// <summary>
    /// 作業の進行処理
    /// </summary>
    private void ProceedCurrentWork()
    {
        int nowTotalSeconds = GetNowTotalSeconds();
        int elapsedSec = 0;

        currentWork.endUnixSec = nowTotalSeconds;

        // 作業中のログの更新
        mainUIDirector.PieChartCtrler.UpdateCurrentPie(currentWork);

        // メーターの更新
        if (currentProject.notificationMode == NotificationMode.Pomodoro)
        {
            if (pomodoroState == PomodoroState.Active)
            {
                elapsedSec = nowTotalSeconds - activeStart;
                UpdatePomodoroActive(elapsedSec, nowTotalSeconds);
            }
            else if (pomodoroState == PomodoroState.Rest)
            {
                elapsedSec = nowTotalSeconds - restStart;
                UpdatePomodoroRest(elapsedSec, nowTotalSeconds);
            }
        }
        else
        {
            elapsedSec = currentWork.endUnixSec - currentWork.startUnixSec;
            print("now:" + elapsedSec);
            mainUIDirector.CurrentWorkMeterCtrler.UpdateMeter(elapsedSec);
        }

        // 作業時間表示の更新（1時間を超えたら桁を増やす）
        int h = elapsedSec / 3600;
        int m = (elapsedSec % 3600) / 60;
        int s = elapsedSec % 60;
        mainUIDirector.CurrentCountTMP.text = h > 0
            ? $"{String.Format("{0:00}", h)}:{String.Format("{0:00}", m)}:{String.Format("{0:00}", s)}"
            : $"{String.Format("{0:00}", m)}:{String.Format("{0:00}", s)}";
    }

    private void UpdatePomodoroActive(int _elapsedSec, int _nowTotalSec)
    {
        mainUIDirector.CurrentWorkMeterCtrler.UpdateActiveMeter(_elapsedSec);

        if (_elapsedSec >= mainUIDirector.CurrentWorkMeterCtrler.MaxMinite * 60.0f)
        {
            // ポモドーロ Restへの変更処理
            restStart = _nowTotalSec;
            pomodoroState = PomodoroState.Rest;
            mainUIDirector.CurrentWorkMeterCtrler.UpdateColor(
                mainUIDirector.CurrentWorkMeterCtrler.RestMeterColor);
        }

    }

    private void UpdatePomodoroRest(int _elapsedSec, int _nowTotalSec)
    {
        mainUIDirector.CurrentWorkMeterCtrler.UpdateRestMeter(_elapsedSec);

        if (_elapsedSec >= mainUIDirector.CurrentWorkMeterCtrler.MaxRestMinute * 60.0f)
        {
            // ポモドーロ Activeへの変更処理
            activeStart = _nowTotalSec;
            pomodoroState = PomodoroState.Active;
            mainUIDirector.CurrentWorkMeterCtrler.UpdateColor(
                currentProject.pieColor.GetWithColorFormat());
        }
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
        mainUIDirector.PieChartCtrler.EndCurrentWork(currentWork);

        time = 0;
    }



    /// <summary>
    /// 設定更新用
    /// </summary>
    /// <param name="_maxMinute"></param>
    public void UpdateWorkMeterMax(float _maxMinute)
    {
        int elapsed = currentWork == null
            ? 0
            : currentWork.endUnixSec - currentWork.startUnixSec;
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
            mainUIDirector.PieChartCtrler.CreateLogPieChart(dayData, project);
            if (appDirector.isClock12h)
            {
                mainUIDirector.PieChartCtrler.SwitchAMPM(!appDirector.IsAm());
            }
        }
    }

    public void SwitchCurrentProject()
    {
        string selectedProject = mainUIDirector.ProjectDropdown.captionText.text;

        // 要らない説
        if (selectedProject != databaseDirector.FetchSelectedProject())
        {
            databaseDirector.SetSelectedProject(selectedProject);
        }

        currentProject = databaseDirector.FindProject(selectedProject);
        Debug.Log("notif:" + currentProject.notificationMode);

        Color c = currentProject.pieColor.GetWithColorFormat();
        mainUIDirector.CurrentWorkMeterCtrler.UpdateColor(c);
        
        if (IsWorking)
        {
            currentWork.projectName = selectedProject;
            mainUIDirector.PieChartCtrler.UpdateCurrentPieColor(c, currentProject.name);
            if (currentProject.notificationMode == NotificationMode.Pomodoro)
            {
                activeStart = GetNowTotalSeconds();
                pomodoroState = PomodoroState.Active;
            }
        }
    }


    private int GetNowTotalSeconds()
        => (int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

    private int GetTodayTotalSeconds()
        => (int)DateTime.Today.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

    private int Get12hTotalSeconds()
        => (int)DateTime.Today.AddHours(12).Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
}
