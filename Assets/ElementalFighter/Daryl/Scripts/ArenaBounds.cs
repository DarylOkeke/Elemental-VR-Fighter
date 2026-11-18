using UnityEngine;

public class ArenaBounds : MonoBehaviour
{
    [SerializeField] private Transform playerRoot;
    [SerializeField] private Vector3 center = Vector3.zero;
    [SerializeField] private Vector3 size = new Vector3(10f, 0f, 10f);
    public Vector3 ClampPosition(Vector3 position)
    {
        position.x = Mathf.Clamp(position.x, center.x - Mathf.Abs(size.x) / 2f, center.x + Mathf.Abs(size.x) / 2f);
        position.z = Mathf.Clamp(position.z, center.z - Mathf.Abs(size.z) / 2f, center.z + Mathf.Abs(size.z) / 2f);
        return position;
    }
    private void LateUpdate() { if (playerRoot != null) playerRoot.position = ClampPosition(playerRoot.position); }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(center.x, 0.05f, center.z), new Vector3(Mathf.Abs(size.x), 0.1f, Mathf.Abs(size.z)));
    }
}
