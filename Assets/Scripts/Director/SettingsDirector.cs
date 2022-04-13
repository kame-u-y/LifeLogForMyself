using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsDirector : MonoBehaviour
{
    private int progressBarMax = 0;

    [SerializeField]
    private GameObject settingsTab;
    private Button generalTabButton;
    private Button projectsTabButton;

    [SerializeField]
    private GameObject generalSettings;
    [SerializeField]
    private GameObject projectsSettings;

    private List<GameObject> projectList;

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
        generalTabButton = settingsTab.transform.Find("General").GetComponent<Button>();
        projectsTabButton = settingsTab.transform.Find("Projects").GetComponent<Button>();

        SwitchSettingsMode(SettingsMode.General);

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

    private void InitializeProjectList()
    {

    }

    public void ApplySettings()
    {

    }
}
