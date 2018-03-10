
--TODO:新添加的local函数
--获取到堆栈信息，犹豫包了一层，这里获取的第3层的堆栈
local debugInfo = function()
	local info = debug.getinfo(3);
	local fileOrClass = info['short_src'] or "UnKonw";
	local line = info['currentline'] or -1;
	local func = info['func'] or"Unkonw";
	if (fileOrClass ~= "Unkonw" and string.find(fileOrClass,".") == nil) then
		fileOrClass = fileOrClass .. ".lua";
	end
	local result = string.split(tostring(func),':');
	return fileOrClass,trim(result[2]),line;
end

--输出日志--
function log(str)
	local fileOrClass,func,line = debugInfo();
	Util.Log(str,"Lua",fileOrClass,func,line);
end
--信息日志--
function logInfo(str)
	local fileOrClass,func,line = debugInfo();
	Util.LogInfo(str,"Lua",fileOrClass,func,line);
end

--警告日志--
function logWarn(str) 
	local fileOrClass,func,line = debugInfo();
	Util.LogWarning(str,"Lua",fileOrClass,func,line);
end

--错误日志--
function logError(str) 
	local fileOrClass,func,line = debugInfo();
	Util.LogError(str,"Lua",fileOrClass,func,line);
end

--查找对象--
function find(str)
	return GameObject.Find(str);
end

function destroy(obj)
	GameObject.Destroy(obj);
end

function newObject(prefab)
	return GameObject.Instantiate(prefab);
end

--创建面板--
function createPanel(name)
	PanelManager:CreatePanel(name);
end

function child(str)
	return transform:FindChild(str);
end

function subGet(childNode, typeName)		
	return child(childNode):GetComponent(typeName);
end

function findPanel(str) 
	local obj = find(str);
	if obj == nil then
		error(str.." is null");
		return nil;
	end
	return obj:GetComponent("BaseLua");
end

--TODO:新添加的函数
--去除字符串首尾空格
function trim(s)
	assert(type(s)=="string")
	return (string.gsub(s, "^%s*(.-)%s*$", "%1")) 
end
--获取number的int部分
function getIntPart(x)
	if x <= 0 then
	   return math.ceil(x);
	end
	if math.ceil(x) == x then
	   x = math.ceil(x);
	else
	   x = math.ceil(x) - 1;
	end
	return x;
end