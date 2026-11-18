using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField, Min(1f)] private float maxHealth = 100f;
    [SerializeField] private UnityEvent onHealthChanged = new UnityEvent();
    [SerializeField] private UnityEvent onKO = new UnityEvent();
    public float MaxHealth => maxHealth;
    public float CurrentHealth { get; private set; }
    public bool IsAlive => CurrentHealth > 0f;
    public UnityEvent OnHealthChanged => onHealthChanged;
    public UnityEvent OnKO => onKO;
    private void Awake() { maxHealth = Mathf.Max(1f, maxHealth); CurrentHealth = maxHealth; }
    public void TakeDamage(float amount)
    {
        if (!IsAlive || amount <= 0f || float.IsNaN(amount) || float.IsInfinity(amount)) return;
        CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
        onHealthChanged.Invoke();
        if (!IsAlive) onKO.Invoke();
    }
    public void Heal(float amount)
    {
        if (!IsAlive || amount <= 0f || float.IsNaN(amount) || float.IsInfinity(amount)) return;
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
        onHealthChanged.Invoke();
    }
    public void ResetHealth() { CurrentHealth = maxHealth; onHealthChanged.Invoke(); }
}
