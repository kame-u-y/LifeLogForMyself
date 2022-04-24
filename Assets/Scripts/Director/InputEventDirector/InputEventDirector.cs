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
    [SerializeField]
    Dropdown projectDropdown;
    [SerializeField]
    Button mainBackground;
    #endregion

    #region popup
    [SerializeField]
    PopupController popupController;
    #endregion

    #region settings
    [SerializeField]
    Button settingsBackground;
    [SerializeField]
    GameObject settingsTab;
    [SerializeField]
    GameObject generalSettings;

    [SerializeField]
    Button notificationSoundPath;
    [SerializeField]
    ToggleGroup settingResizingMode;
    [SerializeField]
    GameObject resizingValueForms;


    [SerializeField]
    ProjectSettingsController projectSettingsController;
    [SerializeField]
    Button projectAdditionButton;
    [SerializeField]
    GameObject projectSettings;
    #endregion

    private static InputEventDirector instance;
    public static InputEventDirector Instance => instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        myInputActions = new MyInputActions();
        InitializeActionMap(myInputActions.UI);

        #region load director
        workingDirector = WorkingDirector.Instance;
        appDirector = AppDirector.Instance;
        windowDirector = WindowDirector.Instance;
        generalSettingsDirector = GeneralSettingsDirector.Instance;
        #endregion
    }

    // Start is called before the first frame update
    private void Start()
    {
        #region main
        playEndButton.onClick.AddListener(workingDirector.ToggleWork);
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

        popupController.transform.Find("PopupBackground")
            .GetComponent<Button>().onClick.AddListener(popupController.ClosePopup);

        // main menu
        popupController.transform.Find("Popup/MainMenu/MainButton")
            .GetComponent<Button>().onClick.AddListener(
             () =>
             {
                 generalSettingsDirector.RevertChanges();
                 appDirector.SwitchGameMode(AppDirector.GameMode.Main);
                 popupController.ClosePopup();
             });

        popupController.transform.Find("Popup/MainMenu/SettingsButton")
            .GetComponent<Button>().onClick.AddListener(
             () =>
             {
                 appDirector.SwitchGameMode(AppDirector.GameMode.Settings);
                 popupController.ClosePopup();
             });

        popupController.transform.Find("Popup/MainMenu/QuitButton")
            .GetComponent<Button>().onClick.AddListener(appDirector.Quit);

        // color picker
        popupController.transform.Find("Popup/ProjectColorPicker/Picker 2.0")
            .GetComponent<ColorPicker>().onValueChanged.AddListener(
            _color =>
            {
                projectSettingsController.UpdateProjectColor(
                    popupController.SelectedProjectId,
                    _color);
            });
        // delete
        popupController.transform.Find("Popup/ProjectDelete/Buttons/CancelButton")
            .GetComponent<Button>().onClick.AddListener(popupController.ClosePopup);

        popupController.transform.Find("Popup/ProjectDelete/Buttons/DeleteButton")
            .GetComponent<Button>().onClick.AddListener(
            () =>
            {
                projectSettingsController.ApplyProjectDelete(
                    popupController.SelectedProjectId);
                popupController.ClosePopup();
            });
        #endregion

        #region settings
        settingsTab.transform.Find("General").GetComponent<Button>().onClick.AddListener(
            () => generalSettingsDirector.SwitchSettingsMode(GeneralSettingsDirector.SettingsMode.General));

        settingsTab.transform.Find("Projects").GetComponent<Button>().onClick.AddListener(
            () => generalSettingsDirector.SwitchSettingsMode(GeneralSettingsDirector.SettingsMode.Projects));

        // general settings
        generalSettings.transform.Find("ProgressBarMax/ItemValue/InputField").GetComponent<InputField>()
            .onValueChanged.AddListener(
            _v => generalSettingsDirector.UpdateProgressBarMax(float.Parse(_v)));

        notificationSoundPath.onClick.AddListener(
            () => generalSettingsDirector.UpdateNotificationSoundPath());

        Toggle[] toggles = settingResizingMode.GetComponentsInChildren<Toggle>();
        foreach (var toggle in toggles)
        {
            toggle.onValueChanged.AddListener(
                _v => generalSettingsDirector.UpdateResizingMode(_v));
        }

        string topScope = "ItemValue";
        string bottomScope = "ItemValue/InputField";
        Func<string, InputField> access = (string s)
            => resizingValueForms.transform.Find($"{topScope}/{s}/{bottomScope}").GetComponent<InputField>();
        
        access("TwoStages/Small").onValueChanged.AddListener(
            _v => generalSettingsDirector.UpdateTwoSmall(int.Parse(_v)));
        access("TwoStages/Medium").onValueChanged.AddListener(
            _v => generalSettingsDirector.UpdateTwoMedium(int.Parse(_v)));

        access("ThreeStages/Small").onValueChanged.AddListener(
            _v => generalSettingsDirector.UpdateThreeSmall(int.Parse(_v)));
        access("ThreeStages/Medium").onValueChanged.AddListener(
            _v => generalSettingsDirector.UpdateThreeMedium(int.Parse(_v)));
        access("ThreeStages/Large").onValueChanged.AddListener(
            _v => generalSettingsDirector.UpdateThreeLarge(int.Parse(_v)));

        // projects settings
        projectAdditionButton.onClick.AddListener(
            () => projectSettingsController.AddNewProject());
        // apply
        generalSettings.transform.Find("DecisionButton/Revert")
            .GetComponent<Button>().onClick.AddListener(
            () => generalSettingsDirector.RevertChanges());
        
        generalSettings.transform.Find("DecisionButton/Apply")
            .GetComponent<Button>().onClick.AddListener(
            () => generalSettingsDirector.ApplySettings());
        
        projectSettings.transform.Find("DecisionButton/Revert")
           .GetComponent<Button>().onClick.AddListener(
            () => projectSettingsController.RevertProjectChanges());
        
        projectSettings.transform.Find("DecisionButton/Apply")
            .GetComponent<Button>().onClick.AddListener(
            () => projectSettingsController.ApplyProjectChanges());
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
