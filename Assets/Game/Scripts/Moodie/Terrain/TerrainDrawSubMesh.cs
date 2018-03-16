using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGame {
    [RequireComponent(typeof(TerrainInfo))]
    public class TerrainDrawSubMesh : MonoBehaviour
    {
        class XLine
        {
            public Vector3 s;
            public Vector3 e;

            public XLine(Vector3 s, Vector3 e)
            {
                this.s = s;
                this.e = e;
            }
        }
        List<XLine> m_Lines;//线的信息

        public float lineSize = 0.1f;//线的宽度
        public Shader shader;//线使用的shader
        [SerializeField]
        public bool isShow;
        private Mesh ml;
        private Material lmat;//线条的材质
        private bool m_Init = false;
        private Vector3 lastCameraFow = Vector3.zero;
        private Vector3 lastPoint = Vector3.zero;
        private int setMeshCount = 0;
        //private int lineCount = 0;
        private void Awake()
        {
            ml = new Mesh();
            lmat = new Material(shader);
            lmat.color = new Color(0, 0, 0, 1f);
        }
        // Use this for initialization
        void Start()
        {
            do
            {
                if (GetData() == false) break;
                m_Init = true;
            } while (false);       
        }

        private void SetMesh()
        {
            var c = Camera.main.transform.forward;
            var p = Camera.main.transform.transform.position;
            if (setMeshCount > 0 && c == lastCameraFow  && lastPoint == p) return;
            ml.Clear();
            
            int len = m_Lines.Count;
            for (int i = 0; i < len; i++)
            {
                var l = m_Lines[i];
                AddLine(ml, MakeQuad(c, p, l.s, l.e, lineSize));
            }
            ml.RecalculateBounds();
            lastCameraFow = c;
            setMeshCount++;
        }

        private void Update()
        {
            DrawMesh();
        }
   
        private bool GetData()
        {
            var info = this.GetComponent<TerrainInfo>();
            if (info.TerrainWidth == 0 || info.TerrainHeight == 0) return false;
            if (m_Lines != null)
                m_Lines = null;
            m_Lines = new List<XLine>();
            var interval = 1;
            var z = 0;//这个特殊处理
            for (int i = 0;i <= info.TerrainWidth;i++)
            {
                var p = i * interval;
                var s = new Vector3(p, z,0);
                var e = new Vector3(p, z, info.TerrainHeight * interval);
                AddLine(s,e);
            }
            for (int i = 0; i <= info.TerrainHeight; i++)
            {
                var p = i * interval;
                var s = new Vector3(0, z, p);
                var e = new Vector3(info.TerrainWidth * interval, z, p);
                AddLine(s, e);
            }
            return true;
        }

        private void DrawMesh()
        {
            if (!m_Init || !isShow) return;
            SetMesh();
            Graphics.DrawMesh(ml, transform.localToWorldMatrix, lmat, 0);
        }

        void AddLine(Vector3 s, Vector3 e)
        {
            m_Lines.Add(new XLine(s, e));
        }

        void AddLine(Mesh m, Vector3[] quad, bool tmp = false)
        {
            int vl = m.vertices.Length;

            Vector3[] vs = m.vertices;
            if (!tmp || vl == 0) vs = resizeVertices(vs, 4);
            else vl -= 4;

            vs[vl] = quad[0];
            vs[vl + 1] = quad[1];
            vs[vl + 2] = quad[2];
            vs[vl + 3] = quad[3];

            int tl = m.triangles.Length;

            int[] ts = m.triangles;
            if (!tmp || tl == 0) ts = resizeTraingles(ts, 6);
            else tl -= 6;
            ts[tl] = vl + 0;
            ts[tl + 1] = vl + 2;
            ts[tl + 2] = vl + 1;
            ts[tl + 3] = vl + 1;
            ts[tl + 4] = vl + 2;
            ts[tl + 5] = vl + 3;

            m.vertices = vs;
            m.triangles = ts;
            
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
        //这个函数就算表面，我要表面法线
        Vector3[] MakeQuad(Vector3 c , Vector3 p , Vector3 s, Vector3 e, float w)
        {

            //logger.debug("s:" + s.ToString() + " e:" + e.ToString() + " w:" + w.ToString());
            w = w / 2.0f;
            Vector3[] q = new Vector3[4];
            Vector3 l = Vector3.Cross(c, e - s);
            if (l == Vector3.zero)
            {
                l = Vector3.Cross(s - p, e - p);
            }

            l.Normalize();
            //logger.debug("c:" + c.ToString() + " l:" + l.ToString());
            q[0] = s + l * w;
            q[1] = s + l * -w;
            q[2] = e + l * w;
            q[3] = e + l * -w;
            var str = "";
            foreach (var i in q)
            {
                str += i.ToString() + " ";
            }
            //logger.debug("q:" + str);
            return q;
        }
    }
}


