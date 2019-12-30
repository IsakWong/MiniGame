using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mini.Core;
using UnityEngine;

public class WorldLevel6 : BaseWorld
{
    // Start is called before the first frame update



    protected int soundCounter = 0;
    public BarController ctrlBar;
    
    GameObject success;
    protected void Awake()
    {
        base.Awake();
        ChangeControlMethod();

        MiniCore.ChangeBgm("3-1");

        WorldSequence.AppendCallback(delegate ()
        {
            //ChangeEnvironmentLight(WorldEnvColor, 1.0f);
            var LevelOverlayView = ViewManager.GetView<LevelOverlayView>(true);
            LevelOverlayView.PlayOutAnim(delegate ()
            {
                LevelOverlayView.CloseSelf();
            });
            Success();
        });

        //WorldSequence.Append(spotLight.DOIntensity(oldIntensity, 1.0f));
        WorldSequence.Pause();

    }

    protected void Start()
    {
        base.Start();
        //MainSpotLight.intensity = 0;
        Main.SetHandLightVisible(false, -1f);
    }
    private void ChangeControlMethod()
    {
        Main.RotateSpeed = 0;
    }

    public override void Win()
    {
        Destroy(ctrlBar.gameObject);
        Destroy(success.gameObject);
        base.Win();
    }
    public void Success()
    {
        GameObject maincanvas = GameObject.FindGameObjectWithTag("Canvas");
        GameObject wordsPrefab = AssetManager.LoadGameObject("Success");
        success = GameObject.Instantiate(wordsPrefab);

        success.transform.SetParent(maincanvas.transform);
        Vector3 pos = new Vector3(0, 0, 0);
        success.transform.localPosition = pos;
    }

}
