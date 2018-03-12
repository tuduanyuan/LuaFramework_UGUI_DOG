using UnityEngine;
using System.Collections;
public class LinesLR : MonoBehaviour
{
    public Shader shader;


    private Vector3 curr;
    private Vector3 last = new Vector3(0, 0, -100.0f);

    private int canvasIndex = 0;
    private float lineSizeLarge = 0.5f;
    private float lineSizeSmall = 0.5f;

    private Color lineColorLarge = new Color(0, 0, 0, 0.5f);
    private Color lineColorSmall = new Color(0, 0, 0, 0.1f);

    private ArrayList points;

    GUIStyle labelStyle;
    GUIStyle linkStyle;

    private float speed = 100f;
    void Start()
    {
        labelStyle = new GUIStyle();
        labelStyle.normal.textColor = Color.black;

        linkStyle = new GUIStyle();
        linkStyle.normal.textColor = Color.blue;

        points = new ArrayList();
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 24), "方向键控制旋转，摁住Shift按键可以放慢旋转速度", labelStyle);
        int vc = canvasIndex + points.Count;
        GUI.Label(new Rect(10, 26, 300, 24), "共 " + vc + " 条线. 摁下‘C’清空所有", labelStyle);

        GUI.Label(new Rect(10, Screen.height - 20, 250, 24), "转载注明出处 zwwdm.com ", labelStyle);
        if (GUI.Button(new Rect(150, Screen.height - 20, 300, 24), "原文链接（点击）", linkStyle))
        {
            Application.OpenURL("http://www.zwwdm.com");
        }
    }

    void Update()
    {
        float sp = speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)) sp = sp * 0.1f;
        if (Input.GetKey(KeyCode.UpArrow)) transform.Rotate(-sp, 0, 0);
        if (Input.GetKey(KeyCode.DownArrow)) transform.Rotate(sp, 0, 0);
        if (Input.GetKey(KeyCode.LeftArrow)) transform.Rotate(0, -sp, 0);
        if (Input.GetKey(KeyCode.RightArrow)) transform.Rotate(0, sp, 0);

        if (Input.GetKeyDown(KeyCode.C))
        {
            points = new ArrayList();
            foreach (Transform line in transform)
            {
                GameObject go = line.gameObject;
                Destroy(go.GetComponent(typeof(LineRenderer)));
                Destroy(line);
            }
        }

        if (Input.GetMouseButton(0))
        {
            curr = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z * -1.0f));
            logger.debug(@"查看一下内容点击位置1:" + curr.ToString());
            curr = transform.InverseTransformPoint(curr);
            logger.debug(@"查看一下内容点击位置2:" + curr.ToString());
            if (last.z != -100.0f)
            {
                createLine(last, curr, lineSizeLarge, lineColorLarge);

                foreach (Vector3 p in points)
                {
                    Vector3 s = p;
                    float d = Vector3.Distance(s, curr);
                    if (d < 1 && Random.value > 0.9f) createLine(s, curr, lineSizeSmall, lineColorSmall);
                }

                points.Add(curr);
            }

            last = curr;
        }
        else
        {
            last.z = -100.0f;
        }


    }

    private void createLine(Vector3 start, Vector3 end, float lineSize, Color c)
    {
        GameObject canvas = new GameObject("canvas" + canvasIndex);
        canvas.transform.parent = transform;
        canvas.transform.rotation = transform.rotation;
        LineRenderer lines = (LineRenderer)canvas.AddComponent<LineRenderer>();
        lines.material = new Material(shader);
        lines.material.color = c;
        lines.useWorldSpace = false;
        lines.SetWidth(lineSize, lineSize);
        lines.SetVertexCount(2);
        lines.SetPosition(0, start);
        lines.SetPosition(1, end);
        canvasIndex++;
    }
}
