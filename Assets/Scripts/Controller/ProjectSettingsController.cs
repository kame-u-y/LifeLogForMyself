using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectSettingsController : MonoBehaviour
{
    [SerializeField]
    GameObject itemTemplate;
    InputEventDirector inputEventDirector;
    List<ProjectItemData> projectItems;



    // Start is called before the first frame update
    void Start()
    {
        inputEventDirector = GameObject.Find("InputEventDirector").GetComponent<InputEventDirector>();
        projectItems = new List<ProjectItemData>();
        
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
        newItem.transform.Find("Values/ProjectName").GetComponent<InputField>().text = "ジャンバラヤ";
        newItem.transform.Find("Values/ProjectColor").GetComponent<Image>().color = new Color(1f, 1f, 0f);
        // ↓うまくいくかな？captionText変えただけだけど...
        newItem.transform.Find("Values/NotificationMode").GetComponent<Dropdown>().captionText.text
            = _project.notificationMode.ToString();
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
