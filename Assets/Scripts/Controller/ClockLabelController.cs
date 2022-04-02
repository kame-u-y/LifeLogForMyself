using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClockLabelController : MonoBehaviour
{
    private TextMeshProUGUI labelN;
    private TextMeshProUGUI labelE;
    private TextMeshProUGUI labelS;
    private TextMeshProUGUI labelW;

    private GameDirector gameDirector;
    private bool pIsClock12h;
    private bool isClockChanged = true;

    // Start is called before the first frame update
    void Start()
    {
        labelE = this.transform.Find("ClockLabelE").GetComponent<TextMeshProUGUI>();
        labelS = this.transform.Find("ClockLabelS").GetComponent<TextMeshProUGUI>();
        labelW = this.transform.Find("ClockLabelW").GetComponent<TextMeshProUGUI>();
        labelN = this.transform.Find("ClockLabelN").GetComponent<TextMeshProUGUI>();

        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
        
        pIsClock12h = gameDirector.isClock12h;
        ChangeClockLabels();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameDirector.isClock12h != pIsClock12h)
        {
            ChangeClockLabels();
            pIsClock12h = gameDirector.isClock12h;
        }
    }

    private void ChangeClockLabels()
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
