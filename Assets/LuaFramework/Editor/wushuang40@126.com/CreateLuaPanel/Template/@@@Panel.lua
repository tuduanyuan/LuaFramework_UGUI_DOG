local transform;
local gameObject;

--和LuaBehaviour承接，算不上调用,和C#连接在一起了
@@@Panel = {};
local this = @@@Panel;

--构造(在LuaBehaviour中调用,后面标记C#)--
function @@@Panel.Awake(obj)
	--baseInit
	gameObject = obj;
	transform = obj.transform;

	this.InitPanel();
end

--C#(默认关闭)--
--function @@@Panel.Start()
--	logInfo("@@@Panel.Start---->>>");
--end
--C#(默认关闭)--
--function @@@Panel.OnClick(...)
--	logInfo("@@@Panel.OnClick---->>>");
--end

--初始化面板(View)--
function @@@Panel.InitPanel()
	logInfo("@@@Panel.InitPanel---->>>");
end

--单击事件--
function @@@Panel.OnDestroy()
	logInfo("@@@Panel.OnDestroy---->>>");
end

