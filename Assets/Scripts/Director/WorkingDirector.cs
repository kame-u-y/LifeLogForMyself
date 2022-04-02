using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    DatabaseDirector databaseDirector;

    void Awake()
    {
        databaseDirector 
            = GameObject.Find("DatabaseDirector").GetComponent<DatabaseDirector>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // �Đ��������o�o������ʔ����������ǁA�܂��A�J�E���g����ƂȂ�񂭂Ȃ��
        isWorking = false;
        playEndImage.ChangeButtonImage(isWorking);

        DayData dayData = databaseDirector.FetchDayData(DateTime.Now.ToString("yyyyMMdd"));
        List<ProjectData> project = databaseDirector.FetchProjectList();
        pieChartController.DisplayTodayPieChart(dayData, project);
    }

    // Update is called once per frame
    void Update()
    {
        if (isWorking)
        {
            // 1�b���ƂɍX�V
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
    /// �O������`��̍X�V���K�v�ȏꍇ�ɌĂ΂��
    /// isClock12h�̕ύX�ʒm
    /// </summary>
    public void CallForNeedUpdateCurrentWorkPiece()
    {
        if (!isWorking) return;
        pieChartController.UpdateCurrentWorkPiece(currentWork);
    }

    public void CallForNeedDisplayTodayPieChart()
    {
        DayData dayData = databaseDirector.FetchDayData(DateTime.Now.ToString("yyyyMMdd"));
        List<ProjectData> project = databaseDirector.FetchProjectList();
        pieChartController.DisplayTodayPieChart(dayData, project);
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
            projectName = "���"
        };
        currentProject = databaseDirector.FindProject(currentWork.projectName);

        playEndImage.ChangeButtonImage(isWorking);
        pieChartController.CreateCurrentWorkPiece(currentWork, currentProject);

        time = 0;
    }

    private void EndWork()
    {
        isWorking = false;
        // todo: �f�[�^���L�^
        databaseDirector.AddEndedWork(currentWork);

        playEndImage.ChangeButtonImage(isWorking);
        time = 0;
    }

    private int GetNowTotalSeconds()
        => (int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
}
