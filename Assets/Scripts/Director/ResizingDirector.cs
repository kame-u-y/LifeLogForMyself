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
    private MainUIDirector mainUIDirector;

    private static ResizingDirector instance;
    public static ResizingDirector Instance => instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

        appDirector = AppDirector.Instance;
        workingDirector = WorkingDirector.Instance;
        mainUIDirector = MainUIDirector.Instance;

        InitializePValues();
        SwitchClockImage();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// �������̂��߂ɂ�����臒l�Ɋւ��Ĕ��΂̒l��O��l�̕ۑ��p�ϐ��ɂԂ�����
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
    /// �E�B���h�E�̒ʏ��ԂƏk����ԂƂŎ��v�̌����ڂ�ύX����
    /// 臒l��菬����or�傫���Ȃ����u�ԂɎ��s
    /// ���[�h����sprite�̎Q�Ɛ��ύX���邱�ƂŐ؂�ւ�
    /// </summary>
    public void SwitchClockImage()
    {
        string spriteMode = "";
        string buttonMode = "";
        string clockMode = "";

        if (IsStateSwitchingToSmall())
        {
            spriteMode = "Resized_Materials";
            mainUIDirector.ClockNumberImage.gameObject.SetActive(false);
            mainUIDirector.MeterMaxTMP.gameObject.SetActive(false);
            mainUIDirector.CurrentCountTMP.fontSize = 150;
        }
        else if (IsStateSwitchingToNormal())
        {
            spriteMode = "Materials";
            mainUIDirector.ClockNumberImage.gameObject.SetActive(true);
            mainUIDirector.MeterMaxTMP.gameObject.SetActive(true);
            mainUIDirector.CurrentCountTMP.fontSize = 100;
        }
        else
        {
            return;
        }

        buttonMode = workingDirector.isWorking ? "End" : "Play";
        clockMode = appDirector.isClock12h ? "12" : "24";

        // sprite�̕ύX����
        mainUIDirector.ClockFrameImage.sprite 
            = LoadSprite($"{spriteMode}/Base/Frame");
        mainUIDirector.TodayFuturePlateImage.sprite 
            = LoadSprite($"{spriteMode}/Base/Plate");
        mainUIDirector.TodayPastPlateImage.sprite 
            = LoadSprite($"{spriteMode}/Base/Plate");
        for (int i = 0; i < mainUIDirector.LogPieContainer.transform.childCount; i++)
        {
            mainUIDirector.LogPieContainer.transform.GetChild(i).GetComponent<Image>().sprite
                = LoadSprite($"{spriteMode}/Base/Plate");
        }
        mainUIDirector.CurrentPieImage.GetComponent<Image>().sprite 
            = LoadSprite($"{spriteMode}/Base/Plate");
        mainUIDirector.WorkMeterPlateImage.sprite 
            = LoadSprite($"{spriteMode}/Base/WorkMeterPlate");

        mainUIDirector.WorkMeterImage.sprite 
            = LoadSprite($"{spriteMode}/Pie/CurrentWorkMeter");
        mainUIDirector.MeterCoverImage.sprite 
            = LoadSprite($"{spriteMode}/Cover/Cover");
        mainUIDirector.PlayEndButtonImage.sprite 
            = LoadSprite($"{spriteMode}/Cover/{buttonMode}Button");

        mainUIDirector.ClockThornImage.sprite 
            = LoadSprite($"{spriteMode}/Cover/Label/Label{clockMode}h_Thorn");
        mainUIDirector.ClockNumberImage.sprite 
            = LoadSprite($"Materials/Cover/Label/Label{clockMode}h_Number");
        
        UpdatePValues(Screen.width, Screen.height);
    }

    /// <summary>
    /// �E�B���h�E�̃T�C�Y���c���������ق��D��ŁA
    /// 臒l��菬�����Ȃ����u�Ԃ�true
    /// </summary>
    /// <returns></returns>
    private bool IsStateSwitchingToSmall()
        => (IsMoreThanThreshold(pScreenWidth) && !IsMoreThanThreshold(Screen.width))
        || (IsMoreThanThreshold(pScreenHeight) && !IsMoreThanThreshold(Screen.height));

    /// <summary>
    /// �E�B���h�E�̃T�C�Y���c���������ق��D��ŁA
    /// 臒l���傫���Ȃ����u�Ԃ�true
    /// </summary>
    /// <returns></returns>
    private bool IsStateSwitchingToNormal()
        => (!IsMoreThanThreshold(pScreenWidth) && IsMoreThanThreshold(Screen.width))
        || (!IsMoreThanThreshold(pScreenHeight) && IsMoreThanThreshold(Screen.height));

    /// <summary>
    /// �^����ꂽ�l���X�N���[����臒l���傫���ꍇtrue
    /// </summary>
    /// <param name="_i"></param>
    /// <returns></returns>
    private bool IsMoreThanThreshold(int _i)
        => _i > ProjectConstants.ScreenThreshold;

    /// <summary>
    /// sprite���[�h�p
    /// </summary>
    /// <param name="_s"></param>
    /// <returns></returns>
    private Sprite LoadSprite(string _s)
        => Resources.Load<Sprite>(_s);
}
