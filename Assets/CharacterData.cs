using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "CharacterData", order = int.MaxValue)]

public class CharacterData : ScriptableObject
{

    public float MaxHp;
    public float MoveSpd;
    public float DashCooldown;
    public float DashSpd;
    public float DashDur;
    public AnimationCurve DashMoveCurve;
    //public float SkillCooldown;
    public GameObject AttackPrefs;
    public float AttackCooldown;
    public float AttackSpd;
    public float AttackDur;

}
