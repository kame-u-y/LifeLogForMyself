using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 時計へのアクセスを管理する
/// </summary>
public class ClockDirector : MonoBehaviour
{
    [SerializeField]
    private Image clockFrame;
    public Image ClockFrame => clockFrame;
    
    [SerializeField]
    private Image todayFuturePlate;
    public Image TodayFuturePlate => todayFuturePlate;

    [SerializeField]
    private Image todayPastPlate;
    public Image TodayPastPlate => todayPastPlate;

    [SerializeField]
    private Image workMeterPlate;
    public Image WorkMeterPlate => workMeterPlate;

    [SerializeField]
    private GameObject logPieContainer;
    public GameObject LogPieContainer => logPieContainer;

    [SerializeField]
    private Image currentPie;
    public Image CurrentPie => currentPie;

    [SerializeField]
    private Image currentWorkMeter;
    public Image CurrentWorkMeter => currentWorkMeter;

    [SerializeField]
    private Image meterCover;
    public Image MeterCover => meterCover;

    [SerializeField]
    private Image playEndButton;
    public Image PlayEndButton => playEndButton;

    [SerializeField]
    private Image clockThorn;
    public Image ClockThorn => clockThorn;

    [SerializeField]
    private Image clockNumber;
    public Image ClockNumber => clockNumber;

    [SerializeField]
    private TextMeshProUGUI meterMaxText;
    public TextMeshProUGUI MeterMaxText => meterMaxText;

    [SerializeField]
    private TextMeshProUGUI currentCountText;
    public TextMeshProUGUI CurrentCountText => currentCountText;

    private static ClockDirector instance;
    public static ClockDirector Instance => instance;


    //private ClockDirector()
    //{
    //    Debug.Log("生成されたぞ");
    //}

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
        }
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
    }


}
