--[[
    Panels管理和初始化分离一下
]]
PanelsSets = {};
local this = PanelsSets;

function PanelsSets.Init()
	this.InitViewPanels();
end

function PanelsSets.InitViewPanels()
	for i = 1, #PanelNames do
		require ("View/"..tostring(PanelNames[i]))
	end
end