using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleComponent
{ 

    /// <summary>
    /// 时间缩放
    /// </summary>
    /// <param name="on">是否开启</param>
    /// <param name="slowMotionTime">缩放后的时间值</param>
    public void SetTimeScale(bool on, float slowMotionTime) 
    {
        float time = on ? slowMotionTime : 1;
        Time.timeScale = time;
    }

}
