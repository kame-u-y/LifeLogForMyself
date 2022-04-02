using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Candlelight;

public class GameDirector : MonoBehaviour
{
    [HideInInspector]
    public bool isClock12h = false;

    private WorkingDirector workingDirector;
    [SerializeField]
    ClockLabelController clockLabelController;
    [SerializeField]
    ClockModeButtonController clockModeButtonController;


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

    public void ChangeClockMode()
    {
        isClock12h = !isClock12h;

        workingDirector.CallForNeedDisplayTodayPieChart();
        workingDirector.CallForNeedUpdateCurrentWorkPiece();
        clockLabelController.ChangeClockLabels();
        clockModeButtonController.ChangeButtonColor();
    }
}
