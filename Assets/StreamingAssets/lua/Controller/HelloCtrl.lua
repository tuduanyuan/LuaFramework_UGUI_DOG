--[[
@author:wushuang
@Gen Create by python
@name:HelloCtrl.lua
@time:2018-02-24 17:46:26
]]--


require "Common/define"


HelloCtrl = {};
local this = HelloCtrl;

local luabehaviour;
local transform;
local gameObject;

--构建函数--
function HelloCtrl.New()
	logWarn("HelloCtrl.New--->>");
	return this;
end
--初始化--
function HelloCtrl.Awake()
	logWarn("HelloCtrl.Awake--->>");
	local e = LuaFramework.PanelManager.PanelType.IntToEnum(0);--搁置
	panelMgr:CreatePanel('Hello', e , this.OnCreate);--关链LuaBehaviour
	
end

--启动事件--
function HelloCtrl.OnCreate(obj)
	logWarn("HelloCtrl.OnCreate--->>");
	gameObject = obj;

	--基本获取
	luabehaviour = gameObject:GetComponent('LuaBehaviour');
	--Start
	logWarn("HelloCtrl Start lua--->>"..gameObject.name);
	
	--慎用
	-- UpdateBeat:Add(function( ... )
    --     this.Update();
    -- end);
    -- LateUpdateBeat:Add(function (...)
    --     this.LateUpdate();
    -- end);
    -- FixedUpdateBeat:Add(function (...)
    --     this.FixedUpdate();
	-- end);
	
	luabehaviour:AddClick( HelloPanel.btLogin, this.LoginEnter);
end
--驱动Update(默认关闭)--
function HelloCtrl.Update()
    
end

function HelloCtrl.LateUpdate()
    
end

function HelloCtrl.FixedUpdate()
    
end

function HelloCtrl.LoginEnter(...)
	Util.loadScene("main");
end

--初始化面板--
function HelloCtrl.InitPanel(objs)
	logWarn("HelloCtrl.InitPanel--->>");
end

--单击事件--
function HelloCtrl.OnClick(go)
	logWarn("HelloCtrl---->>>"..go.name);
end

--关闭事件--
function HelloCtrl.Close()
	panelMgr:ClosePanel(CtrlNames.Message);
end
------自定义函数------
--函数式例--
--function HelloCtrl.fun(obj,...)

--end
