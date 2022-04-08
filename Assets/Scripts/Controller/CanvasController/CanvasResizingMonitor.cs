using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CanvasResizingMonitor : UIBehaviour
{
    public delegate void OnWindowResize();

    public static CanvasResizingMonitor instance = null;
    public OnWindowResize windowResizeEvent;

    [SerializeField]
    private Image frame;
    [SerializeField]
    private Image todayFuturePlate;
    [SerializeField]
    private Image todayPastPlate;
    [SerializeField]
    private Image workMeterPlate;

    [SerializeField]
    private GameObject pieLogWorks;
    [SerializeField]
    private GameObject pieCurrentWorks;
    [SerializeField]
    private Image currentWorkMeter;

    [SerializeField]
    private Image cover;
    [SerializeField]
    private Image playEndButton;
    [SerializeField]
    private Image clockThorn;
    [SerializeField]
    private Image clockNumber;

    private int pScreenWidth;
    private int pScreenHeight;

    [SerializeField]
    GameDirector gameDirector;
    [SerializeField]
    WorkingDirector workingDirector;

    // Start is called before the first frame update
    protected override void Awake()
    {
        instance = this;

        windowResizeEvent = SwitchClockImage;
    }

    protected override void Start()
    {
        //base.Start();
        pScreenWidth = Screen.width;
        pScreenHeight = Screen.height;
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        if (windowResizeEvent != null)
        {
            windowResizeEvent();
        }
    }

    private void SwitchClockImage()
    {
        Debug.Log($"(w,h)=({Screen.width},{Screen.height})");

        //if (IsSameScreenStateWithPrevious())
        //{
        //    Debug.Log("0:" + (IsMoreThanThreshold(pScreenWidth) && IsMoreThanThreshold(Screen.width)));
        //    Debug.Log("1:" + (!IsMoreThanThreshold(pScreenWidth) && !IsMoreThanThreshold(Screen.width)));
        //    Debug.Log("2:" + (IsMoreThanThreshold(pScreenHeight) && IsMoreThanThreshold(Screen.height)));
        //    Debug.Log("3:" + (!IsMoreThanThreshold(pScreenHeight) && !IsMoreThanThreshold(Screen.height)));
        //    return;
        //}
        

        string spriteMode = "";
        string buttonMode = "";
        string clockMode = "";

        if (IsStateSwitchingToResized())
        {
            spriteMode = "Resized_Materials";
            clockNumber.gameObject.SetActive(false);
            Debug.Log("false");
        }
        else if (IsStateSwitchingToNormal())
        {
            spriteMode = "Materials";
            clockNumber.gameObject.SetActive(true);
            Debug.Log("true");
        } 
        else
        {
            return;
        }
        buttonMode = workingDirector.isWorking ? "End" : "Play";
        clockMode = gameDirector.isClock12h ? "12" : "24";

        frame.sprite = LoadSprite($"{spriteMode}/Base/Frame");
        todayFuturePlate.sprite = LoadSprite($"{spriteMode}/Base/Plate");
        todayPastPlate.sprite = LoadSprite($"{spriteMode}/Base/Plate");
        workMeterPlate.sprite = LoadSprite($"{spriteMode}/Base/WorkMeterPlate");

        currentWorkMeter.sprite = LoadSprite($"{spriteMode}/Pie/CurrentWorkMeter");
        cover.sprite = LoadSprite($"{spriteMode}/Cover/Cover");

        playEndButton.sprite = LoadSprite($"{spriteMode}/Cover/{buttonMode}Button");
        
        clockThorn.sprite = LoadSprite($"{spriteMode}/Cover/Label/Label{clockMode}h_Thorn");
        clockNumber.sprite = LoadSprite($"Materials/Cover/Label/Label{clockMode}h_Number");

        UpdatePSize();
    }

    private bool IsSameScreenStateWithPrevious()
        => (IsMoreThanThreshold(pScreenWidth) && IsMoreThanThreshold(Screen.width))
        || (!IsMoreThanThreshold(pScreenWidth) && !IsMoreThanThreshold(Screen.width))
        || (IsMoreThanThreshold(pScreenHeight) && IsMoreThanThreshold(Screen.height))
        || (!IsMoreThanThreshold(pScreenHeight) && !IsMoreThanThreshold(Screen.height));

    private bool IsStateSwitchingToResized()
        => (IsMoreThanThreshold(pScreenWidth) && !IsMoreThanThreshold(Screen.width))
        || (IsMoreThanThreshold(pScreenHeight) && !IsMoreThanThreshold(Screen.height));

    private bool IsStateSwitchingToNormal()
        => (!IsMoreThanThreshold(pScreenWidth) && IsMoreThanThreshold(Screen.width))
        || (!IsMoreThanThreshold(pScreenHeight) && IsMoreThanThreshold(Screen.height));

    private bool IsMoreThanThreshold(int _i)
        => _i > ProjectConstants.ScreenThreshold;

    private Sprite LoadSprite(string _s)
        => Resources.Load<Sprite>(_s);

    private void UpdatePSize()
    {
        pScreenWidth = Screen.width;
        pScreenHeight = Screen.height;
    }
}
