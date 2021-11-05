using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleComponent
{ 

    /// <summary>
    /// ʱ������
    /// </summary>
    /// <param name="on">�Ƿ���</param>
    /// <param name="slowMotionTime">���ź��ʱ��ֵ</param>
    public void SetTimeScale(bool on, float slowMotionTime) 
    {
        float time = on ? slowMotionTime : 1;
        Time.timeScale = time;
    }

}
