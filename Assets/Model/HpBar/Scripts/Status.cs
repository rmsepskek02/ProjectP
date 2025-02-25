using UnityEngine;
using UnityEngine.Events;

public class Status : MonoBehaviour
{
    #region Variables
    // 최대 체력
    [SerializeField] private float maxHealth;
    public float MaxHealth
    {
        get { return maxHealth; }
        private set { maxHealth = value; }
    }
    // 현재 체력
    [SerializeField] private float currentHealth;
    public float CurrentHealth
    {
        get { return currentHealth; }
        private set
        {
            currentHealth = value;

            //죽음 처리
            if (currentHealth <= 0)
            {
                IsDeath = true;
            }
        }
    }
    // 죽음여부
    private bool isDeath = false;
    public bool IsDeath
    {
        get { return isDeath; }
        private set
        {
            isDeath = value;
            //애니메이션
            //animator.SetBool(AnimationString.IsDeath, value);
        }
    }

    // Action
    public UnityAction OnDamaged;           // 데미지를 받을 때 호출하는 이벤트
    #endregion

    private void Start()
    {

    }

    private void Update()
    {

    }

    // 최대체력 세팅
    public void SetMaxHealth(float amount)
    {
        maxHealth = amount;
        CurrentHealth = maxHealth;
    }

    // 데미지 받음
    public void TakeDamage(float damage)
    {
        // 방어력 적용 후 최종 데미지 계산 
        float mitigatedDamage = Mathf.Clamp(damage, 0, Mathf.Infinity);

        // 실질적으로 들어온 데미지 계산 및 유효성 검사
        float realDamage = Mathf.Min(CurrentHealth, mitigatedDamage);

        // 체력 감소
        CurrentHealth -= realDamage;

        // 체력이 0 이하라면 사망 처리
        if (CurrentHealth <= 0f)
        {
            CurrentHealth = 0;
            //Die();
        }
        OnDamaged.Invoke();
    }

    // 체력 회복
    public void Heal(float amount)
    {
        // 힐 적용 전 체력 저장
        float beforeHealth = CurrentHealth;

        // 힐 적용
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0f, maxHealth);

        // 실제 힐량 계산
        float realHeal = CurrentHealth - beforeHealth;
    }
}
