using UnityEngine;

public class BodyHurtboxFollow : MonoBehaviour
{
    [SerializeField] private Transform head;
    [SerializeField] private float shoulderOffset = 0.55f;
    private void LateUpdate()
    {
        if (head == null) return;
        transform.position = head.position - Vector3.up * shoulderOffset;
        transform.rotation = Quaternion.Euler(0f, head.eulerAngles.y, 0f);
    }
}
