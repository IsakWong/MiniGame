using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Mini.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DebugView : BaseView
{

    public GridLayoutGroup ButtonGroup;
    public GameObject ButtonPrefab;
    public Text DebugText;
    public UnityAction action;

    private static StringBuilder _cachedLog = new StringBuilder(1000);

    public static void CacheLog(string log)
    {
        _cachedLog.AppendLine(log);
    }   

    void OnEnable()
    {
    }
    void OnDisable()
    {
    }

    private void Update()
    {
        if (_cachedLog.Length == 0)
        {
            return;
        }
        else
        {
            DebugText.text = DebugText.text + _cachedLog.ToString();
            _cachedLog.Clear();
        }
        
    }
    protected void Start()
    {
        AddButton("打印资源缓存", delegate ()
        {


        });
        AddButton("第一关", delegate ()
        {
            CloseSelf();
            this.Get<GameController>().LoadWorld(1);
        });

        AddButton("第二关", delegate ()
        {
            CloseSelf();
            this.Get<GameController>().LoadWorld(2);
        });

        AddButton("第三关", delegate ()
        {
            CloseSelf();
            this.Get<GameController>().LoadWorld(3);
        });

        AddButton("Boss受击", delegate ()
        {
            CloseSelf();

            foreach (var enemy in this.Get<GameController>().CurrentWorld.Enemies)
            {
                if (enemy.GetType() == typeof(FunnyFaceBoss))
                {
                    FunnyFaceBoss boss = enemy as FunnyFaceBoss;
                    boss.TakeHit();
                }
                
            }

            

        });
        AddButton("强制关卡失败", delegate ()
        {
            CloseSelf(); this.Get<GameController>().Defeat();
        });
        AddButton("下一关", delegate ()
        {
            CloseSelf();
            this.Get<GameController>().NextWorld();
        });
        AddButton("快速胜利", delegate ()
        {
            CloseSelf();
            MiniCore.Get<GameController>().Win();
        });
        AddButton("打印 Game Controller", delegate () { Debug.Log(this.Get<GameController>()); });
        AddButton("打印 Character Controller", delegate () { Debug.Log(this.Get<CharacterController>()); });

    }

    void AddButton(string buttonText, UnityAction action)
    {
        GameObject gameObject = GameObject.Instantiate(ButtonPrefab, ButtonGroup.transform);
        gameObject.GetComponentInChildren<Text>().text = buttonText;
        gameObject.GetComponent<Button>().onClick.AddListener(action);
        gameObject.SetActive(true);
    }

}
