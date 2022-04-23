using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
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
        //audioSource_.clip = audioClip;
    }

    // Start is called before the first frame update
    void Start()
    {
        //clockLabelCtrler.UpdateWorkMaxLabel(maxMinute);

        databaseDirector = GameObject.Find("DatabaseDirector").GetComponent<DatabaseDirector>();

        maxMinute = databaseDirector.FetchProgressMeterMax();
        string selectedProject = databaseDirector.FetchSelectedProject();

        ProjectData p = databaseDirector.FindProject(selectedProject);
        UpdateColor(p.pieColor.GetWithColorFormat());
        clockLabelCtrler.UpdateWorkMaxLabel(maxMinute);

        UpdateNotificationSound(databaseDirector.FetchNotificationSoundPath());
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
    /// �ݒ�X�V�p workingDirector����Ă΂��
    /// loopCount���X�V���K�v
    /// </summary>
    /// <param name="_maxMinute"></param>
    public void UpdateWorkMax(float _maxMinute, int _elapsedTime)
    {
        maxMinute = _maxMinute;
        loopCount = (int) (_elapsedTime / (maxMinute * 60.0f));
        clockLabelCtrler.UpdateWorkMaxLabel(maxMinute);
    }

    /// <summary>
    /// �ݒ�X�V�p
    /// </summary>
    /// <param name="_color"></param>
    public void UpdateColor(Color _color)
    {
        image_.color = _color;
    }

    /// <summary>
    /// �ʒm�� �ݒ�ύX�p
    /// </summary>
    /// <param name="_path"></param>
    public void UpdateNotificationSound(string _path)
    {
        StartCoroutine(UpdateAudioClip(_path));
        
    }

    /// <summary>
    /// �ݒ肳�ꂽ���[�J���̉����t�@�C����audioClip�ɐݒ肷��
    /// </summary>
    /// <param name="_path"></param>
    /// <returns></returns>
    private IEnumerator UpdateAudioClip(string _path)
    {
        string ext = Path.GetExtension(_path);

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip($"file:///{_path}", extentionDictionary[ext]))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
            }
            else
            {
                audioSource_.clip = DownloadHandlerAudioClip.GetContent(www);
            }
        }
    }

    private Dictionary<string, AudioType> extentionDictionary = new Dictionary<string, AudioType>()
    {
        [".mp3"] = AudioType.MPEG,
        [".ogg"] = AudioType.OGGVORBIS,
        [".wav"] = AudioType.WAV,
        [".aiff"] = AudioType.AIFF,
        [".aif"] = AudioType.AIFF,
        [".mod"] = AudioType.MOD,
        [".it"] = AudioType.IT,
        [".s3m"] = AudioType.S3M,
        [".xm"] = AudioType.XM,
    };
}