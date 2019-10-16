using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class SuccessController : MonoBehaviour
{
    public Button BackToMenu;
    WorldLevel6 WorldLevel6;
    // Start is called before the first frame update
    private void Start()
    {
        WorldLevel6 = GameObject.FindGameObjectWithTag("WorldLevel").GetComponent<WorldLevel6>();
        
        BackToMenu.onClick.AddListener(delegate ()
        {
            WorldLevel6.Win();
            /*
            MiniCore.Get<GameController>().CurrentWorld.ResetWorld();
            MiniCore.Get<GameController>().BackToMenu();
            MiniCore.Get<GameController>().Restore();
            */

        });
    }
}
