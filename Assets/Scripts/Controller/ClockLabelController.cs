using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClockLabelController : MonoBehaviour
{
    private TextMeshProUGUI labelN;
    private TextMeshProUGUI labelE;
    private TextMeshProUGUI labelS;
    private TextMeshProUGUI labelW;

    private GameDirector gameDirector;
    
    // Start is called before the first frame update
    void Start()
    {
        labelE = this.transform.Find("ClockLabelE").GetComponent<TextMeshProUGUI>();
        labelS = this.transform.Find("ClockLabelS").GetComponent<TextMeshProUGUI>();
        labelW = this.transform.Find("ClockLabelW").GetComponent<TextMeshProUGUI>();
        labelN = this.transform.Find("ClockLabelN").GetComponent<TextMeshProUGUI>();

        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ChangeClockLabels()
    {
        if (gameDirector.isClock12h)
        {
            labelE.text = "3";
            labelS.text = "6";
            labelW.text = "9";
            labelN.text = "12";
        }
        else
        {
            labelE.text = "6";
            labelS.text = "12";
            labelW.text = "18";
            labelN.text = "24";
        }
    }
}
