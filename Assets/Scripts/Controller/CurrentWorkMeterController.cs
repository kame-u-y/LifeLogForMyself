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

    private void Awake()
    {
        //maxMinute = 1;
        this.GetComponent<Image>().fillAmount = 1.0f;
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

    public void UpdateMeter(int _elapsedTime)
    {
        this.GetComponent<Image>().fillAmount = (float) (1.0f * _elapsedTime / (maxMinute * 60.0f)) % 1.0f;
    }

    public void ChangeColor(Color _color)
    {
        this.GetComponent<Image>().color = _color;
    }
}
