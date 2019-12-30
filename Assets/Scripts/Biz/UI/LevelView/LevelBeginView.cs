using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mini.Core;
using UnityEngine;
using UnityEngine.UI;

public class LevelBeginView : BaseView
{

    public RawImage Title;

    public Texture2D[] textures;

    protected void OnEnable()
    {
        Title.texture = textures[MiniCore.Get<GameController>().WorldIndex];
    }
    // Start is called before the first frame update
    protected new void Awake()
    {
       base.Awake();

    }


}
