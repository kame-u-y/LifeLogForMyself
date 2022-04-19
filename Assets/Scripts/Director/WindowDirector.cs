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
    public static extern IntPtr FindWindow(System.String className, System.String windowName);
    [DllImport("user32.dll")]
    public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
    [DllImport("user32.dll")]
    public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    public static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);
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
    string windowName = "LifeLogForMyself";
    const int WS_DLGFRAME = 0x00400000;
    const int SM_CXSCREEN = 0;
    const int SM_CYSCREEN = 1;
    const int HWND_TOP = 0;
    const int HWND_TOPMOST = -1;

    GameDirector gameDirector;
    InputEventDirector inputEventDirector;
    [SerializeField]
    CanvasResizingMonitor canvasResizingMonitor;


    private int touchCount = 0;
    private bool isDragged = false;

    private int dragStartX = 0;
    private int dragStartY = 0;

    [SerializeField]
    GameObject playEndButton;

    DatabaseDirector databaseDirector;
    ResizingMode resizingMode;

    private int smallScreen = 150;
    public int SmallScreen
    {
        get => smallScreen;
        set => smallScreen = value;
    }

    private int mediumScreen = 450;
    public int MediumScreen
    {
        get => mediumScreen;
        set => mediumScreen = value;
    }

    private int largeScreen = 800;
    public int LargeScreen
    {
        get => largeScreen;
        set => largeScreen = value;
    }

    private void Awake()
    {
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
        inputEventDirector = GameObject.Find("InputEventDirector").GetComponent<InputEventDirector>();
        databaseDirector = GameObject.Find("DatabaseDirector").GetComponent<DatabaseDirector>();
    }

    // Start is called before the first frame update
    void Start()
    {
        window = FindWindow(null, windowName);
        DeleteWindowTitleBar();

        UpdateScreenSize();
        InitializeWindowRect();
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void DeleteWindowTitleBar()
    {
        if (gameDirector.DebugMode) return;

        const int GWL_STYLE = -16;
        int style = GetWindowLong(window, GWL_STYLE);
        style &= ~WS_DLGFRAME;
        SetWindowLong(window, GWL_STYLE, style);
    }

    private void InitializeWindowRect()
    {
        if (gameDirector.DebugMode) return;

        RECT windowRect;
        GetWindowRect(window, out windowRect);
        Debug.Log("initialize windowRect: (" + windowRect.left + ", " + windowRect.right + ", " + windowRect.top + ", " + windowRect.bottom + ") ");


        int screenW = GetSystemMetrics(SM_CXSCREEN);
        int screenH = GetSystemMetrics(SM_CYSCREEN);
        int windowX = (screenW - smallScreen) / 2;
        int windowY = (screenH - smallScreen) / 2;

        SetWindowPos(window, HWND_TOPMOST, windowX, windowY, smallScreen, smallScreen, 0);
    }

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

    /// <summary>
    /// ウィンドウのリサイズ用
    /// ダブルクリックを検知
    /// </summary>
    public void OnResizingButtonClick()
    {
        touchCount++;
        Invoke("HandleRepeatedClick", 0.3f);
    }

    private void HandleRepeatedClick()
    {
        if (!isDragged && touchCount == 2)
        {
            ResizeScreen();
        }
        else if (!isDragged && touchCount == 1)
        {
            gameDirector.ChangeClockMode();
        }
        touchCount = 0;
        isDragged = false;
    }

    private void ResizeScreen()
    {
        if (gameDirector.DebugMode) return;

        Debug.Log("Double Clicked!!");

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

        int screenW = GetSystemMetrics(SM_CXSCREEN);
        int screenH = GetSystemMetrics(SM_CYSCREEN);
        Debug.Log("Screen: " + screenW + ", " + screenH);

        RECT windowRect;
        GetWindowRect(window, out windowRect);

        int newPosX = windowRect.right - resizeSize;
        int newPosY = windowRect.bottom - resizeSize;

        SetWindowPos(window, HWND_TOPMOST, newPosX, newPosY, resizeSize, resizeSize, 0);

        canvasResizingMonitor.UpdatePSize(resizeSize, resizeSize);
        Debug.Log(canvasResizingMonitor.GetPScreenWidth());
        canvasResizingMonitor.SwitchClockImage();

        Debug.Log("LocalPosition (ResizeScreen):" + playEndButton.transform.localPosition);
        Debug.Log("LocalPosition (ResizeScreen):" + playEndButton.transform.position);
    }

    /// <summary>
    /// ウィンドウのドラッグ移動用
    /// </summary>
    /// <param name="_eventData"></param>
    public void OnTargetBeginDrag(BaseEventData _eventData)
    {
        if (gameDirector.DebugMode) return;

        Debug.Log("begin drag");
        Vector2 mousePos = inputEventDirector.GetMousePosition();
        dragStartX = (int)mousePos.x;
        dragStartY = (int)(Screen.height - mousePos.y);
        Debug.Log($"{dragStartX}, {dragStartY}");

        // ドラッグ後のclickイベント回避
        isDragged = true;
    }

    public void OnTargetDrag(BaseEventData _eventData)
    {
        if (gameDirector.DebugMode) return;

        //Debug.Log("drag");
        MoveWindow();
    }

    public void OnTargetEndDrag(BaseEventData _eventData)
    {
        if (gameDirector.DebugMode) return;

        Debug.Log("end drag");
    }

    private void MoveWindow()
    {
        if (gameDirector.DebugMode) return;

        var window = FindWindow(null, windowName);
        POINT mousePoint;
        RECT windowRect;

        GetCursorPos(out mousePoint);
        GetWindowRect(window, out windowRect);
        Debug.Log("windowRect: (" + windowRect.left + ", " + windowRect.top + ")");

        int width = canvasResizingMonitor.GetPScreenWidth();
        int height = canvasResizingMonitor.GetPScreenHeight();
        SetWindowPos(window, HWND_TOPMOST, mousePoint.x - dragStartX, mousePoint.y - dragStartY, width, height, 0);

        // ウィンドウサイズが消滅する
        //SetWindowPos(window, HWND_TOP, mousePoint.X - dragStartX, mousePoint.Y - dragStartY, (int)windowRect.width, (int)windowRect.height, 0);
    }
}
