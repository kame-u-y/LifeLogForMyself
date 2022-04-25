using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// メイン画面のUIへのアクセスを管理する
/// </summary>
public class MainUIDirector : MonoBehaviour
{
    [SerializeField]
    private Button backgroundButton;
    public Button BackgroundButton => backgroundButton;

    [SerializeField]
    private Image clockFrameImage;
    public Image ClockFrameImage => clockFrameImage;

    [SerializeField]
    private Image todayFuturePlateImage;
    public Image TodayFuturePlateImage => todayFuturePlateImage;

    [SerializeField]
    private Image todayPastPlateImage;
    public Image TodayPastPlateImage => todayPastPlateImage;

    [SerializeField]
    private Image workMeterPlateImage;
    public Image WorkMeterPlateImage => workMeterPlateImage;

    [SerializeField]
    private PieChartController pieChartController;
    public PieChartController PieChartController => pieChartController;

    private GameObject logPieContainer;
    public GameObject LogPieContainer => logPieContainer;

    private Image currentPieImage;
    public Image CurrentPieImage => currentPieImage;

    [SerializeField]
    private Image workMeterImage;
    public Image WorkMeterImage => workMeterImage;

    private CurrentWorkMeterController currentWorkMeterCtrler;
    public CurrentWorkMeterController CurrentWorkMeterCtrler => currentWorkMeterCtrler;

    [SerializeField]
    private Image meterCoverImage;
    public Image MeterCoverImage => meterCoverImage;

    [SerializeField]
    private Image playEndButtonImage;
    public Image PlayEndButtonImage => playEndButtonImage;

    private Button playEndButton;
    public Button PlayEndButton => playEndButton;

    [SerializeField]
    private Image clockThornImage;
    public Image ClockThornImage => clockThornImage;

    [SerializeField]
    private Image clockNumberImage;
    public Image ClockNumberImage => clockNumberImage;

    [SerializeField]
    private TextMeshProUGUI meterMaxTMP;
    public TextMeshProUGUI MeterMaxTMP => meterMaxTMP;

    [SerializeField]
    private TextMeshProUGUI currentCountTMP;
    public TextMeshProUGUI CurrentCountTMP => currentCountTMP;

    [SerializeField]
    private ProjectDropdownController projectDropdownCtrler;
    public ProjectDropdownController ProjectDropdownCtrler => projectDropdownCtrler;

    private Dropdown projectDropdown;
    public Dropdown ProjectDropdown => projectDropdown;

    /// <summary>
    /// シングルトン
    /// </summary>
    private static MainUIDirector instance;
    public static MainUIDirector Instance => instance;


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

        currentWorkMeterCtrler = workMeterImage.GetComponent<CurrentWorkMeterController>();

        logPieContainer = pieChartController.transform.Find("LogWorks").gameObject;
        currentPieImage = pieChartController.transform.Find("CurrentWork").GetComponent<Image>();

        playEndButton = playEndButtonImage.GetComponent<Button>();

        projectDropdown = projectDropdownCtrler.GetComponent<Dropdown>();
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
