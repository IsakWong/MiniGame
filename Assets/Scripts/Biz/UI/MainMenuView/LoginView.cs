using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mini.Core;
using UnityEngine;
using UnityEngine.UI;

public class LoginView : BaseView
{
    public RectTransform Panel;
    public Button Login;
    public Button Play;
    public Text usre;
    public InputField field;
    public Text pwd;
    public Button LoginBtn;
    public Button RegBtn;
    public Text Tip;
    public override void OnPlayInAnimation()
    {
        base.OnPlayInAnimation();
    }

    public override void OnPlayOutAnimation()
    {
        base.OnPlayOutAnimation();
    }

    protected  void Awake()
    {
        base.Awake();
        this.Get<GameController>().InitGame();
        RegBtn.onClick.AddListener(delegate()
        {
            var users = MiniCore.GetConfig<UserConfig>().Users;
            bool has = false;
            foreach (var user in users)
            {
                if (usre.text == user.Name )
                {
                    has = true;
                 
                }

            }

            if (!has)
            {
                User u = new User();
                u.Password = pwd.text;
                u.Name = usre.text;
                users.Add(u);
                Tip.gameObject.SetActive(true);
                Tip.text = "注册成功";

            }
        });
        LoginBtn.onClick.AddListener(delegate()
        {
            var users = MiniCore.GetConfig<UserConfig>().Users;
            foreach (var user in users)
            {
                if (usre.text == user.Name && user.Password == field.text)
                {

                    PlayOutAnim(delegate ()
                    {
                        var story = ViewManager.GetView<Story0>();
                        story.PlayInAnim();
                        CloseSelf();
                    });
                    break;
                    
                }
            }
            Tip.gameObject.SetActive(true);

        });
        Play.onClick.AddListener(delegate()
        {
            PlayOutAnim(delegate ()
            {
                
                var story = ViewManager.GetView<Story0>();
                story.PlayInAnim();
                CloseSelf();
            });
        });
        Login.onClick.AddListener(delegate()
        {
            Panel.DOAnchorPosY(300, 1.0f).SetEase(Ease.OutBack);
            
        });
    }

}
