using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        string spriteMode = "";
        string clockMode = "";
        if (IsScreenNormalSize())
        {
            this.transform.Find("ClockNumber").gameObject.SetActive(true);
            spriteMode = "Materials";
        }
        else
        {
            this.transform.Find("ClockNumber").gameObject.SetActive(false);
            spriteMode = "Resized_Materials";
        }

        clockMode = _isClock12h ? "12" : "24";


        this.transform.Find("ClockThorn").GetComponent<Image>().sprite =
            Resources.Load<Sprite>($"{spriteMode}/Cover/Label/Label{clockMode}h_Thorn");
        this.transform.Find("ClockNumber").GetComponent<Image>().sprite =
            Resources.Load<Sprite>($"{spriteMode}/Cover/Label/Label{clockMode}h_Number");
    }

    private bool IsScreenNormalSize()
        => ProjectConstants.IsMoreThanThreshold(Screen.width)
        || ProjectConstants.IsMoreThanThreshold(Screen.height);

    public void ChangeWorkMaxLabel(int _minute)
    {
        this.transform.Find("WorkMeterMax").GetComponent<TextMeshProUGUI>().text = _minute.ToString();
    }
}
