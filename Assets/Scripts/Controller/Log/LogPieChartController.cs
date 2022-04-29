using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogPieChartController : MonoBehaviour
{
    [SerializeField]
    private GameObject pieceTemplate;

    private Transform pieceContainer;

    internal class ProjectSecData
    {
        internal string projectName;
        internal int dayTotalSec;
    }

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        pieceContainer = this.transform.Find("Pieces").transform;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateLogPieChart(List<WorkData> _dayData, List<ProjectData> _projects)
    {
        ResetPieces();

        int dayTotalSec = 0;
        Dictionary<string, int> projectSecDictionary = new Dictionary<string, int>();
        List<ProjectSecData> projectSecData = new List<ProjectSecData>();

        _dayData.ForEach(v =>
        {
            int workSec = v.startUnixSec - v.endUnixSec;
            dayTotalSec += workSec;
            string project = _projects.FindIndex(p => p.name == v.projectName) != -1
                ? v.projectName
                : "No Project";
            if (!projectSecDictionary.ContainsKey(v.projectName))
            {
                projectSecDictionary.Add(v.projectName, 0);
            }
            projectSecDictionary[v.projectName] += workSec;
        });

        foreach (string s in projectSecDictionary.Keys)
        {
            projectSecData.Add(new ProjectSecData()
            {
                projectName = s,
                dayTotalSec = projectSecDictionary[s]
            });
        }

        projectSecData.Sort((a, b) => a.dayTotalSec - b.dayTotalSec);
        Debug.Log(projectSecData.Count);
        int cumulativeSec = 0;
        projectSecData.ForEach(v =>
        {
            ProjectData project = _projects.Find(p => p.name == v.projectName);
            CreatePiece(projectSecDictionary[v.projectName], dayTotalSec, cumulativeSec, project);
            cumulativeSec += projectSecDictionary[v.projectName];
        });

    }

    /// <summary>
    /// LogPieÇÃèâä˙âªèàóù
    /// </summary>
    private void ResetPieces()
    {
        for (int i = 0; i < pieceContainer.childCount; i++)
        {
            Destroy(pieceContainer.GetChild(i).gameObject);
        }
    }

    private void CreatePiece(int _projectSec, int _dayTotalSec, int _cumulativeSec, ProjectData _project)
    {
        GameObject newPiece = Instantiate(pieceTemplate, pieceContainer);

        newPiece.transform.localPosition = Vector3.zero;
        newPiece.GetComponent<Image>().color = _project.pieColor.GetWithColorFormat();
        newPiece.GetComponent<PieController>().ProjectName = _project.name;

        float angle = CalculatePieRotationValue(360.0f, _cumulativeSec, 0, _dayTotalSec, 0);
        newPiece.transform.rotation = Quaternion.Euler(0, 0, -angle);
        newPiece.GetComponent<Image>().fillAmount
            = CalculatePieRotationValue(1.0f, _projectSec, 0, _dayTotalSec, 0);

        newPiece.SetActive(true);
        Debug.Log("hoge");
    }


    private float CalculatePieRotationValue(
        float _rotationMax,
        int _startOfValue,
        int _endOfValue,
        int _startOfAll,
        int _endOfAll)
        => (float)_rotationMax * (_endOfValue - _startOfValue) / (_endOfAll - _startOfAll);
}
