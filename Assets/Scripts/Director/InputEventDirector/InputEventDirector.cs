using HSVPicker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputEventDirector : SingletonMonoBehaviourFast<InputEventDirector>
{
    private MyInputActions myInputActions;

    #region director
    private WorkingDirector workingDirector;
    private AppDirector appDirector;
    private WindowDirector windowDirector;
    private MainUIDirector mainUIDirector;
    private SettingsUIDirector settingsUIDirector;
    private GeneralSettingsDirector generalSettingsDirector;
    private ProjectSettingsDirector projectSettingsDirector;
    private PopupDirector popupDirector;
    private PopupUIDirector popupUIDirector;
    private LogDirector logDirector;
    private LogUIDirector logUIDirector;
    #endregion

    new void Awake()
    {
        base.Awake();

        myInputActions = new MyInputActions();
        InitializeActionMap(myInputActions.UI);

        #region load director
        workingDirector = WorkingDirector.Instance;
        appDirector = AppDirector.Instance;
        windowDirector = WindowDirector.Instance;
        mainUIDirector = MainUIDirector.Instance;
        settingsUIDirector = SettingsUIDirector.Instance;
        generalSettingsDirector = GeneralSettingsDirector.Instance;
        projectSettingsDirector = ProjectSettingsDirector.Instance;
        popupDirector = PopupDirector.Instance;
        popupUIDirector = PopupUIDirector.Instance;
        logDirector = LogDirector.Instance;
        logUIDirector = LogUIDirector.Instance;
        #endregion
    }

    // Start is called before the first frame update
    private void Start()
    {
        #region main
        mainUIDirector.PlayEndButton.onClick.AddListener(workingDirector.ToggleWork);
        mainUIDirector.ProjectDropdown.onValueChanged.AddListener(
            (v) => workingDirector.SwitchCurrentProject());
        mainUIDirector.BackgroundButton.onClick.AddListener(windowDirector.OnResizingButtonClick);
        #endregion

        #region drag
        AddDragEvent(mainUIDirector.BackgroundButton.gameObject);
        //AddDragEvent(settingsUIDirector.BackgroundButton.gameObject);
        #endregion

        #region popup
        myInputActions.UI.RightClick.performed +=
            (_context) => popupDirector.OpenPopup(PopupDirector.PopupMode.MainMenu);

        popupUIDirector.OuterBackgroundButton.onClick.AddListener(popupDirector.ClosePopup);

        // main menu

        // Settings???????????????f?????X???????????`?F?b?N????
        Func<bool> IsAnySettingsChanged = () =>
        {
            bool isSettingsMode = appDirector.CurrentGameMode == AppDirector.GameMode.Settings;
            bool isAnyGeneralSettingsChanged
               = appDirector.CurrentSettingsMode == AppDirector.SettingsMode.General
               && generalSettingsDirector.IsAnySettingsChanged;
            bool isAnyProjectSettingsChanged
               = appDirector.CurrentSettingsMode == AppDirector.SettingsMode.Projects
               && projectSettingsDirector.IsAnySettingsChanged;
            return isSettingsMode && (isAnyGeneralSettingsChanged || isAnyProjectSettingsChanged);
        };

        popupUIDirector.MainButton.onClick.AddListener(
             () =>
             {
                 // settings?????????????K?p?m?F
                 if (IsAnySettingsChanged())
                 {
                     popupDirector.OpenApplySettingsPopup(AppDirector.GameMode.Main);
                 }
                 else
                 {
                     //generalSettingsDirector.RevertChanges();
                     appDirector.SwitchGameMode(AppDirector.GameMode.Main);
                     popupDirector.ClosePopup();
                 }
             });

        popupUIDirector.WatchLogButton.onClick.AddListener(
            () =>
            {
                // settings?????????????K?p?m?F
                if (IsAnySettingsChanged())
                {
                    popupDirector.OpenApplySettingsPopup(AppDirector.GameMode.WatchLog);
                }
                else
                {
                    //generalSettingsDirector.RevertChanges();
                    appDirector.SwitchGameMode(AppDirector.GameMode.WatchLog);
                    popupDirector.ClosePopup();
                }
            });

        popupUIDirector.SettingButton.onClick.AddListener(
             () =>
             {
                 appDirector.SwitchGameMode(AppDirector.GameMode.Settings);
                 popupDirector.ClosePopup();
             });

        popupUIDirector.QuitButton.onClick.AddListener(
            () =>
            {
                // settings?????I???????K?p?m?F
                //if (IsAnySettingsChanged())
                //{
                //    popupDirector.OpenApplySettingsPopup(AppDirector.GameMode.Quit);
                //}
                //else
                //{
                    appDirector.Quit();
                //}
            });

        // color picker
        popupUIDirector.ProjectColorPicker.onValueChanged.AddListener(
            _color =>
            {
                projectSettingsDirector.UpdateProjectColor(
                    popupDirector.SelectedProjectId,
                    _color);
            });
        // delete
        popupUIDirector.ProjectDeleteCancelButton.onClick.AddListener(
            popupDirector.ClosePopup);

        popupUIDirector.ProjectDeleteButton.onClick.AddListener(
            () =>
            {
                projectSettingsDirector.ApplyProjectDelete(
                    popupDirector.SelectedProjectId);
                popupDirector.ClosePopup();
            });

        // alert before delete
        popupUIDirector.AlertBeforeDeleteCancelButton.onClick.AddListener(
            popupDirector.ClosePopup);
        popupUIDirector.AlertBeforeDeleteProceedButton.onClick.AddListener(
            () =>
            {
                bool result = projectSettingsDirector.CheckProjectDuplication();
                if (result)
                {
                    projectSettingsDirector.ApplyProjectChanges();
                    popupDirector.OpenPopup(PopupDirector.PopupMode.ProjectDelete);
                }
                else
                {
                    //popupDirector.ClosePopup();
                    popupDirector.OpenPopup(PopupDirector.PopupMode.AlertNameDuplication);
                }
            });

        // alert name duplication
        popupUIDirector.AlertNameDuplicationOKButton.onClick.AddListener(
            popupDirector.ClosePopup);

        // apply
        popupUIDirector.ApplySettingsCancelButton.onClick.AddListener(
            popupDirector.ClosePopup);

        popupUIDirector.ApplySettingsButton.onClick.AddListener(
            () =>
            {
                // apply????
                bool isGeneralSettings = appDirector.CurrentSettingsMode == AppDirector.SettingsMode.General;
                bool result = projectSettingsDirector.CheckProjectDuplication();
                if (isGeneralSettings || result)
                {
                    appDirector.ApplySettingsAndSwitchMode(
                        popupDirector.DestGameMode);
                    popupDirector.ClosePopup();
                }
                else
                {
                    popupDirector.OpenPopup(PopupDirector.PopupMode.AlertNameDuplication);
                }
            });

        #endregion

        #region settings
        settingsUIDirector.GeneralTabButton.onClick.AddListener(
            () =>
            {
                if (projectSettingsDirector.IsAnySettingsChanged)
                {
                    popupDirector.OpenApplySettingsPopup(
                        AppDirector.GameMode.Settings);
                }
                else
                {
                    appDirector.SwitchSettingsMode(AppDirector.SettingsMode.General);
                }
            });
        settingsUIDirector.ProjectsTabButton.onClick.AddListener(
            () =>
            {
                if (generalSettingsDirector.IsAnySettingsChanged)
                {
                    popupDirector.OpenApplySettingsPopup(
                        AppDirector.GameMode.Settings);
                }
                else
                {
                    appDirector.SwitchSettingsMode(AppDirector.SettingsMode.Projects);
                }
            });

        // general settings
        settingsUIDirector.MeterMaxInput.onValueChanged.AddListener(
            _v => generalSettingsDirector.UpdateProgressBarMax(float.Parse(_v)));

        settingsUIDirector.SoundPathButton.onClick.AddListener(
            () => generalSettingsDirector.UpdateNotificationSoundPath());

        Toggle[] toggles = settingsUIDirector.ResizingModeToggleGroup.GetComponentsInChildren<Toggle>();
        foreach (var toggle in toggles)
        {
            toggle.onValueChanged.AddListener(
                _v => generalSettingsDirector.UpdateResizingMode(_v));
        }

        settingsUIDirector.TwoSmallInput.onValueChanged.AddListener(
            _v => generalSettingsDirector.UpdateTwoSmall(int.Parse(_v)));
        settingsUIDirector.TwoMediumInput.onValueChanged.AddListener(
            _v => generalSettingsDirector.UpdateTwoMedium(int.Parse(_v)));

        settingsUIDirector.ThreeSmallInput.onValueChanged.AddListener(
            _v => generalSettingsDirector.UpdateThreeSmall(int.Parse(_v)));
        settingsUIDirector.ThreeMediumInput.onValueChanged.AddListener(
            _v => generalSettingsDirector.UpdateThreeMedium(int.Parse(_v)));
        settingsUIDirector.ThreeLargeInput.onValueChanged.AddListener(
            _v => generalSettingsDirector.UpdateThreeLarge(int.Parse(_v)));

        // projects settings
        settingsUIDirector.ProjectAdditionButton.onClick.AddListener(
            () => projectSettingsDirector.AddNewProject());

        // apply revert
        settingsUIDirector.GeneralSettingRevertButton.onClick.AddListener(
            () => generalSettingsDirector.RevertChanges());

        settingsUIDirector.GeneralSettingApplyButton.onClick.AddListener(
            () => generalSettingsDirector.ApplySettings());

        settingsUIDirector.ProjectSettingRevertButton.onClick.AddListener(
            () => projectSettingsDirector.RevertProjectChanges());

        settingsUIDirector.ProjectSettingApplyButton.onClick.AddListener(
            () =>
            {
                bool result = projectSettingsDirector.CheckProjectDuplication();
                if (result)
                {
                    projectSettingsDirector.ApplyProjectChanges();
                }
                else
                {
                    popupDirector.OpenPopup(PopupDirector.PopupMode.AlertNameDuplication);
                }
            });
        #endregion

        #region log
        logUIDirector.PreviousButton.onClick.AddListener(
            () => logDirector.DisplayPrevious());
        logUIDirector.NextButton.onClick.AddListener(
            () => logDirector.DisplayNext());
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

    /// <summary>
    /// ???????????v???W?F?N?g???????A?C?e?????C?x???g?o?^
    /// </summary>
    /// <param name="_item">???X???v???W?F?N?g??????GameObject</param>
    /// <param name="_id">Hierarchy?????????????????????v???W?F?N?g?????????????pid</param>
    public void AddProjectSettingItemEvents(GameObject _item, int _id)
    {
        SettingsUIDirector.GetProjectNameInputField(_item).onValueChanged.AddListener(
            _v => projectSettingsDirector.UpdateProjectName(_v, _id));

        SettingsUIDirector.GetProjectColorButton(_item).onClick.AddListener(
            () => popupDirector.OpenProjectColorPickerPopup(_id));

        SettingsUIDirector.GetProjectNotifModeDropdown(_item).onValueChanged.AddListener(
            _v => projectSettingsDirector.UpdateNotifMode(_id));

        SettingsUIDirector.GetProjectMoveUpperButton(_item).onClick.AddListener(
            () => projectSettingsDirector.MoveUpperItem(_id));

        SettingsUIDirector.GetProjectMoveLowerButton(_item).onClick.AddListener(
            () => projectSettingsDirector.MoveLowerItem(_id));

        SettingsUIDirector.GetProjectDeleteButton(_item).onClick.AddListener(
            () => popupDirector.OpenProjectDeletePopup(_id));
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
