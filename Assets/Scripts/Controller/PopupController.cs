using HSVPicker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PopupController : MonoBehaviour
{
    private int selectedProjectId = -1;
    public int SelectedProjectId { get => selectedProjectId; }

    [SerializeField]
    ProjectSettingsController projectSettingsController;

    GameObject background;
    GameObject mainMenu;
    GameObject projectColorPicker;
    GameObject projectDelete;

    ColorPicker colorPicker;

    //public int GetSelectedProjectId() => selectedProjectId;

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

    public void OpenProjectColorPickerPopup(PopupMode _mode, int _projectId)
    {
        selectedProjectId = _projectId;
        Color color = projectSettingsController.FetchProjectColor(_projectId);
        OpenPopup(_mode);
        // OpenPopupより後じゃないとカラーピッカーの適切に初期色が設定できない
        colorPicker.CurrentColor = color;
    }

    //public void OpenProjectDeletePopup(PopupMode _mode, int _projectId)
    //{
    //    selectedProjectId = _projectId;
    //    OpenPopup(_mode);
    //}

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
