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
    //������ �����տ� �� �����͸� �������� �Ŷ� ������ ����� �ư�
    //�̰� �����ϸ� �� �����͸� �÷��̾� �����Ϳ� ���������Ŷ� �����δ� ��ų�� ���� �����ͷ� �����ؾ� �� ��

    [Header("������ �⺻ ����")]
    public float ItemId;
    public ItemType ItemType;
    
    [Space(10f)]

    [Header("��ų ����")]

    public GameObject AttackPrefs;
    public float AttackCooldown;
    public float AttackSpd;
    public float AttackDur;
}
