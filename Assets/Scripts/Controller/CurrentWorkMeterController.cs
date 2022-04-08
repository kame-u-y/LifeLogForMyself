using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentWorkMeterController : MonoBehaviour
{
    [SerializeField]
    private int maxMinute = 1;
    [SerializeField]
    ClockLabelController clockLabelCtrler;

    Image image_;

    private void Awake()
    {
        image_ = this.GetComponent<Image>();
        //maxMinute = 1;
        image_.fillAmount = 1.0f;
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
    }

    public void UpdateMeter(int _elapsedTime)
    {
        image_.fillAmount = (float) (1.0f * _elapsedTime / (maxMinute * 60.0f)) % 1.0f;
    }

    public void ChangeColor(Color _color)
    {
        image_.color = _color;
    }
}
