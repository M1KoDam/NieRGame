public enum Side
{
    Left = -1,
    Right = 1
}

public enum PlayerState
{
    Dead = -2,
    UnActive = -1,
    Active = 0
}

public enum ViewType
{
    Side,
    Top
}

public enum LiftState
{
    MovingDown = -1,
    Idle = 0,
    MovingUp = 1
}

public static class Animation
{
    public const string
        Idle = "Idle",
        Move = "Move",
        Jump = "Jump",
        Fall = "Fall",
        FallEnd = "FallEnd",
        Climb = "Climb",
        Attack1 = "Attack",
        Attack2 = "Attack2",
        Attack3 = "Attack3",
        Attack3End = "Attack3End",
        AttackInAir1 = "AttackInAir",
        AttackInAir2 = "AttackInAir2",
        AttackInAir3 = "AttackInAir3",
        AttackInAir3End = "AttackInAir3End",
        FallAttackStart = "FallAttack",
        FallAttackEnd = "FallAttackEnd",
        FallAttack = "FallAttackFalling",
        GetDamagedClimb = "GetDamagedClimb",
        GetDamagedFromFront = "GetDamaged1",
        GetDamagedFromBehind = "GetDamaged2",
        GetDamagedInAirFromFront = "GetDamagedInAir1",
        GetDamagedInAirFromBehind = "GetDamagedInAir2";
    
    public static readonly string[] GetDamagedAnimations = 
        {
            GetDamagedClimb,
            GetDamagedFromFront,
            GetDamagedFromBehind,
            GetDamagedInAirFromFront,
            GetDamagedInAirFromBehind
        };
}

public static class GoliathStatics
{
    public const float IdleArmAngle = 0;
    public const float IdleHandAngle = 0;

    public const float SwingArmAngle = -150;
    public const float SwingHandAngle = -37;

    public const float KickArmAngle = -30;
    public const float KickHandAngle = 10;
}