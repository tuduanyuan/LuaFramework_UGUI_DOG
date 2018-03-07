using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using LuaInterface;
using System;
using UObject = UnityEngine.Object;

namespace LuaFramework {
    //没有名字没想好
    
    public class PanelManager : Manager {

        public enum PanelType
        {
            Type0 = 0,
            Type1 = 1,
            Type2 = 2,
        };


        // 页面创建总次数
        private static int count = 0;

        // 每一次请求的内容
        private class PanelRequest
        {
            public string name;//这个请求的类型
            public PanelType type;//这个UI是什么类型，主要是区分 messgeBox或者是UI，涉及到重复问题，和转场景保存的问题
            public LuaFunction luaFunc;
            public Action<UObject> sharpFunc;
            public GameObject obj;
        }
        // 请求记录
        private Stack<PanelRequest> requestStack = new Stack<PanelRequest>();
        // 显示的gui的挂载
        private Transform parent;

        Transform Parent {
            get {
                GameObject go = GameObject.FindWithTag("GuiCamera");
                parent = go.transform;
                return parent;
            }
        }
        // Cache的挂载节点
        private Transform cacheParent;

        Transform CacheParent
        {
            get
            {
                if (cacheParent == null)
                {
                    GameObject go = GameObject.FindWithTag("CacheGuiNode");
                    if (go != null) cacheParent = go.transform;
                }
                logger.debug("被拉去了");
                return cacheParent;
            }
        }
        // cache list
        private Dictionary<string, GameObject> cacheList = new Dictionary<string, GameObject>();

        /// <summary>
        /// 创建面板，请求资源管理器
        /// </summary>
        /// <param name="type"></param>
        public void CreatePanel(string name, PanelType type = PanelType.Type0,LuaFunction func = null, Action<UObject> sharpFunc = null) {
            //logger.debug(@"创建面板，请求资源管理器");
            string assetName = name + "Panel";
            string abName = name.ToLower() + AppConst.ExtName;
            logger.debug("CreatePanel  assetName:" + assetName + " abName:" + abName);
            //检查这个界面是不是弹出来了
            if (Parent.FindChild(name) != null) return;
            //如果可能检查缓存(现在不做这个)
            
#if ASYNC_MODE
            ResManager.LoadPrefab(abName, assetName, delegate(UnityEngine.Object[] objs) {
                if (objs.Length == 0) return;
                GameObject prefab = objs[0] as GameObject;
                if (prefab == null) return;
                //成功
                GameObject go = Instantiate(prefab) as GameObject;
                go.name = assetName;
                go.layer = LayerMask.NameToLayer("Default");
                go.transform.SetParent(Parent);
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = Vector3.zero;
                //添加lua代码进入到控制结构
                go.AddComponent<LuaBehaviour>();
                var request = new PanelRequest();
                request.name = name;
                request.type = type;
                request.luaFunc = func;
                request.sharpFunc = sharpFunc;
                request.obj = go;
                requestStack.Push(request);
                if (func != null) func.Call(go);
                if (sharpFunc != null) sharpFunc(go);
                logger.warn("CreatePanel::>> " + name + " " + prefab + " count:" + count + " requestStack.count:" + requestStack.Count);
                count++;
            });
#else
            GameObject prefab = ResManager.LoadAsset<GameObject>(name, assetName);
            if (prefab == null) return;

            GameObject go = Instantiate(prefab) as GameObject;
            go.name = assetName;
            go.layer = LayerMask.NameToLayer("Default");
            go.transform.SetParent(Parent);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            go.AddComponent<LuaBehaviour>();
            //成功
            if (func != null) func.Call(go);
            logger.warn("CreatePanel::>> " + name + " " + prefab);
            count++;
#endif
        }

        /// <summary>
        /// 关闭面板
        /// </summary>
        /// <param name="name"></param>
        public void ClosePanel(string name,bool inCache = false) {
            var panelName = name + "Panel";
            var panelObj = Parent.FindChild(panelName);
            if (panelObj == null) return;
            Destroy(panelObj.gameObject);
            /*
            if (!inCache)
            {
                Destroy(panelObj.gameObject);
            }
            else
            {
                panelObj.SetParent(CacheParent);
                cacheList.Add(name, panelObj.gameObject);
            }*/
        }

        /// <summary>
        /// 检查缓存节点
        /// </summary>
        /// <param name="name"></param>
        public void CheckCache(string name)
        {


        }

        public void OnLevelLoaded(int level)
        {
            parent = null;
        }
    }
}