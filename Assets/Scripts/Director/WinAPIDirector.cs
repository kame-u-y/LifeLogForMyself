using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class WinAPIDirector : MonoBehaviour
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

    string windowName = "LifeLogForMyself";
    //const int WS_CAPTION = 0x00c00000;
    const int WS_DLGFRAME = 0x00400000;

    PracticeInputEventDirector inputEventDirector;
    private int dragStartX = 0;
    private int dragStartY = 0;

    private void Awake()
    {
        inputEventDirector = GameObject.Find("InputEventDirector").GetComponent<PracticeInputEventDirector>();
    }

    // Start is called before the first frame update
    void Start()
    {
        var window = FindWindow(null, windowName);
        const int GWL_STYLE = -16;
        int style = GetWindowLong(window, GWL_STYLE);
        style &= ~WS_DLGFRAME;
        SetWindowLong(window, GWL_STYLE, style);
        const int HWND_TOP = 0;
        SetWindowPos(window, HWND_TOP, 100, 200, 200, 400, 0);

        Debug.Log("api director");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBeginDrag(BaseEventData _eventData)
    {
        Debug.Log("begin drag");
        Vector2 mousePos = inputEventDirector.GetMousePosition();
        dragStartX = (int) mousePos.x;
        dragStartY = (int) mousePos.y;
        Debug.Log($"{dragStartX}, {dragStartY}");
    }

    public void OnDrag(BaseEventData _eventData)
    {
        //Debug.Log("drag");
        MoveWindow();
    }

    public void OnEndDrag(BaseEventData _eventData)
    {
        Debug.Log("end drag");
    }

    public void MoveWindow()
    {
        var window = FindWindow(null, windowName);
        const int HWND_TOP = 0;
        Point point;
        
        GetCursorPos(out point);
        SetWindowPos(window, HWND_TOP, point.X - dragStartX, point.Y - dragStartY, 100, 100, 0);
    }
}
