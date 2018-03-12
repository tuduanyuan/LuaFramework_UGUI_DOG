// External release version 2.0.0

using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Custom character controller, to be used by attaching the component to an object
/// and writing scripts attached to the same object that recieve the "SuperUpdate" message
/// </summary>
public class SuperCharacterController : MonoBehaviour
{
	//调试代码
    [SerializeField]
    Vector3 debugMove = Vector3.zero;
	//设置触发的对象
    [SerializeField]
    QueryTriggerInteraction triggerInteraction;//对象的交互的枚举
    //似乎要去驱动状态机，也是说这个状态机是耦合的

    [SerializeField]
    bool fixedTimeStep;//固定驱动次数

    [SerializeField]
    int fixedUpdatesPerSecond;//固定驱动频率

    [SerializeField]//钳住移动的物体，如果物体掉落在物体上的情况
    bool clampToMovingGround;
    //我不明白下面三个选项
    [SerializeField]
    bool debugSpheres;//在绘制可视化矩形

    [SerializeField]
    bool debugGrounding;//绘制一系列地形的信息

    [SerializeField]
    bool debugPushbackMesssages;//调试推出的时候的消息

    /// <summary>
    /// Describes the Transform of the object we are standing on as well as it's CollisionType, as well
    /// as how far the ground is below us and what angle it is in relation to the controller.
    /// </summary>
    /// 对地面的描述
    [SerializeField]
    public struct Ground
    {
        public RaycastHit hit { get; set; }
        public RaycastHit nearHit { get; set; }
        public RaycastHit farHit { get; set; }
        public RaycastHit secondaryHit { get; set; }
        public SuperCollisionType collisionType { get; set; }
        public Transform transform { get; set; }

        public Ground(RaycastHit hit, RaycastHit nearHit, RaycastHit farHit, RaycastHit secondaryHit, SuperCollisionType superCollisionType, Transform hitTransform)
        {
            this.hit = hit;
            this.nearHit = nearHit;
            this.farHit = farHit;
            this.secondaryHit = secondaryHit;
            this.collisionType = superCollisionType;
            this.transform = hitTransform;
        }
    }
    //体内三个碰撞体实体，这个可以设置，实际的碰撞体，在实际操作的时候需要把自己的给忽略掉
    [SerializeField]
    CollisionSphere[] spheres =
        new CollisionSphere[3] {
            new CollisionSphere(0.5f, true, false),
            new CollisionSphere(1.0f, false, false),
            new CollisionSphere(1.5f, false, true),
        };
    //能走的部分
    public LayerMask Walkable;
    //真正的实体，比如说趴下，或者蹲下，我都要改变超级角色控制器的实体，角色真实的样子
    [SerializeField]
    Collider ownCollider;
    //实体的半径，三个实体组成的大小进行搞一搞
    [SerializeField]
    public float radius = 0.5f;

    public float deltaTime { get; private set; }//每次更新的时间间隔
    public SuperGround currentGround { get; private set; }//当前所处的地面
    public CollisionSphere feet { get; private set; }//脚部
    public CollisionSphere head { get; private set; }//头部

    /// <summary>
    /// Total height of the controller from the bottom of the feet to the top of the head
    /// </summary>
    /// 控制器从底部到顶部的总高度
    public float height { get { return Vector3.Distance(SpherePosition(head), SpherePosition(feet)) + radius * 2; } }

    public Vector3 up { get { return transform.up; } }//朝向上
    public Vector3 down { get { return -transform.up; } }//朝向下

    public List<SuperCollision> collisionData { get; private set; }//获取到的碰撞体的信息
    public Transform currentlyClampedTo { get; set; }//当前钳住的地面
    public float heightScale { get; set; }//高度的缩放
    public float radiusScale { get; set; }//半径的缩放
    public bool manualUpdateOnly { get; set; }//手工update的开关

    public delegate void UpdateDelegate();
    public event UpdateDelegate AfterSingleUpdate;//一个步骤完成的update

    private Vector3 initialPosition;//初始化的位置
    private Vector3 groundOffset;//地形的偏移
    private Vector3 lastGroundPosition;//上一次地形的点
    private bool clamping = true;//钳住地面是否
    private bool slopeLimiting = true;//开启斜率检测

    private List<Collider> ignoredColliders;//忽略的碰撞体
    private List<IgnoredCollider> ignoredColliderStack;//忽略的碰撞体的栈

    private const float Tolerance = 0.05f;//公差
    private const float TinyTolerance = 0.01f;//维差
    private const string TemporaryLayer = "TempCast";
    private const int MaxPushbackIterations = 2;//最大的阻尼
    private int TemporaryLayerIndex;//临时的层的东西
    private float fixedDeltaTime;

    private static SuperCollisionType defaultCollisionType;//对碰撞体的描述描述

    void Awake()
    {
        collisionData = new List<SuperCollision>();

        TemporaryLayerIndex = LayerMask.NameToLayer(TemporaryLayer);

        ignoredColliders = new List<Collider>();
        ignoredColliderStack = new List<IgnoredCollider>();

        currentlyClampedTo = null;

        fixedDeltaTime = 1.0f / fixedUpdatesPerSecond;

        heightScale = 1.0f;

        if (ownCollider)//把自己的碰撞体忽略
            IgnoreCollider(ownCollider);


		//分别找到各个部分的碰撞体
        foreach (var sphere in spheres)
        {
            if (sphere.isFeet)
                feet = sphere;

            if (sphere.isHead)
                head = sphere;
        }

        if (feet == null)
            Debug.LogError("[SuperCharacterController] Feet not found on controller");

        if (head == null)
            Debug.LogError("[SuperCharacterController] Head not found on controller");

        if (defaultCollisionType == null)
            defaultCollisionType = new GameObject("DefaultSuperCollisionType", typeof(SuperCollisionType)).GetComponent<SuperCollisionType>();

        currentGround = new SuperGround(Walkable, this, triggerInteraction);//拿到可以进行移动的信息？

        manualUpdateOnly = false;//同步的话需要看这个，如果是真实的同步又应该是什么样子呢


        gameObject.SendMessage("SuperStart", SendMessageOptions.DontRequireReceiver);
    }

    void Update()
    {
        // If we are using a fixed timestep, ensure we run the main update loop
        // a sufficient number of times based on the Time.deltaTime
        //判断是不是手动执行
        if (manualUpdateOnly)
            return;

        if (!fixedTimeStep)
        {
            //如果不是间隔执行
            deltaTime = Time.deltaTime;
            //执行单步
            SingleUpdate();
            return;
        }
        else
        {
            //如果是按照固定间隔执行
            float delta = Time.deltaTime;

            while (delta > fixedDeltaTime)
            {
                deltaTime = fixedDeltaTime;

                SingleUpdate();

                delta -= fixedDeltaTime;
            }

            if (delta > 0f)
            {
                deltaTime = delta;
                //可能会有积累
                SingleUpdate();
            }
        }
    }
    //手动执行
    public void ManualUpdate(float deltaTime)
    {
        this.deltaTime = deltaTime;

        SingleUpdate();
    }
    //每次update执行的单步
    void SingleUpdate()
    {
        // Check if we are clamped to an object implicity or explicity
        //如果钳住地面或者当前有钳住的对象
        bool isClamping = clamping || currentlyClampedTo != null;

        //如果有当前钳住的对象获得他的transform
        Transform clampedTo = currentlyClampedTo != null ? currentlyClampedTo : currentGround.transform;
        //1可以钳住移动物体
        //正在钳住对象
        //钳住到某个位置
        //2个地面的物体的距离不等于0
        //当前的位置加上距离
        if (clampToMovingGround && isClamping && clampedTo != null && clampedTo.position - lastGroundPosition != Vector3.zero)
            transform.position += clampedTo.position - lastGroundPosition;
        //初始位置
        initialPosition = transform.position;
        //检查地面 1
        ProbeGround(1);
        //调试移动 这句代码是是第一句代码，移动 反推 处理 过程的第一步，假设的话
        transform.position += debugMove * deltaTime;
        //对子节点的对象发消息SuperUpdate
        gameObject.SendMessage("SuperUpdate", SendMessageOptions.DontRequireReceiver);
        //清空 
        collisionData.Clear();
        //递归的阻力
        RecursivePushback(0, MaxPushbackIterations);
        //检查地面
        ProbeGround(2);
        //如果开启了斜率生效
        if (slopeLimiting)
            SlopeLimit();
        //检查地面3
        ProbeGround(3);
        //钳住地面？到底什么是钳住地面？
        if (clamping)
            ClampToGround();

        isClamping = clamping || currentlyClampedTo != null;
        clampedTo = currentlyClampedTo != null ? currentlyClampedTo : currentGround.transform;

        if (isClamping)
            lastGroundPosition = clampedTo.position;

        if (debugGrounding)
            currentGround.DebugGround(true, true, true, true, true);
        // 经过一个阶段
        if (AfterSingleUpdate != null)
            AfterSingleUpdate();//这里面update的东西就多了
    }
    //检查
    void ProbeGround(int iter)
    {
        PushIgnoredColliders();
        currentGround.ProbeGround(SpherePosition(feet), iter);
        PopIgnoredColliders();
    }

    /// <summary>
    /// 防止玩家在一个比物体倾斜的角度更大的角度上行走。
    /// Prevents the player from walking up slopes of a larger angle than the object's SlopeLimit.
    /// </summary>
    /// <returns>True if the controller attemped to ascend a too steep slope and had their movement limited</returns>
    bool SlopeLimit()
    {
        Vector3 n = currentGround.PrimaryNormal();
        float a = Vector3.Angle(n, up);

        if (a > currentGround.superCollisionType.SlopeLimit)
        {
            Vector3 absoluteMoveDirection = Math3d.ProjectVectorOnPlane(n, transform.position - initialPosition);

            // Retrieve a vector pointing down the slope
            Vector3 r = Vector3.Cross(n, down);
            Vector3 v = Vector3.Cross(r, n);

            float angle = Vector3.Angle(absoluteMoveDirection, v);

            if (angle <= 90.0f)
                return false;

            // Calculate where to place the controller on the slope, or at the bottom, based on the desired movement distance
            Vector3 resolvedPosition = Math3d.ProjectPointOnLine(initialPosition, r, transform.position);
            Vector3 direction = Math3d.ProjectVectorOnPlane(n, resolvedPosition - transform.position);

            RaycastHit hit;

            // Check if our path to our resolved position is blocked by any colliders
            if (Physics.CapsuleCast(SpherePosition(feet), SpherePosition(head), radius, direction.normalized, out hit, direction.magnitude, Walkable, triggerInteraction))
            {
                transform.position += v.normalized * hit.distance;
            }
            else
            {
                transform.position += direction;
            }

            return true;
        }

        return false;
    }

    void ClampToGround()
    {
        float d = currentGround.Distance();
        transform.position -= up * d;
    }

    public void EnableClamping()
    {
        clamping = true;
    }

    public void DisableClamping()
    {
        clamping = false;
    }

    public void EnableSlopeLimit()
    {
        slopeLimiting = true;
    }

    public void DisableSlopeLimit()
    {
        slopeLimiting = false;
    }

    public bool IsClamping()
    {
        return clamping;
    }

    /// <summary>
    /// Check if any of the CollisionSpheres are colliding with any walkable objects in the world.
    /// If they are, apply a proper pushback and retrieve the collision data
    /// </summary>
    void RecursivePushback(int depth, int maxDepth)
    {
        //加添忽略的对象
        PushIgnoredColliders();
        //接触标志
        bool contact = false;

        foreach (var sphere in spheres)
        {
            //用三个物体来进行检查
            foreach (Collider col in Physics.OverlapSphere((SpherePosition(sphere)), radius, Walkable, triggerInteraction))
            {
                Vector3 position = SpherePosition(sphere);
                Vector3 contactPoint;
                bool contactPointSuccess = SuperCollider.ClosestPointOnSurface(col, position, radius, out contactPoint);
                
                if (!contactPointSuccess)
                {
                    return;
                }
                                            
                if (debugPushbackMesssages)
                    DebugDraw.DrawMarker(contactPoint, 2.0f, Color.cyan, 0.0f, false);
                    
                Vector3 v = contactPoint - position;
                if (v != Vector3.zero)
                {
                    // Cache the collider's layer so that we can cast against it
                    int layer = col.gameObject.layer;

                    col.gameObject.layer = TemporaryLayerIndex;

                    // Check which side of the normal we are on
                    bool facingNormal = Physics.SphereCast(new Ray(position, v.normalized), TinyTolerance, v.magnitude + TinyTolerance, 1 << TemporaryLayerIndex);

                    col.gameObject.layer = layer;

                    // Orient and scale our vector based on which side of the normal we are situated
                    if (facingNormal)
                    {
                        if (Vector3.Distance(position, contactPoint) < radius)
                        {
                            v = v.normalized * (radius - v.magnitude) * -1;
                        }
                        else
                        {
                            // A previously resolved collision has had a side effect that moved us outside this collider
                            continue;
                        }
                    }
                    else
                    {
                        v = v.normalized * (radius + v.magnitude);
                    }

                    contact = true;

                    transform.position += v;

                    col.gameObject.layer = TemporaryLayerIndex;

                    // Retrieve the surface normal of the collided point
                    RaycastHit normalHit;

                    Physics.SphereCast(new Ray(position + v, contactPoint - (position + v)), TinyTolerance, out normalHit, 1 << TemporaryLayerIndex);

                    col.gameObject.layer = layer;

                    SuperCollisionType superColType = col.gameObject.GetComponent<SuperCollisionType>();

                    if (superColType == null)
                        superColType = defaultCollisionType;

                    // Our collision affected the collider; add it to the collision data
                    var collision = new SuperCollision()
                    {
                        collisionSphere = sphere,
                        superCollisionType = superColType,
                        gameObject = col.gameObject,
                        point = contactPoint,
                        normal = normalHit.normal
                    };

                    collisionData.Add(collision);
                }
            }            
        }

        PopIgnoredColliders();

        if (depth < maxDepth && contact)
        {
            RecursivePushback(depth + 1, maxDepth);
        }
    }

    protected struct IgnoredCollider
    {
        public Collider collider;
        public int layer;

        public IgnoredCollider(Collider collider, int layer)
        {
            this.collider = collider;
            this.layer = layer;
        }
    }
    //把忽略的对象加入进去
    private void PushIgnoredColliders()
    {
        ignoredColliderStack.Clear();

        for (int i = 0; i < ignoredColliders.Count; i++)
        {
            Collider col = ignoredColliders[i];
            ignoredColliderStack.Add(new IgnoredCollider(col, col.gameObject.layer));
            col.gameObject.layer = TemporaryLayerIndex;
        }
    }

    private void PopIgnoredColliders()
    {
        for (int i = 0; i < ignoredColliderStack.Count; i++)
        {
            IgnoredCollider ic = ignoredColliderStack[i];
            ic.collider.gameObject.layer = ic.layer;
        }

        ignoredColliderStack.Clear();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (debugSpheres)
        {
            if (spheres != null)
            {
                if (heightScale == 0) heightScale = 1;

                foreach (var sphere in spheres)
                {
                    Gizmos.color = sphere.isFeet ? Color.green : (sphere.isHead ? Color.yellow : Color.cyan);
                    Gizmos.DrawWireSphere(SpherePosition(sphere), radius);
                }
            }
        }
    }
    //为什么只检查了脚，加上偏移量
    public Vector3 SpherePosition(CollisionSphere sphere)
    {
        if (sphere.isFeet)
            return transform.position + sphere.offset * up;
        else
            return transform.position + sphere.offset * up * heightScale;
    }

    public bool PointBelowHead(Vector3 point)
    {
        return Vector3.Angle(point - SpherePosition(head), up) > 89.0f;
    }

    public bool PointAboveFeet(Vector3 point)
    {
        return Vector3.Angle(point - SpherePosition(feet), down) > 89.0f;
    }

    public void IgnoreCollider(Collider col)
    {
        ignoredColliders.Add(col);
    }

    public void RemoveIgnoredCollider(Collider col)
    {
        ignoredColliders.Remove(col);
    }

    public void ClearIgnoredColliders()
    {
        ignoredColliders.Clear();
    }
    /// <summary>
    /// Super ground.
    /// 这个是一个全新的类，对地形的描述？
	/// 为什么要自己来创建这个，而不是分别绑定到相应的地方去
	/// </summary>
    public class SuperGround
    {
        public SuperGround(LayerMask walkable, SuperCharacterController controller, QueryTriggerInteraction triggerInteraction)
        {
            this.walkable = walkable;
            this.controller = controller;
            this.triggerInteraction = triggerInteraction;
        }

        private class GroundHit
        {
            public Vector3 point { get; private set; }
            public Vector3 normal { get; private set; }
            public float distance { get; private set; }

            public GroundHit(Vector3 point, Vector3 normal, float distance)
            {
                this.point = point;
                this.normal = normal;
                this.distance = distance;
            }
        }

        private LayerMask walkable;//可以行走的mask层
        private SuperCharacterController controller;//
        private QueryTriggerInteraction triggerInteraction;
        
        private GroundHit primaryGround;//主要的地面
        private GroundHit nearGround;//近的地面
        private GroundHit farGround;//远的地面
        private GroundHit stepGround;//踩的地面
        private GroundHit flushGround;//？？？

        public SuperCollisionType superCollisionType { get; private set; }
        public Transform transform { get; private set; }

        private const float groundingUpperBoundAngle = 60.0f;
        private const float groundingMaxPercentFromCenter = 0.85f;
        private const float groundingMinPercentFromcenter = 0.50f;

        /// <summary>
        /// Scan the surface below us for ground. Follow up the initial scan with subsequent scans
        /// designed to test what kind of surface we are standing above and handle different edge cases
        /// 扫描地面下方的表面。 通过随后的扫描进行初始扫描，以便测试我们站在上面的哪种表面，并处理不同的边缘情况
        /// </summary>
        /// <param name="origin">Center of the sphere for the initial SphereCast</param>
        /// <param name="iter">Debug tool to print out which ProbeGround iteration is being run (3 are run each frame for the controller)</param>
        public void ProbeGround(Vector3 origin, int iter)
        {
            ResetGrounds();

            Vector3 up = controller.up;
            Vector3 down = -up;
            //略微向上偏移
            Vector3 o = origin + (up * Tolerance);

            // Reduce our radius by Tolerance squared to avoid failing the SphereCast due to clipping with walls
            // 通过公差平方减少我们的半径，避免由于墙壁剪切而导致SphereCast失败
            float smallerRadius = controller.radius - (Tolerance * Tolerance);

            RaycastHit hit;
            //圆形匹配 单一
            if (Physics.SphereCast(o, smallerRadius, down, out hit, Mathf.Infinity, walkable, triggerInteraction))
            {
                var superColType = hit.collider.gameObject.GetComponent<SuperCollisionType>();

                if (superColType == null)
                {
                    superColType = defaultCollisionType;
                }

                superCollisionType = superColType;
                transform = hit.transform;

                // By reducing the initial SphereCast's radius by Tolerance, our casted sphere no longer fits with
                // our controller's shape. Reconstruct the sphere cast with the proper radius

                //通过容差减少初始SphereCast的半径，我们的铸造球体不再符合我们控制器的形状。 重建以适当半径投射的球体
                //软件的翻译，我不太明白
                //Simulate：模仿----》实际上对hit进行了微微的调整，具体为什么我还得看看文档
                SimulateSphereCast(hit.normal, out hit);
                //整理地面信息，当前位置的，进行描述
                primaryGround = new GroundHit(hit.point, hit.normal, hit.distance);

                // If we are standing on a perfectly flat surface, we cannot be either on an edge,
                // On a slope or stepping off a ledge
                if (Vector3.Distance(Math3d.ProjectPointOnPlane(controller.up, controller.transform.position, hit.point), controller.transform.position) < TinyTolerance)
                {
                    //为什么在投影点和位置很近的时候
                    //就什么什么的了？
                    //如果我们站在一个完美平坦的表面上
                    return;
                }

                // As we are standing on an edge, we need to retrieve the normals of the two
                // faces on either side of the edge and store them in nearHit and farHit
                // 当我们站在边缘时，我们需要检索边缘两侧的两个面的法线并将它们存储在nearHit和farHit

                //中心 算的一个平面的投影
                Vector3 toCenter = Math3d.ProjectVectorOnPlane(up, (controller.transform.position - hit.point).normalized * TinyTolerance);
                //完全不知道什么原理的样子
                Vector3 awayFromCenter = Quaternion.AngleAxis(-80.0f, Vector3.Cross(toCenter, up)) * -toCenter;

                Vector3 nearPoint = hit.point + toCenter + (up * TinyTolerance);
                Vector3 farPoint = hit.point + (awayFromCenter * 3);

                RaycastHit nearHit;
                RaycastHit farHit;

                Physics.Raycast(nearPoint, down, out nearHit, Mathf.Infinity, walkable, triggerInteraction);
                Physics.Raycast(farPoint, down, out farHit, Mathf.Infinity, walkable, triggerInteraction);

                nearGround = new GroundHit(nearHit.point, nearHit.normal, nearHit.distance);
                farGround = new GroundHit(farHit.point, farHit.normal, farHit.distance);

                // If we are currently standing on ground that should be counted as a wall,
                // we are likely flush against it on the ground. Retrieve what we are standing on
                if (Vector3.Angle(hit.normal, up) > superColType.StandAngle)
                {
                    // Retrieve a vector pointing down the slope
                    Vector3 r = Vector3.Cross(hit.normal, down);
                    Vector3 v = Vector3.Cross(r, hit.normal);

                    Vector3 flushOrigin = hit.point + hit.normal * TinyTolerance;

                    RaycastHit flushHit;

                    if (Physics.Raycast(flushOrigin, v, out flushHit, Mathf.Infinity, walkable, triggerInteraction))
                    {
                        RaycastHit sphereCastHit;

                        if (SimulateSphereCast(flushHit.normal, out sphereCastHit))
                        {
                            flushGround = new GroundHit(sphereCastHit.point, sphereCastHit.normal, sphereCastHit.distance);
                        }
                        else
                        {
                            // Uh oh
                        }
                    }
                }

                // If we are currently standing on a ledge then the face nearest the center of the
                // controller should be steep enough to be counted as a wall. Retrieve the ground
                // it is connected to at it's base, if there exists any
                if (Vector3.Angle(nearHit.normal, up) > superColType.StandAngle || nearHit.distance > Tolerance)
                {
                    SuperCollisionType col = null;
                
                    if (nearHit.collider != null)
                    {
                        col = nearHit.collider.gameObject.GetComponent<SuperCollisionType>();
                    }
                    
                    if (col == null)
                    {
                        col = defaultCollisionType;
                    }

                    // We contacted the wall of the ledge, rather than the landing. Raycast down
                    // the wall to retrieve the proper landing
                    if (Vector3.Angle(nearHit.normal, up) > col.StandAngle)
                    {
                        // Retrieve a vector pointing down the slope
                        Vector3 r = Vector3.Cross(nearHit.normal, down);
                        Vector3 v = Vector3.Cross(r, nearHit.normal);

                        RaycastHit stepHit;

                        if (Physics.Raycast(nearPoint, v, out stepHit, Mathf.Infinity, walkable, triggerInteraction))
                        {
                            stepGround = new GroundHit(stepHit.point, stepHit.normal, stepHit.distance);
                        }
                    }
                    else
                    {
                        stepGround = new GroundHit(nearHit.point, nearHit.normal, nearHit.distance);
                    }
                }
            }
            // If the initial SphereCast fails, likely due to the controller clipping a wall,
            // fallback to a raycast simulated to SphereCast data
            else if (Physics.Raycast(o, down, out hit, Mathf.Infinity, walkable, triggerInteraction))
            {
                var superColType = hit.collider.gameObject.GetComponent<SuperCollisionType>();

                if (superColType == null)
                {
                    superColType = defaultCollisionType;
                }

                superCollisionType = superColType;
                transform = hit.transform;

                RaycastHit sphereCastHit;

                if (SimulateSphereCast(hit.normal, out sphereCastHit))
                {
                    primaryGround = new GroundHit(sphereCastHit.point, sphereCastHit.normal, sphereCastHit.distance);
                }
                else
                {
                    primaryGround = new GroundHit(hit.point, hit.normal, hit.distance);
                }
            }
            else
            {
                Debug.LogError("[SuperCharacterComponent]: No ground was found below the player; player has escaped level");
            }
        }

        private void ResetGrounds()
        {
            primaryGround = null;
            nearGround = null;
            farGround = null;
            flushGround = null;
            stepGround = null;
        }

        public bool IsGrounded(bool currentlyGrounded, float distance)
        {
            Vector3 n;
            return IsGrounded(currentlyGrounded, distance, out n);
        }

        public bool IsGrounded(bool currentlyGrounded, float distance, out Vector3 groundNormal)
        {
            groundNormal = Vector3.zero;

            if (primaryGround == null || primaryGround.distance > distance)
            {
                return false;
            }

            // Check if we are flush against a wall
            if (farGround != null && Vector3.Angle(farGround.normal, controller.up) > superCollisionType.StandAngle)
            {
                if (flushGround != null && Vector3.Angle(flushGround.normal, controller.up) < superCollisionType.StandAngle && flushGround.distance < distance)
                {
                    groundNormal = flushGround.normal;
                    return true;
                }

                return false;
            }

            // Check if we are at the edge of a ledge, or on a high angle slope
            if (farGround != null && !OnSteadyGround(farGround.normal, primaryGround.point))
            {
                // Check if we are walking onto steadier ground
                if (nearGround != null && nearGround.distance < distance && Vector3.Angle(nearGround.normal, controller.up) < superCollisionType.StandAngle && !OnSteadyGround(nearGround.normal, nearGround.point))
                {
                    groundNormal = nearGround.normal;
                    return true;
                }

                // Check if we are on a step or stair
                if (stepGround != null && stepGround.distance < distance && Vector3.Angle(stepGround.normal, controller.up) < superCollisionType.StandAngle)
                {
                    groundNormal = stepGround.normal;
                    return true;
                }

                return false;
            }


            if (farGround != null)
            {
                groundNormal = farGround.normal;
            }
            else
            {
                groundNormal = primaryGround.normal;
            }

            return true;
        }

        /// <summary>
        /// To help the controller smoothly "fall" off surfaces and not hang on the edge of ledges,
        /// check that the ground below us is "steady", or that the controller is not standing
        /// on too extreme of a ledge
        /// </summary>
        /// <param name="normal">Normal of the surface to test against</param>
        /// <param name="point">Point of contact with the surface</param>
        /// <returns>True if the ground is steady</returns>
        private bool OnSteadyGround(Vector3 normal, Vector3 point)
        {
            float angle = Vector3.Angle(normal, controller.up);

            float angleRatio = angle / groundingUpperBoundAngle;

            float distanceRatio = Mathf.Lerp(groundingMinPercentFromcenter, groundingMaxPercentFromCenter, angleRatio);

            Vector3 p = Math3d.ProjectPointOnPlane(controller.up, controller.transform.position, point);

            float distanceFromCenter = Vector3.Distance(p, controller.transform.position);

            return distanceFromCenter <= distanceRatio * controller.radius;
        }

        public Vector3 PrimaryNormal()
        {
            return primaryGround.normal;
        }

        public float Distance()
        {
            return primaryGround.distance;
        }

        public void DebugGround(bool primary, bool near, bool far, bool flush, bool step)
        {
            if (primary && primaryGround != null)
            {
                DebugDraw.DrawVector(primaryGround.point, primaryGround.normal, 2.0f, 1.0f, Color.yellow, 0, false);
            }

            if (near && nearGround != null)
            {
                DebugDraw.DrawVector(nearGround.point, nearGround.normal, 2.0f, 1.0f, Color.blue, 0, false);
            }

            if (far && farGround != null)
            {
                DebugDraw.DrawVector(farGround.point, farGround.normal, 2.0f, 1.0f, Color.red, 0, false);
            }

            if (flush && flushGround != null)
            {
                DebugDraw.DrawVector(flushGround.point, flushGround.normal, 2.0f, 1.0f, Color.cyan, 0, false);
            }

            if (step && stepGround != null)
            {
                DebugDraw.DrawVector(stepGround.point, stepGround.normal, 2.0f, 1.0f, Color.green, 0, false);
            }
        }

        /// <summary>
        /// Provides raycast data based on where a SphereCast would contact the specified normal
        /// Raycasting downwards from a point along the controller's bottom sphere, based on the provided
        /// normal
        /// </summary>
        /// <param name="groundNormal">Normal of a triangle assumed to be directly below the controller</param>
        /// <param name="hit">Simulated SphereCast data</param>
        /// <returns>True if the raycast is successful</returns>
        private bool SimulateSphereCast(Vector3 groundNormal, out RaycastHit hit)
        {
            //取夹角
            float groundAngle = Vector3.Angle(groundNormal, controller.up) * Mathf.Deg2Rad;
            //当前的位置
            Vector3 secondaryOrigin = controller.transform.position + controller.up * Tolerance;

            if (!Mathf.Approximately(groundAngle, 0))
            {
                //如果不近似（这个是啥子意思）
                float horizontal = Mathf.Sin(groundAngle) * controller.radius;
                float vertical = (1.0f - Mathf.Cos(groundAngle)) * controller.radius;

                // Retrieve a vector pointing up the slope
                Vector3 r2 = Vector3.Cross(groundNormal, controller.down);
                Vector3 v2 = -Vector3.Cross(r2, groundNormal);


                //检索一个指向斜率的矢量
                secondaryOrigin += Math3d.ProjectVectorOnPlane(controller.up, v2).normalized * horizontal + controller.up * vertical;
            }
            
            if (Physics.Raycast(secondaryOrigin, controller.down, out hit, Mathf.Infinity, walkable, triggerInteraction))
            {
                // Remove the tolerance from the distance travelled
                hit.distance -= Tolerance + TinyTolerance;
                //最后的作用就是改变了一个公差和维差的距离
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

[Serializable]
public class CollisionSphere
{
    public float offset;
    public bool isFeet;
    public bool isHead;

    public CollisionSphere(float offset, bool isFeet, bool isHead)
    {
        this.offset = offset;
        this.isFeet = isFeet;
        this.isHead = isHead;
    }
}

public struct SuperCollision
{
    public CollisionSphere collisionSphere;
    public SuperCollisionType superCollisionType;
    public GameObject gameObject;
    public Vector3 point;
    public Vector3 normal;
}
