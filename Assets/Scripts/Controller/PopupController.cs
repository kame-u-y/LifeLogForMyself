using HSVPicker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PopupController : MonoBehaviour
{
    /// <summary>
    /// Project Settings�Ń|�b�v�A�b�v�𗘗p�����ݒ�ύX�Ɏg�p
    /// ColorPicker, Delete�ɗ��p
    /// </summary>
    private int selectedProjectId = -1;
    public int SelectedProjectId { get => selectedProjectId; }

    [SerializeField]
    private ProjectSettingsController projectSettingsController;

    /// <summary>
    /// UI�\��/��\���p
    /// </summary>
    private GameObject background;
    private GameObject mainMenu;
    private GameObject projectColorPicker;
    private GameObject projectDelete;

    /// <summary>
    /// �J���[�s�b�J�[�A�Z�b�g���g�p
    /// </summary>
    private ColorPicker colorPicker;

    /// <summary>
    /// �|�b�v�A�b�v�̎�ނ��`
    /// �w�肳�ꂽ��ނɂ���ĕ\����ύX���邽��
    /// </summary>
    public enum PopupMode
    {
        MainMenu,
        ProjectColorPicker,
        ProjectDelete
    }

    // Start is called before the first frame update
    void Start()
    {
        background = this.transform.Find("Popup/Image").gameObject;
        mainMenu = this.transform.Find("Popup/MainMenu").gameObject;
        projectColorPicker = this.transform.Find("Popup/ProjectColorPicker").gameObject;
        projectDelete = this.transform.Find("Popup/ProjectDelete").gameObject;

        colorPicker = this.transform.Find("Popup/ProjectColorPicker/Picker 2.0").GetComponent<ColorPicker>();

        this.gameObject.SetActive(false);
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

        if (_mode == PopupMode.MainMenu)
        {
            bool b = true;
            background.SetActive(b);
            mainMenu.SetActive(b);
            projectColorPicker.SetActive(!b);
            projectDelete.SetActive(!b);
        }
        else if (_mode == PopupMode.ProjectColorPicker)
        {
            bool b = true;
            background.SetActive(!b);
            mainMenu.SetActive(!b);
            projectColorPicker.SetActive(b);
            projectDelete.SetActive(!b);
        }
        else if (_mode == PopupMode.ProjectDelete)
        {
            bool b = true;
            background.SetActive(b);
            mainMenu.SetActive(!b);
            projectColorPicker.SetActive(!b);
            projectDelete.SetActive(b);
        }

        this.gameObject.SetActive(true);
    }

    /// <summary>
    /// ProjectSettings�̃J���[�s�b�J�[�p�̃|�b�v�A�b�v��\������
    /// </summary>
    /// <param name="_mode"></param>
    /// <param name="_projectId"></param>
    public void OpenProjectColorPickerPopup(PopupMode _mode, int _projectId)
    {
        selectedProjectId = _projectId;
        Color color = projectSettingsController.FetchProjectColor(_projectId);
        OpenPopup(_mode);
        // OpenPopup���ザ��Ȃ��ƃJ���[�s�b�J�[�̓K�؂ɏ����F���ݒ�ł��Ȃ�
        colorPicker.CurrentColor = color;
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

    public void ClosePopup()
    {
        selectedProjectId = -1;
        this.gameObject.SetActive(false);
    }



}
