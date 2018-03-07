CreateLuaPanel.cs unity的编辑器类
CreateLuaPanel.py 生成文件脚本

注意：
需要python运行环境
要保证输出路径/目录存在
@@@ 是替换的RootName
后续生成：[RootName]Panel.lua / [RootName]Ctrl.lua

CreateLuaPanel.py参数:
string 				argv[1]: rootName 生成文件的根名称："RootName + Panel.lua / RootName + Ctrl.lua". 默认:Hello
string 				argv[2]: rootPath 目录路径，如果没有自动生成子目录:"View & Controller". 默认:.
string 				argv[3]: author 创作者.默认:Unkonw
bool 				argv[4]: cover 是否覆盖文件(如果已经存在的话)，True/False. 默认:False
string LuafileName 	argv[5]：ctrlDemo Ctrl文件模板 只用写名称(含后缀). 默认:Editor/CreateLuaPanel/Template/@@@Ctrl.lua
string LuafileName 	argv[6]：viewDemo View文件模板 只用写名称(含后缀). 默认:Editor/CreateLuaPanel/Template/@@@Panel.lua
string 			 	argv[7]：defines  路径，在此路径下找已有的文件(ctrlnames.lua/panelnames.lua)中加入枚举，如果没有则会创建. 默认:.

使用:
CreateLuaPanel.py RootName rootPath author cover CtrlDemo ViewDemo
使用默认参数输入@
CreateLuaPanel.py RootName rootPath @ @ @ @

Output:
1.rootPath/Controller/RootName + "Ctrl.lua"
2.rootPath/View/RootName + "Panel.lua"

Demo目录:在 CreateLuaPanel/Template
1.@@@Ctrl.lua Ctrl文件模板
2.@@@Panel.lua View文件模板
自行修改
如果自定义模板的话
把定义好的模板放置在Demo目录，@@@是要替换的内容
如：def_@@@Ctrl.lua 设置argv[5]/argv[6]

同时插件还会在指定的目录生成2个文件(ctrlnames.lua/panelnames.lua)
PS:如果不知道就先点击恢复默认值



