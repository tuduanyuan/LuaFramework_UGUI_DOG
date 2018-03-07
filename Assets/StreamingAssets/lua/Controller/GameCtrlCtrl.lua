--[[
@author:wushuang
@Gen Create by python
@name:GameCtrlCtrl.lua
@time:2018-03-02 16:40:50
]]--


require "Common/define"


GameCtrlCtrl = {};
local this = GameCtrlCtrl;

local luabehaviour;
local transform;
local gameObject;

--构建函数--
function GameCtrlCtrl.New()
	logWarn("GameCtrlCtrl.New--->>");
	return this;
end
--初始化--
function GameCtrlCtrl.Awake()
	logWarn("GameCtrlCtrl.Awake--->>");
	local e = LuaFramework.PanelManager.PanelType.IntToEnum(0);--搁置
	panelMgr:CreatePanel('GameCtrl', e , this.OnCreate);--关链LuaBehaviour
	
end

--启动事件--
function GameCtrlCtrl.OnCreate(obj)
	logWarn("GameCtrlCtrl.OnCreate--->>");
	--基本获取 需要的自行添加
	
	gameObject = obj;
	transform = obj.transform;
	luabehaviour = gameObject:GetComponent('LuaBehaviour');
	--Start
	logWarn("GameCtrlCtrl Start lua--->>"..gameObject.name);
	this.AddTouchPanel();
	--被注释掉的，是需要自己处理的，所以要删除button
	--luabehaviour:AddClick(GameCtrlPanel._Camera, this.OnCamera);
	--这个是假象
	--luabehaviour:AddClick(GameCtrlPanel._Rocker, this.OnRocker);
	luabehaviour:AddClick(GameCtrlPanel._Bt0, this.OnBt0);
	--luabehaviour:AddClick(GameCtrlPanel._Bt1, this.OnBt1);
	luabehaviour:AddClick(GameCtrlPanel._Bt2, this.OnBt2);
	luabehaviour:AddClick(GameCtrlPanel._Bt3, this.OnBt3);
	luabehaviour:AddClick(GameCtrlPanel._Bt4, this.OnBt4);
	--慎用(用完记得善后)
	-- UpdateBeat:Add(function( ... )
    --     this.Update();
    -- end);
    -- LateUpdateBeat:Add(function (...)
    --     this.LateUpdate();
    -- end);
    -- FixedUpdateBeat:Add(function (...)
    --     this.FixedUpdate();
    -- end);
end
--驱动Update(默认关闭)--
function GameCtrlCtrl.Update()
    
end

function GameCtrlCtrl.LateUpdate()
    
end

function GameCtrlCtrl.FixedUpdate()
    
end

------自定义函数------
--函数式例--
--function GameCtrlCtrl.fun(obj,...)

--end
function GameCtrlCtrl.AddTouchPanel()
	local touchpanel = XGame.TouchPanel;--这个是特殊的用法，我还是用个名字空间包裹
	--View的控制权，控制视角方向,计算相对的向量
	local touctrl;
	GameCtrlPanel._View_Panel_TouchPanel = GameCtrlPanel._View_Panel:AddComponent(typeof(touchpanel));	
	touctrl = GameCtrlPanel._View_Panel_TouchPanel;
	touctrl.beginHandler = touctrl.beginHandler + this.onViewTouchBegin;
	touctrl.dragHandler = touctrl.dragHandler + this.onViewTouchDrag;
	touctrl.endHandler = touctrl.endHandler + this.onViewTouchEnd;
	
	--Move的控制权，控制人物移动,计算相对的向量	
	GameCtrlPanel._Move_Panel_TouchPanel = GameCtrlPanel._Move_Panel:AddComponent(typeof(touchpanel));
	touctrl = GameCtrlPanel._Move_Panel_TouchPanel;
	touctrl.beginHandler = touctrl.beginHandler + this.onMoveTouchBegin;
	touctrl.dragHandler = touctrl.dragHandler + this.onMoveTouchDrag;
	touctrl.endHandler = touctrl.endHandler + this.onMoveTouchEnd;

	--Camera
	GameCtrlPanel._Camera_TouchPanel = GameCtrlPanel._Camera:AddComponent(typeof(touchpanel));
	touctrl = GameCtrlPanel._Camera_TouchPanel;
	--Bt1
	GameCtrlPanel._Bt1_TouchPanel = GameCtrlPanel._Bt1:AddComponent(typeof(touchpanel));
	touctrl = GameCtrlPanel._Bt1_TouchPanel;
end
--我也不知道怎么写
function GameCtrlCtrl.RemoveTouchPanel()
	
end

--处理View 响应，可能需要单独写逻辑来处理这个
function GameCtrlCtrl.onViewTouchBegin(...)
	local pos = ...;
	log("开始点击:" .. pos.x .. " " .. pos.y);
end

function GameCtrlCtrl.onViewTouchDrag(...)
	local pos = ...;
	log("持续点击:" .. pos.x .. " " .. pos.y);
end

function GameCtrlCtrl.onViewTouchEnd(...)
	local pos = ...;
	log("结束点击:" .. pos.x .. " " .. pos.y);
end
--处理Move
function GameCtrlCtrl.onMoveTouchBegin(...)
	local pos = ...;
	log("开始点击:" .. pos.x .. " " .. pos.y);
end

function GameCtrlCtrl.onMoveTouchDrag(...)
	local pos = ...;
	log("持续点击:" .. pos.x .. " " .. pos.y);
end

function GameCtrlCtrl.onMoveTouchEnd(...)
	local pos = ...;
	log("结束点击:" .. pos.x .. " " .. pos.y);
end

function GameCtrlCtrl.OnCamera()
	log("拖Camera的按钮，这个要想想怎么搞，也许要自己写");
end

function GameCtrlCtrl.OnRocker()
	log("OnRocker 这个是假象，反正我不实现内容");
end

function GameCtrlCtrl.OnBt0()
	log("防御技能");
end

function GameCtrlCtrl.OnBt1()
	log("攻击技能 也可以拖方向的说法");
end

function GameCtrlCtrl.OnBt2()
	log("技能1");
end

function GameCtrlCtrl.OnBt3()
	log("技能2");
end

function GameCtrlCtrl.OnBt4()
	log("技能3");
end

--被通知需要被干掉了,可能需要涉及清理数据，但是现在我却不知道
function GameCtrlCtrl.OnDestroy()

end