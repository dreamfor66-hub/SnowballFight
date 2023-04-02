using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new CharacterData", menuName = "Data/Character")]

public class CharacterData : ScriptableObject
{
    
    [Header("ĳ���� �⺻ ����")]
    public float MaxHp;
    public float MoveSpd;

    [Space(10f)]

    [Header("ĳ���� �뽬 ����")]
    public float DashCooldown;
    public float DashSpd;
    public float DashDur;
    public AnimationCurve DashMoveCurve;
    //public float SkillCooldown;
    [Space(10f)]

    [Header("ĳ���� ���� ����")]
    public GameObject AttackPrefs;
    public float AttackCooldown;
    public float AttackSpd;
    public float AttackDur;

}
