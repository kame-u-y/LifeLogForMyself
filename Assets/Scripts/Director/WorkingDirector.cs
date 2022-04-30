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
    /// ��ƒ��A���Ԋu���ƂɎ��s���邽�߂̃J�E���g
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
        // �Đ��������o�o������ʔ����������ǁA�܂��A�J�E���g����ƂȂ�񂭂Ȃ��
        IsWorking = false;
        // �Đ�/��~�{�^���̕\��������
        mainUIDirector.PlayEndImageCtrler.ChangeButtonImage(IsWorking);
        // ���O�̕\��
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
        // ��Ǝ��Ԃ̕\��
        mainUIDirector.CurrentCountTMP.text = "00:00";
    }

    // Update is called once per frame
    void Update()
    {
        // 1�b���ƂɍX�V
        if (time >= 1.0f)
        {
            // ���n��f�[�^�ɉ����āA24h�O�̃��O�f�[�^���k�߂�
            mainUIDirector.PieChartCtrler.UpdateLogPie(GetNowTotalSeconds());

            // 24���������Ȃ�΁A�f�[�^����V����
            if (GetNowTotalSeconds() == GetTodayTotalSeconds())
            {
                // ���O�̕\��
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
            // 12���Ȃ�΍���̌ߌ�̃��O��\��
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
    /// �J�n/�I���{�^���N���b�N���̃C�x���g
    /// ��Ƃ̊J�n/�I��
    /// </summary>
    public void ToggleWork()
    {
        if (IsWorking)
            EndWork();
        else
            StartWork();
    }

    /// <summary>
    /// ��ƊJ�n����
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
    /// ��Ƃ̐i�s����
    /// </summary>
    private void ProceedCurrentWork()
    {
        int nowTotalSeconds = GetNowTotalSeconds();
        int elapsedSec = 0;

        currentWork.endUnixSec = nowTotalSeconds;

        // ��ƒ��̃��O�̍X�V
        mainUIDirector.PieChartCtrler.UpdateCurrentPie(currentWork);

        // ���[�^�[�̍X�V
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

        // ��Ǝ��ԕ\���̍X�V�i1���Ԃ𒴂����猅�𑝂₷�j
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
            // �|���h�[�� Rest�ւ̕ύX����
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
            // �|���h�[�� Active�ւ̕ύX����
            activeStart = _nowTotalSec;
            pomodoroState = PomodoroState.Active;
            mainUIDirector.CurrentWorkMeterCtrler.UpdateColor(
                currentProject.pieColor.GetWithColorFormat());
        }
    }


    /// <summary>
    /// ��Ƃ̏I������
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
    /// �ݒ�X�V�p
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
    /// �O������`��̍X�V���K�v�ȏꍇ�ɌĂ΂��
    /// isClock12h�̕ύX�ʒm
    /// </summary>
    public void UpdateCurrentPie()
    {
        if (!IsWorking) return;
        mainUIDirector.PieChartCtrler.UpdateCurrentPie(currentWork);
    }

    /// <summary>
    /// �O������
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

        // �v��Ȃ���
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
