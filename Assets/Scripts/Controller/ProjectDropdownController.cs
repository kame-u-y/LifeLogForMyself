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
        currentWorkMeterCtrler.ChangeColor(
            new Color(p.pieColor.r / 255.0f, p.pieColor.g / 255.0f, p.pieColor.b / 255.0f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitItems()
    {
        List<ProjectData> projectList = databaseDirector.FetchProjectList();
        for (int i=0; i<projectList.Count; i++)
        {
            if (i >= dropdown_.options.Count)
            {
                dropdown_.options.Add(new Dropdown.OptionData { text = projectList[i].name });
                return;
            }
            else
            {
                dropdown_.options[i].text = projectList[i].name;
            }
        }
    }
}
