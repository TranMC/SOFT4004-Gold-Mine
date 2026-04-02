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

    private float currentLength;   // chiều dài hiện tại của dây
    private float minLength;       // chiều dài tối thiểu
    float ropeSpriteLength;         // chiều dài thực tế của sprite dây (để scale đúng)

    void Start()
    {
        minLength = Mathf.Abs(hook.localPosition.y); //chiều dài từ hook đến ropeAnchor
        currentLength = minLength;
        ropeSpriteLength = rope.GetComponent<SpriteRenderer>().sprite.bounds.size.y;

    }

    void Update()
    {
        if (isSwinging)
        {
            Swing();
            if (Input.GetKeyDown(KeyCode.DownArrow))
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
        Vector3 hookWorldPos = hook.position;//Lấy vị trí hook trong world
        Vector3 viewPos = Camera.main.WorldToViewportPoint(hookWorldPos);//Convert sang viewport (0 → 1)
        //x < 0	ra ngoài bên trái
        //x > 1	ra ngoài bên phải
        //y < 0	dưới màn
        //y > 1	trên màn

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
        currentLength -= retractSpeed * Time.deltaTime;

        if (currentLength <= minLength)
        {
            currentLength = minLength;
            ResetHook();
        }
    }

    void ResetHook()
    {
        isRetracting = false;
        isSwinging = true;
        currentAngle = 0;
    }

    void UpdateRope()
    {
        // 1. scale dây
        rope.localScale = new Vector3(1, currentLength / ropeSpriteLength, 1); 
        //Không thể gán trực tiếp currentLength vào scale vì Vector3 scale là tỉ lệ, còn currentLength là chiều dài thực tế
        // công thức scale=1/localScale=ropeSpriteLength/currentLength =>  localScale=currentLength/ropeSpriteLength
        //

         // 2. đặt hook theo local 
        hook.localPosition = new Vector3(0, -currentLength, 0);
    }
}