using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Candlelight;
using System;

public class GameDirector : MonoBehaviour
{
    [HideInInspector]
    public bool isClock12h = false;

    private WorkingDirector workingDirector;
    [SerializeField]
    ClockLabelController clockLabelController;
    [SerializeField]
    ClockModeButtonController clockModeButtonController;


    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        workingDirector = GameObject.Find("WorkingDirector").GetComponent<WorkingDirector>();
        workingDirector.CallForNeedDisplayTodayPieChart();
        workingDirector.CallForNeedUpdateCurrentWorkPiece();
        clockLabelController.ChangeClockLabels(isClock12h);
        clockModeButtonController.ChangeButtonColor(isClock12h);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeClockMode()
    {
        isClock12h = !isClock12h;

        workingDirector.CallForNeedDisplayTodayPieChart();
        workingDirector.CallForNeedUpdateCurrentWorkPiece();
        clockLabelController.ChangeClockLabels(isClock12h);
        clockModeButtonController.ChangeButtonColor(isClock12h);
    }

    public int GetSecondOfClockStart()
        => isClock12h && !IsAm()
        ? GetSecondOfToday(12, 0, 0)
        : GetSecondOfToday(0, 0, 0);

    public int GetSecondOfClockEnd()
        => isClock12h && IsAm()
        ? GetSecondOfToday(11, 59, 59)
        : GetSecondOfToday(23, 59, 59);


    private bool IsAm()
        => DateTime.Now.Hour < 12;

    private int GetSecondOfToday(int _h, int _m, int _s)
        => (int)new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, _h, _m, _s)
            .Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
}
