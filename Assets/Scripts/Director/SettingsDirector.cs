using System.Collections;
using System.Collections.Generic;
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

    private int progressBarMax = 0;

    [SerializeField]
    private ToggleGroup settingResizingMode;
    private ResizingMode resizingMode;
    [SerializeField]
    private GameObject largeSettigForm;

    private TwoResizingData twoResizingData;
    private ThreeResizingData threeResizingData;

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
        twoResizingData = new TwoResizingData();
        threeResizingData = new ThreeResizingData();

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

    public void UpdateProgressBarMax(int _v)
    {
        progressBarMax = _v;
        SetAnySettingsChanged(true);
    }

    public void UpdateResizingMode(bool _v)
    {
        if (_v)
        {
            resizingMode = settingResizingMode.GetFirstActiveToggle()
                .GetComponent<ResizingModeToggleController>().resizingMode;
            // 設定の表示変更
            largeSettigForm.SetActive(resizingMode == ResizingMode.ThreeStages);

            SetAnySettingsChanged(true);
        }
    }

    public void UpdateResizingSmall(int _v)
    {
        Debug.Log(_v);
        if (resizingMode == ResizingMode.TwoStages)
        {
            UpdateTwoSmall(_v);
        }
        else
        {
            UpdateThreeSmall(_v);
        }
        SetAnySettingsChanged(true);
    }

    public void UpdateResizingMedium(int _v)
    {
        if (resizingMode == ResizingMode.TwoStages)
            UpdateTwoMedium(_v);
        else
            UpdateThreeMedium(_v);
        SetAnySettingsChanged(true);
    }

    public void UpdateResizingLarge(int _v)
    {
        UpdateThreeLarge(_v);
        SetAnySettingsChanged(true);
    }

    private void UpdateTwoSmall(int _v) => twoResizingData.small = _v;
    private void UpdateTwoMedium(int _v) => twoResizingData.medium = _v;

    private void UpdateThreeSmall(int _v) => threeResizingData.small = _v;
    private void UpdateThreeMedium(int _v) => threeResizingData.medium = _v;
    private void UpdateThreeLarge(int _v) => threeResizingData.large = _v;

    private void SetAnySettingsChanged(bool _b)
    {
        IsAnySettingsChanged = _b;
        settingRevertButton.interactable = _b;
        settingApplyButton.interactable = _b;
    }

    public void RevertChanges()
    {
        // 元に戻す
        isAnySettingsChanged = false;

    }

    public void ApplySettings()
    {
        // large > medium > small
        // になってなかったらエラー todo

        databaseDirector.ApplySettings(
            progressBarMax,
            resizingMode,
            twoResizingData,
            threeResizingData);
            //projectSettingsController.GetProjectDataList());
    }
}
