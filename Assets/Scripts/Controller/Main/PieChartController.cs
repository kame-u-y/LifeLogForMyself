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
        currentWorkPiece = this.transform.Find("CurrentWork").gameObject;
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
        List<WorkData> worksWithinRange = new List<WorkData>();
        worksWithinRange = _dayData.works.FindAll((v) =>
        {
            int startSec = gameDirector.GetSecondOfClockStart();
            int endSec = gameDirector.GetSecondOfClockEnd();
            return v.startUnixSec >= startSec && v.endUnixSec <= endSec;
        });
        worksWithinRange.Sort((a, b) => a.startUnixSec - b.startUnixSec);

        for (int i = worksWithinRange.Count - 1; i >= 0; i--)
        {
            ProjectData p = _projects.Find(v => v.name == worksWithinRange[i].projectName);
            CreateLogWorkPiece(worksWithinRange[i], p);
            //CreateBlankPiece(_dayData.works[i].startUnixSec - 1);
        }

        // パイチャートの幅は基本的にはendUnixSecで決定する

    }

    private void ResetCircle()
    {

        GameObject container = this.transform.Find("LogWorks").gameObject;
        for (int i = 0; i < container.transform.childCount; i++)
        {
            Destroy(container.transform.GetChild(i).gameObject);
        }
        workPiePieces = new List<GameObject>();
    }

    private void CreateLogWorkPiece(WorkData _work, ProjectData _project)
    {
        if (_project == null) return;

        GameObject newPiePiece = Instantiate(CircleImage, Vector3.zero, Quaternion.identity, this.transform.Find("LogWorks").transform);
        newPiePiece.transform.localPosition = new Vector3(0, 0, 0);
        newPiePiece.GetComponent<Image>().color = _project.pieColor.GetWithColorFormat();
        newPiePiece.GetComponent<PieController>().ProjectName = _project.name;
        SetupPiece(_work, newPiePiece);

        newPiePiece.SetActive(true);
        workPiePieces.Add(newPiePiece);
    }

    public void CreateCurrentWorkPiece(WorkData _work, ProjectData _project)
    {
        currentWorkPiece.transform.localPosition = new Vector3(0, 0, 0);
        currentWorkPiece.GetComponent<Image>().color = _project.pieColor.GetWithColorFormat();
        currentWorkPiece.GetComponent<PieController>().ProjectName = _project.name;
        SetupPiece(_work, currentWorkPiece);

        currentWorkPiece.SetActive(true);
    }

    public void UpdateCurrentWorkPiece(WorkData _work)
    {
        SetupPiece(_work, currentWorkPiece);
    }

    /// <summary>
    /// 作業終了時、CurrentWorkPieceのパイ・情報をLogに移す
    /// </summary>
    public void EndCurrentWork()
    {
        GameObject endPiePiece = Instantiate(currentWorkPiece, this.transform.Find("LogWorks").transform);
        //endPiePiece.transform.localPosition = new Vector3(0, 0, 0);
        endPiePiece.GetComponent<PieController>().ProjectName 
            = currentWorkPiece.GetComponent<PieController>().ProjectName;
        workPiePieces.Add(endPiePiece);
        currentWorkPiece.SetActive(false);
    }

    /// <summary>
    /// パイの大きさ、回転位置などを設定
    /// </summary>
    /// <param name="_work"></param>
    /// <param name="_obj"></param>
    private void SetupPiece(WorkData _work, GameObject _obj)
    {
        // (endUnixSec - 今日の始め) / (今日の終わり - 今日の始め)
        int startSec = gameDirector.GetSecondOfClockStart();
        int endSec = gameDirector.GetSecondOfClockEnd();

        float angle = CalculatePieRotationValue(360.0f, startSec, _work.startUnixSec, startSec, endSec);
        _obj.transform.rotation = Quaternion.Euler(0, 0, -angle);

        _obj.GetComponent<Image>().fillAmount
            = CalculatePieRotationValue(1.0f, _work.startUnixSec, _work.endUnixSec, startSec, endSec);
    }

    private float CalculatePieRotationValue(float _rotationMax, int _startOfValue, int _endOfValue, int _startOfAll, int _endOfAll)
        => (float)_rotationMax * (_endOfValue - _startOfValue) / (_endOfAll - _startOfAll);

    /// <summary>
    /// 設定変更反映用
    /// </summary>
    public void UpdateTodayColors(List<ProjectData> _projects)
    {
        workPiePieces.ForEach(v =>
        {
            string pieProjectName = v.GetComponent<PieController>().ProjectName;
            int id = _projects.FindIndex(p => p.name == pieProjectName);
            if (id == -1) id = 0;

            v.GetComponent<Image>().color = _projects[id].pieColor.GetWithColorFormat();
        });
    }

    public void ChangeCurrentColor(Color _color, string _projectName)
    {
        currentWorkPiece.GetComponent<Image>().color = _color;
        currentWorkPiece.GetComponent<PieController>().ProjectName = _projectName;
    }


}