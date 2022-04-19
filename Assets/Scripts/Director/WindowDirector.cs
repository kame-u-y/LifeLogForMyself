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

    int smallScreen = 150;
    int mediumScreen = 450;
    int largeScreen = 800;

    int smallThreshold = 200;
    int mediumThreshold = 500;

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

        int screenW = GetSystemMetrics(SM_CXSCREEN);
        int screenH = GetSystemMetrics(SM_CYSCREEN);
        SetWindowPos(window, HWND_TOPMOST, (screenW - smallScreen) / 2, (screenH - smallScreen) / 2 , smallScreen, smallScreen, 0);
    }

    /// <summary>
    /// �E�B���h�E�̃��T�C�Y�p
    /// �_�u���N���b�N�����m
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

        int resizeWidth = smallScreen;
        int resizeHeight = smallScreen;
        if (Screen.width > mediumThreshold && Screen.height > mediumThreshold)
        {
            //Screen.SetResolution(150, 150, false);
            resizeWidth = smallScreen;
            resizeHeight = smallScreen;
        }
        else if (Screen.width > smallThreshold && Screen.height > smallThreshold)
        {
            resizeWidth = largeScreen;
            resizeHeight = largeScreen;
        }
        else
        {
            resizeWidth = mediumScreen;
            resizeHeight = mediumScreen;
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
    /// �E�B���h�E�̃h���b�O�ړ��p
    /// </summary>
    /// <param name="_eventData"></param>
    public void OnTargetBeginDrag(BaseEventData _eventData)
    {
        Debug.Log("begin drag");
        Vector2 mousePos = inputEventDirector.GetMousePosition();
        dragStartX = (int)mousePos.x;
        dragStartY = (int) (Screen.height - mousePos.y);
        Debug.Log($"{dragStartX}, {dragStartY}");

        // �h���b�O���click�C�x���g���
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
        POINT mousePoint;
        RECT windowRect;

        GetCursorPos(out mousePoint);
        GetWindowRect(window, out windowRect);
        Debug.Log("windowRect: (" + windowRect.left + ", " + windowRect.top + ")");

        int width = canvasResizingMonitor.GetPScreenWidth();
        int height = canvasResizingMonitor.GetPScreenHeight();
        SetWindowPos(window, HWND_TOPMOST, mousePoint.x - dragStartX, mousePoint.y - dragStartY, width, height, 0);

        // �E�B���h�E�T�C�Y�����ł���
        //SetWindowPos(window, HWND_TOP, mousePoint.X - dragStartX, mousePoint.Y - dragStartY, (int)windowRect.width, (int)windowRect.height, 0);
    }
}
