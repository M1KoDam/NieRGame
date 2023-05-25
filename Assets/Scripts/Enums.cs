using UnityEngine;

public enum Side
{
    Left = -1,
    Right = 1
}

public enum State
{
    Dead = -2,
    Patrol = -1,
    Chase = 0,
    Attack = 1
}

public enum PlayerState
{
    Dead = -1,
    Default = 0
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
        {GetDamagedClimb, GetDamagedFromFront, GetDamagedFromBehind, GetDamagedInAirFromFront, GetDamagedInAirFromBehind};
}