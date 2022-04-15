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
        }
        else if (_mode == SettingsMode.Projects)
        {
            bool b = true;
            generalSettings.SetActive(!b);
            projectsSettings.SetActive(b);

            generalTabButton.interactable = b;
            projectsTabButton.interactable = !b;
        }
    }

    public void UpdateProgressBarMax(int _v)
    {
        progressBarMax = _v;
    }

    public void UpdateResizingMode(bool _v)
    {
        if (_v)
        {
            resizingMode = settingResizingMode.GetFirstActiveToggle()
                .GetComponent<ResizingModeToggleController>().resizingMode;
            // 設定の表示変更
            largeSettigForm.SetActive(resizingMode == ResizingMode.ThreeStages);
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
    }

    public void UpdateResizingMedium(int _v)
    {
        if (resizingMode == ResizingMode.TwoStages)
            UpdateTwoMedium(_v);
        else
            UpdateThreeMedium(_v);
    }

    public void UpdateResizingLarge(int _v)
    {
        UpdateThreeLarge(_v);
    }

    private void UpdateTwoSmall(int _v) => twoResizingData.small = _v;
    private void UpdateTwoMedium(int _v) => twoResizingData.medium = _v;

    private void UpdateThreeSmall(int _v) => threeResizingData.small = _v;
    private void UpdateThreeMedium(int _v) => threeResizingData.medium = _v;
    private void UpdateThreeLarge(int _v) => threeResizingData.large = _v;


    public void RevertChanges()
    {
        // 元に戻す

    }

    public void ApplySettings()
    {
        // large > medium > small
        // になってなかったらエラー

        databaseDirector.ApplySettings(
            progressBarMax, 
            resizingMode, 
            twoResizingData, 
            threeResizingData,
            projectSettingsController.GetChangedProjectDataList());
    }
}
