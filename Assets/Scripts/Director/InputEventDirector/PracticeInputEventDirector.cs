using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PracticeInputEventDirector : MonoBehaviour
{
    public MyInputActions myInputActions;

    [SerializeField]
    GameObject practiceImage;
    WinAPIDirector winAPIDirector;

    private void Awake()
    {
        myInputActions = new MyInputActions();
        myInputActions.Enable();

        winAPIDirector = GameObject.Find("WinAPIDirector").GetComponent<WinAPIDirector>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("hoge");
        EventTrigger trigger = practiceImage.GetComponent<EventTrigger>();
        AddEventTrigger(
            trigger,
            EventTriggerType.BeginDrag,
            (_eventData) => winAPIDirector.OnBeginDrag(_eventData));
        AddEventTrigger(
            trigger,
            EventTriggerType.Drag,
            (_eventData) => winAPIDirector.OnDrag(_eventData));
        AddEventTrigger(
            trigger,
            EventTriggerType.EndDrag,
            (_eventData) => winAPIDirector.OnEndDrag(_eventData));
    }

    // Update is called once per frame
    void Update()
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
}
