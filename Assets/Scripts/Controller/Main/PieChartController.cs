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

    private List<LogPieData> logPieList;
    internal class LogPieData
    {
        public GameObject gameObject_;
        public WorkData workData;
    }

    private GameObject currentPie;

    private void Awake()
    {
        logPieList = new List<LogPieData>();
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

    //private List<

    private List<PieStartEndData> timeSequence24h;
    internal class PieStartEndData
    {
        public int start;
        public int end;
    }

    /// <summary>
    /// �\�[�g���Ă�̂Œ���
    /// </summary>
    /// <param name="_dayData"></param>
    /// <param name="_projects"></param>
    public void CreateLogPieChart(List<WorkData> _dayData, List<ProjectData> _projects)
    {
        ResetCircle();

        List<WorkData> worksWithinRange = new List<WorkData>();
        // ���v�������1���ȓ���logWork��I��
        //worksWithinRange = _dayData.FindAll(w => IsWithinOneClockLapAgo(w));
        worksWithinRange = _dayData;
        worksWithinRange.Sort((a, b) => b.startUnixSec - a.startUnixSec);

        timeSequence24h = new List<PieStartEndData>();

        for (int i = worksWithinRange.Count - 1; i >= 0; i--)
        {
            int id = _projects.FindIndex(v => v.name == worksWithinRange[i].projectName);
            if (id == -1)
            {
                id = 0;
            }
            CreateLogPie(worksWithinRange[i], _projects[id]);

            // ���n��f�[�^�̍쐬
            timeSequence24h.Add(new PieStartEndData()
            {
                start = worksWithinRange[i].startUnixSec,
                end = worksWithinRange[i].endUnixSec
            });
        }

    }

    /// <summary>
    /// LogPie�̏���������
    /// </summary>
    private void ResetCircle()
    {
        //GameObject container = mainUIDirector.LogPieContainer;
        GameObject container = this.transform.Find("LogWorks").gameObject;
        for (int i = 0; i < container.transform.childCount; i++)
        {
            Destroy(container.transform.GetChild(i).gameObject);
        }
        logPieList = new List<LogPieData>();
    }

    /// <summary>
    /// ���v�������1���ȓ���WorkData��true
    /// 1���ȓ��ɊJ�n���A�I���������̂��擾����
    /// �i�ۑ����ɁA1�����O�ɊJ�n���A1���ȓ��ɏI���������̂�0:00�ŕ��������j
    /// �H�H�H�H�H���֌W�Ȃ��ˁH���ݎ�������24���Ԉȓ��Ȃ�A0:00�ł̕����Ƃ��Ӗ��Ȃ�������
    /// </summary>
    /// <param name="w"></param>
    /// <returns></returns>
    private bool IsWithinOneClockLapAgo(WorkData w)
        => w.startUnixSec >= appDirector.GetSecondOfOneClockLapAgo()
        && w.endUnixSec <= appDirector.GetSecondOfNow();

    /// <summary>
    /// LogPie�̐���
    /// </summary>
    /// <param name="_work"></param>
    /// <param name="_project"></param>
    private void CreateLogPie(WorkData _work, ProjectData _project)
    {
        GameObject newPie = Instantiate(
            CircleImage,
            Vector3.zero,
            Quaternion.identity,
            this.transform.Find("LogWorks"));
            //mainUIDirector.LogPieContainer.transform);

        newPie.transform.localPosition = new Vector3(0, 0, 0);
        newPie.GetComponent<Image>().color = _project.pieColor.GetWithColorFormat();
        // _work�����{����projectName���w��
        newPie.GetComponent<PieController>().ProjectName = _work.projectName;
        SetupPie(_work, newPie);

        newPie.SetActive(true);
        logPieList.Add(new LogPieData()
        {
            gameObject_ = newPie,
            workData = _work
        });
    }

    public void CreateCurrentPie(WorkData _work, ProjectData _project)
    {
        currentPie.transform.localPosition = new Vector3(0, 0, 0);
        currentPie.GetComponent<Image>().color = _project.pieColor.GetWithColorFormat();
        // _work�����{����projectName���w��
        currentPie.GetComponent<PieController>().ProjectName = _work.projectName;
        SetupPie(_work, currentPie);

        currentPie.SetActive(true);
    }

    public void UpdateCurrentPie(WorkData _work)
    {
        SetupPie(_work, currentPie);
    }

    /// <summary>
    /// ��ƏI�����ACurrentWorkPiece�̃p�C�E����Log�Ɉڂ�
    /// </summary>
    public void EndCurrentWork(WorkData _currentWork)
    {
        GameObject endPiePiece = Instantiate(currentPie, this.transform.Find("LogWorks").transform);

        endPiePiece.GetComponent<PieController>().ProjectName
            = currentPie.GetComponent<PieController>().ProjectName;

        currentPie.SetActive(false);
        logPieList.Add(new LogPieData()
        {
            gameObject_ = endPiePiece,
            workData = _currentWork
        });
        timeSequence24h.Add(new PieStartEndData()
        {
            start = _currentWork.startUnixSec,
            end = _currentWork.endUnixSec,
        });
    }

    /// <summary>
    /// �p�C�̑傫���A��]�ʒu�Ȃǂ�ݒ�
    /// </summary>
    /// <param name="_work"></param>
    /// <param name="_obj"></param>
    private void SetupPie(WorkData _work, GameObject _obj)
    {
        // (endUnixSec - �����̎n��) / (�����̏I��� - �����̎n��)
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

    public void UpdateLogPie(int _now)
    {
        int past24h = _now - 24 * 3600;
        int past24hWorkId = timeSequence24h.FindIndex(v => past24h >= v.start && past24h <= v.end);
        if (past24hWorkId != -1)
        {
            WorkData past24hWorkData = logPieList[past24hWorkId].workData.ShallowCopy();
            past24hWorkData.startUnixSec = past24h;
            SetupPie(past24hWorkData, logPieList[past24hWorkId].gameObject_);
        }



    }

    public void SwitchAMPM(bool _toPM)
    {
        // �\���E��\���̐؂�ւ�
        int yesterdayNoon = GetYesterday12hTotalSeconds();
        int today = GetTodayTotalSeconds();
        int todayNoon = GetToday12hTotalSeconds();

        logPieList.ForEach(pie =>
        {
            int start = pie.workData.startUnixSec;
            int end = pie.workData.endUnixSec;
            bool isYesterdayAfterNoon =
                (start >= yesterdayNoon || end >= yesterdayNoon)
                && (start < today && end < today);
            bool isTodayAfterNoon =
                (start >= todayNoon || end >= todayNoon);

            bool isAfterNoonLog = isYesterdayAfterNoon || isTodayAfterNoon;
            pie.gameObject_.SetActive(_toPM ? isAfterNoonLog : !isAfterNoonLog);
        });
    }

    private int GetYesterday12hTotalSeconds()
        => (int)DateTime.Today.AddDays(-1).AddHours(12).Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

    private int GetTodayTotalSeconds()
        => (int)DateTime.Today.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

    private int GetToday12hTotalSeconds()
        => (int)DateTime.Today.AddHours(12).Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

    /// <summary>
    /// �ݒ�ύX���f�p
    /// </summary>
    public void UpdateTodayColors(List<ProjectData> _projects)
    {
        logPieList.ForEach(v =>
        {
            string pieProjectName = v.gameObject_.GetComponent<PieController>().ProjectName;
            int id = _projects.FindIndex(p => p.name == pieProjectName);
            if (id == -1) id = 0;

            v.gameObject_.GetComponent<Image>().color = _projects[id].pieColor.GetWithColorFormat();
        });
    }

    public void UpdateCurrentPieColor(Color _color, string _projectName)
    {
        currentPie.GetComponent<Image>().color = _color;
        currentPie.GetComponent<PieController>().ProjectName = _projectName;
    }


}