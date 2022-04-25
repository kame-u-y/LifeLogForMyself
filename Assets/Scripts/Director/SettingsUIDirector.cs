using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUIDirector : SingletonMonoBehaviourFast<SettingsUIDirector>
{
    [SerializeField]
    private Button backgroundButton;
    public Button BackgroundButton => backgroundButton;

    [SerializeField]
    private GameObject settingsTabContainer;

    private Button generalTabButton;
    public Button GeneralTabButton => generalTabButton;

    private Button projectTabButton;
    public Button ProjectsTabButton => projectTabButton;

    [SerializeField]
    private GameObject projectsSettingContainer;
    public GameObject ProjectsSettingContainer => projectsSettingContainer;
    
    [Header("General Settings UI")]

    [SerializeField]
    private GameObject generalSettingContainer;
    public GameObject GeneralSettingContainer => generalSettingContainer;

    private InputField meterMaxInput;
    public InputField MeterMaxInput => meterMaxInput;

    [SerializeField]
    private Button soundPathButton;
    public Button SoundPathButton => soundPathButton;

    [SerializeField]
    private TextMeshProUGUI soundPathTMP;
    public TextMeshProUGUI SoundPathTMP => soundPathTMP;

    [SerializeField]
    private ToggleGroup resizingModeToggleGroup;
    public ToggleGroup ResizingModeToggleGroup => resizingModeToggleGroup;


    [SerializeField]
    private GameObject twoStageFormContainer;
    public GameObject TwoStageFormContainer => twoStageFormContainer;

    private InputField twoSmallInput;
    public InputField TwoSmallInput => twoSmallInput;

    private InputField twoMediumInput;
    public InputField TwoMediumInput => twoMediumInput;


    [SerializeField]
    private GameObject threeStageFormContainer;
    public GameObject ThreeStageFormContainer => threeStageFormContainer;

    private InputField threeSmallInput;
    public InputField ThreeSmallInput => threeSmallInput;

    private InputField threeMediumInput;
    public InputField ThreeMediumInput => threeMediumInput;

    private InputField threeLargeInput;
    public InputField ThreeLargeInput => threeLargeInput;


    [SerializeField]
    private Button generalSettingRevertButton;
    public Button GeneralSettingRevertButton => generalSettingRevertButton;

    [SerializeField]
    private Button generalSettingApplyButton;
    public Button GeneralSettingApplyButton => generalSettingApplyButton;

    [Header("Project Settings UI")]
    [SerializeField]
    private Transform projectItemContainer;
    public Transform ProjectItemContainer => projectItemContainer;

    [SerializeField]
    private Button projectAdditionButton;
    public Button ProjectAdditionButton => projectAdditionButton;

    [SerializeField]
    private Button projectSettingRevertButton;
    public Button ProjectSettingRevertButton => projectSettingRevertButton;

    [SerializeField]
    private Button projectSettingApplyButton;
    public Button ProjectSettingApplyButton => projectSettingApplyButton;

    /// <summary>
    /// ƒVƒ“ƒOƒ‹ƒgƒ“
    /// </summary>
    //private static SettingsUIDirector instance;
    //public static SettingsUIDirector Instance => instance;

    new void Awake()
    {
        base.Awake();
        //if (instance == null)
        //{
        //    instance = this;
        //    DontDestroyOnLoad(gameObject);
        //}
        //else
        //{
        //    Destroy(gameObject);
        //    return;
        //}


        generalTabButton = settingsTabContainer.transform.Find("General").GetComponent<Button>();
        projectTabButton = settingsTabContainer.transform.Find("Projects").GetComponent<Button>();

        meterMaxInput = generalSettingContainer.transform
            .Find("ProgressBarMax/ItemValue/InputField").GetComponent<InputField>();

        Func<string, InputField> accessTwo 
            = (string _s) => twoStageFormContainer.transform.Find($"{_s}/ItemValue/InputField").GetComponent<InputField>();
        twoSmallInput = accessTwo("Small");
        twoMediumInput = accessTwo("Medium");

        Func<string, InputField> accessThree
            = (string _s) => threeStageFormContainer.transform.Find($"{_s}/ItemValue/InputField").GetComponent<InputField>();
        threeSmallInput = accessThree("Small");
        threeMediumInput = accessThree("Medium");
        threeLargeInput = accessThree("Large");
    }

    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static TextMeshProUGUI GetProjectIndexTMP(GameObject _item)
        => _item.transform.Find("Values/Index").GetComponent<TextMeshProUGUI>();

    public static InputField GetProjectNameInputField(GameObject _item)
        => _item.transform.Find("Values/ProjectName").GetComponent<InputField>();

    public static Image GetProjectColorImage(GameObject _item)
        => _item.transform.Find("Values/ProjectColor").GetComponent<Image>();
    public static Button GetProjectColorButton(GameObject _item)
        => _item.transform.Find("Values/ProjectColor").GetComponent<Button>();

    public static Dropdown GetProjectNotifModeDropdown(GameObject _item)
        => _item.transform.Find("Values/NotificationMode").GetComponent<Dropdown>();

    public static Button GetProjectMoveUpperButton(GameObject _item)
        => _item.transform.Find("Values/MoveUpper/Button").GetComponent<Button>();

    public static Button GetProjectMoveLowerButton(GameObject _item)
        => _item.transform.Find("Values/MoveLower/Button").GetComponent<Button>();

    public static Button GetProjectDeleteButton(GameObject _item)
        => _item.transform.Find("Values/Delete/Button").GetComponent<Button>();


    public static void SetProjectIndex(GameObject _item, int _id)
        => GetProjectIndexTMP(_item).text = _id.ToString();

    public static void SetProjectName(GameObject _item, string _name)
        => GetProjectNameInputField(_item).text = _name;

    public static void SetProjectColor(GameObject _item, Color _c)
        => GetProjectColorImage(_item).color = _c;

    public static void SetProjectUpperInteractable(GameObject _item, bool _b)
        => GetProjectMoveUpperButton(_item).interactable = _b;

    public static void SetProjectLowerInteractable(GameObject _item, bool _b)
        => GetProjectMoveLowerButton(_item).interactable = _b;
}
