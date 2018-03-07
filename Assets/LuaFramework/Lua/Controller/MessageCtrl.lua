
MessageCtrl = {};
local this = MessageCtrl;

local message;
local transform;
local gameObject;

--构建函数--
function MessageCtrl.New()
	logWarn("MessageCtrl.New--->>");
	return this;
end

function MessageCtrl.Awake()
	--logWarn("MessageCtrl.Awake--->>");
	local e = LuaFramework.PanelManager.PanelType.IntToEnum(0);--搁置
	panelMgr:CreatePanel('Message', e , this.OnCreate);
end

--启动事件--
function MessageCtrl.OnCreate(obj)
	gameObject = obj;

	message = gameObject:GetComponent('LuaBehaviour');
	message:AddClick(MessagePanel.btnClose, this.OnClick);

	logWarn("Start lua--->>"..gameObject.name);
end

--单击事件--
function MessageCtrl.OnClick(go)
	--destroy(gameObject);
	this.Close();
end

--关闭事件--
function MessageCtrl.Close()
	log("我想看的调用");
	panelMgr:ClosePanel(CtrlNames.Message);
end