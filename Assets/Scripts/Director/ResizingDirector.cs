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
    private ClockDirector clockDirector;

    private void Awake()
    {

        appDirector = GameObject.Find("AppDirector").GetComponent<AppDirector>();
        workingDirector = GameObject.Find("WorkingDirector").GetComponent<WorkingDirector>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(ClockDirector.Instance);
        clockDirector = ClockDirector.Instance;

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
    /// 閾値より小さくor大きくなった瞬間に実行
    /// ロードするspriteの参照先を変更することで切り替え
    /// </summary>
    public void SwitchClockImage()
    {
        string spriteMode = "";
        string buttonMode = "";
        string clockMode = "";

        if (IsStateSwitchingToSmall())
        {
            spriteMode = "Resized_Materials";
            clockDirector.ClockNumber.gameObject.SetActive(false);
            clockDirector.MeterMaxText.gameObject.SetActive(false);
            clockDirector.CurrentCountText.fontSize = 150;
        }
        else if (IsStateSwitchingToNormal())
        {
            spriteMode = "Materials";
            clockDirector.ClockNumber.gameObject.SetActive(true);
            clockDirector.MeterMaxText.gameObject.SetActive(true);
            clockDirector.CurrentCountText.fontSize = 100;
        }
        else
        {
            return;
        }

        buttonMode = workingDirector.isWorking ? "End" : "Play";
        clockMode = appDirector.isClock12h ? "12" : "24";

        // spriteの変更処理
        clockDirector.ClockFrame.sprite 
            = LoadSprite($"{spriteMode}/Base/Frame");
        clockDirector.TodayFuturePlate.sprite 
            = LoadSprite($"{spriteMode}/Base/Plate");
        clockDirector.TodayPastPlate.sprite 
            = LoadSprite($"{spriteMode}/Base/Plate");
        for (int i = 0; i < clockDirector.LogPieContainer.transform.childCount; i++)
        {
            clockDirector.LogPieContainer.transform.GetChild(i).GetComponent<Image>().sprite
                = LoadSprite($"{spriteMode}/Base/Plate");
        }
        clockDirector.CurrentPie.GetComponent<Image>().sprite 
            = LoadSprite($"{spriteMode}/Base/Plate");
        clockDirector.WorkMeterPlate.sprite 
            = LoadSprite($"{spriteMode}/Base/WorkMeterPlate");

        clockDirector.CurrentWorkMeter.sprite 
            = LoadSprite($"{spriteMode}/Pie/CurrentWorkMeter");
        clockDirector.MeterCover.sprite 
            = LoadSprite($"{spriteMode}/Cover/Cover");
        clockDirector.PlayEndButton.sprite 
            = LoadSprite($"{spriteMode}/Cover/{buttonMode}Button");

        clockDirector.ClockThorn.sprite 
            = LoadSprite($"{spriteMode}/Cover/Label/Label{clockMode}h_Thorn");
        clockDirector.ClockNumber.sprite 
            = LoadSprite($"Materials/Cover/Label/Label{clockMode}h_Number");
        
        UpdatePValues(Screen.width, Screen.height);
    }

    /// <summary>
    /// ウィンドウのサイズが縦横小さいほう優先で、
    /// 閾値より小さくなった瞬間にtrue
    /// </summary>
    /// <returns></returns>
    private bool IsStateSwitchingToSmall()
        => (IsMoreThanThreshold(pScreenWidth) && !IsMoreThanThreshold(Screen.width))
        || (IsMoreThanThreshold(pScreenHeight) && !IsMoreThanThreshold(Screen.height));

    /// <summary>
    /// ウィンドウのサイズが縦横小さいほう優先で、
    /// 閾値より大きくなった瞬間にtrue
    /// </summary>
    /// <returns></returns>
    private bool IsStateSwitchingToNormal()
        => (!IsMoreThanThreshold(pScreenWidth) && IsMoreThanThreshold(Screen.width))
        || (!IsMoreThanThreshold(pScreenHeight) && IsMoreThanThreshold(Screen.height));

    /// <summary>
    /// 与えられた値がスクリーンの閾値より大きい場合true
    /// </summary>
    /// <param name="_i"></param>
    /// <returns></returns>
    private bool IsMoreThanThreshold(int _i)
        => _i > ProjectConstants.ScreenThreshold;

    /// <summary>
    /// spriteロード用
    /// </summary>
    /// <param name="_s"></param>
    /// <returns></returns>
    private Sprite LoadSprite(string _s)
        => Resources.Load<Sprite>(_s);
}
