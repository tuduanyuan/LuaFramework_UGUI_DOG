--[[
@author:wushuang
@Gen Create by python
@name:HelloPanel.lua
@time:2018-02-24 17:46:26
]]--


local transform;
local gameObject;

--和LuaBehaviour承接，算不上调用,和C#连接在一起了
HelloPanel = {};
local this = HelloPanel;

--构造(在LuaBehaviour中调用,后面标记C#)--
function HelloPanel.Awake(obj)
	--baseInit
	gameObject = obj;
	transform = obj.transform;

	this.InitPanel();
end

--C#(默认关闭)--
--function HelloPanel.Start()
--	logInfo("HelloPanel.Start---->>>");
--end
--C#(默认关闭)--
--function HelloPanel.OnClick(...)
--	logInfo("HelloPanel.OnClick---->>>");
--end

--初始化面板(View)--
function HelloPanel.InitPanel()
	logInfo("HelloPanel.InitPanel---->>>");
	this.btLogin = transform:FindChild("Button").gameObject;
end

--单击事件--
function HelloPanel.OnDestroy()
	logInfo("HelloPanel.OnDestroy---->>>");
end

