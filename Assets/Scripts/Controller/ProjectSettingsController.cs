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

    /// <summary>
    /// Project Settingsの表示初期化
    /// </summary>
    /// <param name="_project"></param>
    public void DisplayItems(List<ProjectData> _project)
    {
        ResetItems();
        _project.ForEach(v => CreateItem(v));
    }

    /// <summary>
    /// 初期化用 Projectのオブジェクトの削除
    /// </summary>
    private void ResetItems()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            Destroy(this.transform.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// 初期化用 テンプレートをもとに、それぞれのProjectのオブジェクト生成
    /// </summary>
    /// <param name="_project"></param>
    private void CreateItem(ProjectData _project)
    {
        GameObject newItem = Instantiate(itemTemplate, Vector3.zero, Quaternion.identity, this.transform);
        newItem.SetActive(true);
        // index
        newItem.transform.Find("Values/Index").GetComponent<TextMeshProUGUI>().text = projectItems.Count.ToString();
        // projectName
        var projectNameField = newItem.transform.Find("Values/ProjectName").GetComponent<InputField>();
        projectNameField.text = _project.name;
        // projectColor
        Color c = new Color(_project.pieColor.r / 255.0f, _project.pieColor.g / 255.0f, _project.pieColor.b / 255.0f);
        newItem.transform.Find("Values/ProjectColor").GetComponent<Image>().color = c;
        //notificationMode
        var dropdown = newItem.transform.Find("Values/NotificationMode").GetComponent<Dropdown>();
        dropdown.value = dropdown.options.FindIndex(v =>
        {
            Debug.Log(v.text + ", " + _project.notificationMode);
            return v.text == _project.notificationMode.ToString();
        });

        // no projectに使えないUIを非表示に
        if (projectItems.Count == 0)
        {
            projectNameField.interactable = false;

            var moveUpper = newItem.transform.Find("Values/MoveUpper/Button").GetComponent<Button>();
            moveUpper.interactable = false;
            Color uc = moveUpper.GetComponent<Image>().color;
            uc.a = 0.0f;
            moveUpper.GetComponent<Image>().color = uc;

            var moveLower = newItem.transform.Find("Values/MoveLower/Button").GetComponent<Button>();
            moveLower.interactable = false;
            Color lc = moveLower.GetComponent<Image>().color;
            lc.a = 0.0f;
            moveLower.GetComponent<Image>().color = lc;

            var delete = newItem.transform.Find("Values/Delete/Button").GetComponent<Button>();
            delete.interactable = false;
            Color dc = delete.GetComponent<Image>().color;
            dc.a = 0.0f;
            delete.GetComponent<Image>().color = dc;
        }
        // add events
        inputEventDirector.AddProjectSettingItemEvents(newItem, projectItems.Count);

        projectItems.Add(new ProjectItemData()
        {
            gameObject_ = newItem,
            projectData = _project
        });
    }

    /// <summary>
    /// プロジェクト名の変更イベント
    /// </summary>
    /// <param name="_value"></param>
    /// <param name="_id"></param>
    public void UpdateProjectName(string _value, int _id)
    {
        projectItems[_id].gameObject_.transform.Find("Values/ProjectName").GetComponent<InputField>().text = _value;
    }

    /// <summary>
    /// プロジェクトカラーの変更イベント
    /// カラーピッカーのポップアップ表示・変更適用
    /// </summary>
    public void DisplayProjectColor()
    {
        // カラーピッカーを表示
    }

    /// <summary>
    /// 通知モード変更イベント
    /// </summary>
    /// <param name="_id"></param>
    public void UpdateNotificationMode(int _id)
    {
        Enum.TryParse(
            projectItems[_id].gameObject_.transform
                .Find("Values/NotificationMode").GetComponent<Dropdown>().captionText.text,
            out projectItems[_id].projectData.notificationMode);
    }

    /// <summary>
    /// イベント：引数のindexのプロジェクトを一つ手前に
    /// hierarchy上のオブジェクトの並びを変更
    /// projectItemsの並び自体は変更しない
    /// </summary>
    /// <param name="_id"></param>
    public void MoveUpperItem(int _id)
    {
        // projectsのindexを入れ替えたいね
        var sibId = projectItems[_id].gameObject_.transform.GetSiblingIndex();
        if (sibId <= 1) return;
        var upperSibId = sibId - 1;
        this.transform.GetChild(upperSibId).Find("Values/Index")
            .GetComponent<TextMeshProUGUI>().text = sibId.ToString();
        projectItems[_id].gameObject_.transform.Find("Values/Index")
            .GetComponent<TextMeshProUGUI>().text = upperSibId.ToString();
        projectItems[_id].gameObject_.transform.SetSiblingIndex(upperSibId);
    }

    /// <summary>
    /// イベント：引数のindexのプロジェクトを一つ後ろに
    /// hierarchy上のオブジェクトの並びを変更
    /// projectItemsの並び自体は変更しない
    /// </summary>
    /// <param name="_id"></param>
    public void MoveLowerItem(int _id)
    {
        var sibId = projectItems[_id].gameObject_.transform.GetSiblingIndex();
        if (sibId == 0 || sibId >= this.transform.childCount - 1) return;
        var lowerSibId = sibId + 1;
        this.transform.GetChild(lowerSibId).Find("Values/Index")
            .GetComponent<TextMeshProUGUI>().text = sibId.ToString();
        projectItems[_id].gameObject_.transform.Find("Values/Index")
            .GetComponent<TextMeshProUGUI>().text = (lowerSibId).ToString();
        projectItems[_id].gameObject_.transform.SetSiblingIndex(lowerSibId);
    }

    /// <summary>
    /// イベント：プロジェクトを削除する
    /// 削除確認アラートのポップアップを表示・実行
    /// </summary>
    /// <param name="_id"></param>
    public void DisplayAlertOfDeleteItem(int _id)
    {
        // popupで削除するか確認
    }

    /// <summary>
    /// 変更を反映させるときに必要なProjectのデータを返す関数
    /// </summary>
    /// <returns></returns>
    public List<ProjectData> GetChangedProjectDataList()
        => projectItems.ConvertAll(v => v.projectData);

    /// <summary>
    /// Projectのデータを格納するクラス
    /// </summary>
    internal class ProjectItemData
    {
        internal GameObject gameObject_;
        internal ProjectData projectData;
    }
}
