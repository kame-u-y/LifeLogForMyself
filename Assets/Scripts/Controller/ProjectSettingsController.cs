using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProjectSettingsController : MonoBehaviour
{
    [SerializeField]
    GameObject itemTemplate;
    InputEventDirector inputEventDirector;
    List<ProjectItemData> projectItems;


    private void Awake()
    {
        projectItems = new List<ProjectItemData>();
        
    }

    // Start is called before the first frame update
    void Start()
    {
        inputEventDirector = GameObject.Find("InputEventDirector").GetComponent<InputEventDirector>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DisplayItems(List<ProjectData> _project)
    {
        ResetItems();
        _project.ForEach(v => CreateItem(v));
    }

    private void ResetItems()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            Destroy(this.transform.GetChild(i).gameObject);
        }
    }

    private void CreateItem(ProjectData _project)
    {
        GameObject newItem = Instantiate(itemTemplate, Vector3.zero, Quaternion.identity, this.transform);
        newItem.SetActive(true);
        Debug.Log(newItem.transform.Find("Values/Index").GetComponent<TextMeshProUGUI>());
        newItem.transform.Find("Values/Index").GetComponent<TextMeshProUGUI>().text = projectItems.Count.ToString();
        var projectNameField = newItem.transform.Find("Values/ProjectName").GetComponent<InputField>();
        projectNameField.text = _project.name;
        if (projectItems.Count == 0)
        {
            projectNameField.interactable = false;
        }
        Color c = new Color(_project.pieColor.r / 255.0f, _project.pieColor.g / 255.0f, _project.pieColor.b / 255.0f);
        newItem.transform.Find("Values/ProjectColor").GetComponent<Image>().color = c;
        // ↓うまくいくかな？captionText変えただけだけど...
        var dropdown = newItem.transform.Find("Values/NotificationMode").GetComponent<Dropdown>();
        //dropdown.AddOptions(new List<string>()
        //{
        //    NotificationMode.None.ToString(),
        //    NotificationMode.Sound.ToString(),
        //    NotificationMode.Pomodoro.ToString()
        //});
        //dropdown.RefreshShownValue();
        dropdown.value = dropdown.options.FindIndex(v => v.text == _project.notificationMode.ToString());

        //dropdown.captionText.text = _project.notificationMode;

        inputEventDirector.AddProjectSettingItemEvents(newItem, projectItems.Count);

        projectItems.Add(new ProjectItemData()
        {
            gameObject_ = newItem,
            projectData = _project
        });
    }

    public void UpdateProjectName(string _value, int _id)
    {
        projectItems[_id].gameObject_.transform.Find("Values/ProjectName").GetComponent<InputField>().text = _value;
    }

    public void DisplayProjectColor()
    {
        // カラーピッカーを表示
    }

    public void UpdateNotificationMode(int _id)
    {
        Enum.TryParse(
            projectItems[_id].gameObject_.transform
                .Find("Values/NotificationMode").GetComponent<Dropdown>().captionText.text,
            out projectItems[_id].projectData.notificationMode);
    }

    public void MoveUpperItem(int _id)
    {

    }

    public void MoveLowerItem(int _id)
    {

    }

    public void DisplayAlertOfDeleteItem(int _id)
    {
        // popupで削除するか確認
    }

    internal class ProjectItemData
    {
        internal GameObject gameObject_;
        internal ProjectData projectData;
    }
}
