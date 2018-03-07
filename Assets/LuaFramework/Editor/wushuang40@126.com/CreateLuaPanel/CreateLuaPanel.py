#!/usr/bin/python
# -*- coding: UTF-8 -*-
#import
import sys
import time
import re
import os
import types
import json

reload(sys)
sys.setdefaultencoding("utf-8")

#Param:
#默认参数
defList = ['Test','.','wushuang','False','@@@Ctrl.lua','@@@Panel.lua',"Test"]
inputList = []
#Timer
runTime = time.strftime('%Y-%m-%d %H:%M:%S',time.localtime(time.time()))
print("Info:at Time " + runTime + " run script:" + sys.argv[0])
print("@----begin Create @@@LuaFile!----")

inputNum = len(sys.argv)
if inputNum > 1:
	inputList = inputList + sys.argv[1:]
if inputList and len(inputList) > 0:
	print("Info:Param:"+str(inputList))
for index in range(len(defList)):
	t_len = len(inputList)
	if t_len > index:
		if inputList[index] == '@':
			inputList[index] = defList[index]
	else:
		#不存在
		inputList.insert(index,defList[index])
#这里输入检查写的比较low，需要的自行去写
if len(inputList) >= 1:
	rootName = inputList[0]
else:
	rootName = defList[0]
if len(inputList) >= 2:
	rootPath = inputList[1]
else:
	rootPath = defList[1]
if len(inputList) >= 3:
	author = inputList[2]
else:
	author = defList[2]
if len(inputList) >= 4:
	cover = inputList[3]
else:
	cover = defList[3]
if len(inputList) >= 5:
	ctrlDemo = inputList[4]
else:
	ctrlDemo = defList[4]
if len(inputList) >= 6:
	viewDemo = inputList[5]
else:
	viewDemo = defList[5]
if len(inputList) >= 7:
	eunmFilePath = inputList[6]
else:
	eunmFilePath = ""
inputSize = len(sys.argv)-1
print("Info:input size is " + str(inputSize) + " :")

if inputSize >= 1 and rootName == defList[0]:
	print("Info:def rootName:" + rootName )
else:
	print("Info:argv[1] rootName:" + rootName )

if inputSize >= 2 and rootPath == defList[1] :
	print("Info:def rootPath:" + rootPath)
else:
	print("Info:argv[2] rootPath:" + rootPath)

if inputSize >= 3 and author == defList[2] :
	print("Info:def author:" + author)
else:
	print("Info:argv[3] author:" + author)

if inputSize >= 4 and cover == defList[3] :
	print("Info:def cover:" + str(cover))
else:
	print("Info:argv[4] cover:" + str(cover))

if inputSize >= 5 and ctrlDemo == defList[4]:
	print("Info:def ctrlDemo:" + ctrlDemo)
else:
	print("Info:argv[5] ctrlDemo:" + ctrlDemo)

if inputSize >= 6 and viewDemo == defList[5]:
	print("Info:def viewDemo:" + viewDemo)
else:
	print("Info:argv[6] viewDemo:" + viewDemo)
	
if inputSize >= 7 and len(eunmFilePath) == 0:
	print("Info:def eunmFilePath:" + "")
else:
	print("Info:argv[7] eunmFilePath:" + eunmFilePath)

print("@-----------Create ing-----------")

def getCoding(strInput):
    '''
    获取编码格式
    '''
    if isinstance(strInput, unicode):
        return u"unicode"
    try:
        strInput.decode("utf8")
        return u'utf8'
    except:
        pass
    try:
        strInput.decode("gbk")
        return u'gbk'
    except:
        pass
	pass


def headInfo(name,path,author = author):
	info = '--[[\n@author:' + author + '\n' + '@Gen Create by python\n' + '@name:' + name + '\n'+'@time:'+ runTime +'\n]]--\n\n\n'
	return info

def addLine(data,lineInfo = '\n'):
	dataLen = len(data)
	if dataLen > 0:
		last = data[dataLen-1]
		if last != '\n':
			data = data + '\n'
	if lineInfo == '\n':
		data = data + lineInfo
	else:
		data = data + lineInfo + '\n'
		
	return data

def ReplaceData(data,rep):
	strData = str(data)
	strinfo = re.compile('@@@')
	newStr = strinfo.sub(rep,strData)
	return newStr
	
def getDemoFileData(selectFile):
	
	demoPath = ""
	runPath = sys.path[0];
	
	if selectFile == "Ctrl":
		demoPath = runPath + "/Template/" + ctrlDemo
	if selectFile == "View":
		demoPath = runPath + "/Template/" + viewDemo
	if demoPath == "" :
		return ""
	demoPath = demoPath.replace('\\','/');
	f = open(demoPath,'r')
	if not (f):
		return ""
	fdata = f.read()#读取全部内容  
	f.close()
	print("Info:get demo data from :" + demoPath)
	return fdata

def writeCtrl(_rootName):
	
	writeData = getDemoFileData("Ctrl")
	if writeData == "":
		return ""
	writeData = ReplaceData(writeData,_rootName)
	return writeData
	
def createCtrlLuaFile(_rootname,_rootpath):
	#Ctrl文件信息
	outPutCtlName = _rootname + "Ctrl.lua";
	outPutCtlPath = _rootpath + "/Controller";
	print("Info:Create Ctl file "+outPutCtlName+" in path:"+outPutCtlPath)
	if not os.path.exists(outPutCtlPath):
		os.mkdir(outPutCtlPath)
	
	#打开Ctl输出文件
	
	CtlFile = outPutCtlPath + '/' + outPutCtlName
	
	if os.path.exists(CtlFile) == True and cover == 'False':
		print('Error:'+outPutCtlName+' ctrlfile in path exist')
		return False
	
	fileOutput = open(CtlFile,'w')
	
	if not fileOutput:
		return False 
	writeData = headInfo(outPutCtlName,outPutCtlPath)
	writeData = writeData + writeCtrl(_rootname)
	
	fileOutput.write(writeData.decode('utf-8'))
	fileOutput.close()
	
	return True

def writeView(_rootName):
	writeData = getDemoFileData("View")
	if writeData == "":
		return ""
	writeData = ReplaceData(writeData,_rootName)
	return writeData

def createViewlLuaFile(_rootname,_rootpath):
	#View文件信息
	outPutViewName = _rootname + "Panel.lua";
	outPutViewPath = _rootpath + "/View";
	print("Info:Create View file "+outPutViewName+" in path:"+outPutViewPath)
	if not os.path.exists(outPutViewPath):
		os.mkdir(outPutViewPath)
	#打开View输出文件
	ViewFile = outPutViewPath + '/' + outPutViewName
	print(ViewFile)
	if os.path.exists(ViewFile) == True and cover == 'False':
		print('Error:'+outPutViewName+' viewfile in path exist')
		return False
	fileOutput = open(ViewFile,'w')
	if not fileOutput:
		return False 
	writeData = headInfo(outPutViewName,outPutViewPath)
	writeData = writeData + writeView(_rootname)
	fileOutput.write(writeData.decode('utf-8'))
	fileOutput.close()
	return True
	pass


def processWriteData(_table,_type):
	easyHeadInfo = u"--这个文件会被外部工具刷新--\n";
	data = "";
	if (_type == u"Ctrl"):
		data = u"CtrlNames = {\n";
	elif ( _type == u"Panel"):
		data = u"PanelNames = {\n";
		pass

	for key in _table:
		
		value = _table[key];
		data += u"	";
		if (_type == u"Ctrl"):
			data += key + u" = \"" + value +u'\",\n';
		elif (_type == u"Panel"): 
			#print key + ":" + _table[key];
			data += "\"" + value + "\",\n";
			pass
		pass
	data +=  u"}\n";
	return easyHeadInfo + data;
	pass
def writeEnum(_path,_table,_tail):
	_fp = open(_path ,'w');
	if not _fp:
		return; 
		pass
	outData = processWriteData(_table,_tail);
	_fp.write(outData);
	_fp.close();
	pass

def addData(_rootName,_table,_tail):

	if (_tail == u"Ctrl"):
		if (_table.has_key(_rootName)):return _table;pass
		_table[_rootName] = str(_rootName) + str(_tail);
	elif (_tail == u"Panel"):
		if (_table.has_key(_rootName + str(_tail))):return _table;pass
		value = str(_rootName) + str(_tail);
		_table[value] = value;
		pass
	return _table;
	pass

def processReadData(data,_tail):
	#获取{}部分
	begin = data.find('{');
	end = data.find('}');
	datastr =  data[begin +1 : end];
	#print datastr;
	datalist = datastr.split(",");
	table = {}; 
	#print datalist
	if datalist[len(datalist) - 1] == u"\n":
		datalist = datalist[0:len(datalist) - 1];
		pass
	for line in datalist:
		if (_tail == u"Ctrl"):
			info = line.strip().split(u"=");
			key = info[0].strip();
			value = info[1].strip();
			value = value[1:len(value)-1];
			table[str(key)] = str(value);
		elif (_tail == u"Panel"):
			value = line.strip();
			value = value[1:len(value)-1];
			table[str(value)] = str(value);
			pass
		pass
	return table;
	pass

def readEnum(path,_tail):
	_fp = open(path,'r');
	if not _fp:
		return; 
		pass

	head = _fp.readline();
	block = _fp.read();
	table = processReadData(block,_tail);
	_fp.close();
	return table;
	pass
def writeEnumToLuaDefine(_rootName,_path):
	#进行文件检查
	ctrlnamespath = _path + "/ctrlnames.lua";
	panelnamespath = _path + "/panelnames.lua";
	if os.path.exists(ctrlnamespath) == True:
		#print u"info: ctrlnames.lua存在,读"
		table = readEnum(ctrlnamespath,u"Ctrl");
		table = addData(_rootName,table,u"Ctrl");
		writeEnum(ctrlnamespath,table,u"Ctrl");
	else:
		print u"info: ctrlnames.lua不存在,创建" + ctrlnamespath
		table = {};
		table = addData(_rootName,table,u"Ctrl");
		writeEnum(ctrlnamespath,table,u"Ctrl");
		pass
	if os.path.exists(panelnamespath) == True:
		#print u"info: panelnames.lua存在,读"
		table = readEnum(panelnamespath,u"Panel");
		table = addData(_rootName,table,u"Panel");
		writeEnum(panelnamespath,table,u"Panel");
	else:
		table = {};
		table = addData(_rootName,table,u"Panel");
		writeEnum(panelnamespath,table,u"Panel");
		pass
	pass
def run():
	isCrtlok = createCtrlLuaFile(rootName,rootPath)
	if isCrtlok == False:
		print("Result:Shut down,create CtrLuaFile failed!!!")
		return
	isViewlok = createViewlLuaFile(rootName,rootPath)
	if isViewlok == False:
		print("Result:Shut down,create ViewLuaFile failed!!!")
		return
	writeEnumToLuaDefine(rootName,eunmFilePath);
	print("Result:create succeed!!!")

run()

print("@-----end Create @@@LuaFile!-----")

	
