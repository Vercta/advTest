using System;
using System.Collections;
using System.Collections.Generic;
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
    private Character character;

    private float lastNonZeroXInput = 1f;

    [Header("控制引用参数")]
    public float speed;
    // public Rigidbody2D rb; 用于从最开始的获取；
    public float jumpForce;
    public float wallJumpForce;
    public float walkSpeed => (float)(speed / 2.5);
    public float runSpeed;
    public float hurtForce;
    public float slideDistance;
    public float slideSpeed;
    public int slidePowerCost;
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
    public bool wallJump;
    public bool isSlide;



    private void Awake()
    {
        #region 组件初始化
        rb = GetComponent<Rigidbody2D>();
        // spriteRenderer = GetComponent<SpriteRenderer>();
        physicsCheck = GetComponent<PhysicsCheck>();
        coll = GetComponent<CapsuleCollider2D>();
        playerAnimation = GetComponent<PlayerAnimation>();
        character = GetComponent<Character>();

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

        #region 滑铲
        inputControl.Gameplay.Slide.started += Slide;
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
        inputDirection = inputControl.Gameplay.Move.ReadValue<UnityEngine.Vector2>();

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
        if (!isCrouch && !wallJump)
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

    #region 跳跃方法
    private void Jump(InputAction.CallbackContext obj)
    {
        if (physicsCheck.isGround)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            // Debug.Log("triggeredJUMP");

            // 打断滑铲协程
            isSlide = false;
            StopAllCoroutines();
        }

        // 蹬墙跳
        else if (physicsCheck.onWall)
        {
            rb.AddForce(new Vector2(-inputDirection.x, 2.5f) * wallJumpForce, ForceMode2D.Impulse);
            wallJump = true;
        }
    }
    #endregion

    #region 攻击方法
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
    #endregion

    #region 滑铲方法

    private void Slide(InputAction.CallbackContext obj)
    {
        if (!isSlide && physicsCheck.isGround && character.currentPower >= slidePowerCost)
        {
            isSlide = true;

            var targetPos = new Vector3(transform.position.x + slideDistance * transform.localScale.x, transform.position.y);

            gameObject.layer = LayerMask.NameToLayer("Enemy");
            StartCoroutine(TriggerSlide(targetPos));

            character.OnSlide(slidePowerCost);
        }
    }
    private IEnumerator TriggerSlide(Vector3 target)
    {
        do
        {
            yield return null;
            if (!physicsCheck.isGround)
                break;

            // touching while slidinig
            if (physicsCheck.touchLeftWall && transform.localScale.x < 0f || physicsCheck.touchRightWall && transform.localScale.x > 0f)
            {
                isSlide = false;
                break;
            }

            rb.MovePosition(new Vector2(transform.position.x + transform.localScale.x * slideSpeed, transform.position.y));

        } while (MathF.Abs(target.x - transform.position.x) > 0.1f);

        isSlide = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    #endregion

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

        // onWall? --> Speed Change
        if (physicsCheck.onWall)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2f);
        else
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);

        if (isDead)
            gameObject.layer = LayerMask.NameToLayer("Enemy");
        else
            gameObject.layer = LayerMask.NameToLayer("Player");

        // 更改状态使得wallJump后能够在地面正常移动（解除wallJump）
        if (wallJump && rb.velocity.y < 0f)
        {
            wallJump = false;
        }
    }

}
