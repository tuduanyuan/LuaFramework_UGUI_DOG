using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CreateLuaPanel : EditorWindow
{
    //输入文件名称
    private string text;
    //生成文件名称 
    //View/Controller
    private string m_ViewFile;
    private string m_ControllerFile;
    //View/Controller路径
    private string m_LuaViewPath;
    private string m_LuaControllerPath;

    private string defPath;//默认定义文件目录，绝对路径
    private string defoutPrePath;//默认预制体生成目录，绝对路径
    private static string inputPath;//文件目录，相对路径(Assets)
    private static string outPrePath;//预制体生成目录，相对路径(Assets)
    private string rootPath;//根目录地址

    private static string enumFilePath;//
    private static string ctrlDemo = @"@@@Ctrl.lua";
    private static string viewDemo = @"@@@Panel.lua";


    //temp
    private string tempOutPrefab;
    // Use this for initialization

	//TODO:定义默认值，没写,如果需要在init里面修改


    void Start () {
		
	}

    

    //一些定义的矩形区域
    private Rect rect;
    private Rect crtlRect;
    private Rect viewRect;
    private Rect enumRect;
    private Rect resRect;


    private bool isCover = false;
    

    private bool isShowInfo = false;

    //private bool isGenGameObject = true;
    private bool isGenPrefabObject = false;
    private GameObject rootGameObject;
    private int initCount = 0;
    private void OnEnable()
    {
        init();
    }

    private void init()
    {
        if (initCount > 0) return;
        initCount++;

		var str = Application.dataPath;
		var index = str.LastIndexOf("Assets");
		rootPath = str.Substring(0, index);

        
		defPath = LuaFramework.AppConst.FrameworkRoot + @"/Lua";

        var t_index = defPath.IndexOf("Assets/");
        inputPath = defPath.Substring(t_index, defPath.Length - t_index);

        defoutPrePath = LuaFramework.AppConst.FrameworkRoot + @"/Builds";

        t_index = defoutPrePath.IndexOf("Assets/");
        outPrePath = tempOutPrefab = defoutPrePath.Substring(t_index, defoutPrePath.Length - t_index);

        rootGameObject = GameObject.Find("GuiCamera");


		var t_enumFilePath = LuaFramework.AppConst.FrameworkRoot + @"/Lua/Common/defines";
		t_index = t_enumFilePath.IndexOf("Assets/");
		enumFilePath = t_enumFilePath.Substring(t_index, t_enumFilePath.Length - t_index);

        ctrlDemo = @"@@@Ctrl.lua";
        viewDemo = @"@@@Panel.lua";

	}

	private void resetDefData(){
		initCount = 0;
		this.init ();
	}


    //绘制窗口时调用
    void OnGUI()
    {
		GUILayout.Label(@"提示:使用之前请注意下文件路径是否正确(如果需要请修改代码)");
        //输入框控件
        text = EditorGUILayout.TextField("(1)输入创建文件名字:", text);
        if (!string.IsNullOrEmpty(text))
        {
            m_ControllerFile = text + "Ctrl.lua";
            m_ViewFile = text + "Panel.lua";
        }
        else
        {           
            m_ControllerFile = string.Empty;
            m_ViewFile = string.Empty;
        }
        GUILayout.Label("ControllerFileName:" + m_ControllerFile);
        GUILayout.Label("ViewFileName:" + m_ViewFile);

        EditorGUILayout.LabelField("(2)输出Lua路径(直接拖入输入框即可,并确保响应文件夹存在)");
        //获得一个长300的框  
        rect = EditorGUILayout.GetControlRect(GUILayout.Width(300));
        //将上面的框作为文本输入框      
        //UnityEngine.Debug.Log("showStr:" + showStr.ToString());
        GUIStyle style = new GUIStyle();
        style.normal.textColor = new Color(1, 1, 1);
        inputPath = EditorGUI.TextField(rect, inputPath);
        //如果鼠标正在拖拽中或拖拽结束时，并且鼠标所在位置在文本输入框内  

        if (rect.Contains(Event.current.mousePosition))
        {
            //改变鼠标的外表      
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
            {
                if (Event.current.type == EventType.DragPerform)
                {
                    inputPath = DragAndDrop.paths[0];                                    
                }
            }
        }
        m_LuaViewPath = inputPath + @"/View";
        m_LuaControllerPath = inputPath + @"/Controller";
        var showStr = m_LuaControllerPath;
        GUILayout.Label("ControllerPath:" + showStr);
        //GUILayout.TextArea(showStr, GUILayout.Width(400),GUILayout.Height(18));
        showStr = m_LuaViewPath;
        GUILayout.Label(@"ViewPath:" + showStr);
        GUILayout.Label(@"(3)预制体生成(包含Resources目录):");
        isGenPrefabObject = GUILayout.Toggle(isGenPrefabObject, "是/否生成预制体(建议不生成)", GUILayout.Width(255));
        if (!isGenPrefabObject)
        {
            if (!string.IsNullOrEmpty(outPrePath))
            {
                tempOutPrefab = outPrePath;
                outPrePath = string.Empty;
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(tempOutPrefab))
            {
                outPrePath = tempOutPrefab;
                tempOutPrefab = string.Empty;
            }    
        }
        resRect = EditorGUILayout.GetControlRect(GUILayout.Width(300));
        outPrePath = EditorGUI.TextField(resRect, outPrePath);
        if (resRect.Contains(Event.current.mousePosition))
        {
            //改变鼠标的外表      
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
            {
                if (Event.current.type == EventType.DragPerform)
                {
                    var str = DragAndDrop.paths[0];
                    if (str.Contains(@"Resources"))
                    {
                        outPrePath = str;
                    }
                }
            }
        }

        //GUILayout.TextArea(showStr, GUILayout.Width(400), GUILayout.Height(18));
        GUILayout.Label(@"(4)如果文件已存在，是否覆盖:");
        isCover = GUILayout.Toggle(isCover, "是/否", GUILayout.Width(55));
        GUILayout.Label(@"(5)设置Lua模板,在Editor Default Resources/LuaTemplate/目录下:");
        GUILayout.Label(@"设置Ctrl模板(包含Ctrl名称&.lua后缀):");
        // 获得一个长300的框
        crtlRect = EditorGUILayout.GetControlRect(GUILayout.Width(300));
        ctrlDemo = EditorGUI.TextField(crtlRect, ctrlDemo);
        //如果鼠标正在拖拽中或拖拽结束时，并且鼠标所在位置在文本输入框内  
        if (crtlRect.Contains(Event.current.mousePosition))
        {
            //改变鼠标的外表      
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
            {
                if (Event.current.type == EventType.DragPerform)
                {
                    var str = DragAndDrop.paths[0];
                    var index = str.LastIndexOf("/");
                    str = str.Substring(index + 1, str.Length - (index + 1));
                    if (str.Contains(@"Ctrl") && str.Contains(@".lua"))
                    {
                        ctrlDemo = str;
                    }
                }
            }
        }        
        GUILayout.Label(@"设置View模板(包含Panel名称&.lua后缀):");
        // 获得一个长300的框
        viewRect = EditorGUILayout.GetControlRect(GUILayout.Width(300));
        viewDemo = EditorGUI.TextField(viewRect, viewDemo);

        if (viewRect.Contains(Event.current.mousePosition))
        {
            //改变鼠标的外表      
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
            {
                if (Event.current.type == EventType.DragPerform)
                {
                    var str = DragAndDrop.paths[0];                                      
                    var index = str.LastIndexOf("/");
                    str = str.Substring(index + 1, str.Length - (index + 1));
                    if (str.Contains(@"Panel") && str.Contains(@".lua"))
                    {
                        viewDemo = str;
                    }
                }
            }
        }
        GUILayout.Label(@"(6)生成GameObecjt,如果是空就不生成:");
        rootGameObject = EditorGUILayout.ObjectField("指定根节点", rootGameObject , typeof(GameObject),true) as GameObject;


        GUILayout.Label(@"(7)指定枚举文件所在文件夹,默认在Assets:");

        enumRect = EditorGUILayout.GetControlRect(GUILayout.Width(300));
        enumFilePath = EditorGUI.TextField(enumRect, enumFilePath);
        if (enumRect.Contains(Event.current.mousePosition))
        {
            //改变鼠标的外表      
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
            {
                if (Event.current.type == EventType.DragPerform)
                {
                    enumFilePath = DragAndDrop.paths[0];
                }
            }
        }

        GUILayout.Label(@"(8)其他,显示过程信息到Log:");
        isShowInfo = GUILayout.Toggle(isShowInfo, @"是/否", GUILayout.Width(55));
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(@"生成", GUILayout.Width(200)))
        {
            //打开一个通知栏
            
            OnTouchEvt();
            
        }
        //文本框显示鼠标在窗口的位置
        //EditorGUILayout.LabelField("鼠标在窗口的位置", Event.current.mousePosition.ToString());          
        if (GUILayout.Button(@"帮助文档", GUILayout.Width(200)))
        {
            //关闭窗口
            OnTouchHelpEvt();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(@"恢复默认值", GUILayout.Width(200)))
        {
            //关闭窗口
            
			resetDefData ();
        }
        if (GUILayout.Button(@"关闭窗口", GUILayout.Width(200)))
        {
            //关闭窗口
            this.Close();
        }
        GUILayout.EndHorizontal();
    }

    private void OnTouchHelpEvt()
    {
#if UNITY_EDITOR_WIN
        RunTxtCmd(this);
#endif
    }

    // Update is called once per frame
    void Update () {
		
	}

	[UnityEditor.MenuItem(@"gwushuang40@126.com/创建Panel", false, 1)]
    public static void ShowWindow()
    {
        CreateLuaPanel window = EditorWindow.GetWindow<CreateLuaPanel>(true, @"创建Panel");
        window.minSize = window.maxSize = new Vector2(410f, 510f);
        UnityEngine.Object.DontDestroyOnLoad(window);
    }

    private void OnTouchEvt()
    {
#if UNITY_EDITOR
        var t_rootName = text;
        var t_inputPath = inputPath;
        var t_author = @"@";
        var t_cover = isCover;
        var t_ctrlDemo = ctrlDemo;
        var t_viewDemo = viewDemo;
        var t_eunmFilePath = enumFilePath;


        var t_scriptPath = Application.dataPath + @"/LuaFramework/Editor/wushuang40@126.com/CreateLuaPanel/CreateLuaPanel.py";

        if (string.IsNullOrEmpty(t_rootName))
        {
            this.ShowNotification(new GUIContent("请输入Root名字，或阅读readme"));
            return;
        }
        string command = t_scriptPath;
        command += @" " + t_rootName.ToString();//argv[1]
        command += @" " + rootPath.ToString()+ t_inputPath.ToString();//argv[2]
        command += @" " + t_author.ToString();//argv[3]
        //argv[4]
        if (t_cover)
        {
            command += @" " + @"True";
        }
        else
        {
            command += @" " + @"False";
        }
        //argv[5]
        command += @" " + t_ctrlDemo;
        //argv[6]
        command += @" " + t_viewDemo;
        //argv[7]
        if (t_eunmFilePath == string.Empty)
        {
            command += @" " + rootPath.ToString() + @"/Assets";
        }
        else
        {
            command += @" " + rootPath.ToString() + t_eunmFilePath;
        }
        
        this.ShowNotification(new GUIContent("创建中，请勿乱动"));
        logger.warn(command);
        RunPythonCmd(this,command);
        var name = t_rootName + "Panel";
        var go = GenGameObject(this, name);
        if (go != null && isGenPrefabObject)
        {
            GenPrefabObject(this, go , outPrePath, t_rootName, name);
        }
#endif
    }
    private static GameObject GenGameObject(object sender,string name)
    {
        var parent = (sender as CreateLuaPanel).rootGameObject;
        if (GameObject.Find(name) == null&& parent != null)
        {
            var obj = new GameObject();
            obj.name = name;
            var recttransform = obj.AddComponent<RectTransform>();
            obj.AddComponent<CanvasRenderer>();
            obj.AddComponent<Image>();
            recttransform.SetParent(parent.transform);
            recttransform.position = Vector3.zero;
            recttransform.anchoredPosition = new Vector2(0, 0);
            return obj;
        }
        return null;
    }

    private static void GenPrefabObject(object sender, GameObject go, string path,string rootname, string name)
    {

        //先生成一下路径
        if (!Directory.Exists(path + @"/" + rootname))
        {
            Directory.CreateDirectory(path + @"/" + rootname);
        }

        path = path + @"/" + rootname;
        if (!string.IsNullOrEmpty(path))
        {
            if (WeEditorUtil.CreatePrefab(go, path, name) == null)
                logger.error("未能成功创建预制体");
        }
    }

    private static void RunPythonCmd(object sender,string args)
    {
        
        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = "python";
        start.Arguments = args;
        start.UseShellExecute = false;
        start.RedirectStandardOutput = true;
        start.CreateNoWindow = true;// '不创建窗口 
        using (Process process = Process.Start(start))
        {
            using (StreamReader reader = process.StandardOutput)
            {
                string result = "";
                while (result != null)
                {
                    result = reader.ReadLine();
                    if (!string.IsNullOrEmpty(result))
                    {
                        
                        var str = result.ToString();
                        if (str.Contains("Result:") == true)
                        {

                            logger.debug(result);
                            if (str.Contains("succeed"))
                            {
                                ((EditorWindow)sender).RemoveNotification();
                                ((EditorWindow)sender).ShowNotification(new GUIContent("创建成功"));
                            }
                            else
                            {
                                ((EditorWindow)sender).ShowNotification(new GUIContent("失败，请检查流程"));
                            }
                        }
                        if (str.Contains("Info:") == true|| str.Contains("@") == true|| str.Contains("Error:") == true)
                        {
                            var o = (CreateLuaPanel)sender;
                            if (o.isShowInfo)
                                logger.debug(result);
                        }
                    }                        
                }
                AssetDatabase.Refresh();
            }
        }
    }
    private static void RunTxtCmd(object sender)
    {    
        var str = Application.dataPath + @"/Editor Default Resources/CreateLuaPanel/README.txt";
        System.Diagnostics.Process.Start(str);        
    }
}
