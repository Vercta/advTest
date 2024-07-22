using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 
/// </summary>
public class PlayerController : MonoBehaviour
{
    public PlayerInputControl inputControl; // Simplified name
    public Vector2 inputDirection;
    private Rigidbody2D rb;
    // private SpriteRenderer spriteRenderer;
    private PhysicsCheck physicsCheck;
    private CapsuleCollider2D coll;
    private PlayerAnimation playerAnimation;

    private float lastNonZeroXInput = 1f;

    [Header("控制引用参数")]
    public float speed;
    // public Rigidbody2D rb; 用于从最开始的获取；
    public float jumpForce;
    public float walkSpeed => (float)(speed / 2.5);
    public float runSpeed;
    public float hurtForce;
    // collider origin param
    private Vector2 originalOffset;
    private Vector2 originalSize;

    [Header("物理材质")]
    public PhysicsMaterial2D normal;
    public PhysicsMaterial2D wall;

    [Header("状态")]
    public bool isCrouch;
    public bool isHurt;
    public bool isDead;
    public bool isAttack;



    private void Awake()
    {
        #region 组件初始化
        rb = GetComponent<Rigidbody2D>();
        // spriteRenderer = GetComponent<SpriteRenderer>();
        physicsCheck = GetComponent<PhysicsCheck>();
        coll = GetComponent<CapsuleCollider2D>();
        playerAnimation = GetComponent<PlayerAnimation>();

        originalOffset = coll.offset;
        originalSize = coll.size;


        inputControl = new PlayerInputControl();
        #endregion

        #region 跳跃
        // once-click to Jump
        inputControl.Gameplay.Jump.started += Jump;
        // performed-click to bigJump
        //inputControl.Gameplay.Jump.performed += bigJump;
        #endregion


        #region 强制步行
        // use button to trigger walk with both keyboad or gamepad

        // runSpeed should not change with a lambda function on the top while it's called
        runSpeed = speed;
        inputControl.Gameplay.Walk.performed += ctx =>
        {
            if (physicsCheck.isGround)
                speed = walkSpeed;
        };

        inputControl.Gameplay.Walk.canceled += ctx =>
        {
            if (physicsCheck.isGround)
                speed = runSpeed;
        };
        #endregion

        #region 攻击
        inputControl.Gameplay.Attack.started += PlayerAttack;
        #endregion

    }
    public void OnEnable()
    {
        inputControl.Enable();
    }

    public void OnDisable()
    {
        inputControl.Disable();
    }

    public void Update()
    {
        inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();
        
        CheckState();
    }

    private void FixedUpdate()
    {
        if (!isHurt && !isAttack)
            Move();
    }

    // //Trigger Test
    // private void OnTriggerStay2D(Collider2D other)
    // {
    //     Debug.Log(other.name);
    // }

    public void Move()
    {
        if (!isCrouch)
        {
            // use Component.Rigidbody to get speed
            rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);
            UpdateFacingDirectin();
        }

        #region 人物翻转基本方法

        // char Flip
        // use if to get Direction
        // int faceDir = (int)transform.localScale.x;
        // if (inputDirection.x > 0)
        //     faceDir = 1;
        // if (inputDirection.x < 1)
        //     faceDir = -1;
        //     // use transform to change Direction
        // transform.localScale = new Vector3(faceDir, 1, 1);


        // use SpriteRenderer to Flip character
        // if (inputDirection.x > 0)
        // {
        //     spriteRenderer.flipX = false;
        // }
        // else if (inputDirection.x < 0)
        // {
        //     spriteRenderer.flipX = true;
        // }
        #endregion

        // char Crouch
        isCrouch = inputDirection.y < -0.5f && physicsCheck.isGround;
        // let collider fit when crouch
        if (isCrouch)
        {
            // when is Crouch to fix collider params
            coll.offset = new Vector2(-0.08f, 0.80f);
            coll.size = new Vector2(0.6f, 1.5f);
        }
        else
        {
            // when ease crouch state to Origin params
            coll.offset = originalOffset;
            coll.size = originalSize;
        }
    }

    // Char.Facing
    private void UpdateFacingDirectin()
    {
        if (inputDirection.x != 0)
        {
            lastNonZeroXInput = inputDirection.x;
            transform.localScale = new Vector3(Mathf.Sign(lastNonZeroXInput), 1, 1);
        }
    }
    private void Jump(InputAction.CallbackContext obj)
    {
        if (physicsCheck.isGround)
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        // Debug.Log("triggeredJUMP");
    }

    // while trigger Attack
    private void PlayerAttack(InputAction.CallbackContext obj)
    {   
        // // Cannot attack in air
        // if (!physicsCheck.isGround)
        // {
        //     return;
        // }
        
        playerAnimation.PlayAttack();
        isAttack = true;
    }

    #region UnityEvent
    // while Hurt char.Forceback
    public void GetHurt(Transform attacker)
    {
        isHurt = true;
        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2((transform.position.x - attacker.position.x), 0).normalized;

        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
    }

    // while Dead
    public void PlayerDead()
    {
        isDead = true;
        inputControl.Gameplay.Disable();
    }
    #endregion

    private void CheckState()
    {
        coll.sharedMaterial = physicsCheck.isGround ? normal : wall;
        
        if (isDead)
            gameObject.layer = LayerMask.NameToLayer("Enemy");
        else
            gameObject.layer = LayerMask.NameToLayer("Player");
    }

}
