using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using LuaInterface;
using System;
using UObject = UnityEngine.Object;

namespace LuaFramework {
    //û������û���
    
    public class PanelManager : Manager {

        public enum PanelType
        {
            Type0 = 0,
            Type1 = 1,
            Type2 = 2,
        };


        // ҳ�洴���ܴ���
        private static int count = 0;

        // ÿһ�����������
        private class PanelRequest
        {
            public string name;//������������
            public PanelType type;//���UI��ʲô���ͣ���Ҫ������ messgeBox������UI���漰���ظ����⣬��ת�������������
            public LuaFunction luaFunc;
            public Action<UObject> sharpFunc;
            public GameObject obj;
        }
        // �����¼
        private Stack<PanelRequest> requestStack = new Stack<PanelRequest>();
        // ��ʾ��gui�Ĺ���
        private Transform parent;

        Transform Parent {
            get {
                GameObject go = GameObject.FindWithTag("GuiCamera");
                parent = go.transform;
                return parent;
            }
        }
        // Cache�Ĺ��ؽڵ�
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
                logger.debug("����ȥ��");
                return cacheParent;
            }
        }
        // cache list
        private Dictionary<string, GameObject> cacheList = new Dictionary<string, GameObject>();

        /// <summary>
        /// ������壬������Դ������
        /// </summary>
        /// <param name="type"></param>
        public void CreatePanel(string name, PanelType type = PanelType.Type0,LuaFunction func = null, Action<UObject> sharpFunc = null) {
            //logger.debug(@"������壬������Դ������");
            string assetName = name + "Panel";
            string abName = name.ToLower() + AppConst.ExtName;
            logger.debug("CreatePanel  assetName:" + assetName + " abName:" + abName);
            //�����������ǲ��ǵ�������
            if (Parent.FindChild(name) != null) return;
            //������ܼ�黺��(���ڲ������)
            
#if ASYNC_MODE
            ResManager.LoadPrefab(abName, assetName, delegate(UnityEngine.Object[] objs) {
                if (objs.Length == 0) return;
                GameObject prefab = objs[0] as GameObject;
                if (prefab == null) return;
                //�ɹ�
                GameObject go = Instantiate(prefab) as GameObject;
                go.name = assetName;
                go.layer = LayerMask.NameToLayer("Default");
                go.transform.SetParent(Parent);
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = Vector3.zero;
                //���lua������뵽���ƽṹ
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
            //�ɹ�
            if (func != null) func.Call(go);
            logger.warn("CreatePanel::>> " + name + " " + prefab);
            count++;
#endif
        }

        /// <summary>
        /// �ر����
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
        /// ��黺��ڵ�
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