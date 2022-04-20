using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectDropdownController : MonoBehaviour
{
    Dropdown dropdown_;

    DatabaseDirector databaseDirector;
    [SerializeField]
    CurrentWorkMeterController currentWorkMeterCtrler;

    // Start is called before the first frame update
    void Start()
    {
        databaseDirector = GameObject.Find("DatabaseDirector").GetComponent<DatabaseDirector>();
        dropdown_ = this.GetComponent<Dropdown>();
        UpdateItems();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateItems()
    {
        List<ProjectData> projectList = databaseDirector.FetchProjectList();
        dropdown_.options = new List<Dropdown.OptionData>();
        dropdown_.ClearOptions();
        for (int i = 0; i < projectList.Count; i++)
        {
            dropdown_.options.Add(new Dropdown.OptionData { text = projectList[i].name });
        }
        dropdown_.RefreshShownValue();

        string selectedName = databaseDirector.FetchSelectedProject();
        dropdown_.value = dropdown_.options.FindIndex(v => v.text == selectedName);
        
    }
}
