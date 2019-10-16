using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Excel;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using FileMode = System.IO.FileMode;

public class DesignerMenuEditor : Editor
{
    static void Read(string path, ref DataSet result)
    {
        FileStream stream = File.Open("Assets/Excels/" + path, FileMode.Open, FileAccess.Read);
        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        result = excelReader.AsDataSet();
    }


    public static int CurrentRow = 0;
    public static int CurrentCol = 0;
    public static DataTable CurrenTable = null;
    public static bool TryToFloat(int j, out float result)
    {
        CurrentCol = j;
        return float.TryParse(CurrenTable.Rows[CurrentRow][j].ToString(), out result);
    }
    public static string TryToStr(int j)
    {
        CurrentCol = j;
        return CurrenTable.Rows[CurrentRow][j].ToString();
    }
    public static bool TryToInt(int j, out int result)
    {
        CurrentCol = j;
        return int.TryParse(CurrenTable.Rows[CurrentRow][j].ToString(), out result);
    }





    public static T GetOrCreateConifg<T>(string path) where T : ScriptableObject
    {
        T t = AssetDatabase.LoadAssetAtPath<T>(path);
        if (t == null)
        {
            t = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(t, path);
        }
        return t;
    }

    public static void ReadEnemyType()
    {
        string path = "Assets/Resources/Generated/EnemyTypeList.asset";
        EnemyTypeConfig config = GetOrCreateConifg<EnemyTypeConfig>(path);

        DataSet dataset = new DataSet();
        Read("物体类型表.xlsx", ref dataset);
        CurrenTable = dataset.Tables[0];

        config.EnemyTypeList.Clear();

        for (CurrentRow = 1; CurrentRow < CurrenTable.Rows.Count; ++CurrentRow)
        {
            EnemyInfo info = new EnemyInfo();
            if (!TryToInt(0, out int type))
            {
                throw new Exception();
            }
            info.Type = (EnemyType)type;
            info.Name = TryToStr(1);
            info.PrefabName = TryToStr(2);
            config.EnemyTypeList.Add(info);
        }


        EditorUtility.SetDirty(config);
        Debug.LogFormat("成功读取到物体类型表的{0}行{1}列，请核对读取是否正确", CurrentRow, CurrentCol);

    }
    public static void ReadLevelEnemyExcel(string targetExcel, string targetConfig)
    {

        string path = "Assets/Resources/Generated/" + targetConfig + ".asset";
        LevelEnemyListConfig config = GetOrCreateConifg<LevelEnemyListConfig>(path);

        DataSet dataset = new DataSet();
        Read(targetExcel + ".xlsx", ref dataset);

        CurrenTable = dataset.Tables[0];

        config.LevelEnemyList.Clear();
        for (CurrentRow = 1; CurrentRow < CurrenTable.Rows.Count - 1; ++CurrentRow)
        {
            EnemyGenerateInfo info = new EnemyGenerateInfo();

            if (!TryToInt(1, out int type))
                break;
            info.Type = (EnemyType) type;

            if (!TryToFloat(2, out info.GenerateTime))
                break;
            if (!TryToFloat(3, out info.Angle))
                break;
            if (!TryToFloat(4, out info.Velocity))
                break;
            if (!TryToFloat(5, out info.VelocityDirectionAngle))
                break;
            if (info.VelocityDirectionAngle < 0)
            {
                info.VelocityDirectionAngle = 180 + info.Angle;
            }

            if (!TryToFloat(8, out info.AngleVelocity))
                break;
            if (!TryToInt(10, out int moveType))
                break;
            info.EnemyMoveType = (MoveType)moveType;
            config.LevelEnemyList.Add(info);
        }
        EditorUtility.SetDirty(config);
        Debug.LogFormat("成功读取到表{0}的{1}行{2}列，请核对读取是否正确", targetExcel, CurrentRow, CurrentCol);

    }
    [MenuItem("策划工具/读表生成游戏配置")]
    public static void ReadExcel()
    {
        ReadEnemyType();
        ReadLevelEnemyExcel("关卡1世界表", "Level1EnemyList");
        ReadLevelEnemyExcel("关卡2世界表", "Level2EnemyList");
        ReadLevelEnemyExcel("关卡3世界表", "Level3EnemyList");
        ReadLevelEnemyExcel("关卡4世界表", "Level4EnemyList");
        ReadLevelEnemyExcel("关卡5世界表", "Level5EnemyList");
        AssetDatabase.SaveAssets();
        Debug.LogFormat("写配置成功");

    }


}
