using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkingDirector : MonoBehaviour
{
    private bool isWorking = false;
    private int startUnixSec;
    private int nowUnixSec;
    private float time;

    [SerializeField]
    PlayEndImageController playEndImage;

    [SerializeField]
    PieChartController pieChartController;

    DatabaseDirector databaseDirector;

    void Awake()
    {
        databaseDirector = GameObject.Find("DatabaseDirector").GetComponent<DatabaseDirector>();
    }

    // Start is called before the first frame update
    void Start()
    {
        isWorking = false;
        playEndImage.ChangeButtonImage(isWorking);

        //databaseDirector.


        //pieChartController.DisplayTodayPieChart();
    }

    // Update is called once per frame
    void Update()
    {
        if (isWorking)
        {
            // 1秒ごとに更新
            if (time >= 1.0f)
            {
                UpdateWorkTime();
                time = 0.0f;
            }
            time += Time.deltaTime;
        }
    }

    private void UpdateWorkTime()
    {
        nowUnixSec = GetNowTotalSeconds();
        // todo: UpdateDisplay
        print("now:" + (nowUnixSec - startUnixSec));
    }

    public void ToggleWork()
    {
        if (isWorking)
            EndWork();
        else
            StartWork();
    }

    private void StartWork()
    {
        isWorking = true;
        startUnixSec = GetNowTotalSeconds();
        nowUnixSec = startUnixSec;
        playEndImage.ChangeButtonImage(isWorking);
        time = 0;
    }

    private void EndWork()
    {
        isWorking = false;
        // todo: データを記録
        databaseDirector.AddEndedWork(
            new WorkData()
            {
                id = 1,
                startUnixSec = this.startUnixSec,
                endUnixSec = this.nowUnixSec,
                projectName = "作業"
            });

        playEndImage.ChangeButtonImage(isWorking);
        time = 0;
    }

    private int GetNowTotalSeconds()
        => (int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
}
