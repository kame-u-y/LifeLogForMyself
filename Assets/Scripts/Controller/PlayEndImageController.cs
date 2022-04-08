using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayEndImageController : MonoBehaviour
{
    private Image image_;

    // Start is called before the first frame update
    void Start()
    {
        image_ = this.GetComponent<Image>();
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
            //GetComponent<Image>().color = new Color(111 / 255.0f, 100 / 255.0f, 100 / 255.0f);
            image_.sprite = Resources.Load<Sprite>(
                IsScreenNormalSize()
                ? "Materials/Cover/EndButton"
                : "Resized_Materials/Cover/EndButton");
        }
        else
        {
            // 再生ボタン
            //GetComponent<Image>().color = new Color(100/255.0f, 111 / 255.0f, 100 / 255.0f);
            image_.sprite = Resources.Load<Sprite>(
                IsScreenNormalSize()
                ? "Materials/Cover/PlayButton"
                : "Resized_Materials/Cover/PlayButton");
        }
    }

    private bool IsScreenNormalSize()
        => ProjectConstants.IsMoreThanThreshold(Screen.width)
        || ProjectConstants.IsMoreThanThreshold(Screen.height);

}
