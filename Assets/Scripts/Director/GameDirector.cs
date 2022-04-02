using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Candlelight;

public class GameDirector : MonoBehaviour
{
    [SerializeField, PropertyBackingField("IsClock12h")]
    private bool isClock12h = false;
    public bool IsClock12h
    {
        get => isClock12h;
        set
        {
            isClock12h = value;
            OnIsClockModeChanged();
        }
    }

    private WorkingDirector workingDirector;
    [SerializeField]
    ClockLabelController clockLabelController;


    private void Awake()
    {
        workingDirector = GameObject.Find("WorkingDirector").GetComponent<WorkingDirector>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnIsClockModeChanged()
    {
        workingDirector.CallUpdatePieForNeed();
        clockLabelController.ChangeClockLabels();
    }
}
