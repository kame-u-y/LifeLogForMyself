using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputEventDirector : MonoBehaviour
{
    public MyInputActions myInputActions;

    [SerializeField]
    Button playEndButton;
    [SerializeField]
    Button toggleClockModeButton;
    [SerializeField]
    Dropdown projectDropdown;
    [SerializeField]
    Button background;

    WorkingDirector workingDirector;
    GameDirector gameDirector;

    private void Awake()
    {
        myInputActions = new MyInputActions();
        InitializeActionMap(myInputActions.UI);

        workingDirector = GameObject.Find("WorkingDirector").GetComponent<WorkingDirector>();
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
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
        background.onClick.AddListener(gameDirector.BackgroundClickHandler);
    }

    // Update is called once per frame
    private void Update()
    {
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
