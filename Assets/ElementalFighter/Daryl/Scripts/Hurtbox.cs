using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Hurtbox : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private UnityEvent<float, Vector3> onHit = new UnityEvent<float, Vector3>();
    public UnityEvent<float, Vector3> OnHit => onHit;
    private void Awake() { if (health == null) health = GetComponentInParent<Health>(); }
    public void ApplyHit(float damage, Vector3 hitPoint)
    {
        if (health == null || !health.IsAlive || damage <= 0f || float.IsNaN(damage) || float.IsInfinity(damage)) return;
        health.TakeDamage(damage);
        onHit.Invoke(damage, hitPoint);
    }
}
