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
        InitItems();
        ProjectData p = databaseDirector.FindProject(dropdown_.options[0].text);
        Color c = new Color(p.pieColor.r, p.pieColor.g, p.pieColor.b);
        currentWorkMeterCtrler.UpdateColor(c);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void InitItems()
    {
        List<ProjectData> projectList = databaseDirector.FetchProjectList();
        dropdown_.options = new List<Dropdown.OptionData>();
        for (int i = 0; i < projectList.Count; i++)
        {
            dropdown_.options.Add(new Dropdown.OptionData { text = projectList[i].name });
        }
        dropdown_.RefreshShownValue();
    }
}
