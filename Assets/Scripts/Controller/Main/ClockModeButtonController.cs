using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClockModeButtonController : MonoBehaviour
{

    //private GameDirector gameDirector;

    // Start is called before the first frame update
    void Start()
    {
        //gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeButtonColor(bool _isClock12h)
    {
        if (_isClock12h)
        {
            this.GetComponent<Image>().color 
                = new Color(115 / 255.0f, 100 / 255.0f, 80 / 255.0f);
        }
        else
        {
            this.GetComponent<Image>().color
                   = new Color(255 / 255.0f, 190 / 255.0f, 100 / 255.0f);
        }
    }
}
