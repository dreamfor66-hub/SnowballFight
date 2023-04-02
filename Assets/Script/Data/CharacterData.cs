using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new CharacterData", menuName = "Data/Character")]

public class CharacterData : ScriptableObject
{
    
    [Header("캐릭터 기본 스펙")]
    public float MaxHp;
    public float MoveSpd;

    [Space(10f)]

    [Header("캐릭터 대쉬 스펙")]
    public float DashCooldown;
    public float DashSpd;
    public float DashDur;
    public AnimationCurve DashMoveCurve;
    //public float SkillCooldown;
    [Space(10f)]

    [Header("캐릭터 공격 스펙")]
    public GameObject AttackPrefs;
    public float AttackCooldown;
    public float AttackSpd;
    public float AttackDur;

}
