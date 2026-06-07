using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    private CapsuleCollider2D coll;
    [Header("检测参数")]
    public bool maual;
    public Vector2 bottomOffset;
    public Vector2 leftOffset;
    public Vector2 rightOffset;
    public float checkRadius;
    public LayerMask groundLayer;
    [Header("状态")]
    public bool isGround;
    public bool isTouchingLeftWall;
    public bool isTouchingRightWall;
    void Awake()
    {
        coll = GetComponent<CapsuleCollider2D>();
        if(!maual)
        {
            leftOffset = new Vector2(coll.bounds.size.x / 2 +coll.offset.x, coll.bounds.size.y / 2);
            rightOffset = new Vector2(-leftOffset.x, leftOffset.y);
        }
    }
    void Update()
    {
        Check();
    }
    public void Check()
    {
        
        isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, bottomOffset.y), checkRadius, groundLayer);
        isTouchingLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(leftOffset.x , leftOffset.y), checkRadius, groundLayer);
        isTouchingRightWall = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(rightOffset.x , rightOffset.y), checkRadius, groundLayer);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, checkRadius);
    }
}
