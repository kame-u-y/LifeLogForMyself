using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsDirector : MonoBehaviour
{
    private DatabaseDirector databaseDirector;

    [SerializeField]
    private GameObject settingsTab;
    private Button generalTabButton;
    private Button projectsTabButton;
    [SerializeField]
    private GameObject generalSettings;
    [SerializeField]
    private GameObject projectsSettings;

    //private List<GameObject> projectList;

    #region general settings data
    private float progressMeterMax = 0;

    [SerializeField]
    private TextMeshProUGUI soundPathLabel;
    private string notificationSoundPath;

    [SerializeField]
    private ToggleGroup settingResizingMode;
    private ResizingMode resizingMode;
    
    [SerializeField]
    private GameObject twoStageForms;
    [SerializeField]
    private GameObject threeStageForms;

    private TwoResizingData twoResizingStages;
    private ThreeResizingData threeResizingStages;
    #endregion


    [SerializeField]
    private ProjectSettingsController projectSettingsController;

    private bool isAnySettingsChanged = false;
    public bool IsAnySettingsChanged
    {
        get => isAnySettingsChanged;
        set => isAnySettingsChanged = value;
    }
    // revert apply
    [SerializeField]
    Button settingRevertButton;
    [SerializeField]
    Button settingApplyButton;

    public enum SettingsMode
    {
        General,
        Projects
    }

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        databaseDirector = GameObject.Find("DatabaseDirector").GetComponent<DatabaseDirector>();

        generalTabButton = settingsTab.transform.Find("General").GetComponent<Button>();
        projectsTabButton = settingsTab.transform.Find("Projects").GetComponent<Button>();

        SwitchSettingsMode(SettingsMode.General);

        UpdateResizingMode(true);
        twoResizingStages = new TwoResizingData();
        threeResizingStages = new ThreeResizingData();

        InitializeGeneralSettings();
        projectSettingsController.DisplayItems(databaseDirector.FetchProjectList());

        
        SetAnySettingsChanged(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SwitchSettingsMode(SettingsMode _mode)
    {
        if (_mode == SettingsMode.General)
        {
            bool b = true;
            generalSettings.SetActive(b);
            projectsSettings.SetActive(!b);

            generalTabButton.interactable = !b;
            projectsTabButton.interactable = b;

            //IsAnySettingsChanged = false;
        }
        else if (_mode == SettingsMode.Projects)
        {
            bool b = true;
            generalSettings.SetActive(!b);
            projectsSettings.SetActive(b);

            generalTabButton.interactable = b;
            projectsTabButton.interactable = !b;

            SetAnySettingsChanged(false);
        }
    }

    private void InitializeGeneralSettings()
    {
        progressMeterMax = databaseDirector.FetchProgressMeterMax();
        InputField meterMax = generalSettings.transform
            .Find("ProgressBarMax/ItemValue/InputField").GetComponent<InputField>();
        Debug.Log(databaseDirector.FetchProgressMeterMax());
        meterMax.text = progressMeterMax.ToString();

        // notification sound path
        notificationSoundPath = databaseDirector.FetchNotificationSoundPath();
        TextMeshProUGUI soundPath = generalSettings.transform
            .Find("NotificationSound/ItemValue/Text (TMP)").GetComponent<TextMeshProUGUI>();
        soundPath.text = notificationSoundPath;

        // resizing mode
        ToggleGroup resizeModeGroup = generalSettings.transform
            .Find("ResizingMode/ItemValue/ModeToggleGroup").GetComponent<ToggleGroup>();
        Toggle[] toggles = resizeModeGroup.GetComponentsInChildren<Toggle>();
        //resizeModeGroup.SetAllTogglesOff();
        resizingMode = databaseDirector.FetchResizingMode();
        foreach (var toggle in toggles)
        {
            if (toggle.GetComponent<ResizingModeToggleController>().resizingMode == resizingMode)
            {
                toggle.isOn = true;
            }
        }

        // resizing stage forms
        string topScope = "ResizingValueForms/ItemValue";
        string bottomScope = "ItemValue/InputField";
        Func<string, InputField> access = (string s)
            => generalSettings.transform.Find($"{topScope}/{s}/{bottomScope}").GetComponent<InputField>();

        InputField twoSmall = access("TwoStages/Small");
        InputField twoMedium = access("TwoStages/Medium");
        twoResizingStages = databaseDirector.FetchTwoResizingStages().ShallowCopy();
        twoSmall.text = twoResizingStages.small.ToString();
        twoMedium.text = twoResizingStages.medium.ToString();
        

        InputField threeSmall = access("ThreeStages/Small");
        InputField threeMedium = access("ThreeStages/Medium");
        InputField threeLarge = access("ThreeStages/Large");
        threeResizingStages = databaseDirector.FetchThreeResizingStages().ShallowCopy();
        threeSmall.text = threeResizingStages.small.ToString();
        threeMedium.text = threeResizingStages.medium.ToString();
        threeLarge.text = threeResizingStages.large.ToString();

        if (resizingMode == ResizingMode.TwoStages)
        {
            twoStageForms.SetActive(true);
            threeStageForms.SetActive(false);
        }
        else if (resizingMode == ResizingMode.ThreeStages)
        {
            twoStageForms.SetActive(false);
            threeStageForms.SetActive(true);
        }
    }

    public void UpdateProgressBarMax(int _v)
    {
        progressMeterMax = _v;
        SetAnySettingsChanged(true);
    }

    public void UpdateNotificationSoundPath()
    {
        notificationSoundPath = OpenFileName.ShowDialog();
        soundPathLabel.text = notificationSoundPath;
    }


    public void UpdateResizingMode(bool _v)
    {
        if (_v)
        {
            resizingMode = settingResizingMode.GetFirstActiveToggle()
                .GetComponent<ResizingModeToggleController>().resizingMode;
            // 設定の表示変更
            if (resizingMode == ResizingMode.TwoStages)
            {
                twoStageForms.SetActive(true);
                threeStageForms.SetActive(false);
            }
            else if (resizingMode == ResizingMode.ThreeStages)
            {
                twoStageForms.SetActive(false);
                threeStageForms.SetActive(true);
            }

            SetAnySettingsChanged(true);
        }
    }


    // TwoStages
    public void UpdateTwoSmall(int _v)
    {
        twoResizingStages.small = _v;
        SetAnySettingsChanged(true);
    }
    public void UpdateTwoMedium(int _v)
    {
        twoResizingStages.medium = _v;
        SetAnySettingsChanged(true);
    }
    
    // ThreeStages
    public void UpdateThreeSmall(int _v)
    {
        threeResizingStages.small = _v;
        SetAnySettingsChanged(true);
    }
    public void UpdateThreeMedium(int _v)
    {
        threeResizingStages.medium = _v;
        SetAnySettingsChanged(true);
    }
    public void UpdateThreeLarge(int _v)
    {
        threeResizingStages.large = _v;
        SetAnySettingsChanged(true);
    }
     
    private void SetAnySettingsChanged(bool _b)
    {
        IsAnySettingsChanged = _b;
        settingRevertButton.interactable = _b;
        settingApplyButton.interactable = _b;
    }

    public void RevertChanges()
    {
        // 元に戻す
        InitializeGeneralSettings();
        isAnySettingsChanged = false;
    }

    public void ApplySettings()
    {
        // large > medium > small
        // になってなかったらエラー todo

        databaseDirector.ApplySettings(
            progressMeterMax,
            notificationSoundPath,
            resizingMode,
            twoResizingStages,
            threeResizingStages);
            //projectSettingsController.GetProjectDataList());
    }
}
