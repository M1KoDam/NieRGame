using System;
using UnityEngine;

public class BigFlyerTop: SmallFlyerTop
{
    private int _maxHp;
    private int _stageTime;
    [SerializeField] private Transform[] supportingGuns;
    [SerializeField] private Transform[] supportingGunsStart;
    
    protected override IState State
        => hp <= 0
            ? new DeadState()
            : OnFlyScene
                ? new AttackState()
                : new GoToSceneState();
    

    protected override void Start()
    {
        base.Start();
        _maxHp = hp;
        _stageTime = 0;
    }

    public override void Attack()
    {
        switch ((double)hp / _maxHp)
        {
            case > 0.90:
                attackRate = 3;
                Stage1();
                break;
            case > 0.80:
                attackRate = 2f;
                Stage2();
                break;
            case > 0.5:
                attackRate = 0.5f;
                Stage3();
                break;
            case > 0.1:
                attackRate = 0.25f;
                Stage4();
                break;
            default:
                Stage5();
                break;
        }
    }

    private void Stage1()
    {
        Debug.Log($"первая стадия, {hp}");
        RushAttack();
    }

    private void Stage2()
    {
        Debug.Log($"вторая стадия, {hp}");
        SupportAttack();
    }

    private void Stage3()
    {
        Debug.Log($"третья стадия, {hp}");
        if (_stageTime > 500 && _stageTime < 1000)
            SupportAttack();
        if (_stageTime <= 500)
            RushAttack();
        
        _stageTime += 1;
        if (_stageTime > 1500)
            _stageTime = 0;
    }

    private void Stage4()
    {
        Debug.Log($"четвёртая стадия, {hp}");
        if (_stageTime >= 1000 || (IsUlt && !IsLookingAtPlayer()))
            Ult();
        else if (_stageTime > 500 && _stageTime < 1000)
            SupportAttack();
        else if (_stageTime <= 500)
            RushAttack();
        
        _stageTime += 1;
        if (_stageTime > 1500)
            _stageTime = 0;
    }
    
    private void Stage5()
    {
        Debug.Log($"пятая стадия, {hp}");
        Ult();
    }

    private void Ult()
    {
        attackRate = 0.1f;
        IsUlt = true;
        Spin();
        if (CanAttack)
        {
            CanAttack = false;
            for (var i = 0; i < supportingGuns.Length; i++)
            {
                var bul = Instantiate(bullet, supportingGuns[i].transform.position, transform.rotation);
                bul.GetComponent<Rigidbody2D>().velocity = (supportingGuns[i].transform.position - supportingGunsStart[i].transform.position).normalized * bul.bulletSpeed;
                Destroy(bul.gameObject, 5f);
            }
            Invoke(nameof(WaitForAttack), attackRate);
        }
    }
    
    private bool IsLookingAtPlayer()
    {
        var angle = Vector2.SignedAngle(EnemyToPlayer, gun.transform.position - explosionCenter.transform.position);
        return Math.Abs(angle) < 6;
    }

    private void Spin()
    {
        Angle -= 2.5f;
        if (Angle <= -360)
            Angle += 360;
        Rb.velocity = Vector2.zero;
    }
    
    public override void GetDamage(int inputDamage, Transform attackVector)
    {
        hp -= inputDamage;
    }
}
