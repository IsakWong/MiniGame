using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LevelVictoryView : BaseView
{
    // Start is called before the first frame update
    
    public WheelWidget Wheel;
    public Text Tip;
    public Watch watch;
    protected new void Start()
    {
        base.Start();
        var overlay = ViewManager.GetView<LevelOverlayView>(false);
        if (overlay != null)
        {
            overlay.PlayOutAnim(delegate()
            {
                overlay.CloseSelf();
            });
        }
        Wheel.OnTrigger = delegate()
        {
            MiniCore.Get<GameController>().NextWorld();
            PlayOutAnim(delegate()
            {
                CloseSelf();
            });
        };
        
    }
    
}
