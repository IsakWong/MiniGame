﻿
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Command
{
    public bool IsExcuting = false;
    public bool IsFinished = false;
    // Start is called before the first frame update
    public virtual void Execute()
    {
        IsExcuting = true;
    }
    public virtual void Finish()
    {
        IsExcuting = false;
        IsFinished = true;
    }
    public virtual void Executing()
    {

    }
}

public class ShowViewCommand<T> : Command where T : BaseView
{
    private bool _isCreate;
    public ShowViewCommand(bool create)
    {
        _isCreate = create;
    }

    public override void Execute()
    {
        base.Execute();
        ViewManager.GetView<T>(_isCreate);
        Finish();
    }
}
public class LoadAssetCommand : Command
{
    private string _assetName;
    private float _loadTime = 0;
    private Func<bool, float,bool> _loadingCallback;
    private bool _isLoaded = false;
    
    public LoadAssetCommand(string assetName, Func<bool, float,bool> loadingCallback)
    {
        _assetName = assetName;
        _loadingCallback = loadingCallback;
    }

    public override void Executing()
    {
        if (_isLoaded)
        {
            OnLoadingAsset(_isLoaded, 1.0f);
        }
    }
    public override void Execute()  
    {
        base.Execute();
        _loadTime = Time.realtimeSinceStartup;
        if (_isLoaded)
        {

        }
        else
        {
            AssetManager.Instance.LoadAssetAsync(_assetName, OnLoadingAsset);
        }
        
    }
    public override void Finish()
    {
        base.Finish();
        _loadTime = Time.realtimeSinceStartup - _loadTime;
        LogManager.Log(string.Format("Asset：<color=#ff0000ff>{0}</color> Load Time：<color=#ff0000ff>{1}</color>", this._assetName,_loadTime));
    }

    private void OnLoadingAsset(bool isFinished, float progress)
    {
        _isLoaded = isFinished;
        if (_loadingCallback != null)
        {
            if (_loadingCallback.Invoke(isFinished, progress) && _isLoaded)
            {
                Finish();
            }
        }
        else
        {
            if (_isLoaded)
                Finish();
        }
        
    }
}
