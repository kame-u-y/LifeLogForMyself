using HSVPicker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PopupDirector : SingletonMonoBehaviourFast<PopupDirector>
{
    /// <summary>
    /// Project Settings�Ń|�b�v�A�b�v�𗘗p�����ݒ�ύX�Ɏg�p
    /// ColorPicker, Delete�ɗ��p
    /// </summary>
    private int selectedProjectId = -1;
    public int SelectedProjectId { get => selectedProjectId; }

    private ProjectSettingsDirector projectSettingsDirector;
    private PopupUIDirector popupUIDirector;

    //private AppDirector.SettingsMode currentSettingsMode;
    //public AppDirector.SettingsMode CurrentSettingsMode => currentSettingsMode;
    private AppDirector.GameMode destGameMode;
    public AppDirector.GameMode DestGameMode => destGameMode;

    /// <summary>
    /// �|�b�v�A�b�v�̎�ނ��`
    /// �w�肳�ꂽ��ނɂ���ĕ\����ύX���邽��
    /// </summary>
    public enum PopupMode
    {
        MainMenu,
        ProjectColorPicker,
        ProjectDelete,
        ApplySettings,
    }


    // Start is called before the first frame update
    void Start()
    {
        projectSettingsDirector = ProjectSettingsDirector.Instance;
        popupUIDirector = PopupUIDirector.Instance;

        popupUIDirector.PopupContainer.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// �|�b�v�A�b�v�̕\������
    /// PopupMode�ɂ��\����ύX
    /// </summary>
    /// <param name="_mode"></param>
    public void OpenPopup(PopupMode _mode)
    {
        popupUIDirector.InnerBackground
            .SetActive(_mode == PopupMode.MainMenu || _mode == PopupMode.ProjectDelete || _mode == PopupMode.ApplySettings);

        popupUIDirector.MainMenuContainer
            .SetActive(_mode == PopupMode.MainMenu);

        popupUIDirector.ProjectColorPickerContainer
            .SetActive(_mode == PopupMode.ProjectColorPicker);

        popupUIDirector.ProjectDeleteContainer
            .SetActive(_mode == PopupMode.ProjectDelete);

        popupUIDirector.ApplySettingsContainer
            .SetActive(_mode == PopupMode.ApplySettings);

        popupUIDirector.PopupContainer.SetActive(true);
    }

    /// <summary>
    /// ProjectSettings�̃J���[�s�b�J�[�p�̃|�b�v�A�b�v��\������
    /// </summary>
    /// <param name="_mode"></param>
    /// <param name="_projectId"></param>
    public void OpenProjectColorPickerPopup(PopupMode _mode, int _projectId)
    {
        selectedProjectId = _projectId;
        Color color = projectSettingsDirector.FetchProjectColor(_projectId);
        OpenPopup(_mode);
        // OpenPopup���ザ��Ȃ��ƃJ���[�s�b�J�[�̓K�؂ɏ����F���ݒ�ł��Ȃ�
        popupUIDirector.ProjectColorPicker.CurrentColor = color;
    }

    /// <summary>
    /// ProjectSettings�̃v���W�F�N�g�폜�p�̃|�b�v�A�b�v��\������
    /// </summary>
    /// <param name="_mode"></param>
    /// <param name="_projectId"></param>
    public void OpenProjectDeletePopup(PopupMode _mode, int _projectId)
    {
        selectedProjectId = _projectId;
        OpenPopup(_mode);
    }

    public void OpenApplySettingsPopup(PopupMode _mode, AppDirector.GameMode _dest)
    {
        destGameMode = _dest;
        OpenPopup(_mode);
    }

    public void ClosePopup()
    {
        selectedProjectId = -1;
        popupUIDirector.PopupContainer.SetActive(false);
    }



}
