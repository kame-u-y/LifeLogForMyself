using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneralSettingsDirector : MonoBehaviour
{
    private DatabaseDirector databaseDirector;
    private AppDirector appDirector;
    private SettingsUIDirector settingsUIDirector;

    #region general settings data
    private float progressMeterMax = 0;

    private string notificationSoundPath;

    private ResizingMode resizingMode;


    private TwoResizingData twoResizingStages;
    private ThreeResizingData threeResizingStages;
    #endregion

    private bool isAnySettingsChanged = false;
    public bool IsAnySettingsChanged
    {
        get => isAnySettingsChanged;
        set => isAnySettingsChanged = value;
    }

    /// <summary>
    /// シングルトン
    /// </summary>
    private static GeneralSettingsDirector instance;
    public static GeneralSettingsDirector Instance => instance;

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
        databaseDirector = DatabaseDirector.Instance;
        appDirector = AppDirector.Instance;
        settingsUIDirector = SettingsUIDirector.Instance;


        UpdateResizingMode(true);
        twoResizingStages = new TwoResizingData();
        threeResizingStages = new ThreeResizingData();

        InitializeGeneralSettings();

        SetAnySettingsChanged(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void InitializeGeneralSettings()
    {
        progressMeterMax = databaseDirector.FetchProgressMeterMax();
        InputField meterMax = settingsUIDirector.GeneralSettingContainer.transform
            .Find("ProgressBarMax/ItemValue/InputField").GetComponent<InputField>();
        Debug.Log(databaseDirector.FetchProgressMeterMax());
        meterMax.text = progressMeterMax.ToString();

        // notification sound path
        notificationSoundPath = databaseDirector.FetchNotificationSoundPath();
        TextMeshProUGUI soundPath = settingsUIDirector.GeneralSettingContainer.transform
            .Find("NotificationSound/ItemValue/Text (TMP)").GetComponent<TextMeshProUGUI>();
        soundPath.text = notificationSoundPath;

        // resizing mode
        ToggleGroup resizeModeGroup = settingsUIDirector.GeneralSettingContainer.transform
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
            => settingsUIDirector.GeneralSettingContainer
                .transform.Find($"{topScope}/{s}/{bottomScope}").GetComponent<InputField>();

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
            settingsUIDirector.TwoStageFormContainer.SetActive(true);
            settingsUIDirector.ThreeStageFormContainer.SetActive(false);
        }
        else if (resizingMode == ResizingMode.ThreeStages)
        {
            settingsUIDirector.TwoStageFormContainer.SetActive(false);
            settingsUIDirector.ThreeStageFormContainer.SetActive(true);
        }
    }

    public void UpdateProgressBarMax(float _v)
    {
        progressMeterMax = _v;
        SetAnySettingsChanged(true);
    }

    public void UpdateNotificationSoundPath()
    {
        notificationSoundPath = OpenFileName.ShowDialog();
        settingsUIDirector.SoundPathTMP.text = notificationSoundPath;
        SetAnySettingsChanged(true);
    }


    public void UpdateResizingMode(bool _v)
    {
        // トグル オンのときだけ処理
        if (!_v) return;

        resizingMode = settingsUIDirector.ResizingModeToggleGroup.GetFirstActiveToggle()
            .GetComponent<ResizingModeToggleController>().resizingMode;
        // 設定の表示変更
        if (resizingMode == ResizingMode.TwoStages)
        {
            settingsUIDirector.TwoStageFormContainer.SetActive(true);
            settingsUIDirector.ThreeStageFormContainer.SetActive(false);
        }
        else if (resizingMode == ResizingMode.ThreeStages)
        {
            settingsUIDirector.TwoStageFormContainer.SetActive(false);
            settingsUIDirector.ThreeStageFormContainer.SetActive(true);
        }

        SetAnySettingsChanged(true);

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
        settingsUIDirector.GeneralSettingRevertButton.interactable = _b;
        settingsUIDirector.GeneralSettingApplyButton.interactable = _b;
    }

    public void RevertChanges()
    {
        // 元に戻す
        InitializeGeneralSettings();
        SetAnySettingsChanged(false);
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
        appDirector.UpdateNotificationSound(notificationSoundPath);

        SetAnySettingsChanged(false);
    }
}
