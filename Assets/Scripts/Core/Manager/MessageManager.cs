using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MessageManager
{

    public static Dictionary<string, Delegate> Senders = new Dictionary<string, Delegate>();

    public static void AddListener(this MonoBehaviour behaviour, string message, Action d)
    {
        if (!Senders.ContainsKey(message))
            Senders.Add(message, d);
        else
            Senders[message] = (Action)Senders[message] + (Action)d;
    }

    public static void RemoveListener(this MonoBehaviour behaviour, string message, Action d)
    {
        if (!Senders.ContainsKey(message))
            return;
        Senders[message] = (Action)Senders[message] - d;
    }

    public static void AddListener<T>(this MonoBehaviour behaviour, string message, Action<T> d)
    {
        if (!Senders.ContainsKey(message))
            Senders.Add(message, d);
        else
            Senders[message] = (Action<T>)Senders[message] + (Action<T>)d;
    }

    public static void RemoveListener<T>(this MonoBehaviour behaviour, string message, Action<T> d)
    {
        if (!Senders.ContainsKey(message))
            return;
        Senders[message] = (Action<T>)Senders[message] - d;
    }

    public static void AddListener<T1, T2>(this MonoBehaviour behaviour, string message, Action<T1, T2> d)
    {
        if (!Senders.ContainsKey(message))
            Senders.Add(message, d);
        else
            Senders[message] = (Action<T1, T2>)Senders[message] + d;
    }

    public static void RemoveListener<T1, T2>(this MonoBehaviour behaviour, string message, Action<T1, T2> d)
    {
        if (!Senders.ContainsKey(message))
            return;
        Senders[message] = (Action<T1, T2>)Senders[message] - d;
    }

    public static void AddListener<T1, T2, T3>(this MonoBehaviour behaviour, string message, Action<T1, T2, T3> d)
    {
        if (!Senders.ContainsKey(message))
            Senders.Add(message, d);
        else
            Senders[message] = (Action<T1, T2, T3>)Senders[message] + (Action<T1, T2, T3>)d;
    }
    public static void RemoveListener<T1, T2, T3>(this MonoBehaviour behaviour, string message, Action<T1, T2, T3> d)
    {
        if (!Senders.ContainsKey(message))
            return;
        Senders[message] = (Action<T1, T2, T3>)Senders[message] - d;
    }

    public static void Emit(string name)
    {
        if (!Senders.ContainsKey(name))
            return;
        Action d = (Action)Senders[name];
        LogManager.Log("Emit Message: " + name);
        if (d != null)
            d.Invoke();
    }
    public static void Emit<T>(string name, T data)
    {
        if (!Senders.ContainsKey(name))
            return;
        Action<T> d = (Action<T>)Senders[name];
        LogManager.Log("Emit Message: " + name);
        if (d != null)
            d.Invoke(data);
    }
    public static void Emit<T1, T2>(string name, T1 data, T2 data2)
    {
        if (!Senders.ContainsKey(name))
            return;
        Action<T1, T2> d = (Action<T1, T2>)Senders[name];
        LogManager.Log("Emit Message: " + name);
        if (d != null)
            d.Invoke(data, data2);
    }

}

public static class GlobalGameMessage
{
    public const string OnLevelBegin = "LevelBegin";
    public const string OnMainCharacterAwake = "MainCharacterAwake";
    public const string OnScoreChange = "ScoreChange";
    public const string OnAbilityProgressChange = "AbilityProgressChange";
    public const string OnComboChange = "ComboChange";
    public const string OnHealthChange = "HealthChange";
    public const string OnLevelOver = "LevelOver";
    public const string OnAbilityTrigger = "AbilityTrigger";
}