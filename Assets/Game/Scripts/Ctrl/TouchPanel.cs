using System;
using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// 虚拟摇杆 不安全，注意
/// </summary>
namespace XGame
{
    public class TouchPanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {

        /// <summary>
        /// 代理定义
        /// </summary>
        public delegate void onBegin(Vector3 pos);
        public delegate void onDrag(Vector3 pos);
        public delegate void onEnd(Vector3 pos);
        /// <summary>
        /// 代理实体
        /// </summary>
        public onBegin beginHandler = delegate { };
        public onDrag dragHandler = delegate { };
        public onEnd endHandler = delegate { };
       
        /// <summary>
        /// 摇杆所处的canvas
        /// </summary>
        private Canvas canvas;
        /// <summary>
        /// 处理间隔，不需要就设置为0
        /// </summary>
        [SerializeField]
        private int m_HandlerInterval = 15;

        public int HandlerInterval
        {
            get { return m_HandlerInterval; }
            set { m_HandlerInterval = value; }
        }

        private float FrameLength = 0;
        /// <summary>
        /// 是否被按下
        /// </summary>
        [HideInInspector]
        public bool isTouch = false;
        /// <summary>
        /// 初始化标记
        /// </summary>
        [HideInInspector]
        private bool refInit = true;
        /// <summary>
        /// curNor:当前的位置，默认为Zero
        /// </summary>
        [HideInInspector]
        public Vector3 curPos = Vector3.zero;
        private Vector3 lastPos = Vector3.zero;
        private void Awake()
        {
            FrameLength = 1.0f / m_HandlerInterval;
            Initialized();
            //监听OB消息
        }

        private void OnDestroy()
        {

        }


        private void Initialized()
        {
            try
            {
                canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
                refInit = false;
            }
            catch (Exception e)
            {
                logger.debug("遥感初始化失败 e:" + e.ToString());
            }
            finally
            {
                if (refInit)
                    this.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 添加代理 
        /// </summary>
        public void addHandler<T>(T handler)
        {
            if (handler == null)
                return;
            if (typeof(T) == typeof(onBegin))
            {
                beginHandler += handler as onBegin;
            }
            else if (typeof(T) == typeof(onDrag))
            {
                dragHandler += handler as onDrag;
            }
            else if (typeof(T) == typeof(onEnd))
            {
                endHandler += handler as onEnd;
            }
        }
        /// <summary>
        /// 删除代理
        /// </summary>
        public void removeHandler<T>(T handler)
        {
            if (handler == null)
                return;
            if (typeof(T) == typeof(onBegin))
            {
                beginHandler -= handler as onBegin;
            }
            else if (typeof(T) == typeof(onDrag))
            {
                dragHandler -= handler as onDrag;
            }
            else if (typeof(T) == typeof(onEnd))
            {
                endHandler -= handler as onEnd;
            }
        }


        /*实现接口*/
        public void OnPointerDown(PointerEventData eventData)
        {
            UpdateValue(eventData);
            BeginHandler();
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            UpdateValue(eventData);
            EndHandler();
        }
        public void OnDrag(PointerEventData eventData)
        {
            UpdateValue(eventData);
        }

        /// <summary>
        /// 更新遥感内的值
        /// </summary>
        /// <param name="eventData"></param>
        private void UpdateValue(PointerEventData eventData)
        {
            this.curPos = ScreenPointToLocalPointInRectangle(eventData.position, canvas);
        }

        /*具体处理*/
        private void BeginHandler()
        {
            isTouch = true;
            if (beginHandler != null)
            {
                beginHandler(this.curPos);
            }
        }

        void DragHandler()
        {
            if (!isTouch) return;
            if (dragHandler != null&& curPos != lastPos)
            {
                dragHandler(this.curPos);
                lastPos = curPos;
            }
        }


        void EndHandler()
        {
            isTouch = false;
            if (beginHandler != null)
            {
                endHandler(this.curPos);
            }
            accumilatedTime = 0;
        }
        /// <summary>
        /// 做响应间隔
        /// </summary>
        private float accumilatedTime = 0f;

        private void FixedUpdate()
        {
            if (isTouch == true)
            {
                accumilatedTime += Time.fixedDeltaTime;
                if (accumilatedTime > FrameLength)
                {
                    accumilatedTime -= FrameLength;
                    DragHandler();
                }
            }
        }

        /// <summary>
        /// 以canvas的形式转换坐标系，只在自己内使用
        /// </summary>
        /// <param name="screenPoint">点击位置</param>
        /// <param name="_canvas">摇杆所在的canvas</param>
        /// <returns></returns>
        private Vector2 ScreenPointToLocalPointInRectangle(Vector2 screenPoint, Canvas _canvas)
        {
            //logger.debug("screenPoint:"+ screenPoint);
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, screenPoint, _canvas.worldCamera, out localPoint))
                return localPoint;
            return Vector2.zero;
        }

    }

}

