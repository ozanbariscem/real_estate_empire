-- I exposed the Log function on DeveloperConsole.cs LoadScript function
function add(...)
    local numbers = {...}

    local total = 0
    local log = ""
    for i = 1, #numbers do
        local num = tonumber(numbers[i])
        if num then
            total = total + num
            log = log..num
            if i != #numbers then
                log = log.." + "
            end
        end
    end

    log = log.."\n= "..total
    Log(log)
    return total
end

-- I exposed the UI class on DeveloperConsole.cs LoadScript function
function clear()
    UI.ClearLog()
end