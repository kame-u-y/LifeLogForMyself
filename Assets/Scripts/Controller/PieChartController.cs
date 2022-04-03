using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieChartController : MonoBehaviour
{
    [SerializeField]
    private GameObject CircleImage;

    private List<GameObject> workPiePieces;
    private GameObject currentWorkPiece;

    private GameDirector gameDirector;


    private void Awake()
    {
        //databaseDirector = GameObject.Find("DatabaseDirector").GetComponent<DatabaseDirector>();
        workPiePieces = new List<GameObject>();

        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
    }

    private void Start()
    {
    }

    private void Update()
    {

    }

    /// <summary>
    /// ソートしてるので注意 参照かな？saveDataに影響があるかも
    /// </summary>
    /// <param name="_dayData"></param>
    /// <param name="_projects"></param>
    public void DisplayTodayPieChart(DayData _dayData, List<ProjectData> _projects)
    {
        ResetCircle();

        // パイチャートを作るよ
        // とりあえず今日の分
        _dayData.works.Sort((a, b) => a.startUnixSec - b.startUnixSec);

        for (int i = _dayData.works.Count-1; i >= 0; i--)
        {
            ProjectData p = _projects.Find(v => v.name == _dayData.works[i].projectName);
            CreatePiePiece(_dayData.works[i], p);
            //CreateBlankPiece(_dayData.works[i].startUnixSec - 1);
        }

        // パイチャートの幅は基本的にはendUnixSecで決定する

    }


    //private void SetPieChartAnimation()
    //{
    //    //ResetCircle();
    //    //databaseDirector.FetchTodayWorks();
    //}

    private void ResetCircle()
    {
        GameObject container = this.transform.Find("LogWorks").gameObject;
        for (int i = 0; i < container.transform.childCount; i++)
        {
            Destroy(container.transform.GetChild(i).gameObject);
        }
    }


    //private void CreateBlankPiece(int _endSec)
    //{
    //    GameObject blank = Instantiate(CircleImage, Vector3.zero, Quaternion.identity, this.transform);
    //    blank.transform.localPosition = new Vector3(0, 0, 0);
    //    //blank.GetComponent<Image>().color = new Color(170.0f / 255.0f, 170.0f / 255.0f, 170.0f / 255.0f);

    //    // (endUnixSec - 今日の始め) / (今日の終わり - 今日の始め)
    //    int todayStartSec = GetSecondOfToday(0, 0, 0);
    //    int todayEndSec = GetSecondOfToday(23, 59, 59);

    //    blank.GetComponent<Image>().fillAmount
    //        = (float) (gameDirector.isClock12h ? 2.0f : 1.0f) * (_endSec - todayStartSec) / (todayEndSec - todayStartSec);
    //    blank.SetActive(true);
    //    workPiePieces.Add(blank);
    //}

    private void CreatePiePiece(WorkData _work, ProjectData _project)
    {
        GameObject newPiePiece = Instantiate(CircleImage, Vector3.zero, Quaternion.identity, this.transform.Find("LogWorks").transform);
        newPiePiece.transform.localPosition = new Vector3(0, 0, 0);
        newPiePiece.GetComponent<Image>().color
            = new Color(_project.pieColor.r, _project.pieColor.g, _project.pieColor.b);




        // (endUnixSec - 今日の始め) / (今日の終わり - 今日の始め)
        int todayStartSec = GetSecondOfToday(0, 0, 0);
        int todayEndSec = GetSecondOfToday(23, 59, 59);

        //// 保存されてるサンプルのendUnixSecがおかしな値だからまともな表示にならない
        //newPiePiece.GetComponent<Image>().fillAmount
        //    = (float) (gameDirector.isClock12h ? 2.0f : 1.0f) * (_work.endUnixSec - todayStartSec) / (todayEndSec - todayStartSec);

        float angle = (gameDirector.isClock12h ? 2 : 1) * 360.0f * (_work.startUnixSec - todayStartSec) / (todayEndSec - todayStartSec);
        newPiePiece.transform.rotation = Quaternion.Euler(0, 0, -angle);
        newPiePiece.GetComponent<Image>().fillAmount
            = (float)(gameDirector.isClock12h ? 2.0f : 1.0f) * (_work.endUnixSec - _work.startUnixSec) / (todayEndSec - todayStartSec);


        newPiePiece.SetActive(true);
        workPiePieces.Add(newPiePiece);
    }


    public void CreateCurrentWorkPiece(WorkData _work, ProjectData _project)
    {
        //CreatePiePiece(_work, _project);
        currentWorkPiece = Instantiate(CircleImage, Vector3.zero, Quaternion.identity, this.transform.Find("CurrentWork").transform);
        currentWorkPiece.transform.localPosition = new Vector3(0, 0, 0);
        currentWorkPiece.GetComponent<Image>().color
            = new Color(_project.pieColor.r, _project.pieColor.g, _project.pieColor.b);




        // (endUnixSec - 今日の始め) / (今日の終わり - 今日の始め)
        int todayStartSec = GetSecondOfToday(0, 0, 0);
        int todayEndSec = GetSecondOfToday(23, 59, 59);

        //// 保存されてるサンプルのendUnixSecがおかしな値だからまともな表示にならない
        //currentWorkPiece.GetComponent<Image>().fillAmount
        //    = (float) (gameDirector.isClock12h ? 2.0f : 1.0f) * (_work.endUnixSec - todayStartSec) / (todayEndSec - todayStartSec);

        float angle = (gameDirector.isClock12h ? 2 : 1) * 360.0f * (_work.startUnixSec - todayStartSec) / (todayEndSec - todayStartSec);
        currentWorkPiece.transform.rotation = Quaternion.Euler(0, 0, -angle);
        currentWorkPiece.GetComponent<Image>().fillAmount
            = (float)(gameDirector.isClock12h ? 2.0f : 1.0f) * (_work.endUnixSec - _work.startUnixSec) / (todayEndSec - todayStartSec);


        currentWorkPiece.SetActive(true);

        //workPiePieces.Add(newPiePiece);
    }

    public void UpdateCurrentWorkPiece(WorkData _work)
    {
        // (endUnixSec - 今日の始め) / (今日の終わり - 今日の始め)
        int todayStartSec = GetSecondOfToday(0, 0, 0);
        int todayEndSec = GetSecondOfToday(23, 59, 59);

        // 保存されてるサンプルのendUnixSecがおかしな値だからまともな表示にならない
        //workPiePieces[currentWorkPieceId].GetComponent<Image>().fillAmount
        //    = (float) (gameDirector.isClock12h ? 2.0f : 1.0f) * (_work.endUnixSec - todayStartSec) / (todayEndSec - todayStartSec);
        float angle = (gameDirector.isClock12h ? 2 : 1) * 360.0f * (_work.startUnixSec - todayStartSec) / (todayEndSec - todayStartSec);
        currentWorkPiece.transform.rotation = Quaternion.Euler(0, 0, -angle);
        currentWorkPiece.GetComponent<Image>().fillAmount
            = (float)(gameDirector.isClock12h ? 2.0f : 1.0f) * (_work.endUnixSec - _work.startUnixSec) / (todayEndSec - todayStartSec);
    }

    public void ChangeCurrentColor(Color _color)
    {
        currentWorkPiece.GetComponent<Image>().color = _color;
    }


    private int GetSecondOfToday(int _h, int _m, int _s)
        => (int)new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, _h, _m, _s)
            .Subtract(new DateTime(1970, 1, 1)).TotalSeconds;


    //private void CreatePieceOfPie(GameObject _obj, )
    //{

    //}
}