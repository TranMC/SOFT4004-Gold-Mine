using UnityEngine;
using UnityEngine.UI;

public class GlowEffect : MonoBehaviour
{
    [Header("Tuy chinh nhip dap")]
    public float pulseSpeed = 3f;      
    public float scaleMultiplier = 0.1f; // Do gian no (0.1 = gian them 10%)

    private Vector3 originalScale;

    void Start()
    {
        // Luu lai kich thuoc hien tai ban dang de trong Editor
        originalScale = transform.localScale;
    }

    void Update()
    {
        // Tao nhip dap nhe nhang xung quanh kich thuoc goc
        float delta = Mathf.Sin(Time.time * pulseSpeed) * scaleMultiplier;
        transform.localScale = originalScale + new Vector3(delta, delta, 0);
    }
}
