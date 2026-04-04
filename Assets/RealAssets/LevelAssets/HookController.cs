using UnityEngine;

public class HookController : MonoBehaviour
{
    [Header("Swing")]
    public float rotateSpeed = 100f;
    public float minAngle = -70f;
    public float maxAngle = 70f;

    [Header("Launch")]
    public float launchSpeed = 10f;
    public float retractSpeed = 15f;

    [Header("Refs")]
    public Transform ropeAnchor;
    public Transform hook;
    public Transform rope;

    private bool isSwinging = true;
    private bool isLaunched = false;
    private bool isRetracting = false;

    private float currentAngle;
    private int direction = 1;

    private float currentLength;
    private float minLength;
    float ropeSpriteLength;

    private ItemController attachedItem = null;

    public bool IsRetracting => isRetracting;
    public ItemController AttachedItem => attachedItem;

    void Start()
    {
        minLength = Mathf.Abs(hook.localPosition.y);
        currentLength = minLength;
        ropeSpriteLength = rope.GetComponent<SpriteRenderer>().sprite.bounds.size.y;
    }

    void Update()
    {
        if (isSwinging)
        {
            Swing();
            if (GetLaunchInput())
            {
                Launch();
            }
        }
        else if (isLaunched)
        {
            Extend();
        }
        else if (isRetracting)
        {
            Retract();
        }

        UpdateRope();
    }

    bool GetLaunchInput()
    {
#if UNITY_ANDROID || UNITY_IOS
        return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
#else
        return Input.GetKeyDown(KeyCode.DownArrow);
#endif
    }

    void Swing()
    {
        currentAngle += direction * rotateSpeed * Time.deltaTime;

        if (currentAngle > maxAngle || currentAngle < minAngle)
        {
            direction *= -1;
        }

        transform.rotation = Quaternion.Euler(0, 0, currentAngle);
    }

    void Launch()
    {
        isSwinging = false;
        isLaunched = true;
    }

    void Extend()
    {
        currentLength += launchSpeed * Time.deltaTime;

        Vector3 hookWorldPos = hook.position;
        Vector3 viewPos = Camera.main.WorldToViewportPoint(hookWorldPos);

        if (viewPos.x <= 0 || viewPos.x >= 1 || viewPos.y <= 0)
        {
            StartRetract();
        }
    }

    void StartRetract()
    {
        isLaunched = false;
        isRetracting = true;
    }

    void Retract()
    {
        float speed = WeightCalculator.Instance != null
            ? WeightCalculator.Instance.GetRetractSpeed(attachedItem)
            : retractSpeed;

        currentLength -= speed * Time.deltaTime;

        if (currentLength <= minLength)
        {
            currentLength = minLength;
            ResetHook();
        }
    }

    void ResetHook()
    {
        attachedItem?.Collect();
        attachedItem = null;

        isRetracting = false;
        isSwinging = true;
        currentAngle = 0;
        direction = 1;
    }

    void UpdateRope()
    {
        rope.localScale = new Vector3(1, currentLength / ropeSpriteLength, 1);
        hook.localPosition = new Vector3(0, -currentLength, 0);
    }

    /// <summary>
    /// Duoc goi tu HookTrigger gan tren child Hook (co CircleCollider2D).
    /// </summary>
    public void OnHookTriggerEnter(Collider2D other)
    {
        if (!isLaunched) return;
        if (attachedItem != null) return;

        if (other.TryGetComponent<ItemController>(out var item))
        {
            attachedItem = item;
            item.AttachToHook(hook);
            StartRetract();
        }
    }
}