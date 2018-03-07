local transform;
local gameObject;

PromptPanel = {};--全局变量
local this = PromptPanel;

--启动事件--
function PromptPanel.Awake(obj)
	gameObject = obj;
	transform = obj.transform;
	logWarn("视图 Awake lua--->>"..gameObject.name);
	this.InitPanel();
end

--初始化面板--
function PromptPanel.InitPanel()
	this.btnOpen = transform:Find("Open").gameObject;
	this.gridParent = transform:Find('ScrollView/Grid');
end

--初始化面板--
function PromptPanel.Start()

end

--单击事件--
function PromptPanel.OnDestroy()
	logWarn("OnDestroy---->>>");
end