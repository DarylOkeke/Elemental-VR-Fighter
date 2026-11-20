using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DashController : MonoBehaviour
{
    [SerializeField] private Transform playerRoot;
    [SerializeField] private Transform viewTransform;
    [SerializeField] private ArenaBounds arenaBounds;
    [SerializeField] private InputActionReference dashAction;
    [SerializeField, Min(0f)] private float dashDistance = 2f;
    [SerializeField, Min(0.01f)] private float dashDuration = 0.18f;
    [SerializeField, Min(0f)] private float cooldown = 1.5f;
    private float cooldownLeft;
    private bool dashing;
    private InputAction activeDashAction;
    public float CooldownRemaining => cooldownLeft;
    public bool IsDashing => dashing;
    private void OnEnable()
    {
        if (dashAction == null || dashAction.action == null) return;
        activeDashAction = dashAction.action.Clone();
        activeDashAction.performed += OnDashPressed;
        activeDashAction.Enable();
    }
    private void OnDisable()
    {
        if (activeDashAction != null)
        {
            activeDashAction.performed -= OnDashPressed;
            activeDashAction.Disable();
            activeDashAction.Dispose();
            activeDashAction = null;
        }
        StopAllCoroutines();
        dashing = false;
    }
    private void Update() { cooldownLeft = Mathf.Max(0f, cooldownLeft - Time.deltaTime); }
    private void OnDashPressed(InputAction.CallbackContext context) { TryDash(); }
    public void TryDash()
    {
        if (!isActiveAndEnabled || dashing || cooldownLeft > 0f || playerRoot == null) return;
        var view = viewTransform != null ? viewTransform : Camera.main != null ? Camera.main.transform : playerRoot;
        var direction = Vector3.ProjectOnPlane(view.forward, Vector3.up);
        if (direction.sqrMagnitude < 0.01f) direction = Vector3.ProjectOnPlane(playerRoot.forward, Vector3.up);
        if (direction.sqrMagnitude < 0.01f) return;
        StartCoroutine(Dash(direction.normalized));
    }
    private IEnumerator Dash(Vector3 direction)
    {
        dashing = true;
        cooldownLeft = Mathf.Max(0f, cooldown);
        var start = playerRoot.position;
        var target = start + direction * Mathf.Max(0f, dashDistance);
        if (arenaBounds != null) target = arenaBounds.ClampPosition(target);
        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, dashDuration);
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            playerRoot.position = Vector3.Lerp(start, target, elapsed / duration);
            yield return null;
        }
        playerRoot.position = target;
        dashing = false;
    }
}
