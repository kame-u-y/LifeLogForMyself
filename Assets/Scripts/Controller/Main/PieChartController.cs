using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieChartController : MonoBehaviour
{
    private AppDirector appDirector;
    private MainUIDirector mainUIDirector;

    [SerializeField]
    private GameObject CircleImage;

    private List<GameObject> logPieList;
    private GameObject currentPie;

    private void Awake()
    {
        logPieList = new List<GameObject>();
        currentPie = this.transform.Find("CurrentWork").gameObject;

        appDirector = AppDirector.Instance;
        mainUIDirector = MainUIDirector.Instance;
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
    public void UpdateLogPieChart(List<WorkData> _dayData, List<ProjectData> _projects)
    {
        ResetCircle();

        List<WorkData> worksWithinRange = new List<WorkData>();

        // 時計ぐるっと1周以内のlogWorkを選別
        worksWithinRange = _dayData.FindAll(w => IsWithinOneClockLapAgo(w));

        worksWithinRange.Sort((a, b) => a.startUnixSec - b.startUnixSec);

        for (int i = worksWithinRange.Count - 1; i >= 0; i--)
        {
            int id = _projects.FindIndex(v => v.name == worksWithinRange[i].projectName);
            if (id == -1)
            {
                id = 0;
            }
            CreateLogPie(worksWithinRange[i], _projects[id]);
        }
    }

    /// <summary>
    /// LogPieの初期化処理
    /// </summary>
    private void ResetCircle()
    {
        GameObject container = mainUIDirector.LogPieContainer;
        for (int i = 0; i < container.transform.childCount; i++)
        {
            Destroy(container.transform.GetChild(i).gameObject);
        }
        logPieList = new List<GameObject>();
    }

    /// <summary>
    /// 時計ぐるっと1周以内のWorkDataはtrue
    /// 1周以内に開始し、終了したものを取得する
    /// （保存時に、1周より前に開始し、1周以内に終了したものは0:00で分割される）
    /// ？？？？？↑関係なくね？現在時刻から24時間以内なら、0:00での分割とか意味ないじゃんね
    /// </summary>
    /// <param name="w"></param>
    /// <returns></returns>
    private bool IsWithinOneClockLapAgo(WorkData w)
        => w.startUnixSec >= appDirector.GetSecondOfOneClockLapAgo()
        && w.endUnixSec <= appDirector.GetSecondOfNow();

    /// <summary>
    /// LogPieの生成
    /// </summary>
    /// <param name="_work"></param>
    /// <param name="_project"></param>
    private void CreateLogPie(WorkData _work, ProjectData _project)
    {
        GameObject newPie = Instantiate(
            CircleImage,
            Vector3.zero, 
            Quaternion.identity, 
            mainUIDirector.LogPieContainer.transform);

        newPie.transform.localPosition = new Vector3(0, 0, 0);
        newPie.GetComponent<Image>().color = _project.pieColor.GetWithColorFormat();
        // _workが持つ本当のprojectNameを指定
        newPie.GetComponent<PieController>().ProjectName = _work.projectName; 
        SetupPie(_work, newPie);

        newPie.SetActive(true);
        logPieList.Add(newPie);
    }

    public void CreateCurrentPie(WorkData _work, ProjectData _project)
    {
        currentPie.transform.localPosition = new Vector3(0, 0, 0);
        currentPie.GetComponent<Image>().color = _project.pieColor.GetWithColorFormat();
        // _workが持つ本当のprojectNameを指定
        currentPie.GetComponent<PieController>().ProjectName = _work.projectName;
        SetupPie(_work, currentPie);

        currentPie.SetActive(true);
    }

    public void UpdateCurrentPie(WorkData _work)
    {
        SetupPie(_work, currentPie);
    }

    /// <summary>
    /// 作業終了時、CurrentWorkPieceのパイ・情報をLogに移す
    /// </summary>
    public void EndCurrentWork()
    {
        GameObject endPiePiece = Instantiate(currentPie, this.transform.Find("LogWorks").transform);
        
        endPiePiece.GetComponent<PieController>().ProjectName
            = currentPie.GetComponent<PieController>().ProjectName;
        
        currentPie.SetActive(false);
        logPieList.Add(endPiePiece);
    }

    /// <summary>
    /// パイの大きさ、回転位置などを設定
    /// </summary>
    /// <param name="_work"></param>
    /// <param name="_obj"></param>
    private void SetupPie(WorkData _work, GameObject _obj)
    {
        // (endUnixSec - 今日の始め) / (今日の終わり - 今日の始め)
        int startSec = appDirector.GetSecondOfClockStart();
        int endSec = appDirector.GetSecondOfClockEnd();

        float angle = CalculatePieRotationValue(360.0f, startSec, _work.startUnixSec, startSec, endSec);
        _obj.transform.rotation = Quaternion.Euler(0, 0, -angle);

        _obj.GetComponent<Image>().fillAmount
            = CalculatePieRotationValue(1.0f, _work.startUnixSec, _work.endUnixSec, startSec, endSec);
    }

    private float CalculatePieRotationValue(
        float _rotationMax, 
        int _startOfValue, 
        int _endOfValue, 
        int _startOfAll, 
        int _endOfAll)
        => (float)_rotationMax * (_endOfValue - _startOfValue) / (_endOfAll - _startOfAll);

    /// <summary>
    /// 設定変更反映用
    /// </summary>
    public void UpdateTodayColors(List<ProjectData> _projects)
    {
        logPieList.ForEach(v =>
        {
            string pieProjectName = v.GetComponent<PieController>().ProjectName;
            int id = _projects.FindIndex(p => p.name == pieProjectName);
            if (id == -1) id = 0;

            v.GetComponent<Image>().color = _projects[id].pieColor.GetWithColorFormat();
        });
    }

    public void UpdateCurrentPieColor(Color _color, string _projectName)
    {
        currentPie.GetComponent<Image>().color = _color;
        currentPie.GetComponent<PieController>().ProjectName = _projectName;
    }


}