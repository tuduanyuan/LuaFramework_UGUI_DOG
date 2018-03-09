using UnityEngine;
using System.Collections;

/// <summary>
/// 这个是一个碰撞体的描述，可以是地形可以是障碍，可以是XX
/// Extend this class to add in any further data you want to be able to access
/// pertaining to an object the controller has collided with
/// 扩展此课程以添加您想要访问的任何其他数据
/// 属于控制器发生碰撞的对象扩展此类以添加任何内容
/// </summary>
public class SuperCollisionType : MonoBehaviour {

    public float StandAngle = 80.0f;//站立的角度
    public float SlopeLimit = 80.0f;//坡的极限状态
}
