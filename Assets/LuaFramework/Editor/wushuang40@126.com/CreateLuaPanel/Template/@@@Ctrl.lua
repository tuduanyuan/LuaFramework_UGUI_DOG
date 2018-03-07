require "Common/define"


@@@Ctrl = {};
local this = @@@Ctrl;

local luabehaviour;
local transform;
local gameObject;

--构建函数--
function @@@Ctrl.New()
	logWarn("@@@Ctrl.New--->>");
	return this;
end
--初始化--
function @@@Ctrl.Awake()
	logWarn("@@@Ctrl.Awake--->>");
	local e = LuaFramework.PanelManager.PanelType.IntToEnum(0);--搁置
	panelMgr:CreatePanel('@@@', e , this.OnCreate);--关链LuaBehaviour
	
end

--启动事件--
function @@@Ctrl.OnCreate(obj)
	logWarn("@@@Ctrl.OnCreate--->>");
	--基本获取 需要的自行添加
	gameObject = obj;
	transform = obj.transform;
	luabehaviour = gameObject:GetComponent('LuaBehaviour');
	--Start
	logWarn("@@@Ctrl Start lua--->>"..gameObject.name);
	
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
end
--驱动Update(默认关闭)--
function @@@Ctrl.Update()
    
end

function @@@Ctrl.LateUpdate()
    
end

function @@@Ctrl.FixedUpdate()
    
end

------自定义函数------
--函数式例--
--function @@@Ctrl.fun(obj,...)

--end
