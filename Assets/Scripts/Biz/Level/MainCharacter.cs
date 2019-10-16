using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;


public class MainCharacter : MonoBehaviour
{
    public float RotateSpeed = 30;
    public Light AbsorbHandLight;
    public Light RejectHandLight;
    public Hand absorbHand;
    public Hand rejectHand;


    protected Rigidbody2D selfRigidbody;
    private DynamicJoystick _controlJoystick;
    private GameController cachedGameController;
    public float oldAbsorbTarget;
    public float oldRejectTarget;

    public float Speed = 1.0f;

    private Tweener rejectHandTweener;
    private Tweener absorbTweener;

    private Quaternion originRot;
    private Vector3 originPos;

    public bool collideWithStar = false;
    private float timer = 0.0f;
    public bool collideWithLoveLetter = false;
    public Vector3 heartStartPos = Vector3.zero;

    private Sequence seq;
    private float angle;

    private int starTimes = 0;
    

    public float scanAngle = 0.0f;
    public float distance = 6.0f;

    public GameObject[] SkillEffects = new GameObject[4];
    public bool autoRotate = true;

    public int ControlType = 0;
    public static bool Unbeatable = false;
    public static bool _isInUnbeatable = false;
    public SpriteRenderer sprite;
    void Awake()
    {
        if (!Input.touchSupported)
            LogManager.Log("触摸不支持，使用鼠标模拟触摸");
        MessageManager.Emit("OnControllerCreated", this);
        cachedGameController = MiniCore.Get<GameController>();
        oldAbsorbTarget = AbsorbHandLight.intensity;
        oldRejectTarget = RejectHandLight.intensity;

        absorbTweener = AbsorbHandLight.DOIntensity(oldAbsorbTarget, 1.0f).SetAutoKill(false).Pause();
        rejectHandTweener = RejectHandLight.DOIntensity(oldRejectTarget, 1.0f).SetAutoKill(false).Pause();

        selfRigidbody = GetComponent<Rigidbody2D>();


        seq = DOTween.Sequence();

        angle = 0;
        seq.SetLoops(21);
        seq.AppendCallback(delegate ()
        {
            CreateEffect(angle, transform.position, false);
            angle += 18;
        });

        originRot = transform.rotation;
        originPos = transform.position;

        timer = 0.0f;

        seq.AppendInterval(0.05f).Pause().SetAutoKill(false);
        Unbeatable = false;
    }

    private void CreateEffect(float angle, Vector3 pos, bool noEnterQ)
    {
        GameObject blue = ObjectManager.CreateManagedObject("YellowProjectile");
        Vector3 dir = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
        if (noEnterQ)
        {
            dir = pos - transform.position;
        }

        dir.Normalize();
        blue.transform.position = pos;
        blue.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        {
            Vector2 dir2d = new Vector2(dir.x, dir.y);
            Vector2 origin = new Vector2(pos.x, pos.y);
            Debug.DrawLine(pos, pos + dir * 30, Color.red, 0.5f);
            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, dir2d, 30f, LayerMask.GetMask("Enemy"));
            foreach (var hit in hits)
            {
                EnemyObject obj = hit.collider.gameObject.GetComponent<EnemyObject>();
                if (obj == null)
                    continue;
                if (obj.GetType() != typeof(CandyEnemy))
                {
                    // GameObject star = null;
                    // star = ObjectManager.CreateManagedObject("StarBloom");
                    // star.transform.position = hit.point;

                    obj.CurrentState = EnemyObject.ObjectState.Obsorbed;
                    MiniCore.Get<CharacterController>().Score += 5;
                }
            }

            hits = Physics2D.RaycastAll(origin, dir2d, 30f, LayerMask.GetMask("Chalk"));
            foreach (var hit in hits)
            {
                EnemyObject obj = hit.collider.gameObject.GetComponent<EnemyObject>();
                if (obj == null)
                    continue;
                // GameObject star = null;
                // star = ObjectManager.CreateManagedObject("StarBloom");
                // star.transform.position = hit.point;

                obj.CurrentState = EnemyObject.ObjectState.Obsorbed;
                MiniCore.Get<CharacterController>().Score += 5;


            }

            hits = Physics2D.RaycastAll(origin, dir2d, 30f, LayerMask.GetMask("Pencil"));
            foreach (var hit in hits)
            {
                EnemyObject obj = hit.collider.gameObject.GetComponent<EnemyObject>();
                if (obj == null)
                    continue;
                // GameObject star = null;
                // star = ObjectManager.CreateManagedObject("StarBloom");
                // star.transform.position = hit.point;

                obj.CurrentState = EnemyObject.ObjectState.Obsorbed;
                MiniCore.Get<CharacterController>().Score += 5;


            }

            hits = Physics2D.RaycastAll(origin, dir2d, 30f, LayerMask.GetMask("PaperBall"));
            foreach (var hit in hits)
            {
                EnemyObject obj = hit.collider.gameObject.GetComponent<EnemyObject>();
                if (obj == null)
                    continue;
                // GameObject star = null;
                // star = ObjectManager.CreateManagedObject("StarBloom");
                // star.transform.position = hit.point;

                obj.CurrentState = EnemyObject.ObjectState.Obsorbed;
                MiniCore.Get<CharacterController>().Score += 5;


            }
        }

    }

    private void CreateEffect1(float angle, Vector3 pos, bool noEnterQ)
    {


    }


    void Start()
    {
        StartCoroutine(AutoRotate());
    }


    public void Reset()
    {
        transform.position = originPos;
        transform.rotation = originRot;
        RotateSpeed = 30;

        this.Get<CharacterController>().AbilityProgress = 0;

        this.Get<CharacterController>().Combo = 0;
        fingerDir = rejectHand.transform.localPosition.normalized;

        absorbHand.ResetHand();
        rejectHand.ResetHand();
        Unbeatable = false;
        _isInUnbeatable = false;
    }
    public void CreateMagicField()
    {
        if (this.Get<CharacterController>().AbilityProgress >= 100)
        {
            SwitchOffEffect();
            ObjectManager.CreateManagedObject("MagicField").transform.position = transform.position;
            this.Get<CharacterController>().AbilityProgress = 0;
            MessageManager.Emit(GlobalGameMessage.OnAbilityTrigger);
        }
    }
    public void SetBlueYellowLightEnable(bool value)
    {
        AbsorbHandLight.gameObject.SetActive(value);
        RejectHandLight.gameObject.SetActive(value);
    }

    public void SetHandLightVisible(bool enable, float duration)
    {
        float absorbTarget = 0;
        float rejectTarget = 0;
        if (enable)
        {
            absorbTarget = oldAbsorbTarget;
            rejectTarget = oldRejectTarget;

        }
        if (duration < 0)
        {
            RejectHandLight.intensity = rejectTarget;
            AbsorbHandLight.intensity = absorbTarget;
            return;
        }
        rejectHandTweener.ChangeStartValue(RejectHandLight.intensity, -1);
        absorbTweener.ChangeStartValue(AbsorbHandLight.intensity, -1);

        rejectHandTweener.ChangeEndValue(rejectTarget, duration);
        absorbTweener.ChangeEndValue(absorbTarget, duration);

        rejectHandTweener.Play();
        absorbTweener.Play();
    }

    #region 输入相关

    private Vector3 oldMousePosition;

    Vector2 deltaMove = Vector2.zero;
    public float TargetAngle;
    public Vector2 fingerDir;

    private void HandleInput()
    {

        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.OSXEditor:
                if (Input.GetMouseButtonDown(0))
                {
                    autoRotate = false;
                    tap = true;
                    tapTime = 0f;
                    oldMousePosition = Input.mousePosition;
                }
                if (Input.GetMouseButton(0))
                {
                    deltaMove = Input.mousePosition - oldMousePosition;
                    deltaMove.x = Mathf.Clamp(deltaMove.x, -100, 100);
                    oldMousePosition = Input.mousePosition;

                    deltaMove.x = Mathf.Clamp(deltaMove.x, -100, 100);
                    Speed = Mathf.Abs(deltaMove.x) / 100f;

                    Vector3 screen = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
                    Vector3 pos = Camera.main.ScreenToWorldPoint(screen);
                    fingerDir = pos - transform.position;
                    fingerDir.Normalize();

                }
                if (Input.GetMouseButtonUp(0))
                {
                    tap = false;
                    if (tapTime < 0.2f)
                    {
                        Vector3 screen = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
                        Vector3 pos = Camera.main.ScreenToWorldPoint(screen);
                        Rect r = new Rect();
                        r.position = new Vector2(transform.position.x - 1f, transform.position.y - 1f);
                        r.width = 2f;
                        r.height = 2f;
                        if (r.Contains(pos))
                        {
                            CreateMagicField();
                        }
                        deltaMove = Vector2.zero;
                    }
                }


                break;
            case RuntimePlatform.IPhonePlayer:
            case RuntimePlatform.Android:

             
                if (Input.touches.Length > 0)
                {
                    if (Input.touches[0].phase == TouchPhase.Began)
                    {
                        tap = true;
                        tapTime = 0f;
                        autoRotate = false;
                    }

                    if (Input.touches[0].phase == TouchPhase.Moved)
                    {
                        deltaMove = Input.touches[0].deltaPosition;
                        deltaMove.x = Mathf.Clamp(deltaMove.x, -100, 100);
                        Speed = Mathf.Abs(deltaMove.x) / 100f;

                        Vector3 screen = new Vector3(Input.touches[0].position.x, Input.touches[0].position.y, 10);
                        Vector3 pos = Camera.main.ScreenToWorldPoint(screen);
                        fingerDir = pos - transform.position;
                        fingerDir.Normalize();
                        BlueDir = rejectHand.transform.localPosition.normalized;
                        TargetAngle = Vector2.SignedAngle(BlueDir, fingerDir);
                    }
                    if (Input.touches[0].phase == TouchPhase.Ended)
                    {
                        tap = false;
                        if (tapTime < 0.2f)
                        {
                            Vector3 screen = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
                            Vector3 pos = Camera.main.ScreenToWorldPoint(screen);
                            Rect r = new Rect();
                            r.position = new Vector2(transform.position.x - 1f, transform.position.y -1f);
                            r.width = 2f;
                            r.height = 2f;
                            if (r.Contains(pos))
                            {
                                CreateMagicField();
                            }
                        }
                    }
                }
             
                break;
        }
    }

    private void ConsumeInput()
    {
        switch (ControlType)
        {
            case 0:
                if (deltaMove.x != 0.0f)
                {
                    selfRigidbody.MoveRotation(selfRigidbody.rotation + deltaMove.x * Time.fixedDeltaTime * RotateSpeed);
                    deltaMove.x = 0;
                }
                break;
            case 1:
                if (fingerDir != Vector2.zero)
                {
                    BlueDir = rejectHand.transform.localPosition.normalized;
                    TargetAngle = Vector2.SignedAngle(BlueDir, fingerDir);
                    selfRigidbody.MoveRotation(TargetAngle);
                }
                break;
        }

    }

    #endregion

    void Update()
    {

        if (cachedGameController.IsPaused)
            return;

        if (tap)
            tapTime += Time.deltaTime;

        HandleInput();

    }

    private float tapTime = 0.0f;
    private bool tap = false;
    public Vector2 BlueDir;
    void StarEffect(Vector3 pos)
    {


        double x = System.Math.Abs(pos.x);
        double y = System.Math.Abs(pos.y);

        float angle = (float)System.Math.Atan2(y, x);
        if (pos.x < 0 && pos.y > 0)
        {
            angle = 180 - angle;
        }
        if (pos.x < 0 && pos.y < 0)
        {
            angle = 180 + angle;
        }
        if (pos.x > 0 && pos.y < 0)
        {
            angle = 360 - angle;
        }

        CreateEffect1(angle, pos, true);


    }

    private float curTime = 0.0f;
    void FixedUpdate()
    {
        if (cachedGameController.IsPaused)
            return;
        if (Unbeatable && !_isInUnbeatable)
        {
            _isInUnbeatable = true;
            StartCoroutine(UnBeatableEffect());
        }
        ConsumeInput();
    }
    

    void OnCollisionEnter2D(Collision2D col)
    {
        EnemyObject obj = col.gameObject.GetComponent<EnemyObject>();
        if (obj != null)
        {
            obj.OnCollideWithBody(this, col);
        }
    }
    public void SwitchOnEffect()//技能值满时打开特效
    {
        int i = 0;
        while (i < 4)
        {
            SkillEffects[i].SetActive(true);
            i++;
        }
    }
    public void SwitchOffEffect()//关闭特效
    {
        int i = 0;
        while (i < 4)
        {
            SkillEffects[i].SetActive(false);
            i++;
        }
    }
    IEnumerator AutoRotate()
    {
        float i = 0; ;
        while (autoRotate)
        {
            i += 0.001f;
            this.transform.localRotation = Quaternion.EulerAngles(0, 0, i % 180 * (float)System.Math.PI);
            yield return new WaitForSeconds(0.016f);
        }
    }

    IEnumerator UnBeatableEffect()
    {
        //Debug.Log("EffectCalled");
        Color color = Color.white;

        /*
        color.a = 0.5f;
        sprite.color = color;
        yield return new WaitForSeconds(1.0f);
        color.a = 1f;
        sprite.color = color;
        */

  
        float a = 0f;
        while (a < 0.7f)
        {
            a += 0.045f;
            color.a = a;
            sprite.color = color;
            yield return new WaitForSeconds(0.016f);
        }
        while (a > 0f)
        {
            a -= 0.045f;
            color.a = a;
            sprite.color = color;
            yield return new WaitForSeconds(0.016f);
        }
        while (a < 0.7f)
        {
            a += 0.045f;
            color.a = a;
            sprite.color = color;
            yield return new WaitForSeconds(0.016f);
        }
        while (a > 0f)
        {
            a -= 0.045f;
            color.a = a;
            sprite.color = color;
            yield return new WaitForSeconds(0.016f);
        }
 
        Unbeatable = false;
        _isInUnbeatable = false;
    }
}
