using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class WindowDirector : MonoBehaviour
{
    [DllImport("user32.dll", EntryPoint = "FindWindow")]
    private static extern IntPtr FindWindow(System.String className, System.String windowName);
    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    private static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);
    [DllImport("user32.dll", EntryPoint = "GetCursorPos")]
    private static extern bool GetCursorPos(out POINT lpPoint);
    [DllImport("user32.dll", EntryPoint = "GetWindowRect")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
    [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
    private static extern int GetSystemMetrics(int nIndex);

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    IntPtr window;
    private string windowName = "LifeLogForMyself";
    private const int WS_DLGFRAME = 0x00400000;
    private const int SM_CXSCREEN = 0;
    private const int SM_CYSCREEN = 1;
    private const int HWND_TOP = 0;
    private const int HWND_TOPMOST = -1;

    private AppDirector appDirector;
    private InputEventDirector inputEventDirector;
    private ResizingDirector resizingDirector;
    //private MainUIDirector mainUIDirector;
    private DatabaseDirector databaseDirector;
    
    [SerializeField]
    private CanvasResizingMonitor canvasResizingMonitor;
    
    private ResizingMode resizingMode;

    private int touchCount = 0;
    private bool isDragged = false;

    private int dragStartX = 0;
    private int dragStartY = 0;

    private int smallScreen = 150;
    private int mediumScreen = 450;
    private int largeScreen = 800;

    /// <summary>
    /// �V���O���g��
    /// </summary>
    private static WindowDirector instance;
    public static WindowDirector Instance => instance;

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
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        appDirector = AppDirector.Instance;
        inputEventDirector = InputEventDirector.Instance;
        databaseDirector = DatabaseDirector.Instance;
        resizingDirector = ResizingDirector.Instance;
        //mainUIDirector = MainUIDirector.Instance;

        window = FindWindow(null, windowName);
        DeleteWindowTitleBar();

        UpdateScreenSize();
        InitializeWindowRect();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// �^�C�g���o�[����������
    /// </summary>
    private void DeleteWindowTitleBar()
    {
        if (appDirector.DebugMode) return;

        const int GWL_STYLE = -16;
        int style = GetWindowLong(window, GWL_STYLE);
        style &= ~WS_DLGFRAME;
        SetWindowLong(window, GWL_STYLE, style);
    }

    /// <summary>
    /// �N�����̃E�B���h�E�̐ݒ�
    /// ����(20220425)�A��ʒ����ɏ������\�����Ă���
    /// </summary>
    private void InitializeWindowRect()
    {
        if (appDirector.DebugMode) return;

        RECT windowRect;
        GetWindowRect(window, out windowRect);
        Debug.Log("initialize windowRect: (" + windowRect.left + ", " + windowRect.right + ", " + windowRect.top + ", " + windowRect.bottom + ") ");


        int screenW = GetSystemMetrics(SM_CXSCREEN);
        int screenH = GetSystemMetrics(SM_CYSCREEN);
        int windowX = (screenW - smallScreen) / 2;
        int windowY = (screenH - smallScreen) / 2;

        SetWindowPos(window, HWND_TOPMOST, windowX, windowY, smallScreen, smallScreen, 0);
    }


    /// <summary>
    /// �E�B���h�E�̃��T�C�Y�p
    /// �_�u���N���b�N�����m
    /// Invoke�̑������Ɏw�肵���b�Ԃł̃N���b�N�񐔂��Ď�����
    /// </summary>
    public void OnResizingButtonClick()
    {
        touchCount++;
        Invoke("HandleRepeatedClick", 0.3f);
    }

    /// <summary>
    /// �E�B���h�E�̃��T�C�Y�p
    /// �_�u���N���b�N�ƃV���O���N���b�N�̔���
    /// </summary>
    private void HandleRepeatedClick()
    {
        if (!isDragged && touchCount == 2)
        {
            ResizeScreen();
        }
        else if (!isDragged && touchCount == 1)
        {
            appDirector.ChangeClockMode();
        }
        touchCount = 0;
        isDragged = false;
    }

    /// <summary>
    /// �E�B���h�E�̃��T�C�Y�p
    /// ���ۂ̃��T�C�Y����
    /// </summary>
    private void ResizeScreen()
    {
        if (appDirector.DebugMode) return;

        int resizeSize = smallScreen;

        if (resizingMode == ResizingMode.TwoStages)
        {
            if (Screen.width > smallScreen && Screen.height > smallScreen)
                resizeSize = smallScreen;
            else 
                resizeSize = mediumScreen;
        }
        else if (resizingMode == ResizingMode.ThreeStages)
        {
            if (Screen.width > mediumScreen && Screen.height > mediumScreen)
                resizeSize = smallScreen;
            else if (Screen.width > smallScreen && Screen.height > smallScreen)
                resizeSize = largeScreen;
            else
                resizeSize = mediumScreen;
        }

        //int screenW = GetSystemMetrics(SM_CXSCREEN);
        //int screenH = GetSystemMetrics(SM_CYSCREEN);

        RECT windowRect;
        GetWindowRect(window, out windowRect);

        int newPosX = windowRect.right - resizeSize;
        int newPosY = windowRect.bottom - resizeSize;

        SetWindowPos(window, HWND_TOPMOST, newPosX, newPosY, resizeSize, resizeSize, 0);

        resizingDirector.UpdatePValues(resizeSize, resizeSize);
        resizingDirector.SwitchClockImage();
    }

    /// <summary>
    /// �E�B���h�E�̃h���b�O�ړ��p
    /// �h���b�O�J�n���̍��W��ۑ�
    /// </summary>
    /// <param name="_eventData"></param>
    public void OnTargetBeginDrag(BaseEventData _eventData)
    {
        if (appDirector.DebugMode) return;

        Vector2 mousePos = inputEventDirector.GetMousePosition();
        dragStartX = (int)mousePos.x;
        dragStartY = (int)(Screen.height - mousePos.y);

        // �h���b�O���click�C�x���g���
        isDragged = true;
    }

    /// <summary>
    /// �E�B���h�E�̃h���b�O�ړ��p
    /// �E�B���h�E�����ۂɈړ�������
    /// </summary>
    /// <param name="_eventData"></param>
    public void OnTargetDrag(BaseEventData _eventData)
    {
        if (appDirector.DebugMode) return;

        MoveWindow();
    }

    /// <summary>
    /// �E�B���h�E�̃h���b�O�ړ��p
    /// ���g�p
    /// </summary>
    /// <param name="_eventData"></param>
    public void OnTargetEndDrag(BaseEventData _eventData)
    {
        if (appDirector.DebugMode) return;
    }

    /// <summary>
    /// OnTargetDrag�̏������e
    /// </summary>
    private void MoveWindow()
    {
        if (appDirector.DebugMode) return;

        var window = FindWindow(null, windowName);

        POINT mousePoint;
        GetCursorPos(out mousePoint);

        RECT windowRect;
        GetWindowRect(window, out windowRect);

        int width = resizingDirector.PScreenWidth;
        int height = resizingDirector.PScreenHeight;
        SetWindowPos(window, HWND_TOPMOST, mousePoint.x - dragStartX, mousePoint.y - dragStartY, width, height, 0);

    }


    /// <summary>
    /// ���T�C�Y�֘A�̐ݒ�ύX���ɗ��p
    /// </summary>
    public void UpdateScreenSize()
    {
        resizingMode = databaseDirector.FetchResizingMode();

        if (resizingMode == ResizingMode.TwoStages)
        {
            TwoResizingData twoResizingStages = databaseDirector.FetchTwoResizingStages();
            smallScreen = twoResizingStages.small;
            mediumScreen = twoResizingStages.medium;
        }
        else if (resizingMode == ResizingMode.ThreeStages)
        {
            ThreeResizingData threeResizingStages = databaseDirector.FetchThreeResizingStages();
            smallScreen = threeResizingStages.small;
            mediumScreen = threeResizingStages.medium;
            largeScreen = threeResizingStages.large;
        }
    }
}
