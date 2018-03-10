﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class LuaFramework_PanelManagerWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(LuaFramework.PanelManager), typeof(Manager));
		L.RegFunction("CreatePanel", CreatePanel);
		L.RegFunction("ClosePanel", ClosePanel);
		L.RegFunction("CheckCache", CheckCache);
		L.RegFunction("OnLevelLoaded", OnLevelLoaded);
		L.RegFunction("__eq", op_Equality);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CreatePanel(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2)
			{
				LuaFramework.PanelManager obj = (LuaFramework.PanelManager)ToLua.CheckObject<LuaFramework.PanelManager>(L, 1);
				string arg0 = ToLua.CheckString(L, 2);
				obj.CreatePanel(arg0);
				return 0;
			}
			else if (count == 3)
			{
				LuaFramework.PanelManager obj = (LuaFramework.PanelManager)ToLua.CheckObject<LuaFramework.PanelManager>(L, 1);
				string arg0 = ToLua.CheckString(L, 2);
				LuaFramework.PanelManager.PanelType arg1 = (LuaFramework.PanelManager.PanelType)ToLua.CheckObject(L, 3, typeof(LuaFramework.PanelManager.PanelType));
				obj.CreatePanel(arg0, arg1);
				return 0;
			}
			else if (count == 4)
			{
				LuaFramework.PanelManager obj = (LuaFramework.PanelManager)ToLua.CheckObject<LuaFramework.PanelManager>(L, 1);
				string arg0 = ToLua.CheckString(L, 2);
				LuaFramework.PanelManager.PanelType arg1 = (LuaFramework.PanelManager.PanelType)ToLua.CheckObject(L, 3, typeof(LuaFramework.PanelManager.PanelType));
				LuaFunction arg2 = ToLua.CheckLuaFunction(L, 4);
				obj.CreatePanel(arg0, arg1, arg2);
				return 0;
			}
			else if (count == 5)
			{
				LuaFramework.PanelManager obj = (LuaFramework.PanelManager)ToLua.CheckObject<LuaFramework.PanelManager>(L, 1);
				string arg0 = ToLua.CheckString(L, 2);
				LuaFramework.PanelManager.PanelType arg1 = (LuaFramework.PanelManager.PanelType)ToLua.CheckObject(L, 3, typeof(LuaFramework.PanelManager.PanelType));
				LuaFunction arg2 = ToLua.CheckLuaFunction(L, 4);
				System.Action<UnityEngine.Object> arg3 = (System.Action<UnityEngine.Object>)ToLua.CheckDelegate<System.Action<UnityEngine.Object>>(L, 5);
				obj.CreatePanel(arg0, arg1, arg2, arg3);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: LuaFramework.PanelManager.CreatePanel");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ClosePanel(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2)
			{
				LuaFramework.PanelManager obj = (LuaFramework.PanelManager)ToLua.CheckObject<LuaFramework.PanelManager>(L, 1);
				string arg0 = ToLua.CheckString(L, 2);
				obj.ClosePanel(arg0);
				return 0;
			}
			else if (count == 3)
			{
				LuaFramework.PanelManager obj = (LuaFramework.PanelManager)ToLua.CheckObject<LuaFramework.PanelManager>(L, 1);
				string arg0 = ToLua.CheckString(L, 2);
				bool arg1 = LuaDLL.luaL_checkboolean(L, 3);
				obj.ClosePanel(arg0, arg1);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: LuaFramework.PanelManager.ClosePanel");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CheckCache(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			LuaFramework.PanelManager obj = (LuaFramework.PanelManager)ToLua.CheckObject<LuaFramework.PanelManager>(L, 1);
			string arg0 = ToLua.CheckString(L, 2);
			obj.CheckCache(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnLevelLoaded(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			LuaFramework.PanelManager obj = (LuaFramework.PanelManager)ToLua.CheckObject<LuaFramework.PanelManager>(L, 1);
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			obj.OnLevelLoaded(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int op_Equality(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.Object arg0 = (UnityEngine.Object)ToLua.ToObject(L, 1);
			UnityEngine.Object arg1 = (UnityEngine.Object)ToLua.ToObject(L, 2);
			bool o = arg0 == arg1;
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

