using HSVPicker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupUIDirector : MonoBehaviour
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

    //ƒVƒ“ƒOƒ‹ƒgƒ“
    private static PopupUIDirector instance;
    public static PopupUIDirector Instance => instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        mainButton = mainMenuContainer.transform.Find("MainButton").GetComponent<Button>();
        settingButton = mainMenuContainer.transform.Find("SettingsButton").GetComponent<Button>();
        quitButton = mainMenuContainer.transform.Find("QuitButton").GetComponent<Button>();

        projectColorPicker = projectColorPickerContainer.transform
            .Find("Picker 2.0").GetComponent<ColorPicker>();

        projectDeleteCancelButton = projectDeleteContainer.transform
            .Find("Buttons/CancelButton").GetComponent<Button>();
        projectDeleteButton = projectDeleteContainer.transform
            .Find("Buttons/DeleteButton").GetComponent<Button>();
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
