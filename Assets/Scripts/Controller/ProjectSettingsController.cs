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
    /// Project Settings�̕\��������
    /// </summary>
    /// <param name="_project"></param>
    public void DisplayItems(List<ProjectData> _project)
    {
        ResetItems();
        _project.ForEach(v => CreateItem(v));
    }

    /// <summary>
    /// �������p Project�̃I�u�W�F�N�g�̍폜
    /// </summary>
    private void ResetItems()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            Destroy(this.transform.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// �������p �e���v���[�g�����ƂɁA���ꂼ���Project�̃I�u�W�F�N�g����
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

        // no project�Ɏg���Ȃ�UI���\����
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
    /// �v���W�F�N�g���̕ύX�C�x���g
    /// </summary>
    /// <param name="_value"></param>
    /// <param name="_id"></param>
    public void UpdateProjectName(string _value, int _id)
    {
        projectItems[_id].gameObject_.transform.Find("Values/ProjectName").GetComponent<InputField>().text = _value;
    }

    /// <summary>
    /// �v���W�F�N�g�J���[�̕ύX�C�x���g
    /// �J���[�s�b�J�[�̃|�b�v�A�b�v�\���E�ύX�K�p
    /// </summary>
    public void DisplayProjectColor()
    {
        // �J���[�s�b�J�[��\��
    }

    /// <summary>
    /// �ʒm���[�h�ύX�C�x���g
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
    /// �C�x���g�F������index�̃v���W�F�N�g�����O��
    /// hierarchy��̃I�u�W�F�N�g�̕��т�ύX
    /// projectItems�̕��ю��͕̂ύX���Ȃ�
    /// </summary>
    /// <param name="_id"></param>
    public void MoveUpperItem(int _id)
    {
        // projects��index�����ւ�������
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
    /// �C�x���g�F������index�̃v���W�F�N�g�������
    /// hierarchy��̃I�u�W�F�N�g�̕��т�ύX
    /// projectItems�̕��ю��͕̂ύX���Ȃ�
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
    /// �C�x���g�F�v���W�F�N�g���폜����
    /// �폜�m�F�A���[�g�̃|�b�v�A�b�v��\���E���s
    /// </summary>
    /// <param name="_id"></param>
    public void DisplayAlertOfDeleteItem(int _id)
    {
        // popup�ō폜���邩�m�F
    }

    /// <summary>
    /// �ύX�𔽉f������Ƃ��ɕK�v��Project�̃f�[�^��Ԃ��֐�
    /// </summary>
    /// <returns></returns>
    public List<ProjectData> GetChangedProjectDataList()
        => projectItems.ConvertAll(v => v.projectData);

    /// <summary>
    /// Project�̃f�[�^���i�[����N���X
    /// </summary>
    internal class ProjectItemData
    {
        internal GameObject gameObject_;
        internal ProjectData projectData;
    }
}
