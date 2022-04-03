using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClockLabelController : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ChangeClockLabels(bool _isClock12h)
    {
        if (_isClock12h)
        {
            this.transform.Find("ClockE").GetComponent<TextMeshProUGUI>().text = "3";
            this.transform.Find("ClockS").GetComponent<TextMeshProUGUI>().text = "6";
            this.transform.Find("ClockW").GetComponent<TextMeshProUGUI>().text = "9";
            this.transform.Find("ClockN").GetComponent<TextMeshProUGUI>().text = "12";
        }
        else
        {
            this.transform.Find("ClockE").GetComponent<TextMeshProUGUI>().text = "6";
            this.transform.Find("ClockS").GetComponent<TextMeshProUGUI>().text = "12";
            this.transform.Find("ClockW").GetComponent<TextMeshProUGUI>().text = "18";
            this.transform.Find("ClockN").GetComponent<TextMeshProUGUI>().text = "24";
        }
    }

    public void ChangeWorkMaxLabel(int _minute)
    {
        this.transform.Find("WorkMeterMax").GetComponent<TextMeshProUGUI>().text = _minute.ToString();
    }
}
