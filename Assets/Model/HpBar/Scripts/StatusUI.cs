using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HealthBar �� ��� ����
/// </summary>
public class StatusUI : MonoBehaviour
{
    #region Variables
    public GameObject healthBar;            // ü�¹� GameObject
    public Image fillHealth;                // ü�¹�
    public TextMeshProUGUI healthText;      // ü�� �ؽ�Ʈ
    [SerializeField] private Status status; // ���¸� �޾ƿ� ��ũ��Ʈ
    #endregion

    void Start()
    {
        status.OnDamaged += SetFillHealth;
        status.OnDamaged += SetHealthText;
        status.OnDamaged.Invoke();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetFillHealth()
    {
        fillHealth.fillAmount = (status.CurrentHealth / status.MaxHealth);
    }
    void SetHealthText()
    {
        healthText.text = $"{Mathf.Round(status.CurrentHealth)}/{status.MaxHealth}";
    }
}