using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    private CapsuleCollider2D coll;

    [Header("检测参数")]
    //To determine is checkField set manually 
    public bool manual;
    public LayerMask groundLayer;
    public float checkRadius;

    // to fix jumpCheck:transform.position
    public Vector2 bottomOffset;
    public Vector2 leftOffset;
    public Vector2 rightOffset;

    [Header("状态")]
    public bool isGround;
    public bool touchLeftWall;
    public bool touchRightWall;

    private void Awake()
    {
        coll = GetComponent<CapsuleCollider2D>();

        // auto set collcheck
        if (!manual)
        {
            // rightOffset = new Vector2((coll.bounds.size.x + coll.offset.x)/2, coll.bounds.size.y /2);
            // leftOffset = new Vector2(- rightOffset.x, rightOffset.y);
            // 将判断范围修正至两端：解决野猪频繁切换方向的问题
            leftOffset = new Vector2(-(coll.bounds.size.x / 2 - coll.offset.x), rightOffset.y);
            rightOffset = new Vector2((coll.bounds.size.x / 2 + coll.offset.x), coll.offset.y);
        }

    }

    private void Update()
    {
        Check();
    }

    public void Check()
    {
        // check ifIsGround to control ifJump
        isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, bottomOffset.y), checkRadius, groundLayer);

        
        // To solve Offset wrong
        leftOffset = new Vector2(-(coll.bounds.size.x / 2 - coll.offset.x * transform.localScale.x), rightOffset.y);
        rightOffset = new Vector2((coll.bounds.size.x / 2 + coll.offset.x * transform.localScale.x), coll.offset.y);

        // check wall touching
        touchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(leftOffset.x, leftOffset.y), checkRadius, groundLayer);
        touchRightWall = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(rightOffset.x, rightOffset.y), checkRadius, groundLayer);
    }

    // to visualize the isGround check radius
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, bottomOffset.y), checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(leftOffset.x, leftOffset.y), checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(rightOffset.x, rightOffset.y), checkRadius);
    }
}
