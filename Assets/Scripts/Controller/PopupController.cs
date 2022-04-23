using HSVPicker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PopupController : MonoBehaviour
{
    /// <summary>
    /// Project Settingsでポップアップを利用した設定変更に使用
    /// ColorPicker, Deleteに利用
    /// </summary>
    private int selectedProjectId = -1;
    public int SelectedProjectId { get => selectedProjectId; }

    [SerializeField]
    private ProjectSettingsController projectSettingsController;

    /// <summary>
    /// UI表示/非表示用
    /// </summary>
    private GameObject background;
    private GameObject mainMenu;
    private GameObject projectColorPicker;
    private GameObject projectDelete;

    /// <summary>
    /// カラーピッカーアセットを使用
    /// </summary>
    private ColorPicker colorPicker;

    /// <summary>
    /// ポップアップの種類を定義
    /// 指定された種類によって表示を変更するため
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
    /// ポップアップの表示処理
    /// PopupModeにより表示を変更
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
    /// ProjectSettingsのカラーピッカー用のポップアップを表示する
    /// </summary>
    /// <param name="_mode"></param>
    /// <param name="_projectId"></param>
    public void OpenProjectColorPickerPopup(PopupMode _mode, int _projectId)
    {
        selectedProjectId = _projectId;
        Color color = projectSettingsController.FetchProjectColor(_projectId);
        OpenPopup(_mode);
        // OpenPopupより後じゃないとカラーピッカーの適切に初期色が設定できない
        colorPicker.CurrentColor = color;
    }

    /// <summary>
    /// ProjectSettingsのプロジェクト削除用のポップアップを表示する
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
