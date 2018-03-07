--[[
@author:wushuang
@Gen Create by python
@name:GameCtrlPanel.lua
@time:2018-03-02 16:40:50
]]--


local transform;
local gameObject;

--和LuaBehaviour承接，算不上调用,和C#连接在一起了
GameCtrlPanel = {};
local this = GameCtrlPanel;

--构造(在LuaBehaviour中调用,后面标记C#)--
function GameCtrlPanel.Awake(obj)
	--baseInit
	gameObject = obj;
	transform = obj.transform;

	this.InitPanel();
end

--C#(默认关闭)--
--function GameCtrlPanel.Start()
--	logInfo("GameCtrlPanel.Start---->>>");
--end
--C#(默认关闭)--
--function GameCtrlPanel.OnClick(...)
--	logInfo("GameCtrlPanel.OnClick---->>>");
--end

--初始化面板(View)--
function GameCtrlPanel.InitPanel()
	logInfo("GameCtrlPanel.InitPanel---->>>");

	this._View = transform:FindChild("View").gameObject;
	this._View_Panel = transform:FindChild("View"):FindChild("Panel").gameObject;


	this._Move = transform:FindChild("Move").gameObject;
	this._Move_Panel = transform:FindChild("Move"):FindChild("Panel").gameObject;

	local ctrl_transform = transform:FindChild("Ctrl");
	this._Ctrl = ctrl_transform.gameObject;

	this._Camera = ctrl_transform:FindChild("Camera").gameObject;
	this._Rocker = ctrl_transform:FindChild("Rocker").gameObject;
	this._Bt0 = ctrl_transform:FindChild("Bt0").gameObject;
	this._Bt1 = ctrl_transform:FindChild("Bt1").gameObject;
	this._Bt2 = ctrl_transform:FindChild("Bt2").gameObject;
	this._Bt3 = ctrl_transform:FindChild("Bt3").gameObject;
	this._Bt4 = ctrl_transform:FindChild("Bt4").gameObject;
end

--单击事件--
function GameCtrlPanel.OnDestroy()
	logInfo("GameCtrlPanel.OnDestroy---->>>");
end

