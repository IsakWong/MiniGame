using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SettingsConfig", menuName = "ScriptableObjects/Settings Config", order = 1)]
public class SettingsConfig : ScriptableObject
{
    //asset name / asset bundle
    public bool FullScreen = false;
    public int ScreenWidth = 1080;
    public int ScreenHeight = 1920;
    public int TargetFrame = 60;


}


