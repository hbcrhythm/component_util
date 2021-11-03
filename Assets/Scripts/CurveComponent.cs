using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveComponent : MonoBehaviour
{
    public CurveAsset curve;

    public Transform wall;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public static Vector3[] CurveToZYPath(AnimationCurve curve, Vector3 start, Vector3 offset, Quaternion rotation, int size)
    {
        var path = new Vector3[size];
        for (int i = 0; i < size; i++)
        {
            float t = (float)i / (size - 1);
            path[i] = CurveZYLerp(curve, start, offset, rotation, t);
        }
        return path;
    }
    public static Vector3 CurveZYLerp(AnimationCurve curve, Vector3 start, Vector3 offset, Quaternion rotation, float t)
    {
        var point = start;
        point.z += t * offset.z;
        point.y += curve.Evaluate(t) * offset.y;
        point = start + rotation * (point - start);
        return point;
    }

    private void OnDrawGizmos()
    {
        //Vector3 jumpOffset = new Vector3(0f, 3f, 7f);
        Vector3 offset = new Vector3(0f, 5f - Vector3.Distance(wall.position, transform.position) * 0.5f , 3 * Vector3.Distance(wall.position, transform.position));
        var path = CurveToZYPath(curve.curves[0], transform.position, offset, transform.rotation, 10);
        DrawLine(path, Color.white, 0.1f);

    }


    public static void DrawLine(Vector3[] points, Color color, float pointSize = 0)
    {
        if (points == null || points.Length < 2) return;
        Gizmos.color = color;
        for (int i = 1; i < points.Length; i++)
        {
            DrawArrow(points[i - 1], points[i], color);
            if (pointSize > 0f) Gizmos.DrawSphere(points[i], pointSize);
        }
    }

    public static void DrawArrow(Vector3 from, Vector3 to, Color color, float arrow = 0.5f)
    {
        Gizmos.color = color;
        Gizmos.DrawLine(from, to);
        Gizmos.DrawLine(to, to + RotateAxis(from - to, Vector3.up, 20f).normalized * arrow);
        Gizmos.DrawLine(to, to + RotateAxis(from - to, Vector3.up, -20f).normalized * arrow);
    }


    public static Vector3 RotateAxis(Vector3 source, Vector3 axis, float angle)
    {
        Quaternion q = Quaternion.AngleAxis(angle, axis);// 旋转系数
        return q * source;// 返回目标点
    }

}
