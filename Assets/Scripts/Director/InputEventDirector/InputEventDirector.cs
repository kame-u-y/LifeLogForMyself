using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputEventDirector : MonoBehaviour
{
    private MyInputActions myInputActions;

    #region director
    WorkingDirector workingDirector;
    GameDirector gameDirector;
    WindowDirector windowDirector;
    SettingsDirector settingsDirector;
    #endregion

    #region main
    [SerializeField]
    Button playEndButton;
    [SerializeField]
    Button toggleClockModeButton;
    [SerializeField]
    Dropdown projectDropdown;
    [SerializeField]
    Button background;
    #endregion

    #region popup
    [SerializeField]
    PopupController popupController;
    Button popupBackground;
    Button mainButton;
    Button settingsButton;
    Button quitButton;
    #endregion

    #region settings
    [SerializeField]
    GameObject settingsTab;
    Button generalTabButton;
    Button projectsTabButton;
    [SerializeField]
    GameObject generalSettings;
    InputField progressBarMax;

    [SerializeField]
    ToggleGroup settingResizingMode;
    [SerializeField]
    GameObject resizingValueForms;
    InputField resizingSmall;
    InputField resizingMedium;
    InputField resizingLarge;

    [SerializeField]
    ProjectSettingsController projectSettingsController;

    [SerializeField]
    Button settingApplyButton;
    #endregion

    private void Awake()
    {
        myInputActions = new MyInputActions();
        InitializeActionMap(myInputActions.UI);

        #region load director
        workingDirector = GameObject.Find("WorkingDirector").GetComponent<WorkingDirector>();
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
        windowDirector = GameObject.Find("WindowDirector").GetComponent<WindowDirector>();
        settingsDirector = GameObject.Find("SettingsDirector").GetComponent<SettingsDirector>();
        #endregion

        #region popup
        popupBackground = popupController.transform.Find("PopupBackground").GetComponent<Button>();
        mainButton = popupController.transform.Find("Popup/ButtonContainer/MainButton").GetComponent<Button>();
        settingsButton = popupController.transform.Find("Popup/ButtonContainer/SettingsButton").GetComponent<Button>();
        quitButton = popupController.transform.Find("Popup/ButtonContainer/QuitButton").GetComponent<Button>();
        Debug.Log(quitButton);
        #endregion

        #region settings
        generalTabButton = settingsTab.transform.Find("General").GetComponent<Button>();
        projectsTabButton = settingsTab.transform.Find("Projects").GetComponent<Button>();
        progressBarMax = generalSettings.transform
            .Find("ProgressBarMax/ItemValue/InputField").GetComponent<InputField>();
        
        resizingSmall = resizingValueForms.transform
            .Find("Small/ItemValue/InputField").GetComponent<InputField>();
        resizingMedium = resizingValueForms.transform
            .Find("Medium/ItemValue/InputField").GetComponent<InputField>();
        resizingLarge = resizingValueForms.transform
            .Find("Large/ItemValue/InputField").GetComponent<InputField>();
        #endregion
    }

    // Start is called before the first frame update
    private void Start()
    {
        #region main
        playEndButton.onClick.AddListener(workingDirector.ToggleWork);
        toggleClockModeButton.onClick.AddListener(gameDirector.ChangeClockMode);
        projectDropdown.onValueChanged.AddListener(
            (v) => workingDirector.ChangeProjectOfCurrentWork());
        background.onClick.AddListener(windowDirector.OnResizingButtonClick);
        #endregion

        #region drag
        EventTrigger trigger = background.gameObject.GetComponent<EventTrigger>();
        AddEventTrigger(
            trigger,
            EventTriggerType.BeginDrag,
            (_eventData) => windowDirector.OnTargetBeginDrag(_eventData));
        AddEventTrigger(
            trigger,
            EventTriggerType.Drag,
            (_eventData) => windowDirector.OnTargetDrag(_eventData));
        AddEventTrigger(
            trigger,
            EventTriggerType.EndDrag,
            (_eventData) => windowDirector.OnTargetEndDrag(_eventData));
        #endregion

        #region popup
        myInputActions.UI.RightClick.performed += (_context) => popupController.OpenPopup(_context);
        popupBackground.onClick.AddListener(popupController.ClosePopup);
        mainButton.onClick.AddListener(
             () =>
             {
                 gameDirector.SwitchGameMode(GameDirector.GameMode.Main);
                 popupController.ClosePopup();
             });
        settingsButton.onClick.AddListener(
             () =>
             {
                 gameDirector.SwitchGameMode(GameDirector.GameMode.Settings);
                 popupController.ClosePopup();
             });
        quitButton.onClick.AddListener(gameDirector.Quit);
        #endregion

        #region settings
        generalTabButton.onClick.AddListener(
            () => settingsDirector.SwitchSettingsMode(SettingsDirector.SettingsMode.General));
        projectsTabButton.onClick.AddListener(
            () => settingsDirector.SwitchSettingsMode(SettingsDirector.SettingsMode.Projects));

        #region general settings
        progressBarMax.onEndEdit.AddListener(
            _v => settingsDirector.UpdateProgressBarMax(int.Parse(_v)));
        Toggle[] toggles = settingResizingMode.GetComponentsInChildren<Toggle>();
        foreach (var toggle in toggles)
        {
            toggle.onValueChanged.AddListener(
                _v => settingsDirector.UpdateResizingMode(_v));
        }
        resizingSmall.onEndEdit.AddListener(
            _v => settingsDirector.UpdateResizingSmall(int.Parse(_v)));
        resizingMedium.onEndEdit.AddListener(
            _v => settingsDirector.UpdateResizingMedium(int.Parse(_v)));
        resizingLarge.onEndEdit.AddListener(
            _v => settingsDirector.UpdateResizingLarge(int.Parse(_v)));
        
        settingApplyButton.onClick.AddListener(
            () => settingsDirector.ApplySettings());
        #endregion

        #region projects settings

        #endregion
        #endregion
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void AddEventTrigger(
       EventTrigger _trigger,
       EventTriggerType _type,
       UnityAction<BaseEventData> _callback)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = _type;
        entry.callback.AddListener(_callback);
        _trigger.triggers.Add(entry);
    }

    public Vector2 GetMousePosition()
        => myInputActions.UI.Point.ReadValue<Vector2>();

    public void AddProjectSettingItemEvents(GameObject _item, int _id)
    {
        _item.transform.Find("Values/ProjectName").GetComponent<InputField>().onEndEdit.AddListener(
            _v => projectSettingsController.UpdateProjectName(_v, _id));
        _item.transform.Find("Values/ProjectColor").GetComponent<Button>().onClick.AddListener(null);
        _item.transform.Find("Values/NotificationMode").GetComponent<Dropdown>().onValueChanged.AddListener(
            _v => projectSettingsController.UpdateNotificationMode(_id));
        _item.transform.Find("Values/MoveUpper/Button").GetComponent<Button>().onClick.AddListener(
            () => projectSettingsController.MoveUpperItem(_id));
        _item.transform.Find("Values/MoveLower/Button").GetComponent<Button>().onClick.AddListener(
            () => projectSettingsController.MoveLowerItem(_id));
        _item.transform.Find("Values/Delete/Button").GetComponent<Button>().onClick.AddListener(null);
    }

    #region SwitchMap
    //public void SwitchBackToPlayer()
    //{
    //    print("switch back to player");
    //    SwitchActionMap(myInputActions.Player);
    //}

    #endregion

    private void InitializeActionMap(InputActionMap _actionMap)
    {
        myInputActions.Disable();
        _actionMap.Enable();
    }

    public void SwitchActionMap(InputActionMap _actionMap)
    {
        Debug.Log(_actionMap);
        if (_actionMap.enabled) return;

        myInputActions.Disable();
        _actionMap.Enable();
    }
}
