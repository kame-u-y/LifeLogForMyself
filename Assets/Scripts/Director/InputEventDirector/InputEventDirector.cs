using HSVPicker;
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
    AppDirector appDirector;
    WindowDirector windowDirector;
    GeneralSettingsDirector generalSettingsDirector;
    #endregion

    #region main
    [SerializeField]
    Button playEndButton;
    //[SerializeField]
    //Button toggleClockModeButton;
    [SerializeField]
    Dropdown projectDropdown;
    [SerializeField]
    Button mainBackground;
    #endregion

    #region popup
    [SerializeField]
    PopupController popupController;
    Button popupBackground;
    // main menu
    Button mainButton;
    Button settingsButton;
    Button quitButton;
    // color picker
    ColorPicker colorPicker;
    // delete
    Button deleteCancelButton;
    Button deleteApplyButton;
    // addition
    //InputField newProjectName;
    //Button newProjectColorPicker;
    //Dropdown newProjectNotificationMode;
    //Button additionCancelButton;
    //Button additionApplyButton;
    #endregion

    #region settings
    [SerializeField]
    Button settingsBackground;
    [SerializeField]
    GameObject settingsTab;
    Button generalTabButton;
    Button projectsTabButton;
    [SerializeField]
    GameObject generalSettings;
    InputField progressMeterMax;

    [SerializeField]
    Button notificationSoundPath;
    [SerializeField]
    ToggleGroup settingResizingMode;
    [SerializeField]
    GameObject resizingValueForms;

    InputField twoResizingSmall;
    InputField twoResizingMedium;
    InputField threeResizingSmall;
    InputField threeResizingMedium;
    InputField threeResizingLarge;

    [SerializeField]
    ProjectSettingsController projectSettingsController;
    [SerializeField]
    Button projectAdditionButton;
    // apply
    Button generalSettingsRevert;
    Button generalSettingsApply;
    [SerializeField]
    GameObject projectSettings;
    Button projectSettingsRevert;
    Button projectSettingsApply;
    #endregion

    private void Awake()
    {
        myInputActions = new MyInputActions();
        InitializeActionMap(myInputActions.UI);

        #region load director
        workingDirector = GameObject.Find("WorkingDirector").GetComponent<WorkingDirector>();
        appDirector = GameObject.Find("AppDirector").GetComponent<AppDirector>();
        windowDirector = GameObject.Find("WindowDirector").GetComponent<WindowDirector>();
        generalSettingsDirector = GameObject.Find("GeneralSettingsDirector").GetComponent<GeneralSettingsDirector>();
        #endregion

        #region popup
        popupBackground = popupController.transform
            .Find("PopupBackground").GetComponent<Button>();
        // main menu
        mainButton = popupController.transform
            .Find("Popup/MainMenu/MainButton").GetComponent<Button>();
        settingsButton = popupController.transform
            .Find("Popup/MainMenu/SettingsButton").GetComponent<Button>();
        quitButton = popupController.transform
            .Find("Popup/MainMenu/QuitButton").GetComponent<Button>();
        // color picker
        colorPicker = popupController.transform
            .Find("Popup/ProjectColorPicker/Picker 2.0").GetComponent<ColorPicker>();
        // delete
        deleteCancelButton = popupController.transform
            .Find("Popup/ProjectDelete/Buttons/CancelButton").GetComponent<Button>();
        deleteApplyButton = popupController.transform
            .Find("Popup/ProjectDelete/Buttons/DeleteButton").GetComponent<Button>();
        //// addition
        //newProjectName = popupController.transform
        //    .Find("Popup/ProjectAddition/ProjectDataForms/ProjectName/ItemValue/InputField").GetComponent<InputField>();
        //newProjectColorPicker = popupController.transform
        //    .Find("Popup/ProjectAddition/ProjectDataForms/ProjectColor/ItemValue/Button").GetComponent<Button>();
        //newProjectNotificationMode = popupController.transform
        //    .Find("Popup/ProjectAddition/ProjectDataForms/NotificationMode/ItemValue/DropDown").GetComponent<Dropdown>();

        #endregion

        #region settings
        generalTabButton = settingsTab.transform.Find("General").GetComponent<Button>();
        projectsTabButton = settingsTab.transform.Find("Projects").GetComponent<Button>();
        progressMeterMax = generalSettings.transform
            .Find("ProgressBarMax/ItemValue/InputField").GetComponent<InputField>();

        string topScope = "ItemValue";
        string bottomScope = "ItemValue/InputField";
        Func<string, InputField> access = (string s)
            => resizingValueForms.transform.Find($"{topScope}/{s}/{bottomScope}").GetComponent<InputField>();
        twoResizingSmall = access("TwoStages/Small");
        twoResizingMedium = access("TwoStages/Medium");
        threeResizingSmall = access("ThreeStages/Small");
        threeResizingMedium = access("ThreeStages/Medium");
        threeResizingLarge = access("ThreeStages/Large");

        generalSettingsRevert = generalSettings.transform
            .Find("DecisionButton/Revert").GetComponent<Button>();
        generalSettingsApply = generalSettings.transform
            .Find("DecisionButton/Apply").GetComponent<Button>();
        projectSettingsRevert = projectSettings.transform
           .Find("DecisionButton/Revert").GetComponent<Button>();
        projectSettingsApply = projectSettings.transform
            .Find("DecisionButton/Apply").GetComponent<Button>();
        #endregion
    }

    // Start is called before the first frame update
    private void Start()
    {
        #region main
        playEndButton.onClick.AddListener(workingDirector.ToggleWork);
        //toggleClockModeButton.onClick.AddListener(appDirector.ChangeClockMode);
        projectDropdown.onValueChanged.AddListener(
            (v) => workingDirector.ChangeProjectOfCurrentWork());
        mainBackground.onClick.AddListener(windowDirector.OnResizingButtonClick);
        #endregion

        #region drag
        AddDragEvent(mainBackground.gameObject);
        AddDragEvent(settingsBackground.gameObject);
        #endregion

        #region popup
        myInputActions.UI.RightClick.performed +=
            (_context) => popupController.OpenPopup(PopupController.PopupMode.MainMenu);
        popupBackground.onClick.AddListener(popupController.ClosePopup);
        // main menu
        mainButton.onClick.AddListener(
             () =>
             {
                 generalSettingsDirector.RevertChanges();
                 appDirector.SwitchGameMode(AppDirector.GameMode.Main);
                 popupController.ClosePopup();
             });
        settingsButton.onClick.AddListener(
             () =>
             {
                 appDirector.SwitchGameMode(AppDirector.GameMode.Settings);
                 popupController.ClosePopup();
             });
        quitButton.onClick.AddListener(appDirector.Quit);
        // color picker
        colorPicker.onValueChanged.AddListener(
            _color =>
            {
                projectSettingsController.UpdateProjectColor(
                    popupController.SelectedProjectId,
                    _color);
            });
        // delete
        deleteCancelButton.onClick.AddListener(popupController.ClosePopup);
        deleteApplyButton.onClick.AddListener(
            () =>
            {
                projectSettingsController.ApplyProjectDelete(
                    popupController.SelectedProjectId);
                popupController.ClosePopup();
            });
        #endregion

        #region settings
        generalTabButton.onClick.AddListener(
            () => generalSettingsDirector.SwitchSettingsMode(GeneralSettingsDirector.SettingsMode.General));
        projectsTabButton.onClick.AddListener(
            () => generalSettingsDirector.SwitchSettingsMode(GeneralSettingsDirector.SettingsMode.Projects));
        // general settings
        progressMeterMax.onValueChanged.AddListener(
            _v => generalSettingsDirector.UpdateProgressBarMax(float.Parse(_v)));
        notificationSoundPath.onClick.AddListener(
            () => generalSettingsDirector.UpdateNotificationSoundPath());
        Toggle[] toggles = settingResizingMode.GetComponentsInChildren<Toggle>();
        foreach (var toggle in toggles)
        {
            toggle.onValueChanged.AddListener(
                _v => generalSettingsDirector.UpdateResizingMode(_v));
        }

        twoResizingSmall.onValueChanged.AddListener(
            _v => generalSettingsDirector.UpdateTwoSmall(int.Parse(_v)));
        twoResizingMedium.onValueChanged.AddListener(
            _v => generalSettingsDirector.UpdateTwoMedium(int.Parse(_v)));

        threeResizingSmall.onValueChanged.AddListener(
            _v => generalSettingsDirector.UpdateThreeSmall(int.Parse(_v)));
        threeResizingMedium.onValueChanged.AddListener(
            _v => generalSettingsDirector.UpdateThreeMedium(int.Parse(_v)));
        threeResizingLarge.onValueChanged.AddListener(
            _v => generalSettingsDirector.UpdateThreeLarge(int.Parse(_v)));

        // projects settings
        projectAdditionButton.onClick.AddListener(
            () => projectSettingsController.AddNewProject());
        // apply
        generalSettingsRevert.onClick.AddListener(
            () => generalSettingsDirector.RevertChanges());
        generalSettingsApply.onClick.AddListener(
            () => generalSettingsDirector.ApplySettings());
        projectSettingsRevert.onClick.AddListener(
            () => projectSettingsController.RevertProjectChanges());
        projectSettingsApply.onClick.AddListener(
            () =>
            {
                Debug.Log("event");
                projectSettingsController.ApplyProjectChanges();
            });
        #endregion
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void AddDragEvent(GameObject _obj)
    {
        EventTrigger trigger = _obj.GetComponent<EventTrigger>();
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
        _item.transform.Find("Values/ProjectName").GetComponent<InputField>().onValueChanged.AddListener(
            _v => projectSettingsController.UpdateProjectName(_v, _id));
        _item.transform.Find("Values/ProjectColor").GetComponent<Button>().onClick.AddListener(
            () => popupController.OpenProjectColorPickerPopup(PopupController.PopupMode.ProjectColorPicker, _id));
        _item.transform.Find("Values/NotificationMode").GetComponent<Dropdown>().onValueChanged.AddListener(
            _v => projectSettingsController.UpdateNotificationMode(_id));
        _item.transform.Find("Values/MoveUpper/Button").GetComponent<Button>().onClick.AddListener(
            () => projectSettingsController.MoveUpperItem(_id));
        _item.transform.Find("Values/MoveLower/Button").GetComponent<Button>().onClick.AddListener(
            () => projectSettingsController.MoveLowerItem(_id));
        _item.transform.Find("Values/Delete/Button").GetComponent<Button>().onClick.AddListener(
            () => popupController.OpenProjectDeletePopup(PopupController.PopupMode.ProjectDelete, _id));
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
