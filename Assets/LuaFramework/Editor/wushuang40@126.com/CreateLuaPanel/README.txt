CreateLuaPanel.cs unity�ı༭����
CreateLuaPanel.py �����ļ��ű�

ע�⣺
��Ҫpython���л���
Ҫ��֤���·��/Ŀ¼����
@@@ ���滻��RootName
�������ɣ�[RootName]Panel.lua / [RootName]Ctrl.lua

CreateLuaPanel.py����:
string 				argv[1]: rootName �����ļ��ĸ����ƣ�"RootName + Panel.lua / RootName + Ctrl.lua". Ĭ��:Hello
string 				argv[2]: rootPath Ŀ¼·�������û���Զ�������Ŀ¼:"View & Controller". Ĭ��:.
string 				argv[3]: author ������.Ĭ��:Unkonw
bool 				argv[4]: cover �Ƿ񸲸��ļ�(����Ѿ����ڵĻ�)��True/False. Ĭ��:False
string LuafileName 	argv[5]��ctrlDemo Ctrl�ļ�ģ�� ֻ��д����(����׺). Ĭ��:Editor/CreateLuaPanel/Template/@@@Ctrl.lua
string LuafileName 	argv[6]��viewDemo View�ļ�ģ�� ֻ��д����(����׺). Ĭ��:Editor/CreateLuaPanel/Template/@@@Panel.lua
string 			 	argv[7]��defines  ·�����ڴ�·���������е��ļ�(ctrlnames.lua/panelnames.lua)�м���ö�٣����û����ᴴ��. Ĭ��:.

ʹ��:
CreateLuaPanel.py RootName rootPath author cover CtrlDemo ViewDemo
ʹ��Ĭ�ϲ�������@
CreateLuaPanel.py RootName rootPath @ @ @ @

Output:
1.rootPath/Controller/RootName + "Ctrl.lua"
2.rootPath/View/RootName + "Panel.lua"

DemoĿ¼:�� CreateLuaPanel/Template
1.@@@Ctrl.lua Ctrl�ļ�ģ��
2.@@@Panel.lua View�ļ�ģ��
�����޸�
����Զ���ģ��Ļ�
�Ѷ���õ�ģ�������DemoĿ¼��@@@��Ҫ�滻������
�磺def_@@@Ctrl.lua ����argv[5]/argv[6]

ͬʱ���������ָ����Ŀ¼����2���ļ�(ctrlnames.lua/panelnames.lua)
PS:�����֪�����ȵ���ָ�Ĭ��ֵ



