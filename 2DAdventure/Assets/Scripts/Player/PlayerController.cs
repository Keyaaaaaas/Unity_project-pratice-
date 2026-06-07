using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    public PlayerInputControl inputControl;
    public PhysicsCheck physicsCheck;
    public Vector2 inputDirection;
    private PlayerAnimation playerAnimation;
    private Rigidbody2D rb;
    private Collider2D coll;
    [Header("基础数值")]
    public float speed;
    public float jumpForce;
    private float originalSpeed;
    public float hurtForce;
    [Header("状态")]
    public bool isDead;
    public bool isHurt;
    public bool isAttack;

    [Header("物理材质")]
    public PhysicsMaterial2D normal;
    public PhysicsMaterial2D Wall;

    private void Awake() {
        inputControl = new PlayerInputControl();
        rb = GetComponent<Rigidbody2D>();//获取角色的刚体组件
        physicsCheck = GetComponent<PhysicsCheck>();//获取物理检测组件
        playerAnimation = GetComponent<PlayerAnimation>();//获取动画组件
        originalSpeed = speed;
        inputControl.Gameplay.Jump.started += Jump;//绑定跳跃事件
        inputControl.Gameplay.Run.started += Run;//绑定跳跃事件
        inputControl.Gameplay.Run.canceled += RunCanceled;
        inputControl.Gameplay.Attack.started += Attack;
        coll = GetComponent<Collider2D>();
    }

    private void OnEnable() {
        inputControl.Enable();
    }
    
    private void OnDisable() {
        inputControl.Disable();
    }

    private void Update() {
        inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();//获取输入的方向值
        CheckState();
    }

    private void FixedUpdate()
    {
        if(!isHurt&&!isAttack)//如果角色没有受伤，允许移动
        {
            Move();
        }

    }

    private void Move()
    {
        rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);


        //使用角色的缩放来控制角色的朝向
        int faceDir = (int)transform.localScale.x;

        if(inputDirection.x > 0 )
        {
            faceDir = 1;
        }
        else if(inputDirection.x < 0)
        {
            faceDir = -1;
        }
        transform.localScale = new Vector3(faceDir, transform.localScale.y, transform.localScale.z);

    }
    private void Jump(InputAction.CallbackContext obj)
    {
        if (physicsCheck.isGround)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);//给角色施加一个向上的力，使其跳跃，使用Impulse模式表示这是一个瞬时的力
        }
    }
    private void Run(InputAction.CallbackContext obj)
    {
        speed = 600;//按下跑步键时，速度设置为跑步速度
    }
    private void RunCanceled(InputAction.CallbackContext obj)
    {
        speed = originalSpeed;//松开跑步键时，速度复原
    }

    private void Attack(InputAction.CallbackContext obj)
    {
        isAttack = true;
        playerAnimation.PlayerAttack();
    }

    #region Animation Event
    public void GetHurt(Transform attacker)
    {
        isHurt = true;
        rb.velocity = Vector2.zero; // 重置角色的速度，确保受伤时不会受到之前的移动影响
        Vector2 hurtDirection = new Vector2((transform.position.x - attacker.position.x), 0).normalized; // 计算受伤方向
        rb.AddForce(hurtDirection * hurtForce, ForceMode2D.Impulse); // 给角色施加一个反方向的力，使其受到击退效果
    }

    public void PlayerDead()
    {
        isDead = true;
        inputControl.Gameplay.Disable(); // 禁用输入控制
    }
    #endregion

    private void CheckState()
    {
        coll.sharedMaterial = physicsCheck.isGround ? normal : Wall;//根据角色是否在地上，切换物理材质
    }
}
