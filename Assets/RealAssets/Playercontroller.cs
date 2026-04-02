using UnityEngine;
 
/// <summary>
/// Quan ly animation cua Player (miner) dua theo trang thai cua HookController.
/// Khong xu ly input hay physics — chi dieu khien Animator.
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private HookController hookController;
 
    private Animator animator;
 
    // Animation parameter names (phai khop voi AnimatorController)
    private static readonly int IsPulling = Animator.StringToHash("IsPulling");
 
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
 
    private void Update()
    {
        if (hookController == null) return;
 
        // IsPulling = true khi hook dang keo ve (isRetracting)
        // Dung reflection-free: expose property tu HookController
        animator.SetBool(IsPulling, hookController.IsRetracting);
    }
}
 