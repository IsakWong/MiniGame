using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName = "UserConfig", menuName = "配置表/用户", order = 1)]
public class UserConfig : ScriptableObject
{
    [SerializeField]
    public List<User> Users = new List<User>();

}

[System.Serializable]
public class User
{
    public string Name;
    public string Password;
}