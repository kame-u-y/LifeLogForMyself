using HSVPicker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupUIDirector : SingletonMonoBehaviourFast<PopupUIDirector>
{
    [SerializeField]
    private GameObject popupContainer;
    public GameObject PopupContainer => popupContainer;

    [SerializeField]
    private Button outerBackgroundButton;
    public Button OuterBackgroundButton => outerBackgroundButton;

    [SerializeField]
    private GameObject innerBackground;
    public GameObject InnerBackground => innerBackground;

    [SerializeField]
    private GameObject mainMenuContainer;
    public GameObject MainMenuContainer => mainMenuContainer;

    private Button mainButton;
    public Button MainButton => mainButton;

    private Button watchLogButton;
    public Button WatchLogButton => watchLogButton;

    private Button settingButton;
    public Button SettingButton => settingButton;

    private Button quitButton;
    public Button QuitButton => quitButton;

    [SerializeField]
    private GameObject projectColorPickerContainer;
    public GameObject ProjectColorPickerContainer => projectColorPickerContainer;

    private ColorPicker projectColorPicker;
    public ColorPicker ProjectColorPicker => projectColorPicker;

    [SerializeField]
    private GameObject projectDeleteContainer;
    public GameObject ProjectDeleteContainer => projectDeleteContainer;

    private Button projectDeleteCancelButton;
    public Button ProjectDeleteCancelButton => projectDeleteCancelButton;

    private Button projectDeleteButton;
    public Button ProjectDeleteButton => projectDeleteButton;

    [SerializeField]
    private GameObject alertBeforeDeleteContainer;
    public GameObject AlertBeforeDeleteContainer => alertBeforeDeleteContainer;

    private Button alertBeforeDeleteCancelButton;
    public Button AlertBeforeDeleteCancelButton => alertBeforeDeleteCancelButton;

    private Button alertBeforeDeleteProceedButton;
    public Button AlertBeforeDeleteProceedButton => alertBeforeDeleteProceedButton;

    [SerializeField]
    private GameObject alertNameDuplicationContainer;
    public GameObject AlertNameDuplicationContainer => alertNameDuplicationContainer;

    private Button alertNameDuplicationOKButton;
    public Button AlertNameDuplicationOKButton => alertNameDuplicationOKButton;

    [SerializeField]
    private GameObject applySettingsContainer;
    public GameObject ApplySettingsContainer => applySettingsContainer;

    private Button applySettingsCancelButton;
    public Button ApplySettingsCancelButton => applySettingsCancelButton;

    private Button applySettingsButton;
    public Button ApplySettingsButton => applySettingsButton;
    
    new void Awake()
    {
        base.Awake();

        mainButton = mainMenuContainer.transform.Find("MainButton").GetComponent<Button>();
        watchLogButton = mainMenuContainer.transform.Find("WatchLogButton").GetComponent<Button>();
        settingButton = mainMenuContainer.transform.Find("SettingsButton").GetComponent<Button>();
        quitButton = mainMenuContainer.transform.Find("QuitButton").GetComponent<Button>();

        projectColorPicker = projectColorPickerContainer.transform
            .Find("Picker 2.0").GetComponent<ColorPicker>();

        projectDeleteCancelButton = projectDeleteContainer.transform
            .Find("Buttons/CancelButton").GetComponent<Button>();
        projectDeleteButton = projectDeleteContainer.transform
            .Find("Buttons/DeleteButton").GetComponent<Button>();

        alertBeforeDeleteCancelButton = alertBeforeDeleteContainer.transform
            .Find("Buttons/CancelButton").GetComponent<Button>();
        alertBeforeDeleteProceedButton = alertBeforeDeleteContainer.transform
            .Find("Buttons/ApplyAndProceedButton").GetComponent<Button>();

        alertNameDuplicationOKButton = alertNameDuplicationContainer.transform
            .Find("Buttons/OKButton").GetComponent<Button>();

        applySettingsCancelButton = applySettingsContainer.transform
            .Find("Buttons/CancelButton").GetComponent<Button>();
        applySettingsButton = applySettingsContainer.transform
            .Find("Buttons/ApplyButton").GetComponent<Button>();

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
