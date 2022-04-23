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

    private AppDirector appDirector;



    private void Awake()
    {
        //databaseDirector = GameObject.Find("DatabaseDirector").GetComponent<DatabaseDirector>();
        workPiePieces = new List<GameObject>();

        appDirector = GameObject.Find("AppDirector").GetComponent<AppDirector>();
        currentWorkPiece = this.transform.Find("CurrentWork").gameObject;
    }

    private void Start()
    {

    }

    private void Update()
    {

    }

    /// <summary>
    /// �\�[�g���Ă�̂Œ��� �Q�Ƃ��ȁHsaveData�ɉe�������邩��
    /// </summary>
    /// <param name="_dayData"></param>
    /// <param name="_projects"></param>
    public void DisplayTodayPieChart(List<WorkData> _dayData, List<ProjectData> _projects)
    {
        ResetCircle();

        // �p�C�`���[�g������
        // �Ƃ肠���������̕�
        List<WorkData> worksWithinRange = new List<WorkData>();
        //worksWithinRange = _dayData.FindAll((v) =>
        //{
        //    int startSec = appDirector.GetSecondOfClockStart();
        //    int endSec = appDirector.GetSecondOfClockEnd();
        //    return v.startUnixSec >= startSec && v.endUnixSec <= endSec;
        //});
        // ���v����O���猻�݂܂ł�works�̂ݕ\��
        worksWithinRange = _dayData.FindAll(
            v => v.startUnixSec >= appDirector.GetSecondOfOneClockLapAgo()
                 && v.endUnixSec <= appDirector.GetSecondOfNow());

        worksWithinRange.Sort((a, b) => a.startUnixSec - b.startUnixSec);

        for (int i = worksWithinRange.Count - 1; i >= 0; i--)
        {
            int id = _projects.FindIndex(v => v.name == worksWithinRange[i].projectName);
            if (id == -1) id = 0;
            CreateLogWorkPiece(worksWithinRange[i], _projects[id]);
            //CreateBlankPiece(_dayData.works[i].startUnixSec - 1);
        }

        // �p�C�`���[�g�̕��͊�{�I�ɂ�endUnixSec�Ō��肷��

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
        newPiePiece.GetComponent<PieController>().ProjectName = _work.projectName;
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
    /// ��ƏI�����ACurrentWorkPiece�̃p�C�E����Log�Ɉڂ�
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
    /// �p�C�̑傫���A��]�ʒu�Ȃǂ�ݒ�
    /// </summary>
    /// <param name="_work"></param>
    /// <param name="_obj"></param>
    private void SetupPiece(WorkData _work, GameObject _obj)
    {
        // (endUnixSec - �����̎n��) / (�����̏I��� - �����̎n��)
        int startSec = appDirector.GetSecondOfClockStart();
        int endSec = appDirector.GetSecondOfClockEnd();

        float angle = CalculatePieRotationValue(360.0f, startSec, _work.startUnixSec, startSec, endSec);
        _obj.transform.rotation = Quaternion.Euler(0, 0, -angle);

        _obj.GetComponent<Image>().fillAmount
            = CalculatePieRotationValue(1.0f, _work.startUnixSec, _work.endUnixSec, startSec, endSec);
    }

    private float CalculatePieRotationValue(float _rotationMax, int _startOfValue, int _endOfValue, int _startOfAll, int _endOfAll)
        => (float)_rotationMax * (_endOfValue - _startOfValue) / (_endOfAll - _startOfAll);

    /// <summary>
    /// �ݒ�ύX���f�p
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