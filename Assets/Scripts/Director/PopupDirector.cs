using HSVPicker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PopupDirector : SingletonMonoBehaviourFast<PopupDirector>
{
    /// <summary>
    /// Project Settingsでポップアップを利用した設定変更に使用
    /// ColorPicker, Deleteに利用
    /// </summary>
    private int selectedProjectId = -1;
    public int SelectedProjectId { get => selectedProjectId; }

    private ProjectSettingsDirector projectSettingsDirector;
    private PopupUIDirector popupUIDirector;

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

    // シングルトン
    //private static PopupDirector instance;
    //public static PopupDirector Instance => instance;

    //private void Awake()
    //{

    //    //if (instance == null)
    //    //{
    //    //    instance = this;
    //    //    DontDestroyOnLoad(gameObject);
    //    //}
    //    //else
    //    //{
    //    //    Destroy(gameObject);
    //    //    return;
    //    //}

    //}

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
    /// ポップアップの表示処理
    /// PopupModeにより表示を変更
    /// </summary>
    /// <param name="_mode"></param>
    public void OpenPopup(PopupMode _mode)
    {
        popupUIDirector.InnerBackground
            .SetActive(_mode == PopupMode.MainMenu || _mode == PopupMode.ProjectDelete);

        popupUIDirector.MainMenuContainer
            .SetActive(_mode == PopupMode.MainMenu);

        popupUIDirector.ProjectColorPickerContainer
            .SetActive(_mode == PopupMode.ProjectColorPicker);

        popupUIDirector.ProjectDeleteContainer
            .SetActive(_mode == PopupMode.ProjectDelete);

        popupUIDirector.PopupContainer.SetActive(true);
    }

    /// <summary>
    /// ProjectSettingsのカラーピッカー用のポップアップを表示する
    /// </summary>
    /// <param name="_mode"></param>
    /// <param name="_projectId"></param>
    public void OpenProjectColorPickerPopup(PopupMode _mode, int _projectId)
    {
        selectedProjectId = _projectId;
        Color color = projectSettingsDirector.FetchProjectColor(_projectId);
        OpenPopup(_mode);
        // OpenPopupより後じゃないとカラーピッカーの適切に初期色が設定できない
        popupUIDirector.ProjectColorPicker.CurrentColor = color;
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
        popupUIDirector.PopupContainer.SetActive(false);
    }



}
