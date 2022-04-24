using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProjectSettingsController : MonoBehaviour
{
    /// <summary>
    /// Project�̃f�[�^���i�[����N���X
    /// </summary>
    internal class ProjectItemData
    {
        internal GameObject gameObject_;
        internal ProjectData projectData;
    }

    /// <summary>
    /// Key: projectItems��id
    /// Value: 
    ///     id��gameObject�̂���index
    ///     projectItems[id].gameObject_.GetSiblingIndex()
    /// </summary>
    private Dictionary<int, int> projectOrderDictionary;

    DatabaseDirector databaseDirector;

    [SerializeField]
    GameObject itemTemplate;
    InputEventDirector inputEventDirector;
    List<ProjectItemData> projectItems;

    List<ProjectData> before;

    // revert apply
    [SerializeField]
    Button settingRevertButton;
    [SerializeField]
    Button settingApplyButton;

    private bool isAnySettingsChanged = false;
    public bool IsAnySettingsChanged
    {
        get => isAnySettingsChanged;
        set => isAnySettingsChanged = value;
    }

    private void Awake()
    {
        projectItems = new List<ProjectItemData>();

    }

    // Start is called before the first frame update
    void Start()
    {
        databaseDirector = GameObject.Find("DatabaseDirector").GetComponent<DatabaseDirector>();
        inputEventDirector = InputEventDirector.Instance;

        projectOrderDictionary = new Dictionary<int, int>();

        SetAnySettingsChanged(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Project Settings�̕\��������
    /// </summary>
    /// <param name="_project"></param>
    public void InitializeItems(List<ProjectData> _projects)
    {
        before = new List<ProjectData>(_projects);
        ResetItems();
        before.ForEach(v => CreateItem(v));

        SetAnySettingsChanged(false);
    }

    public void RevertProjectChanges()
    {
        ResetItems();
        projectItems = new List<ProjectItemData>();
        before.ForEach(v => CreateItem(v));

        SetAnySettingsChanged(false);
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
        GameObject newItem = SetUpProjectItem(_project);
        // add events
        inputEventDirector.AddProjectSettingItemEvents(newItem, projectItems.Count);

        projectOrderDictionary.Add(projectItems.Count, projectItems.Count);

        projectItems.Add(new ProjectItemData()
        {
            gameObject_ = newItem,
            projectData = _project.ShallowCopy()
        });
    }

    public void AddNewProject()
    {
        ProjectData newProject = new ProjectData();
        GameObject newItem = SetUpProjectItem(newProject);
        // add events
        inputEventDirector.AddProjectSettingItemEvents(newItem, projectItems.Count);

        projectOrderDictionary.Add(projectItems.Count, projectItems.Count);

        projectItems.Add(new ProjectItemData()
        {
            gameObject_ = newItem,
            projectData = newProject
        });


        SetAnySettingsChanged(true);
    }

    private GameObject SetUpProjectItem(ProjectData _project)
    {
        GameObject newItem = Instantiate(itemTemplate, Vector3.zero, Quaternion.identity, this.transform);
        // index
        newItem.transform.Find("Values/Index").GetComponent<TextMeshProUGUI>().text = projectItems.Count.ToString();
        // projectName
        var projectNameField = newItem.transform.Find("Values/ProjectName").GetComponent<InputField>();
        projectNameField.text = _project.name;
        // projectColor
        newItem.transform.Find("Values/ProjectColor").GetComponent<Image>().color
            = _project.pieColor.GetWithColorFormat();
        //notificationMode
        var dropdown = newItem.transform.Find("Values/NotificationMode").GetComponent<Dropdown>();
        dropdown.value = dropdown.options.FindIndex(v =>
        {
            //Debug.Log(v.text + ", " + _project.notificationMode);
            return v.text == _project.notificationMode.ToString();
        });

        var moveUpper = newItem.transform.Find("Values/MoveUpper/Button").GetComponent<Button>();
        var moveLower = newItem.transform.Find("Values/MoveLower/Button").GetComponent<Button>();
        // no project�Ɏg���Ȃ�UI���\����
        if (projectItems.Count == 0)
        {
            projectNameField.interactable = false;

            moveUpper.interactable = false;
            Color uc = moveUpper.GetComponent<Image>().color;
            uc.a = 0.0f;
            moveUpper.GetComponent<Image>().color = uc;

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

        if (projectItems.Count == 1)
        {
            moveUpper.interactable = false;
        }
        if (projectItems.Count == before.Count - 1)
        {
            moveLower.interactable = false;
        }

        // no project��projectName��interactable�����炿�炷��̂�active���Ō�ɂ���
        newItem.SetActive(true);

        return newItem;
    }

    /// <summary>
    /// �v���W�F�N�g���̕ύX�C�x���g
    /// </summary>
    /// <param name="_value"></param>
    /// <param name="_id"></param>
    public void UpdateProjectName(string _value, int _id)
    {
        projectItems[_id].gameObject_.transform.Find("Values/ProjectName").GetComponent<InputField>().text = _value;
        projectItems[_id].projectData.name = _value;

        SetAnySettingsChanged(true);
    }

    /// <summary>
    /// PopupController�p�J���[�s�b�J�[�̕\�����̐F�ݒ�
    /// </summary>
    /// <param name="_id"></param>
    /// <returns></returns>
    public Color FetchProjectColor(int _id)
        => projectItems[_id].projectData.pieColor.GetWithColorFormat();

    /// <summary>
    /// �v���W�F�N�g�J���[�̕ύX�C�x���g
    /// �J���[�s�b�J�[�̃|�b�v�A�b�v�\���E�ύX�K�p
    /// </summary>
    public void UpdateProjectColor(int _id, Color _c)
    { 
        projectItems[_id].gameObject_.transform
            .Find("Values/ProjectColor").GetComponent<Image>().color = _c;
        projectItems[_id].projectData.pieColor = ColorData.ConvertColorToColorData(_c);
        SetAnySettingsChanged(true);
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

        SetAnySettingsChanged(true);
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
        var upper = this.transform.GetChild(upperSibId);
        var target = projectItems[_id].gameObject_.transform;

        upper.Find("Values/Index").GetComponent<TextMeshProUGUI>().text = sibId.ToString();
        target.Find("Values/Index").GetComponent<TextMeshProUGUI>().text = upperSibId.ToString();
        target.SetSiblingIndex(upperSibId);

        // Order Dictionary�̕ύX
        var upperItemId = projectItems.FindIndex(
            v => v.projectData.name == upper.Find("Values/ProjectName").GetComponent<InputField>().text);
        projectOrderDictionary[upperItemId] = sibId;
        projectOrderDictionary[_id] = upperSibId;

        // �{�^����interactable�؂�ւ�
        if (sibId == 2)
        {
            upper.Find("Values/MoveUpper/Button").GetComponent<Button>().interactable = true;
            target.Find("Values/MoveUpper/Button").GetComponent<Button>().interactable = false;
        }
        else if (sibId == projectItems.Count - 1)
        {
            upper.Find("Values/MoveLower/Button").GetComponent<Button>().interactable = false;
            target.Find("Values/MoveLower/Button").GetComponent<Button>().interactable = true;
        }


        SetAnySettingsChanged(true);
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
        var lower = this.transform.GetChild(lowerSibId);
        var target = projectItems[_id].gameObject_.transform;

        lower.Find("Values/Index").GetComponent<TextMeshProUGUI>().text = sibId.ToString();
        target.Find("Values/Index").GetComponent<TextMeshProUGUI>().text = (lowerSibId).ToString();
        target.SetSiblingIndex(lowerSibId);

        //// Order Dictionary�̕ύX
        var lowerItemId = projectItems.FindIndex(
            v => v.projectData.name == lower.Find("Values/ProjectName").GetComponent<InputField>().text);
        projectOrderDictionary[lowerItemId] = sibId;
        projectOrderDictionary[_id] = lowerSibId;

        if (sibId == projectItems.Count - 2)
        {
            lower.Find("Values/MoveLower/Button").GetComponent<Button>().interactable = true;
            target.Find("Values/MoveLower/Button").GetComponent<Button>().interactable = false;
        }
        else if (sibId == 1)
        {
            lower.Find("Values/MoveUpper/Button").GetComponent<Button>().interactable = false;
            target.Find("Values/MoveUpper/Button").GetComponent<Button>().interactable = true;
        }

        SetAnySettingsChanged(true);
    }

    /// <summary>
    /// �C�x���g�F�v���W�F�N�g���폜����
    /// �폜�m�F�A���[�g�̃|�b�v�A�b�v��\���E���s
    /// </summary>
    /// <param name="_id"></param>
    public void ApplyProjectDelete(int _id)
    {
        // setting���project���폜
        var sibId = projectItems[_id].gameObject_.transform.GetSiblingIndex();
        Debug.Log(_id + "," + sibId);
        for (int i = sibId + 1; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).Find("Values/Index")
                .GetComponent<TextMeshProUGUI>().text = (i - 1).ToString();

            // Order Dictionary�̕ύX
            var iName = this.transform.GetChild(i).Find("Values/ProjectName").GetComponent<InputField>().text;
            var iProjectId = projectItems.FindIndex(v => v.projectData.name == iName);
            projectOrderDictionary[iProjectId] = i - 1;
        }
        projectOrderDictionary.Remove(_id);
        Destroy(projectItems[_id].gameObject_);
        // savedata��project�폜�𔽉f
        databaseDirector.ApplyProjectDelete(projectItems[_id].projectData.name);
        projectItems.RemoveAt(_id);
        before = GetProjectDataList();
    }

    public void ApplyProjectChanges()
    {
        Debug.Log("apply project");
        var sortedProjects = new List<ProjectData>();
        for (int i=0; i<projectItems.Count; i++)
        {
            sortedProjects.Add(null);
        }

        foreach (KeyValuePair<int, int> kvp in projectOrderDictionary)
        {
            Debug.Log($"{projectItems[kvp.Key].projectData.name}: {kvp.Value}");
            sortedProjects[kvp.Value] = projectItems[kvp.Key].projectData;    
        }

        //databaseDirector.ApplyProjectSettings(GetProjectDataList());
        databaseDirector.ApplyProjectSettings(sortedProjects);

        SetAnySettingsChanged(false);
    }

    /// <summary>
    /// �ύX�𔽉f������Ƃ��ɕK�v��Project�̃f�[�^��Ԃ��֐�
    /// </summary>
    /// <returns></returns>
    public List<ProjectData> GetProjectDataList()
        => projectItems.ConvertAll(v => v.projectData);

    private void SetAnySettingsChanged(bool _b)
    {
        IsAnySettingsChanged = _b;
        settingRevertButton.interactable = _b;
        settingApplyButton.interactable = _b;
    }

}
