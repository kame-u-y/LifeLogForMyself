using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentWorkMeterController : MonoBehaviour
{
    [SerializeField]
    private int maxMinute = 1;
    private int loopCount = 0;
    [SerializeField]
    ClockLabelController clockLabelCtrler;
    
    private AudioSource audioSource_;
    [SerializeField]
    private AudioClip audioClip;

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
        clockLabelCtrler.ChangeWorkMaxLabel(maxMinute);

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

    public void ChangeColor(Color _color)
    {
        image_.color = _color;
    }
}
