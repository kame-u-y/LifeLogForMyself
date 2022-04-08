using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TodayPastPlateController : MonoBehaviour
{
    GameDirector gameDirector;
    private float time;

    // Start is called before the first frame update
    void Start()
    {
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
    }

    // Update is called once per frame
    void Update()
    {
        // 1•b‚²‚Æ‚ÉXV
        if (time >= 1.0f)
        {
            UpdatePastPlate();
            time = 0.0f;
        }
        time += Time.deltaTime;

    }

    public void UpdatePastPlate()
    {
        int startSec = gameDirector.GetSecondOfClockStart();
        int endSec = gameDirector.GetSecondOfClockEnd();
        int nowSec = GetNowSecond();
        var hoge = (float)(1.0f * (nowSec - startSec) / (endSec - startSec));
        this.GetComponent<Image>().fillAmount = hoge;
        Debug.Log($"past:({hoge})");
    }

    private bool IsAm()
        => DateTime.Now.Hour < 12;

    private int GetSecondOfToday(int _h, int _m, int _s)
        => (int)new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, _h, _m, _s)
            .Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

    private int GetNowSecond()
        => (int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
}
