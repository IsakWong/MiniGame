using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level1EnemyList", menuName = "配置表/关卡敌人生成表", order =2)]
[System.Serializable]
public class LevelEnemyListConfig : ScriptableObject
{
    //asset name / asset bundle
    public List<EnemyGenerateInfo> LevelEnemyList = new List<EnemyGenerateInfo>(100);

    [NonSerialized]
    public int Index = 0;
    [NonSerialized]
    private static List<EnemyGenerateInfo> result = new List<EnemyGenerateInfo>(100);

    public void ResetIndex()
    {
        Index = 0;
    }
    public List<EnemyGenerateInfo> GetEnemyByTime(float generateTime)
    {
        result.Clear();
        for (int i = Index; i < LevelEnemyList.Count; ++i)
        {
            if (LevelEnemyList[i].GenerateTime < generateTime   + Time.fixedDeltaTime)
            {
                result.Add(LevelEnemyList[i]);
                Index++;
            }
            else
            {
                break;
            }

        }
        return result;
    }
}


[System.Serializable]
public enum MoveType
{
    Line = 0,
    Curve = 1,
    Specific = 2,
    Unknown,
}

[System.Serializable]
public class EnemyGenerateInfo
{
    public EnemyType Type = EnemyType.Bandage;
    public MoveType EnemyMoveType = MoveType.Line;
    public float GenerateTime = 0;
    public float Angle = 0;
    public float AngleVelocity = 0;
    public float VelocityDirectionAngle = 0;
    public float Velocity = 5;
    public float Drag = 0;
    public float AngleDrag = 0;

}


