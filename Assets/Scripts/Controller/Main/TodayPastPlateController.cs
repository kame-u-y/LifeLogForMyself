using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TodayPastPlateController : MonoBehaviour
{
    AppDirector appDirector;
    private float time;

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
        // 1•b‚²‚Æ‚ÉXV
        if (time >= 1.0f)
        {
            UpdatePlate();
            time = 0.0f;
        }
        time += Time.deltaTime;

    }

    public void UpdatePlate()
    {
        int startSec = appDirector.GetSecondOfClockStart();
        //int startSec = appDirector.GetSecondOfNow();
        int endSec = appDirector.GetSecondOfClockEnd();
        int nowSec = GetNowSecond();
        //int clockStartSec = appDirector.GetSecondOfClockStart();

        var hoge = (float)(1.0f * (nowSec - startSec) / (endSec - startSec));
        //float angle = CalculatePieRotationValue(360.0f, clockStartSec, startSec, clockStartSec, endSec);
        //this.transform.rotation = Quaternion.Euler(0, 0, -angle);
        //this.GetComponent<Image>().fillAmount = hoge;
        this.GetComponent<Image>().fillAmount = 1;
            //CalculatePieRotationValue(1.0f, startSec, endSec, clockStartSec, endSec);
        //Debug.Log($"past:({hoge})");

    }

    private bool IsAm()
        => DateTime.Now.Hour < 12;


    private float CalculatePieRotationValue(float _rotationMax, int _startOfValue, int _endOfValue, int _startOfAll, int _endOfAll)
        => (float)_rotationMax * (_endOfValue - _startOfValue) / (_endOfAll - _startOfAll);

    private int GetSecondOfToday(int _h, int _m, int _s)
        => (int)new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, _h, _m, _s)
            .Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

    private int GetNowSecond()
        => (int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
}
