using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Candlelight;
using System;
using System.Runtime.InteropServices;
using System.Drawing;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AppDirector : SingletonMonoBehaviourFast<AppDirector>
{
    [SerializeField]
    private bool debugMode = false;
    public bool DebugMode
    {
        get => debugMode;
    }

    [HideInInspector]
    public bool isClock12h = false;

    private WorkingDirector workingDirector;
    [SerializeField]
    ClockLabelController clockLabelController;
    //[SerializeField]
    //ClockModeButtonController clockModeButtonController;
    [SerializeField]
    TodayPastPlateController todayPastPlateController;
    [SerializeField]
    TodayFuturePlateController todayFuturePlateController;
    [SerializeField]
    CurrentWorkMeterController currentWorkMeterController;

    [SerializeField]
    GameObject mainContainer;
    [SerializeField]
    GameObject settingsContainer;
    [SerializeField]
    GameObject buttonContainer;
    Button mainButton;
    Button settingsButton;


    public enum GameMode
    {
        Main,
        Settings,
        WatchLog
    }

    //private static AppDirector instance;
    //public static AppDirector Instance {
        
    //}

    new private void Awake()
    {
        base.Awake();

    }

    // Start is called before the first frame update
    void Start()
    {
        workingDirector = WorkingDirector.Instance;

        UpdateClockElements();

        mainButton = buttonContainer.transform.Find("MainButton").GetComponent<Button>();
        settingsButton = buttonContainer.transform.Find("SettingsButton").GetComponent<Button>();
        
        SwitchGameMode(GameMode.Main);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SwitchGameMode(GameMode _mode)
    {
        if (_mode == GameMode.Main)
        {
            bool b = true;
            mainContainer.transform.localScale = Vector3.one;
            settingsContainer.transform.localScale = Vector3.zero;
            // watchLogContainer.SetActive(false);

            mainButton.interactable = !b;
            settingsButton.interactable = b;
        }
        else if (_mode == GameMode.Settings)
        {
            bool b = true;
            mainContainer.transform.localScale = Vector3.zero;
            settingsContainer.transform.localScale = Vector3.one;

            // watchLogContainer.SetActive(false);

            mainButton.interactable = b;
            settingsButton.interactable = !b;
        }
        else if (_mode == GameMode.WatchLog)
        {
            bool b = true;
            mainContainer.transform.localScale = Vector3.zero;
            settingsContainer.transform.localScale = Vector3.zero;
            // watchLogContainer.SetActive(true);

            mainButton.interactable = b;
            settingsButton.interactable = b;
        }
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
        //clockModeButtonController.ChangeButtonColor(isClock12h);
        todayPastPlateController.UpdatePlate();
        todayFuturePlateController.UpdatePlate();
    }

    public void UpdateNotificationSound(string _path)
    {
        currentWorkMeterController.UpdateNotificationSound(_path);
    }

    /// <summary>
    /// 今日の午前or午後の時計の始まりのunixSecを取得
    /// </summary>
    /// <returns></returns>
    public int GetSecondOfClockStart()
        => isClock12h && !IsAm()
        ? GetSecondOfToday(12, 0, 0)
        : GetSecondOfToday(0, 0, 0);

    /// <summary>
    /// 今日の午前or午後の時計の終わりのunixSecを取得
    /// </summary>
    /// <returns></returns>
    public int GetSecondOfClockEnd()
        => isClock12h && IsAm()
        ? GetSecondOfToday(11, 59, 59)
        : GetSecondOfToday(23, 59, 59);

    /// <summary>
    /// 現在のunixSecを取得
    /// </summary>
    /// <returns></returns>
    public int GetSecondOfNow()
        => (int) DateTime.Now
        .Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

    /// <summary>
    /// 時計一周前(24hまたは12h)のunixSecを取得
    /// </summary>
    /// <returns></returns>
    public int GetSecondOfOneClockLapAgo()
        => isClock12h
        ? GetSecondOfAddDaysFromNow(-0.5)
        : GetSecondOfAddDaysFromNow(-1);
        


    private bool IsAm()
        => DateTime.Now.Hour < 12;

    private int GetSecondOfToday(int _h, int _m, int _s)
        => (int)new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, _h, _m, _s)
            .Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

    private int GetSecondOfAddDaysFromNow(double _offset)
        => (int)DateTime.Now.AddDays(_offset)
        .Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

    public void Quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_STANDALONE
            UnityEngine.Application.Quit();
        #endif
    }
}
