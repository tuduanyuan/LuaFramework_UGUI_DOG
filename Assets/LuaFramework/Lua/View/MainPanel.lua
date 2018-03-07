--[[
@author:wushuang
@Gen Create by python
@name:MainPanel.lua
@time:2018-03-01 14:58:29
]]--


local transform;
local gameObject;

--和LuaBehaviour承接，算不上调用,和C#连接在一起了
MainPanel = {};
local this = MainPanel;

--构造(在LuaBehaviour中调用,后面标记C#)--
function MainPanel.Awake(obj)
	--baseInit
	gameObject = obj;
	transform = obj.transform;

	this.InitPanel();
end

--C#(默认关闭)--
--function MainPanel.Start()
--	logInfo("MainPanel.Start---->>>");
--end
--C#(默认关闭)--
--function MainPanel.OnClick(...)
--	logInfo("MainPanel.OnClick---->>>");
--end

--初始化面板(View)--
function MainPanel.InitPanel()
	logInfo("MainPanel.InitPanel---->>>");
	this.btGameBegin = transform:FindChild("Button").gameObject;
end

--单击事件--
function MainPanel.OnDestroy()
	logInfo("MainPanel.OnDestroy---->>>");
end

