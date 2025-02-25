using UnityEngine;
using UnityEngine.Events;

public class Status : MonoBehaviour
{
    #region Variables
    // �ִ� ü��
    [SerializeField] private float maxHealth;
    public float MaxHealth
    {
        get { return maxHealth; }
        private set { maxHealth = value; }
    }
    // ���� ü��
    [SerializeField] private float currentHealth;
    public float CurrentHealth
    {
        get { return currentHealth; }
        private set
        {
            currentHealth = value;

            //���� ó��
            if (currentHealth <= 0)
            {
                IsDeath = true;
            }
        }
    }
    // ��������
    private bool isDeath = false;
    public bool IsDeath
    {
        get { return isDeath; }
        private set
        {
            isDeath = value;
            //�ִϸ��̼�
            //animator.SetBool(AnimationString.IsDeath, value);
        }
    }

    // Action
    public UnityAction OnDamaged;           // �������� ���� �� ȣ���ϴ� �̺�Ʈ
    #endregion

    private void Start()
    {

    }

    private void Update()
    {

    }

    // �ִ�ü�� ����
    public void SetMaxHealth(float amount)
    {
        maxHealth = amount;
        CurrentHealth = maxHealth;
    }

    // ������ ����
    public void TakeDamage(float damage)
    {
        // ���� ���� �� ���� ������ ��� 
        float mitigatedDamage = Mathf.Clamp(damage, 0, Mathf.Infinity);

        // ���������� ���� ������ ��� �� ��ȿ�� �˻�
        float realDamage = Mathf.Min(CurrentHealth, mitigatedDamage);

        // ü�� ����
        CurrentHealth -= realDamage;

        // ü���� 0 ���϶�� ��� ó��
        if (CurrentHealth <= 0f)
        {
            CurrentHealth = 0;
            //Die();
        }
        OnDamaged.Invoke();
    }

    // ü�� ȸ��
    public void Heal(float amount)
    {
        // �� ���� �� ü�� ����
        float beforeHealth = CurrentHealth;

        // �� ����
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0f, maxHealth);

        // ���� ���� ���
        float realHeal = CurrentHealth - beforeHealth;
    }
}
