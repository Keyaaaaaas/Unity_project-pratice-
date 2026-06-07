using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [Header("基础数值")]
    public float nomalSpeed;
    public float currentSpeed;
    public float chaseSpeed;
    public float hurtForce;

    private Vector3 faceDir;
    protected Rigidbody2D rb;
    protected Animator anim;

    protected PhysicsCheck physicsCheck;
    public Transform attacker;
    [Header("计时器")]
    public float waitTime;
    public float waitCounter;
    public bool wait;

    [Header("状态")]
    public bool isHurt;

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentSpeed = nomalSpeed;
        physicsCheck = GetComponent<PhysicsCheck>();
        waitCounter = waitTime;
    }

    public void Update()
    {
        faceDir = new Vector3(-transform.localScale.x, 0, 0);

        if(((physicsCheck.isTouchingLeftWall && faceDir.x < 0) || (physicsCheck.isTouchingRightWall && faceDir.x > 0)))
        {
            wait = true;
            anim.SetBool("Walk", false);
        }
        TimeCounter();
    }

    public void FixedUpdate()
    {
        if (!isHurt)
        {
            Move();
        }

    }
    public virtual void Move()
    {
        rb.velocity = new Vector2(faceDir.x * currentSpeed * Time.deltaTime, rb.velocity.y);
    }

    public void TimeCounter()
    {
        if(wait)
        {
            waitCounter -= Time.deltaTime;
            if(waitCounter <= 0)
            {
                if(physicsCheck.isTouchingLeftWall && faceDir.x < 0)
                {
                    transform.localScale = new Vector3(-1,1,1);
                }
                else if(physicsCheck.isTouchingRightWall && faceDir.x > 0)
                {
                    transform.localScale = new Vector3(1,1,1);
                }
                wait = false;
                waitCounter = waitTime;
            }
        }
    }
    public void OntakeDamage(Transform attackTrans)
    {
        attacker = attackTrans;
        //受伤之后转向攻击者方向
        if(attackTrans.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(-1,1,1);
        }
        if(attackTrans.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(1,1,1);
        }

        //受伤被击退
        isHurt = true;
        anim.SetTrigger("Hurt");
        Vector2 hurtDir = new Vector2(transform.position.x - attackTrans.position.x, 0).normalized;
        StartCoroutine(OnHurt(hurtDir));
    }
    private IEnumerator OnHurt(Vector2 hurtDir)
    {
        rb.AddForce(hurtDir * hurtForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f);
        isHurt = false;
    }

}
