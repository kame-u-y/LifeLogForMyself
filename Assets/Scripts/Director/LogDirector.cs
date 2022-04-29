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
        UpdateLogPieChart();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateLogPieChart()
    {
        List<WorkData> dayData = databaseDirector.FetchDayData(displayLogOffset);
        appDirector.isClock12h = false;
        if (dayData != null)
        {
            List<ProjectData> project = databaseDirector.FetchProjectList();
            logUIDirector.LogPieChartCtrler.CreateLogPieChart(dayData, project);
        }
    }

    private void UpdateLogDateLabel()
    {
        string d = DateTime.Today.AddDays(displayLogOffset).ToString("yyyy / MM / dd");
        logUIDirector.LogDateTMP.text = d;
    }

    public void DisplayPrevious()
    {
        displayLogOffset--;
        UpdateLogDateLabel();
        UpdateLogPieChart();
    }

    public void DisplayNext()
    {
        displayLogOffset++;
        UpdateLogDateLabel();
        UpdateLogPieChart();
    }


}
