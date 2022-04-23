using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CanvasResizingMonitor : UIBehaviour
{
    public delegate void OnWindowResize();
    public OnWindowResize windowResizeEvent;

    ResizingDirector resizingDirector;

    // Start is called before the first frame update
    protected override void Awake()
    {
        resizingDirector = GameObject.Find("ResizingDirector").GetComponent<ResizingDirector>();
    }

    protected override void Start()
    {
        windowResizeEvent = resizingDirector.SwitchClockImage;
    }

    /// <summary>
    /// �ő剻���ꂽ�L�����o�X�����ƂɁA�E�B���h�E�̃��T�C�Y�����m����
    /// </summary>
    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        if (windowResizeEvent != null)
        {
            windowResizeEvent();
        }
    }
}
