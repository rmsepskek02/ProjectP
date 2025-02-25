using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HealthBar 의 기능 정의
/// </summary>
public class StatusUI : MonoBehaviour
{
    #region Variables
    public GameObject healthBar;            // 체력바 GameObject
    public Image fillHealth;                // 체력바
    public TextMeshProUGUI healthText;      // 체력 텍스트
    [SerializeField] private Status status; // 상태를 받아올 스크립트
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