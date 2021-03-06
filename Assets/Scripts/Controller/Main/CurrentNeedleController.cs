using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentNeedleController : MonoBehaviour
{
    //[SerializeField]
    //private bool isClock12h = false;
    AppDirector appDirector;

    private void Awake()
    {
        appDirector = AppDirector.Instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DrawCurrentNeedle();
    }

    /// <summary>
    /// 現在時刻を時・分をもとに表示
    /// 12hと24hをisClock12hで切り替え
    /// </summary>
    void DrawCurrentNeedle()
    {
        DateTime dt = DateTime.Now;
        float angle = (appDirector.isClock12h ? 2 : 1) * 360.0f * (dt.Hour * 60.0f + dt.Minute) / (24.0f * 60.0f);
        this.transform.rotation = Quaternion.Euler(0, 0, -angle);
    }
}
