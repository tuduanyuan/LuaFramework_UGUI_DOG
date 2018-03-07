using System;
using System.Collections.Generic;
using System.Text;
using Utility;
/// <summary> 日志类 </summary>
public interface ILog
{
    /// <summary> debug 信息 </summary>
    void debug(string source, string format, params object[] args);
    /// <summary> info 信息 </summary>
    void info(string source, string format, params object[] args);
    /// <summary> 警告 信息 </summary>
    void warn(string source, string format, params object[] args);
    /// <summary> 错误 信息 </summary>
    void error(string source, string format, params object[] args);
}
/// <summary> 日志类 </summary>
public static class logger
{
    static logger()
    {
        SetLog(LoggerManager.GetInstance());
    }
    private static ILog log = null;
    /// <summary> 设置日志对象 </summary>
    public static void SetLog(ILog ilog)
    {
        log = ilog;
    }
    /********************************/
    /// <summary> debug输出 </summary>
    public static void debug(object format)
    {
        if (log == null) return;
        log.debug("C#", "{0}", format);
    }
    /// <summary> debug输出 </summary>
    public static void debug(string format)
    {
        if (log == null) return;
        log.debug("C#", "{0}", format);
    }
    /// <summary> debug输出 </summary>
    public static void debug(string format, params object[] args)
    {
        if (log == null) return;
        log.debug("C#", format, args);
    }
    public static void debug(string source, string format, params object[] args)
    {
        if (log == null) return;
        log.debug(source, format, args);
    }

    public static void lua_debug(string fileOrClass,string func , int line, string format, params object[] args)
    {
        if (log == null) return;
        debug("Lua", "{0}{1}{2}{3}", fileOrClass, func, line, string.Format(format, args));
    }
    /********************************/
    /// <summary> info输出 </summary>
    public static void info(object format)
    {
        if (log == null) return;
        log.info("C#", "{0}", format);
    }
    /// <summary> info输出 </summary>
    public static void info(string format)
    {
        if (log == null) return;
        log.info("C#", "{0}", format);
    }
    /// <summary> info输出 </summary>
    public static void info(string format, params object[] args)
    {
        if (log == null) return;
        log.info("C#", format, args);
    }
    public static void info(string source, string format, params object[] args)
    {
        if (log == null) return;
        log.info(source, format, args);
    }
    public static void lua_info(string fileOrClass, string func, int line, string format, params object[] args)
    {
        info("Lua", "{0}{1}{2}{3}", fileOrClass, func, line, string.Format(format, args));
    }
    /********************************/
    /// <summary> warn输出 </summary>
    public static void warn(object format)
    {
        if (log == null) return;
        log.warn("C#","{0}", format);
    }
    /// <summary> warn输出 </summary>
    public static void warn(string format)
    {
        if (log == null) return;
        log.warn("C#","{0}", format);
    }
    /// <summary> 警告输出 </summary>
    public static void warn(string format, params object[] args)
    {
        if (log == null) return;
        log.warn("C#",format, args);
    }
    public static void warn(string source, string format, params object[] args)
    {
        if (log == null) return;
        log.warn(source, format, args);
    }
    public static void lua_warn(string fileOrClass, string func, int line, string format, params object[] args)
    {
        warn("Lua", "{0}{1}{2}{3}", fileOrClass, func, line, string.Format(format, args));
    }
    /********************************/
    /// <summary> error输出 </summary>
    public static void error(object format)
    {
        if (log == null) return;
        log.error("C#","{0}", format);
    }
    /// <summary> error输出 </summary>
    public static void error(string format)
    {
        if (log == null) return;
        log.error("C#","{0}", format);
    }
    /// <summary> 错误输出 </summary>
    public static void error(string format, params object[] args)
    {
        if (log == null) return;
        log.error("C#",format, args);
    }
    public static void error(string source, string format, params object[] args)
    {
        if (log == null) return;
        log.error(source, format, args);
    }
    public static void lua_error(string fileOrClass, string func, int line, string format, params object[] args)
    {
        error("Lua", "{0}{1}{2}{3}", fileOrClass, func, line, string.Format(format, args));
    }
}


