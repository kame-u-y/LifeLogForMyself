using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogDirector : SingletonMonoBehaviourFast<LogDirector>
{
    AppDirector appDirector;
    DatabaseDirector databaseDirector;
    LogUIDirector logUIDirector;

    int displayLogOffset = -1;

    new void Awake()
    {
        base.Awake();
        appDirector = AppDirector.Instance;
        databaseDirector = DatabaseDirector.Instance;
        logUIDirector = LogUIDirector.Instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateLog();
        UpdateLogDateLabel();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DisplayPrevious()
    {
        displayLogOffset--;

        UpdateLog();
        UpdateLogDateLabel();
    }

    public void DisplayNext()
    {
        displayLogOffset++;

        UpdateLog();
        UpdateLogDateLabel();
    }


    private void UpdateLog()
    {
        List<WorkData> dayData = databaseDirector.FetchDayData(displayLogOffset);
        appDirector.isClock12h = false;
        if (dayData != null)
        {
            List<ProjectData> projects = databaseDirector.FetchProjectList();
            logUIDirector.ClockLogCtrler.CreateLogPieChart(dayData, projects);
            logUIDirector.LogPieChartCtrler.CreateLogPieChart(dayData, projects);
        }
    }

    private void UpdateLogDateLabel()
    {
        string d = DateTime.Today.AddDays(displayLogOffset).ToString("yyyy / MM / dd");
        logUIDirector.LogDateTMP.text = d;
    }


}
