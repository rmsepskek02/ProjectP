using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    int health, attack;
    public int Health { get { return health; } }
    public int Attack { get { return attack; } }

    // 생성자 - health, attack 값을 매개변수로 받아 초기화
    public Monster(int health, int attack)
    {
        this.health = health;
        this.attack = attack;
    }

    // 메서드 - damage 값을 매개변수로 받아서 health를 감산
    public void OnDamage(int damage)
    {
        health -= damage;
    }
    private void Start()
    {
        // Slime, Goblin 각각 인스턴스 생성
        Slime slime = new Slime(100,10);
        Goblin goblin = new Goblin(200,20);

        // Slime, Goblin 각각 데미지를 30씩 입음
        slime.OnDamage(30);
        goblin.OnDamage(30);

        // Slime, Goblin 각각 health 출력
        Debug.Log($"슬라임 체력: {slime.Health}");
        Debug.Log($"고블린 체력: {goblin.Health}");
    }
}
