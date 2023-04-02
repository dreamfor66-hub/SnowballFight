using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Throw,
    Spawn,
    
}

[CreateAssetMenu(fileName = "new ItemData", menuName = "Data/Item")]
public class ItemData : ScriptableObject
{
    //아이템 프리팹에 이 데이터를 끼워넣을 거라 프리팹 사양은 됐고
    //이걸 습득하면 이 데이터를 플레이어 데이터에 끼워넣을거라서 실제로는 스킬에 있을 데이터로 대응해야 할 듯

    [Header("아이템 기본 스펙")]
    public float ItemId;
    public ItemType ItemType;
    
    [Space(10f)]

    [Header("스킬 스펙")]

    public GameObject AttackPrefs;
    public float AttackCooldown;
    public float AttackSpd;
    public float AttackDur;
}
