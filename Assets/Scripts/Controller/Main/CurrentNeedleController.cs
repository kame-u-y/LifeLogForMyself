using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentNeedleController : MonoBehaviour
{
    //[SerializeField]
    //private bool isClock12h = false;
    GameDirector gameDirector;

    private void Awake()
    {
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DrawCurrentNeedle();
    }

    /// <summary>
    /// Œ»İ‚ğE•ª‚ğ‚à‚Æ‚É•\¦
    /// 12h‚Æ24h‚ğisClock12h‚ÅØ‚è‘Ö‚¦
    /// </summary>
    void DrawCurrentNeedle()
    {
        DateTime dt = DateTime.Now;
        float angle = (gameDirector.isClock12h ? 2 : 1) * 360.0f * (dt.Hour * 60.0f + dt.Minute) / (24.0f * 60.0f);
        this.transform.rotation = Quaternion.Euler(0, 0, -angle);
    }
}
