using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class WeEditorUtil{

    public static UnityEngine.Object CreatePrefab(GameObject go, string path , string name)
	{
        var root = Application.dataPath;
        var index = root.LastIndexOf("Assets");
        root = root.Substring(0, index);
        var directoryPath = root + @"/" + path;
        //logger.debug("生成文件夹路径:" + directoryPath);
        if (!Directory.Exists(directoryPath))
        {
            try
            {
                Directory.CreateDirectory(directoryPath);
            }
            catch(Exception e)
            {
                logger.error(e);
            }
            
        }

        if (Directory.Exists(path + @"/" + name + @".prefab"))
        {
            return null;
        }
        //PrefabUtility.CreatePrefab(path + @"/"+ name + @".prefab", go);
        //UnityEngine.Object tempPrefab = PrefabUtility.CreateEmptyPrefab(path + @"/"+ name + @".prefab");
        /// PrefabUtility.ConnectGameObjectToPrefab(go, PrefabUtility.CreatePrefab(path + @"/" + name + @".prefab", go));
        //PrefabUtility.ReplacePrefab(go, tempPrefab)
        return PrefabUtility.ConnectGameObjectToPrefab(go, PrefabUtility.CreatePrefab(path + @"/" + name + @".prefab", go));
    }
}
