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

    //private AppDirector.SettingsMode currentSettingsMode;
    //public AppDirector.SettingsMode CurrentSettingsMode => currentSettingsMode;
    private AppDirector.GameMode destGameMode;
    public AppDirector.GameMode DestGameMode => destGameMode;

    /// <summary>
    /// ポップアップの種類を定義
    /// 指定された種類によって表示を変更するため
    /// </summary>
    public enum PopupMode
    {
        MainMenu,
        ProjectColorPicker,
        ProjectDelete,
        AlertBeforeDelete,
        AlertNameDuplication,
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
    /// ポップアップの表示処理
    /// PopupModeにより表示を変更
    /// </summary>
    /// <param name="_mode"></param>
    public void OpenPopup(PopupMode _mode)
    {
        popupUIDirector.InnerBackground
            .SetActive(
                _mode == PopupMode.MainMenu 
                || _mode == PopupMode.ProjectDelete 
                || _mode == PopupMode.AlertBeforeDelete
                || _mode == PopupMode.AlertNameDuplication
                || _mode == PopupMode.ApplySettings);

        popupUIDirector.MainMenuContainer
            .SetActive(_mode == PopupMode.MainMenu);

        popupUIDirector.ProjectColorPickerContainer
            .SetActive(_mode == PopupMode.ProjectColorPicker);

        popupUIDirector.ProjectDeleteContainer
            .SetActive(_mode == PopupMode.ProjectDelete);

        popupUIDirector.AlertBeforeDeleteContainer
            .SetActive(_mode == PopupMode.AlertBeforeDelete);

        popupUIDirector.AlertNameDuplicationContainer
            .SetActive(_mode == PopupMode.AlertNameDuplication);

        popupUIDirector.ApplySettingsContainer
            .SetActive(_mode == PopupMode.ApplySettings);

        popupUIDirector.PopupContainer.SetActive(true);
    }

    /// <summary>
    /// ProjectSettingsのカラーピッカー用のポップアップを表示する
    /// </summary>
    /// <param name="_mode"></param>
    /// <param name="_projectId"></param>
    public void OpenProjectColorPickerPopup(int _projectId)
    {
        selectedProjectId = _projectId;
        Color color = projectSettingsDirector.FetchProjectColor(_projectId);
        OpenPopup(PopupMode.ProjectColorPicker);
        // OpenPopupより後じゃないとカラーピッカーの適切に初期色が設定できない
        popupUIDirector.ProjectColorPicker.CurrentColor = color;
    }

    /// <summary>
    /// ProjectSettingsのプロジェクト削除用のポップアップを表示する
    /// </summary>
    /// <param name="_mode"></param>
    /// <param name="_projectId"></param>
    public void OpenProjectDeletePopup(int _projectId)
    {
        selectedProjectId = _projectId;

        if (projectSettingsDirector.IsAnySettingsChanged)
        {
            OpenPopup(PopupMode.AlertBeforeDelete);
        }
        else
        {
            OpenPopup(PopupMode.ProjectDelete);
        }
    }

    public void OpenApplySettingsPopup(AppDirector.GameMode _dest)
    {
        destGameMode = _dest;
        OpenPopup(PopupMode.ApplySettings);
    }

    public void ClosePopup()
    {
        selectedProjectId = -1;
        popupUIDirector.PopupContainer.SetActive(false);
    }



}
