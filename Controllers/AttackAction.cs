using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : MonoBehaviour
{
    public string _name;

    public enum Action_Types
    {
        //Attack_Self, // target is self, for surround attack
        Attack_Melee, // target is enemy, one grid away, can be phy or magical
        Attack_Range, // target is enemy, two grid away, can be phy or magical
        //Support_Close, // target is ally, one grid away
        //Support_Range, // target is ally, two grid away
        //Support_Self, // target is self, for surround heal/buff
    };
    public Action_Types _actionType = Action_Types.Attack_Melee;

    public enum Splash_Patterns
    {
        One_Target, // just target
        //Target_Self, // affect target, also self in some way
        //Line_Two, // affect target, and one space behind
        //Line_Three, // affect target, and two spaces behind
        //Line_Five, // affect target, and 4 spaces behind
        //Adjacent_Target, // affect target, and all adjacent spaces from target
        //Xcross_Target, // affect target, and the X adjacent spaces (e.g. lowerRight) from target
        //Xcross_NoTarget, // affect the X adjacent spaces (e.g. lowerRight) from target
        //Square_Target, // affect target, affect all 8 squares around target
        //Square_NoTarget, // does not affect target, affect all 8 squares around target
        //Adjacent_NoTarget, // affect all adjacent spaces from target, but no target
        //Horizontal_Three, // affect target, and the spaces above and below target
        //Chain_Three, // affect target, then chain up to 2 other targets of same side (friend/enemy)
        //Chain_Five, // affect target, then chain up to 4 other targets of same side (friend/enemy)
    };
    public Splash_Patterns _splashPattern = Splash_Patterns.One_Target;

    public enum Damage_Types
    {
        Slash,
        Thrust,
        Magic_Fire,
        //Blunt,
        //Slash,
        //Thrust,
        //Magic_Fire,
        //Magic_Ice,
        //Magic_Lighting,
        //Magic_Neutral,
    };
    public Damage_Types _damageType = Damage_Types.Slash;

    // chance the status effects will be activated
    //[Range(0.0f, 1.0f)]
    //public float _statusChance = 0.5f;
    //[Range(0, 100)]
    //public int _staminaCost = 10;

    public int _power;

    public int _missChance;

    public int _momentum;

    public int _missMomentum;

    public int _statusChance;

    public enum Status_Effect
    {
        None,
    };
    public Status_Effect _statusEffect = Status_Effect.None;
    // stamina damage suffered by target
    //[Range(-100, 100)]
    //public int _staminaDmg = 0;
    //[Range(0, 80)]
    //public int _adrenalineReq = 0;
    //[Range(-50, 50)]
    //public int _adrenalineSelfEffect = 20;
    //[Range(-50, 50)]
    //public int _adrenalineTargetEffect = 10;

    //public enum Bounce_Types
    //{
    //    None, // affected squares after the first one suffer same effect as 1st one 
    //    Half, // affected squares after the first one suffer half same effect as previous one 
    //    Random, // affected squares suffer random effect between zero and amount suffered by 1st one 
    //    Reverse, // affected squares suffer the reverse effect as 1st one
    //    Reverse_half,
    //    Reverse_random,
    //};
    //public Bounce_Types _bounceType = Bounce_Types.None;

    //// defensive status changes
    //public bool _canProtect; // increase physical def
    //public bool _canShell; // increase magical def
    //public bool _canStrengthen; // increase phy dmg
    //public bool _canFocus; // increase magical dmg
    //public bool _canCleanse; // remove negative status effects, not including exhaustion

    //// offensive status changes
    //public bool _canDispel; // remove positive status effects
    //public bool _canPoision; // dec stamina recovery rate
    //public bool _canBleed; // lose health
    //public bool _canDaze; // can move, but can't take action
    //public bool _canBurn; // lose health, lower def 
    //public bool _canFreeze; // movement reduced

    public bool _canBeCloseCountered; // can be close countered
    public bool _canBeFarCountered; // can be far countered
    public bool _canBeDodged; // attack completely dodged
    public bool _canBeBlocked; // attack partially blocked
}
