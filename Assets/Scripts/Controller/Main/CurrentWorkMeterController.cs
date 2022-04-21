using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentWorkMeterController : MonoBehaviour
{
    [SerializeField]
    private float maxMinute = 1;
    private int loopCount = 0;
    [SerializeField]
    ClockLabelController clockLabelCtrler;
    
    private AudioSource audioSource_;
    [SerializeField]
    private AudioClip audioClip;

    private DatabaseDirector databaseDirector;

    Image image_;

    private void Awake()
    {
        image_ = this.GetComponent<Image>();
        //maxMinute = 1;
        image_.fillAmount = 1.0f;

        audioSource_ = this.gameObject.GetComponent<AudioSource>();
        audioSource_.clip = audioClip;
    }

    // Start is called before the first frame update
    void Start()
    {
        //clockLabelCtrler.UpdateWorkMaxLabel(maxMinute);

        databaseDirector = GameObject.Find("DatabaseDirector").GetComponent<DatabaseDirector>();
        maxMinute = databaseDirector.FetchProgressMeterMax();
        string selectedProject = databaseDirector.FetchSelectedProject();
        Debug.Log(selectedProject);
        ProjectData p = databaseDirector.FindProject(selectedProject);
        UpdateColor(p.pieColor.GetWithColorFormat());
        clockLabelCtrler.UpdateWorkMaxLabel(maxMinute);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeMeter()
    {
        image_.fillAmount = 1.0f;
        loopCount = 0;
    }

    public void UpdateMeter(int _elapsedTime)
    {
        var ratio = (float)(1.0f * _elapsedTime / (maxMinute * 60.0f));
        image_.fillAmount = ratio % 1.0f;
        if (ratio - loopCount >= 1)
        {
            audioSource_.Play();
            loopCount++;
        }
    }

    /// <summary>
    /// 設定更新用 workingDirectorから呼ばれる
    /// loopCountも更新が必要
    /// </summary>
    /// <param name="_maxMinute"></param>
    public void UpdateWorkMax(float _maxMinute, int _elapsedTime)
    {
        maxMinute = _maxMinute;
        loopCount = (int) (_elapsedTime / (maxMinute * 60.0f));
        clockLabelCtrler.UpdateWorkMaxLabel(maxMinute);
    }

    /// <summary>
    /// 設定更新用
    /// </summary>
    /// <param name="_color"></param>
    public void UpdateColor(Color _color)
    {
        image_.color = _color;
    }
}
