@log工具 - 可以输出到文件
没有写开关

格式:[Type][Time:dd HH:mm:ss:ffff][source:C#橙色/Lua紫色][file/class@function:line]:内容

Type：
D->Debug 白色，在Lua 和 C#都可以打印出来全套表单 
I->Info 绿色,快速log,Update中主要使用
W->Warn 黄色，警告
E->Error 红色

file/class@function:line:可能的类和文件，非Debug且来源是Lua，这个项目是没有的
内容:Log内容