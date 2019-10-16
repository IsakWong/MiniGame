using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName = "GuideConfig", menuName = "配置表/新手引导表", order = 1)]
public class GuideConfig : ScriptableObject
{
    [SerializeField]
    public List<bool> IsGuided = new List<bool>();

}