using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using LuaFramework;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Utility {
    public enum LoggerLevel
    {
        Debug = 1,
        Info = 2,
        Warn = 3,
        Error = 4,
    }

    public class LoggerManager : Singleton<LoggerManager>, ILog
    {
        private class LogInfo
        {
            private string m_FileName;
            public LogInfo(string fileName)
            {
                m_FileName = string.Format("{0}{1}.log", AppConst.PersistentLogPath, fileName);
                if (!Data.HasPublic(fileName))
                {
                    FileUtil.CreateFile(m_FileName, "");
                }
                else
                {
                    long time;
                    if (long.TryParse(Data.LoadPublic(fileName), out time) && !TimeUtil.IsSameDay(TimeUtil.GetDateTime(), time))
                    {
                        FileUtil.CreateFile(m_FileName, "");
                    }
                }
                Data.SavePublic(fileName, TimeUtil.GetDateTime().ToString());
            }
            public void Write(byte[] bytes)
            {
                if (bytes == null || bytes.Length == 0) return;
                lock (this)
                {
                    FileStream stream = null;
                    try
                    {
                        stream = new FileStream(m_FileName, FileMode.Append);
                        stream.Write(bytes, 0, bytes.Length);
                    }
                    finally
                    {
                        if (stream != null) stream.Dispose();
                    }
                }
            }
            public byte[] GetBuffer(bool compress)
            {
                lock (this)
                {
                    return compress ? GZipUtil.Compress(FileUtil.GetFileBuffer(m_FileName)) : FileUtil.GetFileBuffer(m_FileName);
                }
            }
        }
        private uint mLogLevel = 0;
        private LogInfo mConsoleLog;
        private LogInfo mInfoLog;
        private LogInfo mWarnLog;
        private LogInfo mErrorLog;
        private Dictionary<int, string> dic = new Dictionary<int, string>();
        public LoggerManager()
        {
            FileUtil.CreateDirectory(AppConst.PersistentLogPath);
            mConsoleLog = new LogInfo("console");
            mInfoLog = new LogInfo("info");
            mWarnLog = new LogInfo("warn");
            mErrorLog = new LogInfo("error");
            AddLogLevel(LoggerLevel.Debug);
            AddLogLevel(LoggerLevel.Info);
            AddLogLevel(LoggerLevel.Warn);
            AddLogLevel(LoggerLevel.Error);
            DefaultColor();
        }
        ~LoggerManager()
        {
            mConsoleLog = null;
            mInfoLog = null;
            mWarnLog = null;
            mErrorLog = null;

            dic = null;
        }
        protected override void InitializeInstance() {
            RegisterLogCallback(OutPut);
        }

        protected override void ShutdownInstance() {
            UnRegisterLogCallback(OutPut);
        }

        public void RegisterLogCallback(Application.LogCallback handler)
        {
            Application.logMessageReceivedThreaded += handler;
        }
        public void UnRegisterLogCallback(Application.LogCallback handler)
        {
            Application.logMessageReceivedThreaded -= handler;
        }
        public void AddLogLevel(LoggerLevel logLevel)
        {
            if ((mLogLevel & (uint)(1 << (int)logLevel)) == 0)
                mLogLevel |= (uint)(1 << (int)logLevel);
        }
        public void DelLogLevel(LoggerLevel logLevel)
        {
            if ((mLogLevel & (uint)(1 << (int)logLevel)) != 0)
                mLogLevel ^= (uint)(1 << (int)logLevel);
        }
        public bool IsLogLevel(LoggerLevel logLevel)
        {
            uint ret = (mLogLevel & (uint)(1 << (int)logLevel));
            return (ret != 0);
        }
        public void ClearLevel()
        {
            mLogLevel = 0;
        }

        string LevelToStr(LoggerLevel level)
        {
            var str = "";
            switch(level)
            {
                case LoggerLevel.Debug:
                    str = "D";
                    break;
                case LoggerLevel.Error:
                    str = "E";
                    break;
                case LoggerLevel.Info:
                    str = "I";
                    break;
                case LoggerLevel.Warn:
                    str = "W";
                    break;
            }
            return str;
        }
        void DefaultColor()
        {
            dic.Add(0, "#000000");//黑色->Unkonw
            //自己配色，简单使用 level对应
            dic.Add(1, "#FFFFFF");//白色 debug
            dic.Add(2, "#00FF14");//绿色 info
            dic.Add(3, "#F5FF03");//黄色 warn
            dic.Add(4, "#B64A4E");//红色 error
            //文件
            dic.Add(5, "#FFA300");//橙色->C#文件
            dic.Add(6, "#FF00EB");//紫色->Lua文件
        }
        /// <summary>
        /// 头信息，但是除了debug的东西，其他的东西都会混进来
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="level"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        string GetPrefix(StackFrame frame, LoggerLevel level,string source)
        {
            if (frame == null) return "";
            var method = frame.GetMethod();
            if (method == null) return "";
            string fileOrClass = "UnKonw";
            string func = "UnKonw";
            int line = -1;
            string tip = "@";
            if (source == "C#") {
                fileOrClass = method.ReflectedType.Name;
                func = method.Name;
                line = frame.GetFileLineNumber();
                source = DrawStrColor(dic[5], source);
            }
            else if (source == "Lua") {
                source = DrawStrColor(dic[6], source);
                tip = DrawStrColor(dic[3], tip);
            } 
            else source = DrawStrColor(dic[0], source);
            return string.Format("[{0}][{1}][{2}][{3}{4}{5}:{6}] : ", LevelToStr(level), DateTime.Now.ToString("dd HH:mm:ss:ffff"), source, method.ReflectedType.Name,tip,method.Name, frame.GetFileLineNumber());
        }

        string GetLuaPrefix(LoggerLevel level,string fileName,string func,int line, string source)
        {
            source = DrawStrColor(dic[6], source);
            string tip = "@";
            if (fileName == "UnKonw")
            {
                tip = DrawStrColor(dic[3], tip);
                return string.Format("[{0}][{1}][{2}][{3}] : ", LevelToStr(level), DateTime.Now.ToString("dd HH:mm:ss:ffff"), source , tip);
            }
            return string.Format("[{0}][{1}][{2}][{3}@{4}:{5}] : ", LevelToStr(level), DateTime.Now.ToString("dd HH:mm:ss:ffff"), source, fileName, func, line);
        }

        private void debugFrame(string type, string msg, StackFrame frame, params object[] args)
        {
            if (!IsLogLevel(LoggerLevel.Debug)) return;
            //cat 匹配下是不是lua 的代码所打印的内容
            if (type == "" || type.Length <= 0 || type == string.Empty) type = "Unkown";
            if (type == "Lua") {
                string str = (string)args[3];
                str = DrawStrColor(dic[(int)LoggerLevel.Debug], str);
                UnityEngine.Debug.Log(GetLuaPrefix(LoggerLevel.Debug,(string)args[0], (string)args[1], (int)args[2], type) + str);
            }
            else {
                string str = DrawStrColor(dic[(int)LoggerLevel.Debug], string.Format(msg, args));
                UnityEngine.Debug.Log(GetPrefix(frame, LoggerLevel.Debug, type) + str);
                UnDrawString(GetPrefix(frame, LoggerLevel.Debug, type) + str);
            } 
        }
        private void infoFrame(string type, string msg, StackFrame frame, params object[] args)
        {
            if (!IsLogLevel(LoggerLevel.Info)) return;
            if (type == ""|| type.Length <=0||type == string.Empty) type = "Unkown";

            if (type == "Lua")
            {
                string str = (string)args[3];
                str = DrawStrColor(dic[(int)LoggerLevel.Info], str);
                UnityEngine.Debug.Log(GetLuaPrefix(LoggerLevel.Info, (string)args[0], (string)args[1], (int)args[2], type) + str);
            }
            else
            {
                string str = DrawStrColor(dic[(int)LoggerLevel.Info], string.Format(msg, args));
                UnityEngine.Debug.Log(GetPrefix(frame, LoggerLevel.Debug, type) + str);
                UnDrawString(GetPrefix(frame, LoggerLevel.Info, type) + str);
            }
            //string str = DrawStrColor(dic[(int)LoggerLevel.Info], string.Format(msg, args));
            //UnityEngine.Debug.Log(GetPrefix(frame,LoggerLevel.Info, type) + str);
        }
        private void warnFrame(string type, string msg, StackFrame frame, params object[] args)
        {
            if (!IsLogLevel(LoggerLevel.Warn)) return;
            if (type == "" || type.Length <= 0 || type == string.Empty) type = "Unkown";

            if (type == "Lua")
            {
                string str = (string)args[3];
                str = DrawStrColor(dic[(int)LoggerLevel.Warn], str);
                UnityEngine.Debug.Log(GetLuaPrefix(LoggerLevel.Warn, (string)args[0], (string)args[1], (int)args[2], type) + str);
            }
            else
            {
                string str = DrawStrColor(dic[(int)LoggerLevel.Warn], string.Format(msg, args));
                UnityEngine.Debug.Log(GetPrefix(frame, LoggerLevel.Warn, type) + str);
            }
            //string str = DrawStrColor(dic[(int)LoggerLevel.Warn], string.Format(msg, args));
            //UnityEngine.Debug.LogWarning(GetPrefix(frame,LoggerLevel.Warn, type) + str);
        }
        private void errorFrame(string type ,string msg, StackFrame frame, params object[] args)
        {
            if (!IsLogLevel(LoggerLevel.Error)) return;
            if (type == "" || type.Length <= 0 || type == string.Empty) type = "Unkown";

            if (type == "Lua")
            {
                string str = (string)args[3];
                str = DrawStrColor(dic[(int)LoggerLevel.Error], str);
                UnityEngine.Debug.Log(GetLuaPrefix(LoggerLevel.Error, (string)args[0], (string)args[1], (int)args[2], type) + str);
            }
            else
            {
                string str = DrawStrColor(dic[(int)LoggerLevel.Error], string.Format(msg, args));
                UnityEngine.Debug.Log(GetPrefix(frame, LoggerLevel.Error, type) + str);
            }

            //string str = DrawStrColor(dic[(int)LoggerLevel.Error], string.Format(msg, args));
            //UnityEngine.Debug.LogError(GetPrefix(frame, LoggerLevel.Error, type) + str);
        }

        /*interface*/
        public void debug(string msg)
        {
            debug("{0}", msg);
        }
        public void info(string msg)
        {
            info("{0}", msg);
        }
        public void warn(string msg)
        {
            warn("{0}", msg);
        }
        public void error(string msg)
        {
            error("{0}", msg);
        }

        /*
         * interface
         */
        public void debug(string source, string msg, params object[] args)
        {
#if false// SCORPIO_UWP && !UNITY_EDITOR
            debugFrame(type , msg, null, args);
#else
            StackFrame frame = new StackFrame(2, true);
            if (source == "Lua")
            {
                debugFrame(source, msg, null, args);
            }
            else
            {
                debugFrame(source, msg, frame, args);
            }
#endif
        }
        public void info(string source, string msg, params object[] args)
        {
#if false// SCORPIO_UWP && !UNITY_EDITOR
            infoFrame(type,msg, null, args);
#else
            StackFrame frame = new StackFrame(2, true);
            infoFrame(source, msg, frame, args);
#endif
        }
        public void warn(string source, string msg, params object[] args)
        {
#if false//SCORPIO_UWP && !UNITY_EDITOR
            warnFrame(type,msg, null, args);
#else
            StackFrame frame = new StackFrame(2, true);
            warnFrame(source, msg, frame, args);
#endif
        }
        public void error(string source, string msg, params object[] args)
        {
#if false//SCORPIO_UWP && !UNITY_EDITOR
            errorFrame(type,msg, null, args);
#else
            StackFrame frame = new StackFrame(2, true);
            errorFrame(source, msg, frame, args);
#endif
        }
        void OutPut(string condition, string stackTrace, LogType type)
        {
#if UNITY_EDITOR || !UNITY_WEBGL
            string hint = string.Format("[{0}] {1}\n", type, condition);

			//string hint = string.Format("[{0}][{1}] {2}\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"), type, condition);
            byte[] buffer = Encoding.UTF8.GetBytes(hint);
            mConsoleLog.Write(buffer);
            if (type == LogType.Log)
            {
                mInfoLog.Write(buffer);
            }
            else if (type == LogType.Warning)
            {
                mWarnLog.Write(buffer);
            }
            else
            {
                mErrorLog.Write(buffer);
            }
#endif
        }
        public byte[] GetBuffer(bool compress)
        {
#if UNITY_EDITOR || !UNITY_WEBGL
            return mConsoleLog.GetBuffer(compress);
#else
		return new byte[0];
#endif
        }
        /// <summary>
        /// 把内容添加色
        /// </summary>
        /// <returns>The red to string.</returns>
        /// <param name="color">String.颜色，这个是html标签颜色和Unity无关</param>
        /// <param name="msg">String.差不多就那个样子</param>
        /// <param name="args">String.</param>
        public static string DrawStrColor(string color, string str){
            string s = str + "</color>";
            string c = "<color=" + color + ">";
            //string s = "<color=darkblue>" + string.Format (msg, args) + "</color>";
            //UnityEngine.Debug.Log(c + s);
            return c + s;
            
        }
		/// <summary>
		/// 去除作色
		/// </summary>
		/// <param name="str">String.</param>
		public static string UnDrawString(string str){
			//我需要去正则匹配到<color=*****> 和 </color> ->把这个替换成字符
			//UnityEngine.Debug.Log ("匹配结果:" + reStr);
			string PageInputStr = str;
			var RegexStr1 = @"<color=.*?>";
            var RegexStr2 = @"</color>";
            Regex rep_regex1 = new Regex(RegexStr1);
            Regex rep_regex2 = new Regex(RegexStr2);
            //UnityEngine.Debug.Log(string.Format("用户输入信息：{0}", PageInputStr));
			var outPut = rep_regex1.Replace (PageInputStr, "");
            outPut = rep_regex2.Replace(outPut, "");
            //UnityEngine.Debug.Log(string.Format ("页面显示信息：{0}", outPut));
            return outPut;

        }
    }
}

