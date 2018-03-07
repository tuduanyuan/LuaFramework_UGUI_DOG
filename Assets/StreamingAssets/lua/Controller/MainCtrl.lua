--[[
@author:wushuang
@Gen Create by python
@name:MainCtrl.lua
@time:2018-03-01 14:58:29
]]--


require "Common/define"


MainCtrl = {};
local this = MainCtrl;

local luabehaviour;
local transform;
local gameObject;

--构建函数--
function MainCtrl.New()
	logWarn("MainCtrl.New--->>");
	return this;
end
--初始化--
function MainCtrl.Awake()
	logWarn("MainCtrl.Awake--->>");
	local e = LuaFramework.PanelManager.PanelType.IntToEnum(0);--搁置
	panelMgr:CreatePanel('Main', e , this.OnCreate)--关链LuaBehaviour
	
end

--启动事件--
function MainCtrl.OnCreate(obj)
	logWarn("MainCtrl.OnCreate--->>");
	
	--基本获取 需要的自行添加
	gameObject = obj;
	transform = obj.transform;
	luabehaviour = gameObject:GetComponent('LuaBehaviour');
	--Start
	logWarn("MainCtrl Start lua--->>"..gameObject.name);
	
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

	luabehaviour:AddClick( MainPanel.btGameBegin, this.Run);
end
--驱动Update(默认关闭)--
function PromptCtrl.Update()
    
end

function PromptCtrl.LateUpdate()
    
end

function PromptCtrl.FixedUpdate()
    
end

------自定义函数------
--函数式例--
--function MainCtrl.fun(obj,...)

--end

function MainCtrl.Run(...)
	Util.loadScene("game");
end
