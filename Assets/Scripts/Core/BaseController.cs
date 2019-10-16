using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using Object = System.Object;

public class BaseController
{
    public BaseView DisplayView = null;
    public bool EnableFixedUpdate = false;

    private static StringBuilder sb = new StringBuilder(200);
    public override string ToString()
    {
        Type tt = GetType();

        BindingFlags flag = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;
        FieldInfo[] fields = tt.GetFields(flag);
        sb.AppendLine(tt.Name);
        foreach (var field in fields)
        {
            sb.Append(field.Name);
            sb.Append(" : ");
            sb.Append(field.GetValue(this));
            sb.AppendLine();
        }
        string result = sb.ToString();
        sb.Clear();
        return result;
    }

    public static bool DelegateToSearchCriteria(MemberInfo objMemberInfo, Object objSearch)
    {
        if (objMemberInfo.Name.ToString() == objSearch.ToString())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public BaseController()
    {

    }

    public virtual void FixedUpdate()
    {

    }

    public virtual void Update()
    {

    }

    public virtual void Init()  
    {

    }
}
