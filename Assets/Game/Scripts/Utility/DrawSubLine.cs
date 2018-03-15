using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace XGame {
    
    //这个面片必须时刻保持
    //先不管我画的对不对，我就只给2个坐标， 然后把这个坐标放置到世界坐标系里去，使用Drawmesh

    public class DrawSubLine : MonoBehaviour
    {


        public float lineSize = 0.1f;//线的宽度
        public Shader shader;//线使用的shader

        public Vector2 size = new Vector2(100,100);//我整体的面板打大小

        private Mesh ml;
        private Material lmat;//线条的材质

        public bool isDebug;

        private void Awake()
        {
            ml = new Mesh();
            lmat = new Material(shader);
            lmat.color = new Color(0, 0, 0, 1f);
        }
        // Use this for initialization
        void Start()
        {
            //初始化材质
            var s = new Vector3(0, 0, 10);
            var e = new Vector3(10, 0, 20);
            AddLine(ml, MakeQuad(s, e, lineSize));
            Debug.DrawLine(s, e);
            DrawMesh();
        }
        private void Update()
        {
            //DrawMesh();
            var s = new Vector3(0, 0, 10);
            var e = new Vector3(10, 0, 20);
            //Debug.DrawLine(s, e,Color.red,1);
        }
        private void InitData()
        {
            var t_width = Mathf.RoundToInt(size.x);
            var t_heigh = Mathf.RoundToInt(size.y);
            for (int i = 0; i<= t_width && t_width != 0; i++)
            {

                //AddLine(ms, MakeQuad(s, e, lineSize));
            }

            for (int i = 0; i <= t_heigh && t_heigh != 0; i++)
            {
                //var s = new Vector3(0 * lineUnitLength, 0.1f, i * lineUnitLength);
                //var e = new Vector3(t_width * lineUnitLength, 0.1f, i * lineUnitLength);
                //logger.debug("s:" + s.ToString() + " e:" + e.ToString());
                //AddLine(ml, MakeQuad(s, e, lineSize));
                //DrawMesh();
            }
        }

        private void DrawMesh()
        {
            //Graphics.DrawMeshNow
            Graphics.DrawMesh(ml, transform.localToWorldMatrix, lmat, 0);
            //Graphics.DrawMesh(ms, transform.localToWorldMatrix, smat, 0);
        }

        private void ShowLine()
        {
            
        }

        void AddLine(Mesh m, Vector3[] quad)
        {
            m.Clear();
            int vl = m.vertices.Length;
            Vector3[] vs = m.vertices;
            vs = resizeVertices(vs, 4);
            
            vs[vl] = quad[0];
            vs[vl + 1] = quad[1];
            vs[vl + 2] = quad[2];
            vs[vl + 3] = quad[3];
            var ts = m.triangles;
            ts = resizeTraingles(ts, 6);
            ts[0] = vl + 0;
            ts[1] = vl + 2;
            ts[2] = vl + 1;
            ts[3] = vl + 1;
            ts[4] = vl + 2;
            ts[5] = vl + 3;

            m.vertices = vs;
            m.triangles = ts;
            m.RecalculateBounds();
        }

        Vector3[] resizeVertices(Vector3[] ovs, int ns)
        {
            Vector3[] nvs = new Vector3[ovs.Length + ns];
            for (int i = 0; i < ovs.Length; i++) nvs[i] = ovs[i];
            return nvs;
        }

        int[] resizeTraingles(int[] ovs, int ns)
        {
            int[] nvs = new int[ovs.Length + ns];
            for (int i = 0; i < ovs.Length; i++) nvs[i] = ovs[i];
            return nvs;
        }

        Vector3[] MakeQuad(Vector3 s, Vector3 e, float w)
        {

            logger.debug("s:" + s.ToString() + " e:" + e.ToString() + " w:" + w.ToString());
            w = w / 2.0f;
            Vector3[] q = new Vector3[4];

            Vector3 c = Camera.main.transform.forward;
            //Vector3 n = Vector3.Cross(c, e);
            Vector3 l = Vector3.Cross(c, e - s);
            l.Normalize();
            logger.debug("c:" + c.ToString() + " l:" + l.ToString());
            q[0] = s + l * w;
            q[1] = s + l * -w;
            q[2] = e + l * w;
            q[3] = e + l * -w;
            var str = "";
            foreach(var i in q)
            {
                str += i.ToString() + " ";
            }
            logger.debug("q:" + str);
            return q;
        }
    }

}

