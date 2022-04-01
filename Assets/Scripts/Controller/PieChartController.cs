using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieChartController : MonoBehaviour
{
    [SerializeField]
    private GameObject CircleImage;

    //private DatabaseDirector databaseDirector;

    //private GameObject currentNeedle;

    private void Awake()
    {
        //databaseDirector = GameObject.Find("DatabaseDirector").GetComponent<DatabaseDirector>();
    }

    private void Start()
    {
        
    }

    private void Update()
    {

    }

    public void DisplayTodayPieChart(DayData _dayData)
    {
        ResetCircle();

        // パイチャートを作るよ
        // とりあえず今日の分
        //print("pre Sorted");
        //_dayData.works.ForEach(v =>
        //{
        //    print(v.startUnixSec);
        //});
        _dayData.works.Sort((a, b) => a.startUnixSec - b.startUnixSec);
        //print("Sorted");
        //_dayData.works.ForEach(v =>
        //{
        //    print(v.startUnixSec);
        //});


    }

    private void SetPieChartAnimation()
    {
        //ResetCircle();
        //databaseDirector.FetchTodayWorks();
    }

    private void ResetCircle()
    {
        for (int i=0; i<this.transform.childCount; i++)
        {
            Destroy(this.transform.GetChild(i).gameObject);
        }
    }

    //private void CreatePieceOfPie(GameObject _obj, )
    //{

    //}
}