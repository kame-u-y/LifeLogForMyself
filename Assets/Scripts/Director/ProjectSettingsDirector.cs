using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProjectSettingsDirector : SingletonMonoBehaviourFast<ProjectSettingsDirector>
{
    DatabaseDirector databaseDirector;
    InputEventDirector inputEventDirector;
    SettingsUIDirector settingsUIDirector;
    
    // �e���v���[�g�I�u�W�F�N�g
    [SerializeField]
    GameObject itemTemplate;
 
    // �ύX�O����ێ�
    List<ProjectData> beforeChangeData;
    // �ύX�����ێ�
    List<ProjectItemData> projectItems;

    /// <summary>
    /// Project�̃f�[�^���i�[����N���X
    /// </summary>
    internal class ProjectItemData
    {
        internal GameObject gameObject_;
        internal ProjectData projectData;
    }

    /// <summary>
    /// ���ѕς�����ێ�
    /// projectItems���index(Key)�ƁA
    /// gameObject�Ƃ��Ă�siblingIndex(Value)�̎���
    /// </summary>
    private Dictionary<int, int> projectOrderDictionary;

    private bool isAnySettingsChanged = false;
    public bool IsAnySettingsChanged => isAnySettingsChanged;


    new void Awake()
    {
        base.Awake();
        projectItems = new List<ProjectItemData>();
    }

    // Start is called beforeChangeData the first frame update
    void Start()
    {
        databaseDirector = DatabaseDirector.Instance;
        inputEventDirector = InputEventDirector.Instance;
        settingsUIDirector = SettingsUIDirector.Instance;

        projectOrderDictionary = new Dictionary<int, int>();

        InitializeItems(databaseDirector.FetchProjectList());

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
        beforeChangeData = new List<ProjectData>(_projects);
        ResetItems();
        beforeChangeData.ForEach(v => CreateItem(v));

        SetAnySettingsChanged(false);
    }

    public void RevertProjectChanges()
    {
        ResetItems();
        projectItems = new List<ProjectItemData>();
        projectOrderDictionary = new Dictionary<int, int>();
        beforeChangeData.ForEach(v => CreateItem(v));

        SetAnySettingsChanged(false);
    }


    /// <summary>
    /// �������p Project�̃I�u�W�F�N�g�̍폜
    /// </summary>
    private void ResetItems()
    {
        Debug.Log(settingsUIDirector.ProjectItemContainer);
        for (int i = 0; i < settingsUIDirector.ProjectItemContainer.childCount; i++)
        {
            Destroy(settingsUIDirector.ProjectItemContainer.GetChild(i).gameObject);
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

        if (projectOrderDictionary.ContainsKey(projectItems.Count))
        {
            projectOrderDictionary[projectItems.Count] = projectItems.Count;
        }
        else
        {
            projectOrderDictionary.Add(projectItems.Count, projectItems.Count);
        }

        projectItems.Add(new ProjectItemData()
        {
            gameObject_ = newItem,
            projectData = newProject
        });


        SetAnySettingsChanged(true);
    }

    private GameObject SetUpProjectItem(ProjectData _project)
    {
        GameObject newItem = Instantiate(itemTemplate, settingsUIDirector.ProjectItemContainer);

        // index
        SettingsUIDirector.SetProjectIndex(newItem, projectItems.Count);

        // projectName
        InputField nameInput = SettingsUIDirector.GetProjectNameInputField(newItem);
        nameInput.text = _project.name;
        if (projectItems.Count == 0)
        {
            nameInput.interactable = false;
        }

        // projectColor
        SettingsUIDirector.SetProjectColor(newItem, _project.pieColor.GetWithColorFormat());

        // notificationMode
        Dropdown notifDropdown = SettingsUIDirector.GetProjectNotifModeDropdown(newItem);
        notifDropdown.value = notifDropdown.options.FindIndex(
            v => v.text == _project.notificationMode.ToString());

        // move upper / lower
        var moveUpper = SettingsUIDirector.GetProjectMoveUpperButton(newItem);
        var moveLower = SettingsUIDirector.GetProjectMoveLowerButton(newItem);
        // no project�Ɏg���Ȃ�UI���\����
        if (projectItems.Count == 0)
        {
            moveUpper.interactable = false;
            Color uc = moveUpper.GetComponent<Image>().color;
            uc.a = 0.0f;
            moveUpper.GetComponent<Image>().color = uc;

            moveLower.interactable = false;
            Color lc = moveLower.GetComponent<Image>().color;
            lc.a = 0.0f;
            moveLower.GetComponent<Image>().color = lc;
        }
        // ��[���[�̃v���W�F�N�g��MoveButton�̏���
        if (projectItems.Count == 1)
        {
            moveUpper.interactable = false;
        }
        if (projectItems.Count == beforeChangeData.Count - 1)
        {
            moveLower.interactable = false;
        }

        // delete
        if (projectItems.Count == 0)
        {
            var delete = SettingsUIDirector.GetProjectDeleteButton(newItem);
            delete.interactable = false;
            Color dc = delete.GetComponent<Image>().color;
            dc.a = 0.0f;
            delete.GetComponent<Image>().color = dc;
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
        SettingsUIDirector.SetProjectName(projectItems[_id].gameObject_, _value);
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
        SettingsUIDirector.SetProjectColor(projectItems[_id].gameObject_, _c);
        projectItems[_id].projectData.pieColor = ColorData.ConvertColorToColorData(_c);
        SetAnySettingsChanged(true);
    }

    /// <summary>
    /// �ʒm���[�h�ύX�C�x���g
    /// </summary>
    /// <param name="_id"></param>
    public void UpdateNotifMode(int _id)
    {
        Dropdown dropdown = SettingsUIDirector.GetProjectNotifModeDropdown(projectItems[_id].gameObject_);
        Enum.TryParse(
            dropdown.captionText.text,
            out projectItems[_id].projectData.notificationMode);

        SetAnySettingsChanged(true);
    }

    /// <summary>
    /// �C�x���g�F������index�̃v���W�F�N�g�����O��
    /// hierarchy��̃I�u�W�F�N�g�̕��т�ύX
    /// projectItems�̕��ю��͕̂ύX���Ȃ�
    /// </summary>
    /// <param name="_projectId"></param>
    public void MoveUpperItem(int _projectId)
    {
        // projects��index�����ւ�������
        int targetSibId = projectItems[_projectId].gameObject_.transform.GetSiblingIndex();

        if (targetSibId <= 1) return;

        int upperSibId = targetSibId - 1;
        GameObject upper = settingsUIDirector.ProjectItemContainer.GetChild(upperSibId).gameObject;
        GameObject target = projectItems[_projectId].gameObject_;

        SettingsUIDirector.SetProjectIndex(upper, targetSibId);
        SettingsUIDirector.SetProjectIndex(target, upperSibId);

        target.transform.SetSiblingIndex(upperSibId);

        // Order Dictionary�̕ύX
        InputField nameInput = SettingsUIDirector.GetProjectNameInputField(upper);
        int upperItemId = projectItems.FindIndex(
            v => v.projectData.name == nameInput.text);
        projectOrderDictionary[upperItemId] = targetSibId;
        projectOrderDictionary[_projectId] = upperSibId;

        // �{�^����interactable�؂�ւ�
        if (targetSibId == 2)
        {
            SettingsUIDirector.SetProjectUpperInteractable(upper, true);
            SettingsUIDirector.SetProjectUpperInteractable(target, false);
        }
        else if (targetSibId == projectItems.Count - 1)
        {
            SettingsUIDirector.SetProjectLowerInteractable(upper, false);
            SettingsUIDirector.SetProjectLowerInteractable(target, true);
        }


        SetAnySettingsChanged(true);
    }

    /// <summary>
    /// �C�x���g�F������index�̃v���W�F�N�g�������
    /// hierarchy��̃I�u�W�F�N�g�̕��т�ύX
    /// projectItems�̕��ю��͕̂ύX���Ȃ�
    /// </summary>
    /// <param name="_projectId"></param>
    public void MoveLowerItem(int _projectId)
    {
        int targetSibId = projectItems[_projectId].gameObject_.transform.GetSiblingIndex();

        if (targetSibId == 0 || targetSibId >= settingsUIDirector.ProjectItemContainer.childCount - 1) 
            return;

        int lowerSibId = targetSibId + 1;
        GameObject lower = settingsUIDirector.ProjectItemContainer.GetChild(lowerSibId).gameObject;
        GameObject target = projectItems[_projectId].gameObject_;

        SettingsUIDirector.SetProjectIndex(lower, targetSibId);
        SettingsUIDirector.SetProjectIndex(target, lowerSibId);
        target.transform.SetSiblingIndex(lowerSibId);

        //// Order Dictionary�̕ύX
        InputField nameInput = SettingsUIDirector.GetProjectNameInputField(lower);
        int lowerItemId = projectItems.FindIndex(
            v => v.projectData.name == nameInput.text);
        projectOrderDictionary[lowerItemId] = targetSibId;
        projectOrderDictionary[_projectId] = lowerSibId;

        if (targetSibId == projectItems.Count - 2)
        {
            SettingsUIDirector.SetProjectLowerInteractable(lower, true);
            SettingsUIDirector.SetProjectLowerInteractable(target, false);
        }
        else if (targetSibId == 1)
        {
            SettingsUIDirector.SetProjectUpperInteractable(lower, false);
            SettingsUIDirector.SetProjectUpperInteractable(target, true);
        }

        SetAnySettingsChanged(true);
    }

    /// <summary>
    /// �C�x���g�F�v���W�F�N�g���폜����
    /// �폜�m�F�A���[�g�̃|�b�v�A�b�v��\���E���s
    /// </summary>
    /// <param name="_projectId"></param>
    public void ApplyProjectDelete(int _projectId)
    {
        // setting���project���폜
        var targetSibId = projectItems[_projectId].gameObject_.transform.GetSiblingIndex();
        Debug.Log(_projectId + "," + targetSibId);

        // �폜�Ώۈȍ~�̃C���f�b�N�X��������炷
        for (int sibId = targetSibId + 1; sibId < settingsUIDirector.ProjectItemContainer.childCount; sibId++)
        {
            GameObject iItem = settingsUIDirector.ProjectItemContainer.GetChild(sibId).gameObject;

            SettingsUIDirector.SetProjectIndex(iItem, sibId - 1);

            // Order Dictionary�̕ύX
            string iName = SettingsUIDirector.GetProjectNameInputField(iItem).text;
            int iProjectId = projectItems.FindIndex(v => v.projectData.name == iName);
            projectOrderDictionary[iProjectId] = sibId - 1;
        }

        projectOrderDictionary.Remove(_projectId);
        Destroy(projectItems[_projectId].gameObject_);

        // savedata��project�폜�𔽉f
        if (_projectId < beforeChangeData.Count)
        {
            databaseDirector.ApplyProjectDelete(projectItems[_projectId].projectData.name);
        }
        projectItems.RemoveAt(_projectId);
        if (_projectId < beforeChangeData.Count)
        {
            beforeChangeData = GetProjectDataList();
        }
    }

    public bool IsNewProjectItem(int _projectId)
        => _projectId >= beforeChangeData.Count;

    public void ApplyProjectChanges()
    {
        Debug.Log("apply project");
        var sortedProjects = new List<ProjectData>();
        for (int i = 0; i < projectItems.Count; i++)
        {
            sortedProjects.Add(null);
        }

        foreach (KeyValuePair<int, int> kvp in projectOrderDictionary)
        {
            Debug.Log($"{projectItems[kvp.Key].projectData.name}: {kvp.Value}");
            sortedProjects[kvp.Value] = projectItems[kvp.Key].projectData;
        }

        databaseDirector.ApplyProjectSettings(sortedProjects);
        beforeChangeData = GetProjectDataList();

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
        isAnySettingsChanged = _b;
        settingsUIDirector.ProjectSettingRevertButton.interactable = _b;
        settingsUIDirector.ProjectSettingApplyButton.interactable = _b;
    }


}
