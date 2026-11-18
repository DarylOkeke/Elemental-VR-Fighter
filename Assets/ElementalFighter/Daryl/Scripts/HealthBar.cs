using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Slider slider;
    private void OnEnable()
    {
        if (health != null) health.OnHealthChanged.AddListener(Refresh);
        Refresh();
    }
    private void Start() { Refresh(); }
    private void OnDisable() { if (health != null) health.OnHealthChanged.RemoveListener(Refresh); }
    private void Refresh()
    {
        if (health == null || slider == null) return;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.SetValueWithoutNotify(health.CurrentHealth / Mathf.Max(1f, health.MaxHealth));
    }
}
