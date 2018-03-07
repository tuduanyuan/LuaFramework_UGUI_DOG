using UnityEngine;
using System.Collections;

/*
 * Example implementation of the SuperStateMachine and SuperCharacterController
 */
[RequireComponent(typeof(SuperCharacterController))]
[RequireComponent(typeof(PlayerInputController))]
//玩家状态机
public class PlayerMachine : SuperStateMachine {

    //首先我需要去除掉float
    //我们的模型
    public Transform AnimatedMesh;
    //走路的速度
    public int WalkSpeed = 4;
    //走路的加速的度
    public int WalkAcceleration = 30;
    //跳跃的加速的度
    public int JumpAcceleration = 5;
    //高
    public int JumpHeight = 3;
    //重力
    public float Gravity = 25;

    // Add more states by comma separating them
    // 现在有的状态，如果是动画虚拟机，有几种状态就写几种
    enum PlayerStates { Idle, Walk, Jump, Fall };//Fall落下
    // 超级控制器的实例
    private SuperCharacterController controller;

    // current velocity 当前的移动速度
    private Vector3 moveDirection;
    // current direction our character's art is facing
    // 看的方向
    public Vector3 lookDirection { get; private set; }
    // 输入
    private PlayerInputController input;

	void Start () {
	    // Put any code here you want to run ONCE, when the object is initialized

        input = gameObject.GetComponent<PlayerInputController>();

        // Grab the controller object from our object
        controller = gameObject.GetComponent<SuperCharacterController>();
		
		// Our character's current facing direction, planar to the ground
        lookDirection = transform.forward;

        // Set our currentState to idle on startup
        currentState = PlayerStates.Idle;
	}
    //状态响应之前
    protected override void EarlyGlobalSuperUpdate()
    {
		// Rotate out facing direction horizontally based on mouse input
        // (Taking into account that this method may be called multiple times per frame)
        lookDirection = Quaternion.AngleAxis(input.Current.MouseInput.x * (controller.deltaTime / Time.deltaTime), controller.up) * lookDirection;
        // Put any code in here you want to run BEFORE the state's update function.
        // This is run regardless of what state you're in
    }
    //状态响应之后
    protected override void LateGlobalSuperUpdate()
    {
        // Put any code in here you want to run AFTER the state's update function.
        // This is run regardless of what state you're in

        // Move the player by our velocity every frame
        transform.position += moveDirection * controller.deltaTime;
        // Rotate our mesh to face where we are "looking"
        AnimatedMesh.rotation = Quaternion.LookRotation(lookDirection, controller.up);
    }

    //获取地面
    private bool AcquiringGround()
    {
        return controller.currentGround.IsGrounded(false, 0.01f);
    }
    //维持地面 悬空
    private bool MaintainingGround()
    {
        return controller.currentGround.IsGrounded(true, 0.5f);
    }
    //不知道这个是怎么旋转的
    public void RotateGravity(Vector3 up)
    {
        //改变上方向的朝向
        lookDirection = Quaternion.FromToRotation(transform.up, up) * lookDirection;
    }

    /// <summary>
    /// Constructs a vector representing our movement local to our lookDirection, which is
    /// controlled by the camera
    /// 构造一个矢量，表示我们的运动局部到我们的方向，由摄像机控制
    /// </summary>
    private Vector3 LocalMovement()
    {
        Vector3 right = Vector3.Cross(controller.up, lookDirection);

        Vector3 local = Vector3.zero;

        if (input.Current.MoveInput.x != 0)
        {
            local += right * input.Current.MoveInput.x;
        }

        if (input.Current.MoveInput.z != 0)
        {
            local += lookDirection * input.Current.MoveInput.z;
        }

        return local.normalized;
    }

    // Calculate the initial velocity of a jump based off gravity and desired maximum height attained
    //计算一个基于重力的跳跃的初始速度和所期望的最大高度
    private float CalculateJumpSpeed(float jumpHeight, float gravity)
    {
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }

    /*void Update () {
	 * Update is normally run once on every frame update. We won't be using it
     * in this case, since the SuperCharacterController component sends a callback Update 
     * called SuperUpdate. SuperUpdate is recieved by the SuperStateMachine, and then fires
     * further callbacks depending on the state
     * 更新通常在每个帧更新时运行一次。 在这种情况下，我们不会使用它，
     * 因为SuperCharacterController组件发送一个名为SuperUpdate的回调更新。 
     * SuperUpdate由SuperStateMachine接收，然后根据状态触发进一步的回调
	}*/

    // Below are the three state functions. Each one is called based on the name of the state,
    // so when currentState = Idle, we call Idle_EnterState. If currentState = Jump, we call
    // Jump_SuperUpdate()
    // 默认状态开始的时候
    void Idle_EnterState()
    {
        controller.EnableSlopeLimit();
        controller.EnableClamping();
    }
    //默认状态的update
    void Idle_SuperUpdate()
    {
        // Run every frame we are in the idle state

        if (input.Current.JumpInput)
        {
            currentState = PlayerStates.Jump;
            return;
        }
        //悬空
        if (!MaintainingGround())
        {
            currentState = PlayerStates.Fall;
            return;
        }
        //走
        if (input.Current.MoveInput != Vector3.zero)
        {
            currentState = PlayerStates.Walk;
            return;
        }

        // Apply friction to slow us to a halt
        // 用摩擦来减缓我们的速度
        moveDirection = Vector3.MoveTowards(moveDirection, Vector3.zero, 10.0f * controller.deltaTime);
    }

    void Idle_ExitState()
    {
        // Run once when we exit the idle state
    }

    void Walk_SuperUpdate()
    {
        if (input.Current.JumpInput)
        {
            currentState = PlayerStates.Jump;
            return;
        }

        if (!MaintainingGround())
        {
            currentState = PlayerStates.Fall;
            return;
        }

        //操作状态的切换
        if (input.Current.MoveInput != Vector3.zero)
        {
            moveDirection = Vector3.MoveTowards(moveDirection, LocalMovement() * WalkSpeed, WalkAcceleration * controller.deltaTime);
        }
        else
        {
            currentState = PlayerStates.Idle;
            return;
        }
    }

    void Jump_EnterState()
    {
        controller.DisableClamping();
        controller.DisableSlopeLimit();

        moveDirection += controller.up * CalculateJumpSpeed(JumpHeight, Gravity);
    }

    void Jump_SuperUpdate()
    {
        Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
        Vector3 verticalMoveDirection = moveDirection - planarMoveDirection;

        if (Vector3.Angle(verticalMoveDirection, controller.up) > 90 && AcquiringGround())
        {
            moveDirection = planarMoveDirection;
            currentState = PlayerStates.Idle;
            return;            
        }

        planarMoveDirection = Vector3.MoveTowards(planarMoveDirection, LocalMovement() * WalkSpeed, JumpAcceleration * controller.deltaTime);
        verticalMoveDirection -= controller.up * Gravity * controller.deltaTime;

        moveDirection = planarMoveDirection + verticalMoveDirection;
    }

    void Fall_EnterState()
    {
        controller.DisableClamping();
        controller.DisableSlopeLimit();

        // moveDirection = trueVelocity;
    }

    void Fall_SuperUpdate()
    {
        if (AcquiringGround())
        {
            moveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
            currentState = PlayerStates.Idle;
            return;
        }

        moveDirection -= controller.up * Gravity * controller.deltaTime;
    }
}
