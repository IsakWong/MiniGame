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
///

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

    /// <summary>
    /// color 转换hex
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static string ColorToHex(Color color)
    {
        int r = Mathf.RoundToInt(color.r * 255.0f);
        int g = Mathf.RoundToInt(color.g * 255.0f);
        int b = Mathf.RoundToInt(color.b * 255.0f);
        int a = Mathf.RoundToInt(color.a * 255.0f);
        string hex = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", r, g, b, a);
        return hex;
    }

    /// <summary>
    /// hex转换到color
    /// </summary>
    /// <param name="hex"></param>
    /// <returns></returns>
    public static Color HexToColor(string hex)
    {
        byte br = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte bg = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte bb = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        byte cc = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        float r = br / 255f;
        float g = bg / 255f;
        float b = bb / 255f;
        float a = cc / 255f;
        return new Color(r, g, b, a);
    }

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
    public static void Log(string message, Color color)
    {
        DateTime dt = DateTime.Now;
        sb.AppendFormat("<color={0}>[{1}]:{2}</color>", ColorToHex(color), dt.ToString(), message);
        Debug.Log(sb.ToString());
        sb.Clear();
    }
}
