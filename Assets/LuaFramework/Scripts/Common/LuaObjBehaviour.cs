using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LuaFramework
{
    public class LuaObjBehaviour : MonoBehaviour
    {
        protected void Awake()
        {
            logger.info("LuaBehaviour Awake --->>gameObject:" + gameObject.ToString() + " name:" + name);
            Util.CallMethod(name, "Awake", gameObject);
        }

        protected void Start()
        {
            Util.CallMethod(name, "Start");
        }


        private void OnDestroy()
        {
            Util.CallMethod(name, "OnDestroy");
            logger.debug("~obj" + name + " was destroy!");
        }
    }
}
