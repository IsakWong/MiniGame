using System;
using UnityEngine;
using System.Collections;
using System.Text;


/// <summary>
/// TODO 陈立信
/// 
///  分级Debug Log + Log 持久化
/// 
/// </summary>
public static class LogManager 
{
    static StringBuilder sb = new StringBuilder();

    [Flags]
    public enum LogMask
    {
        None = 0,
        Debug = 1,
        Collison = 2,
        Message = 4,
    }

    public static LogMask LogLevel = LogMask.Debug | LogMask.Collison | LogMask.Message;



    public static void LogError(string message)
    {
        DateTime dt = System.DateTime.Now;
        sb.AppendFormat("[{0}]:{1}", dt.ToString(), message);
        Debug.LogError(sb.ToString());
        sb.Clear();
    }
    
    public static void LogErrorFormat(string message)
    {
        DateTime dt = DateTime.Now;
        sb.AppendFormat("[{0}]:{1}", dt.ToString(), message);
        Debug.LogError(sb.ToString());
        sb.Clear();
    }
    public static void Log(string message)
    {
        DateTime dt = DateTime.Now;
        sb.AppendFormat("[{0}]:{1}", dt.ToString(), message);
        Debug.Log(sb.ToString());
        sb.Clear();
    }
}
