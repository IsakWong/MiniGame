using System.Collections;
using System.Collections.Generic;
using Mini.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LevelDefeatView : BaseView
{
    // Start is called before the first frame update
    
    public Button Restart;
    public Button BacktoMenu;

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
        Restart.onClick.AddListener(delegate()
        {
            MiniCore.Get<GameController>().Restart();
            PlayOutAnim(delegate ()
            {
                CloseSelf();
            });
        });
        BacktoMenu.onClick.AddListener(delegate ()
        {
            MiniCore.Get<GameController>().BackToMenu();
            PlayOutAnim(delegate ()
            {
                CloseSelf();
            });
        });
    }
    
}
