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
    public static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);

    IntPtr window;
    string windowName = "LifeLogForMyself";
    const int WS_DLGFRAME = 0x00400000;

    [SerializeField]
    InputEventDirector inputEventDirector;
    [SerializeField]
    CanvasResizingMonitor canvasResizingMonitor;

    private int touchCount = 0;

    private int dragStartX = 0;
    private int dragStartY = 0;

    private void Awake()
    {
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

        const int HWND_TOP = 0;
        Rect windowRect;
        GetWindowRect(window, out windowRect);
        Debug.Log("initialize windowRect: (" + windowRect.width + ", " + windowRect.height + ")");
        SetWindowPos(window, HWND_TOP, 800, 800, 100, 100, 0);
    }

    /// <summary>
    /// ウィンドウのリサイズ用
    /// ダブルクリックを検知
    /// </summary>
    public void OnResizingButtonClick()
    {
        touchCount++;
        Invoke("ResizeScreen", 0.3f);
    }

    private void ResizeScreen()
    {
        if (touchCount != 2)
        {
            touchCount = 0;
            return;
        }
        else
        {
            touchCount = 0;
        }
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
        Screen.SetResolution(resizeWidth, resizeHeight, false);

        canvasResizingMonitor.UpdatePSize(resizeWidth, resizeHeight);
        Debug.Log(canvasResizingMonitor.GetPScreenWidth());
        canvasResizingMonitor.SwitchClockImage();
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
        const int HWND_TOP = 0;
        Point mousePoint;
        Rect windowRect;

        GetCursorPos(out mousePoint);
        GetWindowRect(window, out windowRect);
        Debug.Log("windowRect: (" + windowRect.width + ", " + windowRect.height + ")");

        int width = canvasResizingMonitor.GetPScreenWidth();
        int height = canvasResizingMonitor.GetPScreenHeight();
        SetWindowPos(window, HWND_TOP, mousePoint.X - dragStartX, mousePoint.Y - dragStartY, width, height, 0);

        // ウィンドウサイズが消滅する
        //SetWindowPos(window, HWND_TOP, mousePoint.X - dragStartX, mousePoint.Y - dragStartY, (int)windowRect.width, (int)windowRect.height, 0);
    }
}
