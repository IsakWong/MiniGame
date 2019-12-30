
using Mini.Core;
using UnityEngine;

public class CharacterController : BaseController
{


    public Light spotLight;
    private float _ablilityProgress = 0.0f;

    public int _combo = 0;
    public int Combo
    {
        get { return _combo; }
        set
        {
            MessageManager.Emit<int, int>(GlobalGameMessage.OnComboChange, _combo, value);
            _combo = value;
        }
    }
    public float AbilityProgress
    {
        get { return _ablilityProgress; }
        set
        {
            if (value >=100)
            {
                value = 100;
                GameObject.FindObjectOfType<MainCharacter>().SwitchOnEffect();
            }
            MessageManager.Emit<float, float>(GlobalGameMessage.OnAbilityProgressChange, _ablilityProgress, value);
            _ablilityProgress = value;
        }
    }

    private float _score = 0.0f;
    public float Score
    {
        get { return _score; }
        set
        {
            //Handheld.Vibrate();
            MessageManager.Emit<float,float>(GlobalGameMessage.OnScoreChange,_score,value);
            _score = value;
        }
    }

    private int _currentHealth = 3;
    public int CurrentHealth
    {
        get { return _currentHealth; }
        set
        {
            if (value < 0)
            {
                return;
            }

            if (value < _currentHealth)
            {
                if (MainCharacter.Unbeatable) return;
                MainCharacter.Unbeatable = true;
                MiniCore.PlaySound("扣血音效");
                Combo = 0;
                
            }
            MessageManager.Emit<int, int>(GlobalGameMessage.OnHealthChange, _currentHealth, value);
            _currentHealth = value;
            if (_currentHealth == 0)
                MessageManager.Emit(GlobalGameMessage.OnLevelOver);

        }
    }

  

  

}
