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
            // ��~�{�^��
            GetComponent<Image>().color = new Color(255, 0, 0);
        }
        else
        {
            // �Đ��{�^��
            GetComponent<Image>().color = new Color(0, 255, 0);
        }
    }
}
