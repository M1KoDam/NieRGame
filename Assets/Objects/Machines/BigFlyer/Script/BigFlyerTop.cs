using System;
using System.Collections;
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
            : moveSpot.Count == 0
                ? new IdleState()
                : OnFlightScene
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
        else if (_stageTime <= 500)
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
    
    public override void Die()
    {
        IgnoreCollision(true);
        Rb.velocity += fallDirection;
        Animator.Play("BigFlyerTopDestroy");

        if (CurDestructionTime <= 0)
        {
            if (FaceOrientation is Side.Right)
            {
                transform.Rotate(new Vector3(0, 180, 0));
            }

            var tempPosition = transform.position;
            var tempRotation = transform.rotation;
            var tempLocalScale = transform.localScale;

            Destroy(gameObject);

            var smallFlyerDestroyingCopy = Instantiate(enemyDestroying, tempPosition, tempRotation);
            smallFlyerDestroyingCopy.transform.localScale = tempLocalScale;
            smallFlyerDestroyingCopy.Activate();
            Destroy(smallFlyerDestroyingCopy.gameObject, 5f);

            StartCoroutine(CreateExplosion(explosionCenter.position, tempRotation));
            StartCoroutine(CreateExplosion(supportingGuns[0].position, tempRotation));
            StartCoroutine(CreateExplosion(supportingGuns[1].position, tempRotation));
            StartCoroutine(CreateExplosion(supportingGuns[2].position, tempRotation));
            StartCoroutine(CreateExplosion(supportingGuns[3].position, tempRotation));
        }
        else
            CurDestructionTime -= Time.deltaTime;
    }

    private IEnumerator CreateExplosion(Vector3 position, Quaternion rotation)
    {
        var smallFlyerExplosion = Instantiate(explosion, position, rotation);
        smallFlyerExplosion.force = 150000;
        smallFlyerExplosion.explosionScale = 1.5f;
        smallFlyerExplosion.Explode();
        yield return new WaitForSeconds(0.1f);
    }
    
    public override void GetDamage(int inputDamage, Transform attackVector)
    {
        hp -= inputDamage;
    }
}
