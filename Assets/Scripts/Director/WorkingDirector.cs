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
    /// ��ƒ��A���Ԋu���ƂɎ��s���邽�߂̃J�E���g
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
        // �Đ��������o�o������ʔ����������ǁA�܂��A�J�E���g����ƂȂ�񂭂Ȃ��
        IsWorking = false;
        // �Đ�/��~�{�^���̕\��������
        mainUIDirector.PlayEndImageCtrler.ChangeButtonImage(IsWorking);
        // ���O�̕\��
        List<WorkData> dayData = databaseDirector.Fetch24hData();
        if (dayData != null)
        {
            List<ProjectData> project = databaseDirector.FetchProjectList();
            mainUIDirector.PieChartCtrler.UpdateLogPieChart(dayData, project);
        }
        // ��Ǝ��Ԃ̕\��
        mainUIDirector.CurrentCountTMP.text = "00:00";
    }

    // Update is called once per frame
    void Update()
    {
        if (IsWorking)
        {
            // 1�b���ƂɍX�V
            if (time >= 1.0f)
            {
                ProceedCurrentWork();
                time = 0.0f;
            }
            time += Time.deltaTime;
        }
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

        time = 0;
    }

    /// <summary>
    /// ��Ƃ̐i�s����
    /// </summary>
    private void ProceedCurrentWork()
    {
        currentWork.endUnixSec = GetNowTotalSeconds();

        // �o�ߎ���
        int elapsed = currentWork.endUnixSec - currentWork.startUnixSec;
        print("now:" + elapsed);

        // ��ƒ��̃��O�̍X�V
        mainUIDirector.PieChartCtrler.UpdateCurrentPie(currentWork);
        // ���[�^�[�̍X�V
        mainUIDirector.CurrentWorkMeterCtrler.UpdateMeter(elapsed);
        // ��Ǝ��ԕ\���̍X�V�i1���Ԃ𒴂����猅�𑝂₷�j
        int h = elapsed / 3600;
        int m = (elapsed % 3600) / 60;
        int s = elapsed % 60;
        mainUIDirector.CurrentCountTMP.text = h > 0
            ? $"{String.Format("{0:00}", h)}:{String.Format("{0:00}", m)}:{String.Format("{0:00}", s)}"
            : $"{String.Format("{0:00}", m)}:{String.Format("{0:00}", s)}";
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
        mainUIDirector.PieChartCtrler.EndCurrentWork();
        
        time = 0;
    }



    /// <summary>
    /// �ݒ�X�V�p
    /// </summary>
    /// <param name="_maxMinute"></param>
    public void UpdateWorkMeterMax(float _maxMinute)
    {
        int elapsed = currentWork.endUnixSec - currentWork.startUnixSec;
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
