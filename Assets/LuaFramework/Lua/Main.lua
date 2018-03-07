--主入口函数。从这里开始lua逻辑
function Main()					
	print("logic start")	 		
end

--场景切换通知(似乎外部并没有调用,我自己添加了)
function OnLevelWasLoaded(level)
	log("OnLevelWasLoaded:level:" .. tostring(level));
	collectgarbage("collect")
	Time.timeSinceLevelLoad = 0
	Game.OnLevelWasLoaded(level);
end

function OnApplicationQuit()
	log("游戏准备退出，清理资源做后续工作");
end