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
    WorkingDirector workingDirector;

    private void Awake()
    {
        myInputActions = new MyInputActions();
        InitializeActionMap(myInputActions.UI);


        workingDirector = GameObject.Find("WorkingDirector").GetComponent<WorkingDirector>();
        //myInputActions.Player.Interact.performed += PlayerInteract;
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Talking
        //talkingDirector = gameDirector.transform.Find("TalkingDirector").GetComponent<TalkingDirector>();
        //myInputActions.Talking.Progress.performed += TalkingProgress;
        playEndButton.onClick.AddListener(workingDirector.ToggleWork);
    }

    // Update is called once per frame
    private void Update()
    {
    }

    //private void PlayerInteract(InputAction.CallbackContext _context)
    //{ 
    //}

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
