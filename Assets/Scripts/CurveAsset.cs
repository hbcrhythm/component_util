using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CurveAsset")]
public class CurveAsset : ScriptableObject
{
    [SerializeField]
    public AnimationCurve[] curves;
}