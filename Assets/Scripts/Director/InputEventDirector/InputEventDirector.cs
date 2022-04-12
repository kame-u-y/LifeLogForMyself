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

    [SerializeField]
    Button playEndButton;
    [SerializeField]
    Button toggleClockModeButton;
    [SerializeField]
    Dropdown projectDropdown;
    [SerializeField]
    Button background;

    [SerializeField]
    PopupController popupController;
    Button popupBackground;
    Button mainButton;
    Button settingsButton;
    Button quitButton;


    WorkingDirector workingDirector;
    GameDirector gameDirector;
    WindowDirector windowDirector;

    private void Awake()
    {
        myInputActions = new MyInputActions();
        InitializeActionMap(myInputActions.UI);

        workingDirector = GameObject.Find("WorkingDirector").GetComponent<WorkingDirector>();
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
        windowDirector = GameObject.Find("WindowDirector").GetComponent<WindowDirector>();

        popupBackground = popupController.transform.Find("PopupBackground").GetComponent<Button>();
        mainButton = popupController.transform.Find("Popup/ButtonContainer/MainButton").GetComponent<Button>();
        settingsButton = popupController.transform.Find("Popup/ButtonContainer/SettingsButton").GetComponent<Button>();
        quitButton = popupController.transform.Find("Popup/ButtonContainer/QuitButton").GetComponent<Button>();
        Debug.Log(quitButton);
    }

    // Start is called before the first frame update
    private void Start()
    {
        playEndButton.onClick.AddListener(workingDirector.ToggleWork);
        toggleClockModeButton.onClick.AddListener(gameDirector.ChangeClockMode);
        //projectDropdown..AddListener()
        //string selectedProject = projectDropdown.captionText.text;
        projectDropdown.onValueChanged.AddListener(
            (v) => workingDirector.ChangeProjectOfCurrentWork());
        background.onClick.AddListener(windowDirector.OnResizingButtonClick);

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
