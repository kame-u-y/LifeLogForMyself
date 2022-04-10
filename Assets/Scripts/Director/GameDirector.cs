using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Candlelight;
using System;
using System.Runtime.InteropServices;
using System.Drawing;
using UnityEngine.EventSystems;

public class GameDirector : MonoBehaviour
{
    [HideInInspector]
    public bool isClock12h = false;

    private WorkingDirector workingDirector;
    [SerializeField]
    ClockLabelController clockLabelController;
    [SerializeField]
    ClockModeButtonController clockModeButtonController;
    [SerializeField]
    TodayPastPlateController todayPastPlateController;


    private void Awake()
    {
        workingDirector = GameObject.Find("WorkingDirector").GetComponent<WorkingDirector>();
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateClockElements();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeClockMode()
    {
        isClock12h = !isClock12h;
        UpdateClockElements();
    }

    private void UpdateClockElements()
    {
        workingDirector.CallForNeedDisplayTodayPieChart();
        workingDirector.CallForNeedUpdateCurrentWorkPiece();
        clockLabelController.ChangeClockLabels(isClock12h);
        clockModeButtonController.ChangeButtonColor(isClock12h);
        todayPastPlateController.UpdatePastPlate();
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
