using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField]
    private GameObject workMeterMax;
    [SerializeField]
    private TextMeshProUGUI currentCountText;
    //private GameObject currentCount;


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
        InitializePValues();
        SwitchClockImage();
    }

    /// <summary>
    /// 初期化のためにあえて閾値に関して反対の値を前回値の保存用変数にぶちこむ
    /// </summary>
    private void InitializePValues()
    {
        if (Screen.width < ProjectConstants.ScreenThreshold 
            || Screen.height < ProjectConstants.ScreenThreshold)
        {
            pScreenWidth = ProjectConstants.ScreenThreshold + 1;
            pScreenHeight = ProjectConstants.ScreenThreshold + 1;
        }
        else
        {
            pScreenWidth = ProjectConstants.ScreenThreshold - 1;
            pScreenHeight = ProjectConstants.ScreenThreshold - 1;
        }
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

        string spriteMode = "";
        string buttonMode = "";
        string clockMode = "";

        if (IsStateSwitchingToResized())
        {
            spriteMode = "Resized_Materials";
            clockNumber.gameObject.SetActive(false);
            workMeterMax.SetActive(false);
            currentCountText.fontSize = 150;
        }
        else if (IsStateSwitchingToNormal())
        {
            spriteMode = "Materials";
            clockNumber.gameObject.SetActive(true);
            workMeterMax.SetActive(true);
            currentCountText.fontSize = 100;
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
