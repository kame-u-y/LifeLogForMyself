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
    public static extern bool GetCursorPos(out Point lpPoint);
    [DllImport("user32.dll", EntryPoint = "GetWindowRect")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
    [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
    private static extern int GetSystemMetrics(int nIndex);
    
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

    private void Awake()
    {
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
        inputEventDirector = GameObject.Find("InputEventDirector").GetComponent<InputEventDirector>();
    }

    // Start is called before the first frame update
    void Start()
    {
        window = FindWindow(null, windowName);
        DeleteWindowTitleBar();
        InitializeWindowRect();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void DeleteWindowTitleBar()
    {
        const int GWL_STYLE = -16;
        int style = GetWindowLong(window, GWL_STYLE);
        style &= ~WS_DLGFRAME;
        SetWindowLong(window, GWL_STYLE, style);
    }

    private void InitializeWindowRect()
    {

        RECT windowRect;
        GetWindowRect(window, out windowRect);
        Debug.Log("initialize windowRect: (" + windowRect.left + ", " + windowRect.right + ", " + windowRect.top + ", " + windowRect.bottom +") ");
        SetWindowPos(window, HWND_TOPMOST, 800, 800, 100, 100, 0);
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
        
        Debug.Log("Double Clicked!!");

        int resizeWidth = 150;
        int resizeHeight = 150;
        if (Screen.width > 500 && Screen.height > 500)
        {
            //Screen.SetResolution(150, 150, false);
            resizeWidth = 150;
            resizeHeight = 150;
        }
        else if (Screen.width > 200 && Screen.height > 200)
        {
            resizeWidth = 800;
            resizeHeight = 800;
        }
        else
        {
            resizeWidth = 450;
            resizeHeight = 450;
        }
        //Screen.SetResolution(resizeWidth, resizeHeight, false);

        //int width = canvasResizingMonitor.GetPScreenWidth();
        //int height = canvasResizingMonitor.GetPScreenHeight();
        int screenW = GetSystemMetrics(SM_CXSCREEN);
        int screenH = GetSystemMetrics(SM_CYSCREEN);
        Debug.Log("Screen: " + screenW + ", "+ screenH);

        RECT windowRect;
        GetWindowRect(window, out windowRect);

        //int windowCenterX = (windowRect.left + windowRect.right) / 2;
        //int windowCenterY = (windowRect.top + windowRect.bottom) / 2;
        //int windowW = windowRect.right - windowRect.left;
        //int windowH = windowRect.bottom - windowRect.top;
        //int newPosX = windowCenterX < screenW / 2
        //    ? 0 : windowRect.right - resizeWidth;
        //int newPosY = windowCenterY < screenH / 2
        //    ? 0 : windowRect.bottom - resizeHeight;

        //int newPosX = (windowRect.left + windowRect.right) / 2 - resizeWidth / 2;
        //int newPosY = (windowRect.top + windowRect.bottom) / 2 - resizeHeight / 2;

        int newPosX = windowRect.right - resizeWidth;
        int newPosY = windowRect.bottom - resizeHeight;

        SetWindowPos(window, HWND_TOPMOST, newPosX, newPosY, resizeWidth, resizeHeight, 0);
        
        canvasResizingMonitor.UpdatePSize(resizeWidth, resizeHeight);
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
        Debug.Log("begin drag");
        Vector2 mousePos = inputEventDirector.GetMousePosition();
        dragStartX = (int)mousePos.x;
        dragStartY = (int) (Screen.height - mousePos.y);
        Debug.Log($"{dragStartX}, {dragStartY}");

        // ドラッグ後のclickイベント回避
        isDragged = true;
    }

    public void OnTargetDrag(BaseEventData _eventData)
    {
        //Debug.Log("drag");
        MoveWindow();
    }

    public void OnTargetEndDrag(BaseEventData _eventData)
    {
        Debug.Log("end drag");
    }

    private void MoveWindow()
    {
        var window = FindWindow(null, windowName);
        Point mousePoint;
        RECT windowRect;

        GetCursorPos(out mousePoint);
        GetWindowRect(window, out windowRect);
        Debug.Log("windowRect: (" + windowRect.left + ", " + windowRect.top + ")");

        int width = canvasResizingMonitor.GetPScreenWidth();
        int height = canvasResizingMonitor.GetPScreenHeight();
        SetWindowPos(window, HWND_TOPMOST, mousePoint.X - dragStartX, mousePoint.Y - dragStartY, width, height, 0);

        // ウィンドウサイズが消滅する
        //SetWindowPos(window, HWND_TOP, mousePoint.X - dragStartX, mousePoint.Y - dragStartY, (int)windowRect.width, (int)windowRect.height, 0);
    }
}
