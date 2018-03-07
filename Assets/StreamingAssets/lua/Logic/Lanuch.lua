--[[
    资源的热更新不在这里
    游戏开始前的准备工作
]]

local Lanuch = {}
local this = Lanuch;
function Lanuch.Init()
    log("游戏Lanuch");
    this.initCommon();
    this.externalData();
    this.initLoadingRes();
    this.initAccredit();
    this.initNet();
end
--部分初始化的内容
function Lanuch.initCommon()
    --UI的东西
    PanelsSets.Init();
    CtrlManager.Init();
    require ("Element/"..tostring("Cube"));
end
--获取外部信息
function Lanuch.externalData()

end
--预加载资源调用
function Lanuch.initLoadingRes()

end
--[[
    唤起授权，可以是SDK,也可以是UDP登录，等等
    net我建议是在授权之后第一个loading中去连接
]]
function Lanuch.initAccredit()

end
--初始化网络
function Lanuch.initNet()
    AppConst.SocketPort = 2012;
    AppConst.SocketAddress = "127.0.0.1";
    networkMgr:SendConnect();
end









return Lanuch;