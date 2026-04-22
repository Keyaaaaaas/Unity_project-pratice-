using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    public PlayerInputControl inputControl;
    public PhysicsCheck physicsCheck;
    public Vector2 inputDirection;
    private Rigidbody2D rb;
    [Header("基础数值")]
    public float speed;
    public float jumpForce;
    private float originalSpeed;


    private void Awake() {
        inputControl = new PlayerInputControl();
        rb = GetComponent<Rigidbody2D>();//获取角色的刚体组件
        physicsCheck = GetComponent<PhysicsCheck>();//获取物理检测组件
        originalSpeed = speed;

        inputControl.Gameplay.Jump.started += Jump;//绑定跳跃事件
        inputControl.Gameplay.Run.started += Run;//绑定跳跃事件
        inputControl.Gameplay.Run.canceled += RunCanceled;
    }

    private void OnEnable() {
        inputControl.Enable();
    }
    
    private void OnDisable() {
        inputControl.Disable();
    }

    private void Update() {
        inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();//获取输入的方向值
    }

    private void FixedUpdate()
    {
        Move();
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
}
