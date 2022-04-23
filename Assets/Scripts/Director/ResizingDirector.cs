using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResizingDirector : MonoBehaviour
{
    private int pScreenWidth;
    public int PScreenWidth
    {
        get => pScreenWidth;
    }

    private int pScreenHeight;
    public int PScreenHeight
    {
        get => pScreenHeight;
    }

    private AppDirector appDirector;
    private WorkingDirector workingDirector;

    [SerializeField]
    private Image frame;
    [SerializeField]
    private Image todayFuturePlate;
    [SerializeField]
    private Image todayPastPlate;
    [SerializeField]
    private Image workMeterPlate;

    [SerializeField]
    private GameObject logPieContainer;
    [SerializeField]
    private GameObject currentPie;
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

    private void Awake()
    {

        appDirector = GameObject.Find("AppDirector").GetComponent<AppDirector>();
        workingDirector = GameObject.Find("WorkingDirector").GetComponent<WorkingDirector>();
    }

    // Start is called before the first frame update
    void Start()
    {

        InitializePValues();
        SwitchClockImage();
    }

    // Update is called once per frame
    void Update()
    {

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

    public void UpdatePValues(int _w, int _h)
    {
        pScreenWidth = _w;
        pScreenHeight = _h;
    }


    /// <summary>
    /// ウィンドウの通常状態と縮小状態とで時計の見た目を変更する
    /// </summary>
    public void SwitchClockImage()
    {
        string spriteMode = "";
        string buttonMode = "";
        string clockMode = "";

        if (IsStateSwitchingToSmall())
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
            //UpdatePSize();
            return;
        }

        buttonMode = workingDirector.isWorking ? "End" : "Play";
        clockMode = appDirector.isClock12h ? "12" : "24";

        frame.sprite = LoadSprite($"{spriteMode}/Base/Frame");
        todayFuturePlate.sprite = LoadSprite($"{spriteMode}/Base/Plate");
        todayPastPlate.sprite = LoadSprite($"{spriteMode}/Base/Plate");
        for (int i = 0; i < logPieContainer.transform.childCount; i++)
        {
            logPieContainer.transform.GetChild(i).GetComponent<Image>().sprite
                = LoadSprite($"{spriteMode}/Base/Plate");
        }
        currentPie.GetComponent<Image>().sprite = LoadSprite($"{spriteMode}/Base/Plate");
        workMeterPlate.sprite = LoadSprite($"{spriteMode}/Base/WorkMeterPlate");

        currentWorkMeter.sprite = LoadSprite($"{spriteMode}/Pie/CurrentWorkMeter");
        cover.sprite = LoadSprite($"{spriteMode}/Cover/Cover");

        playEndButton.sprite = LoadSprite($"{spriteMode}/Cover/{buttonMode}Button");

        clockThorn.sprite = LoadSprite($"{spriteMode}/Cover/Label/Label{clockMode}h_Thorn");
        clockNumber.sprite = LoadSprite($"Materials/Cover/Label/Label{clockMode}h_Number");

        UpdatePValues(Screen.width, Screen.height);
    }

    private bool IsStateSwitchingToSmall()
        => (IsMoreThanThreshold(pScreenWidth) && !IsMoreThanThreshold(Screen.width))
        || (IsMoreThanThreshold(pScreenHeight) && !IsMoreThanThreshold(Screen.height));

    private bool IsStateSwitchingToNormal()
        => (!IsMoreThanThreshold(pScreenWidth) && IsMoreThanThreshold(Screen.width))
        || (!IsMoreThanThreshold(pScreenHeight) && IsMoreThanThreshold(Screen.height));

    private bool IsMoreThanThreshold(int _i)
        => _i > ProjectConstants.ScreenThreshold;

    private Sprite LoadSprite(string _s)
        => Resources.Load<Sprite>(_s);
}
