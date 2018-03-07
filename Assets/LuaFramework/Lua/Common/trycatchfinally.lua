--[[TODO:try-catch-finally
    Lua原生并没有提供try-catch-finally的语法来捕获异常处理，
    但是提供了pcall/xpcall等接口，
    可在保护模式下执行lua函数。
    例如
    try
    {
        -- try 代码块
        function ()
            error("error message")
        end,

        -- catch 代码块
        catch 
        {
            -- 发生异常后，被执行
            function (errors)
                print(errors)
            end
        },

        -- finally 代码块
        finally 
        {
            -- 最后都会执行到这里
            function (ok, errors)
                -- 如果try{}中存在异常，ok为true，errors为错误信息，否则为false，errors为try中的返回值
            end
        }
    }
]]
--[[
    Code
]]


local _traceback = function(errors)
     -- make results
     local level = 2    
     while true do    
 
         -- get debug info
         local info = debug.getinfo(level, "Sln")

         -- end?
         if not info or (info.name and info.name == "xpcall") then
             break
         end

         -- function?
         if info.what == "C" then
             results = results .. string.format("    [C]: in function '%s'\n", info.name)
         elseif info.name then 
             results = results .. string.format("    [%s:%d]: in function '%s'\n", info.short_src, info.currentline, info.name)    
         elseif info.what == "main" then
             results = results .. string.format("    [%s:%d]: in main chunk\n", info.short_src, info.currentline)    
             break
         else
             results = results .. string.format("    [%s:%d]:\n", info.short_src, info.currentline)    
         end
 
         -- next
         level = level + 1    
     end    
 
     -- ok?
     return results
end

function try(block)
    -- get the try function
    local try = block[1]
    assert(try)

    -- get catch and finally functions
    local funcs = block[2]
    if funcs and block[3] then
        table.join2(funcs, block[2])
    end

    -- try to call it
    --local ok, errors = pcall(try)
    local ok, errors = xpcall(try, _traceback)
    
    if not ok then
        -- run the catch function
        if funcs and funcs.catch then
            funcs.catch(errors)
        end
    end

    -- run the finally function
    if funcs and funcs.finally then
        funcs.finally(ok, errors)
    end

    -- ok?
    if ok then
        return errors
    end
end
