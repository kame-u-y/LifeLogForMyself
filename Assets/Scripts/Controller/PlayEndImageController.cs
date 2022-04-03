using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayEndImageController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void ChangeButtonImage(bool _isWorking)
    {
        if (_isWorking)
        {
            // 停止ボタン
            GetComponent<Image>().color = new Color(111 / 255.0f, 100 / 255.0f, 100 / 255.0f);
            GetComponent<Image>().sprite = Resources.Load<Sprite>("endButton02");
        }
        else
        {
            // 再生ボタン
            GetComponent<Image>().color = new Color(100/255.0f, 111 / 255.0f, 100 / 255.0f);
            GetComponent<Image>().sprite = Resources.Load<Sprite>("playButton01");
        }
    }
}
