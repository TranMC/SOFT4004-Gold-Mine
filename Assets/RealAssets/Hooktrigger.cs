using UnityEngine;
 
/// <summary>
/// Gan len GameObject Hook (co CircleCollider2D).
/// Chuyen su kien trigger sang HookController o parent.
/// </summary>
public class HookTrigger : MonoBehaviour
{
    private HookController hookController;
 
    private void Awake()
    {
        // Tim HookController tren parent (HookRoot)
        hookController = GetComponentInParent<HookController>();
    }
 
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"HookTrigger hit: {other.gameObject.name}");
        hookController?.OnHookTriggerEnter(other);
    }
}
 