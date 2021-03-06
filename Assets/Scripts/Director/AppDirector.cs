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
    private SettingsUIDirector settingsUIDirector;

    [SerializeField]
    ClockLabelController clockLabelController;
    //[SerializeField]
    //ClockModeButtonController clockModeButtonController;
    //[SerializeField]
    //TodayPastPlateController todayPastPlateController;
    [SerializeField]
    TodayFuturePlateController todayFuturePlateController;
    [SerializeField]
    CurrentWorkMeterController currentWorkMeterController;

    [SerializeField]
    GameObject mainContainer;
    [SerializeField]
    GameObject watchLogContainer;
    [SerializeField]
    GameObject settingsContainer;
    [SerializeField]
    GameObject buttonContainer;

    Button mainButton;
    Button watchLogButton;
    Button settingsButton;


    public enum GameMode
    {
        Main,
        Settings,
        WatchLog,
        Quit,
    }

    private GameMode currentGameMode;
    public GameMode CurrentGameMode => currentGameMode;

    public enum SettingsMode
    {
        General,
        Projects
    }

    private SettingsMode currentSettingsMode;
    public SettingsMode CurrentSettingsMode => currentSettingsMode;


    //private static AppDirector instance;
    //public static AppDirector Instance {

    //}

    new private void Awake()
    {
        base.Awake();

        workingDirector = WorkingDirector.Instance;
        settingsUIDirector = SettingsUIDirector.Instance;
    }

    // Start is called before the first frame update
    void Start()
    {

        UpdateClockElements();

        mainButton = buttonContainer.transform.Find("MainButton").GetComponent<Button>();
        watchLogButton = buttonContainer.transform.Find("WatchLogButton").GetComponent<Button>();
        settingsButton = buttonContainer.transform.Find("SettingsButton").GetComponent<Button>();

        SwitchGameMode(GameMode.Main);
        SwitchSettingsMode(SettingsMode.General);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SwitchGameMode(GameMode _mode)
    {
        currentGameMode = _mode;

        if (_mode == GameMode.Main)
        {
            mainContainer.transform.localScale = Vector3.one;
            watchLogContainer.transform.localScale = Vector3.zero;
            settingsContainer.transform.localScale = Vector3.zero;

            bool b = true;
            mainButton.interactable = !b;
            watchLogButton.interactable = b;
            settingsButton.interactable = b;
        }
        else if (_mode == GameMode.Settings)
        {
            mainContainer.transform.localScale = Vector3.zero;
            watchLogContainer.transform.localScale = Vector3.zero;
            settingsContainer.transform.localScale = Vector3.one;

            bool b = true;
            mainButton.interactable = b;
            watchLogButton.interactable = b;
            settingsButton.interactable = !b;
        }
        else if (_mode == GameMode.WatchLog)
        {
            mainContainer.transform.localScale = Vector3.zero;
            watchLogContainer.transform.localScale = Vector3.one;
            settingsContainer.transform.localScale = Vector3.zero;

            bool b = true;
            mainButton.interactable = b;
            watchLogButton.interactable = !b;
            settingsButton.interactable = b;
        }
        else if (_mode == GameMode.Quit)
        {
            Quit();
        }
    }


    public void SwitchSettingsMode(SettingsMode _mode)
    {
        currentSettingsMode = _mode;

        if (_mode == SettingsMode.General)
        {
            bool b = true;
            settingsUIDirector.GeneralSettingContainer.SetActive(b);
            settingsUIDirector.ProjectsSettingContainer.SetActive(!b);

            settingsUIDirector.GeneralTabButton.interactable = !b;
            settingsUIDirector.ProjectsTabButton.interactable = b;

            //IsAnySettingsChanged = false;
        }
        else if (_mode == SettingsMode.Projects)
        {
            bool b = true;
            settingsUIDirector.GeneralSettingContainer.SetActive(!b);
            settingsUIDirector.ProjectsSettingContainer.SetActive(b);

            settingsUIDirector.GeneralTabButton.interactable = b;
            settingsUIDirector.ProjectsTabButton.interactable = !b;

            //SetAnySettingsChanged(false);
        }
    }

    /// <summary>
    /// ApplySettings??popup?????p
    /// settings?????X?????????????K?p?m?F
    /// </summary>
    /// <param name="_destMode"></param>
    public void ApplySettingsAndSwitchMode(GameMode _destMode)
    {
        SettingsMode destSettings = SettingsMode.General;

        if (currentSettingsMode == SettingsMode.General)
        {
            GeneralSettingsDirector.Instance.ApplySettings();
            destSettings = SettingsMode.Projects;
        }
        else if (currentSettingsMode == SettingsMode.Projects)
        {
            ProjectSettingsDirector.Instance.ApplyProjectChanges();
            destSettings = SettingsMode.General;
        }

        SwitchGameMode(_destMode);
        if (_destMode == GameMode.Settings)
        {
            SwitchSettingsMode(destSettings);
        }
    }

    public void ChangeClockMode()
    {
        isClock12h = !isClock12h;
        UpdateClockElements();
    }

    private void UpdateClockElements()
    {
        workingDirector.UpdateLogPieChart();
        workingDirector.UpdateCurrentPie();
        clockLabelController.ChangeClockLabels(isClock12h);
        //clockModeButtonController.ChangeButtonColor(isClock12h);
        //todayPastPlateController.UpdatePlate();
        todayFuturePlateController.UpdatePlate();
    }

    public void UpdateNotificationSound(string _path)
    {
        currentWorkMeterController.UpdateNotificationSound(_path);
    }

    /// <summary>
    /// ?????????Oor?????????v???n??????unixSec??????
    /// </summary>
    /// <returns></returns>
    public int GetSecondOfClockStart()
        => isClock12h && !IsAm()
        ? GetSecondOfToday(12, 0, 0)
        : GetSecondOfToday(0, 0, 0);

    /// <summary>
    /// ?????????Oor?????????v???I??????unixSec??????
    /// </summary>
    /// <returns></returns>
    public int GetSecondOfClockEnd()
        => isClock12h && IsAm()
        ? GetSecondOfToday(11, 59, 59)
        : GetSecondOfToday(23, 59, 59);

    /// <summary>
    /// ??????unixSec??????
    /// </summary>
    /// <returns></returns>
    public int GetSecondOfNow()
        => (int)DateTime.Now
        .Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

    /// <summary>
    /// ???v?????O(24h??????12h)??unixSec??????
    /// </summary>
    /// <returns></returns>
    public int GetSecondOfOneClockLapAgo()
        => isClock12h
        ? GetSecondOfAddDaysFromNow(-0.5)
        : GetSecondOfAddDaysFromNow(-1);



    public bool IsAm()
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
