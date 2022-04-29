using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogUIDirector : SingletonMonoBehaviourFast<LogUIDirector>
{
    [SerializeField]
    private PieChartController logPieChartCtrler;
    public PieChartController LogPieChartCtrler => logPieChartCtrler;

    [SerializeField]
    private Button previousButton;
    public Button PreviousButton => previousButton;

    [SerializeField]
    private Button nextButton;
    public Button NextButton => nextButton;

    [SerializeField]
    private TextMeshProUGUI logDateTMP;
    public TextMeshProUGUI LogDateTMP => logDateTMP;

    
    [SerializeField]
    private Image clockFrameImage;
    public Image ClockFrameImage => clockFrameImage;

    [SerializeField]
    private Image clockPlateImage;
    public Image ClockPlateImage => clockPlateImage;

    [SerializeField]
    private Image clockThornImage;
    public Image ClockThornImage => clockThornImage;

    [SerializeField]
    private Image clockNumberImage;
    public Image ClockNumberImage => clockNumberImage;

    [SerializeField]
    private Image innerFrameImage;
    public Image InnerFrameImage => innerFrameImage;

    [SerializeField]
    private Image innerPlateImage;
    public Image InnerPlateImage => innerPlateImage;

    [SerializeField]
    private GameObject logPieContainer;
    public GameObject LogPieContainer => logPieContainer;

    new void Awake()
    {
        base.Awake();
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
