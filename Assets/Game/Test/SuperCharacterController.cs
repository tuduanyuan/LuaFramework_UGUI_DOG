using UnityEngine;
using System;
using System.Collections.Generic;
namespace Test {

    public class SuperCharacterController : MonoBehaviour
    {

        [SerializeField]
        float radius = 0.5f;

        private bool contact;

        // Update is called once per frame
        void Update()
        {

            contact = false;

            foreach (Collider col in Physics.OverlapSphere(transform.position, radius))
            {
                Vector3 contactPoint = col.ClosestPointOnBounds(transform.position);
                //始终在找离物体表面最近的一个点
                if (col is BoxCollider)
                {
                    contactPoint = ClosestPointOn((BoxCollider)col, transform.position);
                }
                else if (col is SphereCollider)
                {
                    contactPoint = ClosestPointOn((SphereCollider)col, transform.position);
                }




                DebugDraw.DrawMarker(contactPoint, 2.0f, Color.red, 0.0f, false);

                Vector3 v = transform.position - contactPoint;

                transform.position += Vector3.ClampMagnitude(v, Mathf.Clamp(radius - v.magnitude, 0, radius));

                contact = true;
            }
        }

        Vector3 ClosestPointOn(BoxCollider collider, Vector3 to)
        {
            if (collider.transform.rotation == Quaternion.identity)
            {
                return collider.ClosestPointOnBounds(to);
            }

            return closestPointOnOBB(collider, to);
        }

        Vector3 closestPointOnOBB(BoxCollider collider, Vector3 to)
        {
            //把一个坐标转换到内部来

            //按照内部坐标处理

            //然后找到 中线点到撞击点的向量
            //进行剪裁，到obb盒子的表面点，
            //再转世界坐标
            // Cache the collider transform
            var ct = collider.transform;

            // Firstly, transform the point into the space of the collider
            //首先，将这个点转换为对撞机的空间
            var local = ct.InverseTransformPoint(to);

            // Now, shift it to be in the center of the box
            //一般是000 ab = b - a 这个是一个向量
            local = local - collider.center;

            // Inverse scale it by the colliders scale
            var localNorm =
            new Vector3(
            Mathf.Clamp(local.x, -collider.size.x * 0.5f, collider.size.x * 0.5f),
            Mathf.Clamp(local.y, -collider.size.y * 0.5f, collider.size.y * 0.5f),
            Mathf.Clamp(local.z, -collider.size.z * 0.5f, collider.size.z * 0.5f)
            );

            // Now we undo our transformations
            localNorm += collider.center;

            // Return resulting point
            return ct.TransformPoint(localNorm);
        }



        Vector3 ClosestPointOn(SphereCollider collider, Vector3 to)
        {
            Vector3 p;
            //a b = b - a;
            //起点是碰撞体的位置，终点是我自己的位置
            p = to - collider.transform.position;
            p.Normalize();

            p *= collider.radius * collider.transform.localScale.x;

            //已经到达了表面的向量

            //ab = a + b;
            p += collider.transform.position;
            //获取到空间最近的那个点
            return p;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = contact ? Color.cyan : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }

}

