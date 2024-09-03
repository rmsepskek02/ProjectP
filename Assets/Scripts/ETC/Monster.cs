using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    int health, attack;
    public int Health { get { return health; } }
    public int Attack { get { return attack; } }

    // ������ - health, attack ���� �Ű������� �޾� �ʱ�ȭ
    public Monster(int health, int attack)
    {
        this.health = health;
        this.attack = attack;
    }

    // �޼��� - damage ���� �Ű������� �޾Ƽ� health�� ����
    public void OnDamage(int damage)
    {
        health -= damage;
    }
    private void Start()
    {
        // Slime, Goblin ���� �ν��Ͻ� ����
        Slime slime = new Slime(100,10);
        Goblin goblin = new Goblin(200,20);

        // Slime, Goblin ���� �������� 30�� ����
        slime.OnDamage(30);
        goblin.OnDamage(30);

        // Slime, Goblin ���� health ���
        Debug.Log($"������ ü��: {slime.Health}");
        Debug.Log($"��� ü��: {goblin.Health}");
    }
}
