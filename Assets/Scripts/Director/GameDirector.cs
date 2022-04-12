using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Candlelight;
using System;
using System.Runtime.InteropServices;
using System.Drawing;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    private void Awake()
    {
        workingDirector = GameObject.Find("WorkingDirector").GetComponent<WorkingDirector>();
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateClockElements();
        mainContainer.SetActive(true);
        settingsContainer.SetActive(false);

        mainButton = buttonContainer.transform.Find("MainButton").GetComponent<Button>();
        settingsButton = buttonContainer.transform.Find("SettingsButton").GetComponent<Button>();
        mainButton.interactable = false;
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
            mainContainer.SetActive(b);
            settingsContainer.SetActive(!b);
            // watchLogContainer.SetActive(false);

            mainButton.interactable = !b;
            settingsButton.interactable = b;
        }
        else if (_mode == GameMode.Settings)
        {
            bool b = true;
            mainContainer.SetActive(!b);
            settingsContainer.SetActive(b);
            // watchLogContainer.SetActive(false);

            mainButton.interactable = b;
            settingsButton.interactable = !b;
        }
        else if (_mode == GameMode.WatchLog)
        {
            bool b = true;
            mainContainer.SetActive(!b);
            settingsContainer.SetActive(!b);
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


    public void Quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_STANDALONE
            UnityEngine.Application.Quit();
        #endif
    }
}
